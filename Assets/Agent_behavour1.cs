using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Directions { Forward = 0, BackWard = 1, Left = 2, Right = 3 }
public enum Resource { Wood = 0, Ore = 1, Clay = 2 }

public class Agent_behavour1 : MonoBehaviour {


    public Point currentLocation;
    public Point Intent;
    public Inventory inventory;

    public int Id;
    public string Team;
    public bool isBusyGathering = false;


    //these are checks that happen while I gather resources
    public bool[] gatheredFromNeighborus;
    public bool coroutineStarted;
    public int gatheringIndex = 0;

    public bool[] PossibleMoves;
    public bool[] resources;
    public bool thereAreResources;
    public bool checkedIfThereAreResources;
    public bool thereIsConflict;
    public bool checkedForConflict;
    public bool priority_highest;
    public bool rolledTheDice;
    public bool AlreadyCountedConflict;

    public Directions dir;
    public Vector3 dirOffsetToMove;
    public Vector3 startpoz;
    public float Priority = -1;

    public bool ismoving = false;
    public bool ValidDir = false;
    public bool thereAreMoves;
    
    public static int conf_count=0;

    public int timestep = 0;

    //section for PathFinding;
    public bool foundPath;
    public bool[,] agentMap;
    public List<Point> pathList;
    public int pathIndex = -1;
    public Point droppingPoint;
    public bool startedCoroutineForPath;
    public bool InventoryIsFull;
    public Material mat;
    public Material matDefault;

    public void Start()
    {
        GameManagement.agents.Add(this);
        inventory = new Inventory();
        PossibleMoves = new bool[4];
        gatheredFromNeighborus = new bool[4];
        resources = new bool[4];
        agentMap = new bool[GameManagement.assets_holder.GetLength(0), GameManagement.assets_holder.GetLength(1)];
        pathList = new List<Point>();
        pathIndex = -1;
        droppingPoint = GameManagement.RedBaseLocation;
        Debug.Log("Droping Point:" + droppingPoint.ToString());
        transform.GetChild(0).GetComponent<Text>().text = Team + Id;
        transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = Team + Id;
    }

