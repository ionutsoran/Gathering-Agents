using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;



public class AgentBehaviour : MonoBehaviour {

    // Use this for initialization
    Vector3 startdist;
    bool ismoving = false;
    bool startmove = false;
    bool finishedmoving = false;
    //bool checkedresource = false;
    bool isgathering = false;
    bool pickedirection = false;
    bool rightdirection = false;

    bool hastowait = false;
    bool hastochangedir = false;
    bool hastogather = false;
    bool waitingforRequest = false; //tell game manager my intension and tell me what to do

    bool[] checked_neighbours;
    int chk_nei_count = 0;
    Directions dir;
    public int agentpoz_i;
    public int agentpoz_j;

    public int Id;
    public string Team;
    public Inventory inventory;
    public Point intent;

    void Start()
    {
        inventory = new Inventory();
        startmove = true;
        transform.GetChild(0).GetComponent<Text>().text = Team + (Id+1);
        transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = Team + (Id+1);
        checked_neighbours = new bool[4];
    }


	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (!rightdirection)
        {
            startdist = transform.position;
            dir = (Directions)Random.Range(1, 4.99f);
            switch (dir)
            {
                case Directions.Forward:
                    if (RightIndex(agentpoz_i + 1, agentpoz_j))
                        if (GameManagement.assets_holder[agentpoz_i + 1, agentpoz_j] == null)
                            rightdirection = true;
                    break;

                case Directions.BackWard:
                    if (RightIndex(agentpoz_i - 1, agentpoz_j))
                        if (GameManagement.assets_holder[agentpoz_i - 1, agentpoz_j] == null)
                            rightdirection = true;
                    break;

                case Directions.Left:
                    if (RightIndex(agentpoz_i, agentpoz_j - 1))
                        if (GameManagement.assets_holder[agentpoz_i, agentpoz_j - 1] == null)
                            rightdirection = true;
                    break;

                case Directions.Right:
                    if (RightIndex(agentpoz_i, agentpoz_j + 1))
                        if (GameManagement.assets_holder[agentpoz_i, agentpoz_j + 1] == null)
                            rightdirection = true;
                    break;
            }
        }

        if (CheckedResource())
        {
            hastogather = false;
            Debug.Log("Cand e Adervarat?"+CheckedResource().ToString());
            if (rightdirection)
                ismoving = true;
        }
        else
        {
            hastogather = true;
            if(isgathering==false)
                Go_through_all_neighbours_with_resource(agentpoz_i, agentpoz_j);
        }
            

        #region moving transition
        if (ismoving)
        {
            
            switch(dir)
            {
                case Directions.Forward:
                    if (Mathf.Abs(startdist.z - transform.position.z) < 1)
                        transform.position += new Vector3(0, 0, 0.33f * Time.deltaTime);
                    else
                        finishedmoving = true;
                    break;
                case Directions.BackWard:
                    if (Mathf.Abs(startdist.z - transform.position.z) < 1)
                        transform.position += new Vector3(0, 0, -0.33f * Time.deltaTime);
                    else
                        finishedmoving = true;
                    break;
                case Directions.Left:
                    if (Mathf.Abs(startdist.x - transform.position.x) < 1)
                        transform.position += new Vector3(-0.33f * Time.deltaTime, 0, 0);
                    else
                        finishedmoving = true;
                        break;
                case Directions.Right:
                    if (Mathf.Abs(startdist.x - transform.position.x) < 1)
                        transform.position += new Vector3(0.33f * Time.deltaTime, 0, 0);
                    else
                        finishedmoving = true;
                    break;
            }
            

        }
        #endregion

        #region updating the new position
        if (finishedmoving)
        {
            finishedmoving = false;
            switch (dir)
            {
                case Directions.Forward:
                    GameManagement.assets_holder[agentpoz_i + 1, agentpoz_j] = gameObject;
                    GameManagement.assets_holder[agentpoz_i, agentpoz_j] = null;
                    agentpoz_i++;
                    break;

                case Directions.BackWard:
                    GameManagement.assets_holder[agentpoz_i - 1, agentpoz_j] = gameObject;
                    GameManagement.assets_holder[agentpoz_i, agentpoz_j] = null;
                    agentpoz_i--;
                    break;

                case Directions.Left:
                    GameManagement.assets_holder[agentpoz_i, agentpoz_j - 1] = gameObject;
                    GameManagement.assets_holder[agentpoz_i, agentpoz_j] = null;
                    agentpoz_j--;
                    break;

                case Directions.Right:
                    GameManagement.assets_holder[agentpoz_i, agentpoz_j + 1] = gameObject;
                    GameManagement.assets_holder[agentpoz_i, agentpoz_j] = null;
                    agentpoz_j++;
                    break;
            }
            //startmove = true;
            Reset_neighbour_checkers();
            ismoving = false;
            rightdirection = false;
        }
        #endregion