    public void FixedUpdate()
    {
        if (!ValidDir)
        {
            PossibleMoves = new bool[4];
            PickDirection(currentLocation.x, currentLocation.y);
        }     

        if(!InventoryIsFull)
        {
            if(!rolledTheDice)
            {
                Priority = Random.value;
                rolledTheDice = true;
                //Debug.Log(""+ System.Math.Round(Priority, 3)+" "+Time.time);
            }

            if (!checkedIfThereAreResources)
            {

                CheckIfThereAreResouces(currentLocation.x, currentLocation.y);
                checkedIfThereAreResources = true;
            }
            else
            {

                if (thereAreResources && !HasCheckTheResources())
                {
                    if (!coroutineStarted)
                        CollectLocalResouces();
                    isBusyGathering = true;
                }


                if (thereAreResources && HasCheckTheResources() || !thereAreResources)
                {
                    isBusyGathering = false;
                    if (inventory.InventoryIsFull())
                        InventoryIsFull = true;
                    //ismoving = true;
                    if (!ismoving)
                    {

                        CheckPossibleMoves(currentLocation.x, currentLocation.y);
                        bool notAgoodDirection = false;

                        switch ((int)dir)
                        {
                            case 0:
                                if (!PossibleMoves[0])
                                    notAgoodDirection = true;
                                break;

                            case 1:
                                if (!PossibleMoves[1])
                                    notAgoodDirection = true;
                                break;

                            case 2:
                                if (!PossibleMoves[2])
                                    notAgoodDirection = true;
                                break;

                            case 3:
                                if (!PossibleMoves[3])
                                    notAgoodDirection = true;
                                break;
                            default: notAgoodDirection = false; break;
                        }

                        if (notAgoodDirection)
                            PickDirection(currentLocation.x,currentLocation.y);

                        CheckForConflict(currentLocation.x, currentLocation.y);

                        if (conf_count > 1 && thereIsConflict)
                        {
                            if (priority_highest)
                            {
                                ismoving = true;
                                GameManagement.assets_holder[Intent.x, Intent.y] = gameObject;
                                thereIsConflict = false;
                                AlreadyCountedConflict = false;
                                conf_count--;
                            }
                              
                            Debug.Log("(" + PossibleMoves[0] + " " + PossibleMoves[1] + " " + PossibleMoves[2] + " " + PossibleMoves[3] + ")" + "Direction:" + (int)dir + " " + Team + " " + (Id + 1) + " IsConflict" + thereIsConflict + " Priority:" + System.Math.Round(Priority, 3) + " Current:" + currentLocation.ToString() + " Intent:" + Intent.ToString() + " Ts:" + timestep);
                            Time.timeScale = 0;

                        }
                        else
                        {
                            if (thereIsConflict && !AlreadyCountedConflict)
                            {
                                conf_count++;
                                AlreadyCountedConflict = true;
                            }

                            if (thereIsConflict && AlreadyCountedConflict)
                                ValidDir = false;
                              
                        }

                        if (!thereIsConflict)
                        {
                            if (AlreadyCountedConflict)
                            {
                                conf_count--;
                                AlreadyCountedConflict = false;
                            }

                            GameManagement.assets_holder[Intent.x, Intent.y] = gameObject;
                            ismoving = true;
                        }
                            
                      
                    }
                    #region workinprogress
                    /* if(!checkedForConflict)
                     {
                         CheckForConflict(currentLocation.x,currentLocation.y);
                         checkedForConflict = true;
                     }
                     else
                     {
                         if(!ismoving)
                         {
                             if (!thereIsConflict && thereAreMoves)
                             {
                                 ismoving = true;
                                 //Debug.Log("No conflict and Has Moves!");
                             }


                             if (thereIsConflict)
                             {

                                 Debug.Log("(" + PossibleMoves[0] + " " + PossibleMoves[1] + " " + PossibleMoves[2] + " " + PossibleMoves[3] + ")" + "Direction:" + (int)dir + " " + Team + " " + (Id+1)+ " IsConflict"+ thereIsConflict+" Priority:"+ System.Math.Round(Priority, 3) + " Current:"+currentLocation.ToString()+" Intent:"+Intent.ToString()+" Ts:"+timestep);
                                 if (priority_highest)
                                 {
                                     ismoving = true;
                                     Debug.Log(Team + (Id+1) + " has highest score!");
                                     // Debug.Log("Priority " + System.Math.Round(Priority, 3) + Team + Id);
                                   //  Debug.Log("IsConflict" + thereIsConflict + " " + Time.time + " " + Team + " " + Id);
                                 }
                                 else
                                 {
                                     ValidDir = false;
                                     checkedForConflict = false;
                                       // checkedForConflict = false;
                                       CheckIfConflict(currentLocation.x, currentLocation.y);

                                    // Debug.Log("IsConflict" + thereIsConflict + " " + Time.time + " " + Team + " " + Id);
                                      if (!thereIsConflict)
                                      {
                                          Time.timeScale = 0f;
                                          ismoving = true;
                                      }
                                      else
                                          ValidDir = false;

                                     // if (Priority != 0)
                                     //   Debug.Log("Priority " + System.Math.Round(Priority, 3) + Team + Id);
                                     // Priority = 0;
                                 }
                                 Time.timeScale = 0;
                             }
                           //  else
                               //  Debug.Log("(" + PossibleMoves[0] + " " + PossibleMoves[1] + " " + PossibleMoves[2] + " " + PossibleMoves[3] + ")" + "Direction:" + (int)dir + " " + Team + " " + (Id+1) + " IsConflict" + thereIsConflict + " Current:" + currentLocation.ToString() + " Intent:" + Intent.ToString() + " Ts:" + timestep);

                         }

                     }*/
                    #endregion
                }

            }
        }
        else
        {
            if (!(currentLocation.x == droppingPoint.x && currentLocation.y == droppingPoint.y))
            {
                //  Debug.Log("Inventory is Full!");if(!ismoving)
                Debug.Log(currentLocation.ToString() + " " + droppingPoint.ToString());

                if (!rolledTheDice)
                {
                    Priority = Random.value;
                    rolledTheDice = true;
                    //Debug.Log(""+ System.Math.Round(Priority, 3)+" "+Time.time);
                }

                if (foundPath)
                {
                    if (!ismoving)
                    {
                        //startpoz = transform.position;
                        Intent = pathList[pathIndex];
                        GetDirectionFromLastPostion(currentLocation.x, currentLocation.y, pathList[pathIndex].x, pathList[pathIndex].y);
                        //Time.timeScale = 0;
                        CheckForConflict(currentLocation.x, currentLocation.y);


                        if (!thereIsConflict)
                        {
                            GameManagement.assets_holder[Intent.x, Intent.y] = gameObject;
                            Debug.Log(pathList[pathIndex].ToString() + "Locatia in lista " + currentLocation.ToString() + " Indexul:" + pathIndex);
                            ismoving = true;
                        }
                    }
                }
            }
            else
                //Debug.Log(currentLocation.ToString()+" "+ droppingPoint.ToString()+ "Numi intra aici?");
                GiveResourcesToBase();
        }

        if (ismoving)
            Move();

    }

    public void PickDirection(int x, int y)
    {
        startpoz = transform.position;
        CheckPossibleMoves(x, y);
        List<int> tempList = new List<int>();
        for (int i = 0; i < PossibleMoves.Length; i++)
            if (PossibleMoves[i])
                tempList.Add(i);

        if(tempList.Count>0)
        {
            dir =(Directions)(tempList[Random.Range(0, tempList.Count)]);
            string s="(";
            for (int i = 0; i < tempList.Count; i++)
                s += ""+tempList[i] +" ";
            s += ") direction." + dir;
           // Debug.Log(s+"   ("+ PossibleMoves[0]+", "+ PossibleMoves[1]+", "+ PossibleMoves[2]+", "+ PossibleMoves[3]+")");
            ValidDir = true;

            switch (dir)
            {
                case Directions.Forward:
                    Intent = new Point(x + 1, y);
                   // Debug.Log(Intent.x + " " + Intent.y + " " + GameManagement.assets_holder.GetLength(0) + GameManagement.assets_holder.GetLength(0));
                    dirOffsetToMove = new Vector3(0, 0, 0.33f * Time.deltaTime);

                    break;

                case Directions.BackWard:
                    Intent = new Point(x - 1, y);
                  //  Debug.Log(Intent.x + " " + Intent.y + " " + GameManagement.assets_holder.GetLength(0) + GameManagement.assets_holder.GetLength(0));
                    dirOffsetToMove = new Vector3(0, 0, -0.33f * Time.deltaTime);

                    break;

                case Directions.Left:
                    Intent = new Point(x, y - 1);
                   // Debug.Log(Intent.x + " " + Intent.y + " " + GameManagement.assets_holder.GetLength(0) + GameManagement.assets_holder.GetLength(0));
                    dirOffsetToMove = new Vector3(-0.33f * Time.deltaTime, 0, 0);

                    break;

                case Directions.Right:
                    Intent = new Point(x, y + 1);
                  //  Debug.Log(Intent.x + " " + Intent.y + " " + GameManagement.assets_holder.GetLength(0) + GameManagement.assets_holder.GetLength(0));
                    dirOffsetToMove = new Vector3(0.33f * Time.deltaTime, 0, 0);

                    break;
            }
        }
        
    }

    //checks the posible move the agent can make right now withouth checking with other agents and sets the ThereAreMoves flag to true or false
    //and also the positions the agent can go
    public void CheckPossibleMoves(int x, int y)
    {

        if (RightIndex(x + 1, y))
        {
            if (GameManagement.assets_holder[x + 1, y] == null)
                PossibleMoves[0] = true;

        }

        if (RightIndex(x - 1, y))
        {
            if (GameManagement.assets_holder[x - 1, y] == null)
                PossibleMoves[1] = true;
        }

        if (RightIndex(x, y - 1))
        {
            if (GameManagement.assets_holder[x, y - 1] == null)
                PossibleMoves[2] = true;
        }

        if (RightIndex(x, y + 1))
        {
            if (GameManagement.assets_holder[x, y + 1] == null)
                PossibleMoves[3] = true;
        }

        thereAreMoves = false;
        string s = "";
        for (int i = 0; i < PossibleMoves.Length; i++)
        
            if (PossibleMoves[i])
            {
                s+=i+", ";
                thereAreMoves = true;
            }
        s += Team + " " + (Id+1) + ")";
       // Debug.Log(s+" Ts:"+timestep);
                
      
    }


    //checks if there are resources nearby and sets the thereAreResources flag to true and also the adjecent resources locations 
    public void CheckIfThereAreResouces(int x, int y)
    {
        thereAreResources = false;

        if (RightIndex(x + 1, y))
            if (GameManagement.assets_holder[x + 1, y] != null)
                if (GameManagement.assets_holder[x + 1, y].tag == "Tree" || GameManagement.assets_holder[x + 1, y].tag == "Ore"
                    || GameManagement.assets_holder[x + 1, y].tag == "Clay")
                {
                    thereAreResources = true;
                    resources[0] = true;
                }


        if (RightIndex(x - 1, y))
            if (GameManagement.assets_holder[x - 1, y] != null)
                if (GameManagement.assets_holder[x - 1, y].tag == "Tree" || GameManagement.assets_holder[x - 1, y].tag == "Ore"
                || GameManagement.assets_holder[x - 1, y].tag == "Clay")
                {
                    thereAreResources = true;
                    resources[1] = true;
                }

        if (RightIndex(x, y - 1))
            if (GameManagement.assets_holder[x, y - 1] != null)
                if (GameManagement.assets_holder[x, y - 1].tag == "Tree" || GameManagement.assets_holder[x, y - 1].tag == "Ore"
                || GameManagement.assets_holder[x, y - 1].tag == "Clay")
                {
                    thereAreResources = true;
                    resources[2] = true;
                }

        if (RightIndex(x, y + 1))
            if (GameManagement.assets_holder[x, y + 1] != null)
                if (GameManagement.assets_holder[x, y + 1].tag == "Tree" || GameManagement.assets_holder[x, y + 1].tag == "Ore"
                || GameManagement.assets_holder[x, y + 1].tag == "Clay")
                {
                    thereAreResources = true;
                    resources[3] = true;
                }

    }

    //depending on the case we start a coroutine that collects all neighbouring resources one at each call until all are collected
    public void CollectLocalResouces()
    {
        switch (gatheringIndex)
        {            
            case 0:
                if (resources[gatheringIndex]&& !TellIfFullofThatResource(currentLocation.x+1, currentLocation.y))
                {
                    coroutineStarted = true;
                    StartCoroutine(GatheringTime(3.0f, currentLocation.x + 1, currentLocation.y));
                }
                else
                {
                    gatheredFromNeighborus[gatheringIndex] = true;
                    gatheringIndex++;
                }
                 if (TellIfFullofThatResource(currentLocation.x + 1, currentLocation.y))
                    Debug.Log("Full of this Material!");
                break;

            case 1:
                if (resources[gatheringIndex] && !TellIfFullofThatResource(currentLocation.x - 1, currentLocation.y))
                {
                    coroutineStarted = true;
                    StartCoroutine(GatheringTime(3.0f, currentLocation.x - 1, currentLocation.y));
                }
                else
                {
                    gatheredFromNeighborus[gatheringIndex] = true;
                    gatheringIndex++;
                }
                if (TellIfFullofThatResource(currentLocation.x - 1, currentLocation.y))
                    Debug.Log("Full of this Material!");
                break;

            case 2:
                if (resources[gatheringIndex] && !TellIfFullofThatResource(currentLocation.x, currentLocation.y - 1))
                {
                    coroutineStarted = true;
                    StartCoroutine(GatheringTime(3.0f, currentLocation.x, currentLocation.y - 1));
                }
                else
                {
                    gatheredFromNeighborus[gatheringIndex] = true;
                    gatheringIndex++;
                }
                if (TellIfFullofThatResource(currentLocation.x, currentLocation.y - 1))
                    Debug.Log("Full of this Material!");
                break;

            case 3:
                if (resources[gatheringIndex] && !TellIfFullofThatResource(currentLocation.x, currentLocation.y + 1))
                {
                    coroutineStarted = true;
                    StartCoroutine(GatheringTime(3.0f, currentLocation.x, currentLocation.y + 1));
                }
                else
                {
                    gatheredFromNeighborus[gatheringIndex] = true;
                    gatheringIndex++;
                }
                if (TellIfFullofThatResource(currentLocation.x, currentLocation.y + 1))
                    Debug.Log("Full of this Material!");
                break;
        }
      
    }