        //  transform.position += new Vector3(0.33f * Time.deltaTime, 0, 0);
    }

    public bool RightIndex(int x, int y)
    {
        if ((x >= 0 && x < GameManagement.assets_holder.GetLength(0)) && (y >= 0 && y < GameManagement.assets_holder.GetLength(1)))
        {
           // Debug.Log(x + " " + GameManagement.assets_holder.GetLength(0) + "  :  " + y + " " + GameManagement.assets_holder.GetLength(1) + " true");
            return true;
        }
       // Debug.Log(x + " " + GameManagement.assets_holder.GetLength(0) + "  :  " + y + " " + GameManagement.assets_holder.GetLength(1) + " false");
        return false;
    }

    public void GatherResource(int x, int y)
    {
        if(RightIndex(x,y)&& GameManagement.assets_holder[x, y]!=null)
            switch(GameManagement.assets_holder[x  ,y].tag)
            {
                case "Tree": inventory.AddReource(Resource.Wood); break;
                case "Ore": inventory.AddReource(Resource.Wood); break;
                case "Clay": inventory.AddReource(Resource.Wood); break;
                default:break;
            }
    }

    public bool CheckResource(int x, int y)
    {
        if(RightIndex(x,y)&& GameManagement.assets_holder[x, y] != null)
            switch (GameManagement.assets_holder[x, y].tag)
            {
                case "Tree": return true;
                case "Ore": return true;
                case "Clay": return true;
                default:return false;
            }
        return false;
    }

    public bool CheckedResource()
    {
        for (int i = 0; i < checked_neighbours.Length; i++)
            if (checked_neighbours[i]==false)
                return false;
        return true;
    }

    public void Reset_neighbour_checkers()
    {
        for (int i = 0; i < checked_neighbours.Length; i++)
            checked_neighbours[i] = false;
        chk_nei_count = 0;
    }

    public void Go_through_all_neighbours_with_resource(int x, int y)
    {
        if (chk_nei_count == 0)
        { 
            if (CheckResource(x + 1, y))
            {

                isgathering = true;
                StartCoroutine(GatheringTime(3.0f, x + 1, y));
            }
            else
            {
                //Debug.Log("Incep sa verific 1-ul vecin!");
                checked_neighbours[chk_nei_count] = true;
                chk_nei_count++;
            }
        }
           
        else 
        if (chk_nei_count == 1)
        {
           
            if (CheckResource(x - 1, y))
            {

                isgathering = true;
                StartCoroutine(GatheringTime(3.0f, x - 1, y));
            }
            else
            {
                checked_neighbours[chk_nei_count] = true;
                chk_nei_count++;
               
            }
        }
           
        else
        if (chk_nei_count == 2)
        {
            
            if (CheckResource(x, y - 1))
            {

                isgathering = true;
                StartCoroutine(GatheringTime(3.0f, x, y - 1));
            }
            else
            {
                checked_neighbours[chk_nei_count] = true;
                chk_nei_count++;
                
            }
        }
            
        else
        {
            if (chk_nei_count == 3)
            {

                if (CheckResource(x, y + 1))
                {
                    isgathering = true;
                    StartCoroutine(GatheringTime(3.0f, x, y + 1));
                }
                else
                {
                    checked_neighbours[chk_nei_count] = true;
                    chk_nei_count++;

                }
            }
        }
        
            
    }

    public IEnumerator GatheringTime(float sec,int x, int y)
    {
        yield return new WaitForSeconds(sec);
        GatherResource(x, y);
        DestroyImmediate(GameManagement.assets_holder[x, y]);
        Debug.Log("Am distrus chesita de [" + x + " ," + y + "]");
        GameManagement.assets_holder[x, y] = null;
        isgathering = false;
        checked_neighbours[chk_nei_count] = true;
        chk_nei_count++;
    }

    public bool CheckNeigbourAgents(int x, int y)
    {
        if (RightIndex(x + 1, y))
            if (GameManagement.assets_holder[x + 1, y].tag == "R_agent")
                return true;
        
        if (RightIndex(x + 1, y + 1))
            if (GameManagement.assets_holder[x + 1, y + 1].tag == "R_agent")
                return true;

        if (RightIndex(x + 1, y - 1))
            if (GameManagement.assets_holder[x + 1, y - 1].tag == "R_agent")
                return true;


        if (RightIndex(x, y + 1))
            if (GameManagement.assets_holder[x, y + 1].tag == "R_agent")
                return true;

        if (RightIndex(x, y - 1))
            if (GameManagement.assets_holder[x, y - 1].tag == "R_agent")
                return true;


        if (RightIndex(x - 1, y))
            if (GameManagement.assets_holder[x -1, y].tag == "R_agent")
                return true;

        if (RightIndex(x - 1, y - 1))
            if (GameManagement.assets_holder[x - 1, y - 1].tag == "R_agent")
                return true;

        if (RightIndex(x - 1, y + 1))
            if (GameManagement.assets_holder[x - 1, y + 1].tag == "R_agent")
                return true;

        return false;

    }
}