    //still needs testing
    public bool TellIfFullofThatResource(int x, int y)
    {
        if(RightIndex(x,y))
        {
            if (GameManagement.assets_holder[x, y] != null)
                switch (GameManagement.assets_holder[x, y].tag)
                {
                    case "Clay":
                        if (inventory.ResourceFull(Resource.Clay))
                            return true;
                        return false;

                    case "Tree":
                        if (inventory.ResourceFull(Resource.Wood))
                            return true;
                        return false;

                    case "Ore":
                        if (inventory.ResourceFull(Resource.Ore))
                            return true;
                        return false;
                    default: return false;
                }
            return false;
        }
        
        return false;
    }

    //checks again if it is a resource and an existing location and then gathers if possible
    public void GatherResource(int x, int y)
    {
        if (RightIndex(x, y) && GameManagement.assets_holder[x, y] != null)
            switch (GameManagement.assets_holder[x, y].tag)
            {
                case "Tree":                  
                    if(!inventory.ResourceFull(Resource.Wood))
                    {
                        inventory.AddReource(Resource.Wood);
                        DestroyImmediate(GameManagement.assets_holder[x, y]);
                     //   Debug.Log("Am distrus chesita de [" + x + " ," + y + "]");
                     //   Debug.Log("Wood" + inventory.wood_resource);
                        GameManagement.assets_holder[x, y] = null;
                    }
                   // else
                     //   Debug.Log("Full of Wood");
                    break;

                case "Ore":                  
                    if (!inventory.ResourceFull(Resource.Ore))
                    {
                        inventory.AddReource(Resource.Ore);
                        DestroyImmediate(GameManagement.assets_holder[x, y]);
                     //   Debug.Log("Am distrus chesita de [" + x + " ," + y + "]");
                     //   Debug.Log("Ore" + inventory.ore_resource);
                        GameManagement.assets_holder[x, y] = null;
                    }
                   // else
                      //  Debug.Log("Full of Ore");
                    break;

                case "Clay":                  
                    if (!inventory.ResourceFull(Resource.Clay))
                    {
                        inventory.AddReource(Resource.Clay);
                        DestroyImmediate(GameManagement.assets_holder[x, y]);
                     //   Debug.Log("Am distrus chesita de [" + x + " ," + y + "]");
                     //   Debug.Log("Clay" + inventory.clay_resource);
                        GameManagement.assets_holder[x, y] = null;
                    }
                    //else
                       // Debug.Log("Full of Clay");
                    break;

                default: break;
            }
    }

    //gathering function that waits a number of seconds before collecting and setting the corespoding properties
    public IEnumerator GatheringTime(float sec, int x, int y)
    {
        yield return new WaitForSeconds(sec);
        GatherResource(x, y);
        coroutineStarted = false;
        gatheredFromNeighborus[gatheringIndex] = true;
        gatheringIndex++;
    }

/*
    public void CheckIfConflict(int x,int y)
    {
        // Debug.Log("Alo?");
        thereIsConflict = false;
        priority_highest = true;

        if (RightIndex(x - 2, y))
            if (GameManagement.assets_holder[x - 2, y] != null)
                if (GameManagement.assets_holder[x - 2, y].tag == "R_Agent" || GameManagement.assets_holder[x - 2, y].tag == "B_Agent")
                    if (GameManagement.assets_holder[x - 2, y].GetComponent<Agent_behavour1>().Intent == Intent 
                        && !GameManagement.assets_holder[x - 2, y].GetComponent<Agent_behavour1>().isBusyGathering)
                    {
                        // Debug.Log(GameManagement.assets_holder[x - 2, y].GetComponent<Agent_behavour1>().Intent.ToString() + " " + Intent.ToString());
                        if (Priority < GameManagement.assets_holder[x - 2, y].GetComponent<Agent_behavour1>().Priority)
                            priority_highest = false;
                        thereIsConflict = true;
                    }

        if (RightIndex(x - 1, y - 1))
            if (GameManagement.assets_holder[x - 1, y - 1] != null)
                if (GameManagement.assets_holder[x - 1, y - 1].tag == "R_Agent" || GameManagement.assets_holder[x - 1, y - 1].tag == "B_Agent")
                    if (GameManagement.assets_holder[x - 1, y - 1].GetComponent<Agent_behavour1>().Intent == Intent
                        && !GameManagement.assets_holder[x - 1, y - 1].GetComponent<Agent_behavour1>().isBusyGathering)
                    {
                        //  Debug.Log(GameManagement.assets_holder[x - 1, y - 1].GetComponent<Agent_behavour1>().Intent.ToString() + " " + Intent.ToString());
                        if (Priority < GameManagement.assets_holder[x - 1, y - 1].GetComponent<Agent_behavour1>().Priority)
                            priority_highest = false;
                        thereIsConflict = true;
                    }

        if (RightIndex(x - 1, y + 1))
            if (GameManagement.assets_holder[x - 1, y + 1] != null)
                if (GameManagement.assets_holder[x - 1, y + 1].tag == "R_Agent" || GameManagement.assets_holder[x - 1, y + 1].tag == "B_Agent")
                    if (GameManagement.assets_holder[x - 1, y + 1].GetComponent<Agent_behavour1>().Intent == Intent
                        && !GameManagement.assets_holder[x - 1, y + 1].GetComponent<Agent_behavour1>().isBusyGathering)
                    {
                        // Debug.Log(GameManagement.assets_holder[x - 1, y + 1].GetComponent<Agent_behavour1>().Intent.ToString() + " " + Intent.ToString());
                        if (Priority < GameManagement.assets_holder[x - 1, y + 1].GetComponent<Agent_behavour1>().Priority)
                            priority_highest = false;
                        thereIsConflict = true;
                    }

        if (RightIndex(x, y- 2))
            if (GameManagement.assets_holder[x, y - 2] != null)
                if (GameManagement.assets_holder[x, y - 2].tag == "R_Agent" || GameManagement.assets_holder[x, y - 2].tag == "B_Agent")
                    if (GameManagement.assets_holder[x, y - 2].GetComponent<Agent_behavour1>().Intent == Intent
                        && !GameManagement.assets_holder[x, y - 2].GetComponent<Agent_behavour1>().isBusyGathering)
                    {
                        //   Debug.Log(GameManagement.assets_holder[x, y - 2].GetComponent<Agent_behavour1>().Intent.ToString() + " " + Intent.ToString());
                        if (Priority < GameManagement.assets_holder[x, y - 2].GetComponent<Agent_behavour1>().Priority)
                            priority_highest = false;
                        thereIsConflict = true;
                    }


        if (RightIndex(x, y + 2))
            if (GameManagement.assets_holder[x, y + 2] != null)
                if (GameManagement.assets_holder[x, y + 2].tag == "R_Agent" || GameManagement.assets_holder[x, y + 2].tag == "B_Agent")
                    if (GameManagement.assets_holder[x, y + 2].GetComponent<Agent_behavour1>().Intent == Intent
                        && !GameManagement.assets_holder[x, y + 2].GetComponent<Agent_behavour1>().isBusyGathering)
                    {
                        //   Debug.Log(GameManagement.assets_holder[x, y + 2].GetComponent<Agent_behavour1>().Intent.ToString() + " " + Intent.ToString());
                        if (Priority < GameManagement.assets_holder[x, y + 2].GetComponent<Agent_behavour1>().Priority)
                            priority_highest = false;
                        thereIsConflict = true;
                    }

        if (RightIndex(x + 1, y - 1))
            if (GameManagement.assets_holder[x + 1, y - 1] != null)
                if (GameManagement.assets_holder[x + 1, y - 1].tag == "R_Agent" || GameManagement.assets_holder[x + 1, y - 1].tag == "B_Agent")
                    if (GameManagement.assets_holder[x + 1, y - 1].GetComponent<Agent_behavour1>().Intent == Intent
                        && !GameManagement.assets_holder[x + 1, y - 1].GetComponent<Agent_behavour1>().isBusyGathering)
                    {
                        //   Debug.Log(GameManagement.assets_holder[x + 1, y - 1].GetComponent<Agent_behavour1>().Intent.ToString() + " " + Intent.ToString());
                        if (Priority < GameManagement.assets_holder[x + 1, y - 1].GetComponent<Agent_behavour1>().Priority)
                            priority_highest = false;
                        thereIsConflict = true;
                    }

        if (RightIndex(x + 1, y + 1))
            if (GameManagement.assets_holder[x + 1, y + 1] != null)
                if (GameManagement.assets_holder[x + 1, y + 1].tag == "R_Agent" || GameManagement.assets_holder[x + 1, y + 1].tag == "B_Agent")
                    if (GameManagement.assets_holder[x + 1, y + 1].GetComponent<Agent_behavour1>().Intent == Intent
                        && !GameManagement.assets_holder[x + 1, y + 1].GetComponent<Agent_behavour1>().isBusyGathering)
                    {
                        //   Debug.Log(GameManagement.assets_holder[x + 1, y + 1].GetComponent<Agent_behavour1>().Intent.ToString() + " " + Intent.ToString());
                        if (Priority < GameManagement.assets_holder[x + 1, y + 1].GetComponent<Agent_behavour1>().Priority)
                            priority_highest = false;
                        thereIsConflict = true;
                    }

        if (RightIndex(x + 2, y))
            if (GameManagement.assets_holder[x + 2, y] != null)
                if (GameManagement.assets_holder[x + 2, y].tag == "R_Agent" || GameManagement.assets_holder[x + 2, y].tag == "B_Agent")
                    if (GameManagement.assets_holder[x + 2, y].GetComponent<Agent_behavour1>().Intent == Intent
                        && !GameManagement.assets_holder[x + 2, y].GetComponent<Agent_behavour1>().isBusyGathering)
                    {
                        //    Debug.Log(GameManagement.assets_holder[x + 2, y].GetComponent<Agent_behavour1>().Intent.ToString() + " " + Intent.ToString());
                        if (Priority < GameManagement.assets_holder[x + 2, y].GetComponent<Agent_behavour1>().Priority)
                            priority_highest = false;
                        thereIsConflict = true;
                    }
       
    }
*/

    public void CheckForConflict(int x, int y)
    {
        thereIsConflict = false;
        priority_highest = true;
        if (CheckConflictAdjecent(x - 2, y, ref priority_highest))
            thereIsConflict = true;

        if (CheckConflictAdjecent(x - 1, y - 1, ref priority_highest))
            thereIsConflict = true;

        if (CheckConflictAdjecent(x - 1, y + 1, ref priority_highest))
            thereIsConflict = true;

        if (CheckConflictAdjecent(x, y - 2, ref priority_highest))
            thereIsConflict = true;

        if (CheckConflictAdjecent(x, y + 2, ref priority_highest))
            thereIsConflict = true;

        if (CheckConflictAdjecent(x + 1, y - 1, ref priority_highest))
            thereIsConflict = true;

        if (CheckConflictAdjecent(x + 1, y + 1, ref priority_highest))
            thereIsConflict = true;

        if (CheckConflictAdjecent(x + 2, y, ref priority_highest))
            thereIsConflict = true;
    }

    public bool CheckConflictAdjecent(int x,int y, ref bool priorityH)
    {
        if (RightIndex(x , y))
            if (GameManagement.assets_holder[x, y] != null)
                if (GameManagement.assets_holder[x, y].tag == "R_Agent" || GameManagement.assets_holder[x, y].tag == "B_Agent")
                    if (GameManagement.assets_holder[x , y].GetComponent<Agent_behavour1>().Intent == Intent
                        && !GameManagement.assets_holder[x, y].GetComponent<Agent_behavour1>().isBusyGathering)
                    {
                        //    Debug.Log(GameManagement.assets_holder[x + 2, y].GetComponent<Agent_behavour1>().Intent.ToString() + " " + Intent.ToString());
                        if (Priority < GameManagement.assets_holder[x, y].GetComponent<Agent_behavour1>().Priority)
                            priorityH= false;
                        return true;
                    }
        return false;
    }

    //the actual moving transition
    public void Move()
    {
        if (dir == Directions.Forward || dir == Directions.BackWard)
        {
            if (Mathf.Abs(startpoz.z - transform.position.z) < 1)
                transform.position += dirOffsetToMove;
            else
            {
                ismoving = false;
                if (!foundPath)
                    ValidDir = false;
                gatheredFromNeighborus = new bool[4];
                resources = new bool[4];
                gatheringIndex = 0;
                checkedIfThereAreResources = false;
                thereAreResources = false;
                checkedForConflict = false;
                thereIsConflict = false;
                priority_highest = false;
                Priority = -1;
                GameManagement.assets_holder[Intent.x, Intent.y] = gameObject;
                GameManagement.assets_holder[currentLocation.x, currentLocation.y] = null;
               // GameManagement.grid[Intent.x, Intent.y].GetComponent<Renderer>().material = mat;
                currentLocation = new Point(Intent.x, Intent.y);
                rolledTheDice = false;
                //  Debug.Log("Linie Noua!"+timestep);
                timestep++;
                if (foundPath)
                    pathIndex++;
                if (foundPath)
                    Debug.Log(currentLocation.ToString());
                if(InventoryIsFull)
                {
                    startpoz = transform.position;
                    if (!startedCoroutineForPath)
                    {
                        StartCoroutine(FindPathTillFound());
                        startedCoroutineForPath = true;
                    }
                }
                //  Time.timeScale = 0;
            }
               
        }

        if (dir == Directions.Left || dir == Directions.Right)
        {
            if (Mathf.Abs(startpoz.x - transform.position.x) < 1)
                transform.position += dirOffsetToMove;
            else
            {
                ismoving = false;
                if(!foundPath)
                    ValidDir = false;
                gatheredFromNeighborus = new bool[4];
                resources = new bool[4];
                gatheringIndex = 0;
                checkedIfThereAreResources = false;
                thereAreResources = false;
                checkedForConflict = false;
                thereIsConflict = false;
                priority_highest = false;
                Priority = -1;
                rolledTheDice = false;
                GameManagement.assets_holder[Intent.x, Intent.y] = gameObject;
                GameManagement.assets_holder[currentLocation.x, currentLocation.y] = null;
                //GameManagement.grid[Intent.x, Intent.y].GetComponent<Renderer>().material = mat;
                currentLocation = new Point(Intent.x, Intent.y);
               // Debug.Log("Linie Noua! "+timestep);
              //  Time.timeScale = 0;
                timestep++;
                if (foundPath)
                    pathIndex++;
                if (foundPath)
                    Debug.Log(currentLocation.ToString());
                if (InventoryIsFull)
                {
                    startpoz = transform.position;
                    if (!startedCoroutineForPath)
                    {
                        StartCoroutine(FindPathTillFound());
                        startedCoroutineForPath = true;
                    }
                }
            }
        }

    }

    //checks if we're out of bounds or not!
    public bool RightIndex(int x, int y)
    {
        if (x >= GameManagement.assets_holder.GetLength(0) || x < 0)
            return false;
        if (y >= GameManagement.assets_holder.GetLength(1) || y < 0)
            return false;

        return true;
    }

    //checks wether or not we went through all the resources
    public bool HasCheckTheResources()
    {
        for (int i = 0; i < gatheredFromNeighborus.Length; i++)
            if (!gatheredFromNeighborus[i])
                return false;
        return true;
    }


    //path finding
    public void PathToHouse(int x, int y, int dx, int dy)
    {

       // Debug.Log("Alo?");
        if (x == dx && y == dy)
            foundPath = true;

        if (!foundPath)
        {
            if (System.Math.Abs(x - dx) <= System.Math.Abs(y - dy))
            {
                if (x < dx && y < dy)
                    PathToCollect(x + 1, y, x, y + 1, x, y - 1, x - 1, y, dx, dy);
                if (x < dx && y > dy)
                    PathToCollect(x + 1, y, x, y - 1, x, y + 1, x - 1, y, dx, dy);
                if (x > dx && y < dy)
                    PathToCollect(x - 1, y, x, y + 1, x, y - 1, x + 1, y, dx, dy);
                if (x > dx && y > dy)
                    PathToCollect(x - 1, y, x, y - 1, x, y + 1, x + 1, y, dx, dy);

                if (x == dx && y < dy)
                    PathToCollect(x, y + 1, x - 1, y, x + 1, y, x, y - 1, dx, dy);
                if (x == dx && y > dy)
                    PathToCollect(x, y - 1, x - 1, y, x + 1, y, x, y + 1, dx, dy);


            }
            else
            {
                if (x <= dx && y <= dy)
                    PathToCollect(x, y + 1, x + 1, y, x - 1, y, x, y - 1, dx, dy);
                if (x <= dx && y > dy)
                    PathToCollect(x, y - 1, x + 1, y, x - 1, y, x, y + 1, dx, dy);
                if (x > dx && y <= dy)
                    PathToCollect(x, y + 1, x - 1, y, x + 1, y, x, y - 1, dx, dy);
                if (x > dx && y > dy)
                    PathToCollect(x, y - 1, x - 1, y, x + 1, y, x, y + 1, dx, dy);

                if (x < dx && y == dy)
                    PathToCollect(x + 1, y, x, y - 1, x, y + 1, x - 1, y, dx, dy);
                if (x > dx && y == dy)
                    PathToCollect(x - 1, y, x, y - 1, x, y + 1, x + 1, y, dx, dy);
            }
        }
    }

    public void PathToCollect(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4, int dx, int dy)
    {

        if (PathIsOpenInLocationXY(x1, y1))
            PathToHouse(x1, y1, dx, dy);
        else
             if (PathIsOpenInLocationXY(x2, y2))
            PathToHouse(x2, y2, dx, dy);
        else
                 if (PathIsOpenInLocationXY(x3, y3))
            PathToHouse(x3, y3, dx, dy);
        else
                     if (PathIsOpenInLocationXY(x4, y4))
            PathToHouse(x4, y4, dx, dy);
    }

    public bool PathIsOpenInLocationXY(int x, int y)
    {
        if (RightIndex(x,y) && !foundPath)
            if (GameManagement.assets_holder[x, y]!=null)
            {
                if ((GameManagement.assets_holder[x, y].tag == "R_Agent" || GameManagement.assets_holder[x, y].tag == "B_Agent") && !agentMap[x, y])
                {
                    agentMap[x, y] = true;
                    pathIndex++;
                    pathList.Add(new Point(x, y));
                   // Debug.Log("Prima " + pathIndex);
                    return true;
                }
                return false;
            }
            else
            {
                if (!agentMap[x,y])
                {
                    agentMap[x, y] = true;
                    pathIndex++;
                    pathList.Add(new Point(x, y));
                   // Debug.Log("Doua " + pathIndex);
                    return true;
                }

                return false;
            }

        return false;
    }

    public void CheckIfHasMove(int dx, int dy)
    {
        Point t = pathList[pathIndex];
        if (RightIndex(t.x, t.y))
        {
            int moves = 0;
            if (CheckIFCanMoveThere(t.x + 1, t.y))
                moves++;
            if (CheckIFCanMoveThere(t.x - 1, t.y))
                moves++;
            if (CheckIFCanMoveThere(t.x, t.y + 1))
                moves++;
            if (CheckIFCanMoveThere(t.x, t.y - 1))
                moves++;
            if (moves <= 0)
            {
                agentMap[pathList[pathIndex].x, pathList[pathIndex].y] = false;
                pathList.RemoveAt(pathIndex);  
                pathIndex--;
               // Debug.Log("Treia " + pathIndex);
                if (pathIndex >= 0)
                    CheckIfHasMove(dx, dy);
            }
        }
    }

    public bool CheckIFCanMoveThere(int x, int y)
    {
        if (RightIndex(x, y))
        {
            if (GameManagement.assets_holder[x, y] != null)
            {
                if ((GameManagement.assets_holder[x, y].tag == "R_Agent" || GameManagement.assets_holder[x, y].tag == "B_Agent") && !agentMap[x, y])
                    return true;

                return false;
            }
            else
            {
                if (!agentMap[x, y])
                    return true;
            }
            
            return false;
        }          
           
        return false;
    }

    public void GetDirectionFromLastPostion(int currentX,int currentY,int nextX, int NextY)
    {
        if (currentX > nextX)
        {
            dir = Directions.BackWard;
            dirOffsetToMove = new Vector3(0, 0, -0.33f * Time.deltaTime); 
        }
        else
            if(currentX<nextX)
        {
            dir = Directions.Forward;
            dirOffsetToMove = new Vector3(0, 0, 0.33f * Time.deltaTime);
        }
        else
            if(currentY>NextY)
        {
            dir = Directions.Left;
            dirOffsetToMove = new Vector3(-0.33f * Time.deltaTime, 0, 0);
        }
        else
            if(currentY<NextY)
        {
            dir = Directions.Right;
            dirOffsetToMove = new Vector3(0.33f * Time.deltaTime, 0, 0);

        }
       // Debug.Log("Am fost aici in get Direction!");
        
    }

    public IEnumerator FindPathTillFound()
    {

        Point t = new Point(currentLocation.x, currentLocation.y);
        Debug.Log("starting position:" + t.ToString());
        PathToHouse(t.x, t.y, droppingPoint.x, droppingPoint.y);
      //  CheckIfHasMove(droppingPoint.x, droppingPoint.y);
         do
         {
             PathToHouse(pathList[pathIndex].x,pathList[pathIndex].y,droppingPoint.x,droppingPoint.y);
             CheckIfHasMove(droppingPoint.x, droppingPoint.y);
         } while (!foundPath);
        //foundPath = true;
        pathIndex = 0;
        for(int i=0;i<pathList.Count;i++)
            GameManagement.grid[pathList[i].x, pathList[i].y].GetComponent<Renderer>().material = mat;
        yield return new WaitUntil(()=>foundPath==true);
        Debug.Log(currentLocation.ToString()+" When finished!");
    }

    public void GiveResourcesToBase()
    {

        GameManagement.RedHouseWoodDeposit += 3;
        GameManagement.RedHouseOreDeposit += 3;
        GameManagement.RedHouseClayDeposit += 3;
        inventory.EmptyResources();

        GameManagement.RedHouseText1.GetComponent<Text>().text = "" + GameManagement.RedHouseWoodDeposit + " " + GameManagement.RedHouseOreDeposit + " " +
            GameManagement.RedHouseClayDeposit;
        GameManagement.RedHouseText2.GetComponent<Text>().text = "" + GameManagement.RedHouseWoodDeposit + " " + GameManagement.RedHouseOreDeposit + " " +
            GameManagement.RedHouseClayDeposit;

        for (int i = 0; i < pathList.Count; i++)
            GameManagement.grid[pathList[i].x, pathList[i].y].GetComponent<Renderer>().material = matDefault;
        pathList.Clear();
        pathIndex = -1;
        InventoryIsFull = false;
        foundPath = false;
        ValidDir = false;
        checkedIfThereAreResources = false;
        PossibleMoves = new bool[4];
        gatheredFromNeighborus = new bool[4];
        startedCoroutineForPath = false;
        for (int i = 0; i < agentMap.GetLength(0); i++)
            for (int j = 0; j < agentMap.GetLength(1); j++)
                agentMap[i, j] = false;
        resources = new bool[4];
    }
}

public class Point
{
    public int x;
    public int y;


    public Point()
    {

    }

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return x + " " + y;
    }

    public static bool operator == (Point p1, Point p2)
    {
        if (p1.x == p2.x && p1.y == p2.y)
            return true;
        return false;
    }

    public static bool operator !=(Point p1, Point p2)
    {
        if (p1.x != p2.x && p1.y != p2.y)
            return true;
        return false;
    }


}

public class Inventory
{
    public int wood_resource;
    public int ore_resource;
    public int clay_resource;

    public Inventory()
    {
        wood_resource = 0;
        ore_resource = 0;
        clay_resource = 0;
    }

    public void AddReource(Resource res)
    {
        switch (res)
        {
            case Resource.Wood:
                if (wood_resource < 3)
                    wood_resource++;
                break;
            case Resource.Ore:
                if (ore_resource < 3)
                    ore_resource++;
                break;
            case Resource.Clay:
                if (clay_resource < 3)
                    clay_resource++;
                break;

        }

    }

    public bool ResourceFull(Resource res)
    {
        switch (res)
        {
            case Resource.Wood:
                if (wood_resource == 3)
                    return true;
                return false;
            case Resource.Ore:
                if (ore_resource == 3)
                    return true;
                return false;
            case Resource.Clay:
                if (clay_resource == 3)
                    return true;
                return false;

            default: return false;
        }
    }

    public bool InventoryIsFull()
    {
        if (clay_resource == 3 && wood_resource == 3 && ore_resource == 3)
            return true;
        return false;
    }

    public void EmptyResources()
    {
        wood_resource = 0;
        clay_resource = 0;
        ore_resource = 0;
    }
}
