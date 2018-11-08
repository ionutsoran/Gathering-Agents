using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum GridObjects {Clay, Ore, Wood, BedRock, AgentB, AgentR, Terrain, HouseB ,HouseA}

public class GameManagement : MonoBehaviour {


    public static GameObject[,] grid;
    public GameObject[] grid_assets;
    public static GameObject[,] assets_holder;
    public Camera[] cams;

    public Material mat;
    public GameObject main_camera;
    public float offset_cam = 5;
    public float offset=5;
    public int n = 3;
    public float cam_angle=30;
    public Texture2D map;
    public List<Color> color_taggs;
    public int cam_index = 1;
    public Font font1;
    public Font font2;

    public static int contor_red=0;
    public static int contor_blue=0;

    public static Point RedBaseLocation;
    public static Point BlueBaseLocation;

    public static GameObject RedHouseText1;
    public static GameObject RedHouseText2;
    public static int RedHouseWoodDeposit;
    public static int RedHouseOreDeposit;
    public static int RedHouseClayDeposit;

    //temporary
    public bool temporary = false;
    public static List<Agent_behavour1> agents;
    public bool[,] agentMap;
    public List<Point> pathList;
    public bool foundPath;
    public int pathIndex;
    public Material mat2;
    public Material mat3;
    public int countItems;

    void Start ()
    {
        agents = new List<Agent_behavour1>();
        pathList = new List<Point>();
        pathIndex = -1;
        Pre_process_grid_units(n);

        assets_holder = new GameObject[map.width,map.height];
        for (int i = 0; i < map.width; i++)
            for (int j = 0; j < map.height; j++)
            {
                Instantiate_prefs_on_map(i, j); 
                
            }

        agentMap = new bool[grid.GetLength(0), grid.GetLength(1)];
       /* PathToHouse(3, 3, 14, 14);
        do
        {
            PathToHouse(pathList[pathIndex].x, pathList[pathIndex].y, 14, 14);
            CheckIfHasMove(14, 14);

        } while (!foundPath);

        for (int i = 0; i < pathList.Count; i++)
            if (!(pathList[i].x== 14 && pathList[i].y == 14))
                grid[pathList[i].x, pathList[i].y].GetComponent<Renderer>().material = mat2;
        //for (int i = 0; i < pathList.Count; i++)
           // if (pathList[i].x == 14 && pathList[i].y == 14)
             //   grid[pathList[i].x, pathList[i].y].GetComponent<Renderer>().material = mat3;
*/
    }

    void Update()
    {
     /*   string s = "";
        for (int i = 0; i < agents.Count; i++)
            s += "(" + agents[i].Intent.x + ", " + agents[i].Intent.y+")";
        Debug.Log(s);*/
    }
    void Pre_process_grid_units(int n)
    {
        grid = new GameObject[n, n];
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Quad);
        temp.transform.position = new Vector3(0, 0, 0);
        Vector3 cc;

        //Generate my Grid
        for (int i=0;i<n;i++)
            for(int j=0;j<n;j++)
            {
                grid[i,j] = GameObject.CreatePrimitive(PrimitiveType.Quad);
                cc = temp.transform.position;
                grid[i,j].transform.rotation = Quaternion.Euler(90, 0, 0);
                grid[i, j].transform.position = new Vector3(j*offset+cc.x, 0, i*offset+cc.z); //nu mai trebe sa schimb
                grid[i, j].name = "Grid[" + j + ", " + i + "]";
                grid[i, j].GetComponent<Renderer>().material = mat;
                grid[i, j].transform.SetParent(transform.GetChild(0));
            }

        //destroy my temporary quad from where I start
        Destroy(temp);

        //set camera offset;
        cc = grid[n / 2, n / 2].transform.position;
        Arrange_cameras(cc);
        //main_camera.transform.position = new Vector3(cc.x, n, -(cc.z+offset_cam/2));
        //main_camera.transform.rotation = Quaternion.Euler(cam_angle, 0, 0);
    }

    public void Instantiate_prefs_on_map(int x, int y)
    {

        if (map.GetPixel(x, y) == color_taggs[0])
            assets_holder[x, y] = Instantiate(grid_assets[0], grid[x, y].transform.position, Quaternion.identity);
        else
        if (map.GetPixel(x, y) == color_taggs[1])
        {
            //if(!temporary)
            {
                assets_holder[x, y] = Instantiate(grid_assets[1], grid[x, y].transform.position, Quaternion.identity);
                assets_holder[x, y].GetComponent<Agent_behavour1>().currentLocation = new Point(x, y);
                assets_holder[x, y].GetComponent<Agent_behavour1>().Id = contor_red+1;
                assets_holder[x, y].GetComponent<Agent_behavour1>().Team = "Red";
                assets_holder[x, y].name = "Red" + (contor_red + 1);
                //assets_holder[x, y].GetComponent<Agent_behavour1>().enabled = false;
                Debug.Log(x + " " + y + "Red" + contor_red);
                contor_red++;
                temporary = true;
            }
            
        }
            
        else
        if (map.GetPixel(x, y) == color_taggs[2])
        {
            assets_holder[x, y] = Instantiate(grid_assets[2], grid[x, y].transform.position, Quaternion.identity);
   
            RedBaseLocation = new Point(x, y-1);
            Debug.Log(RedBaseLocation.ToString());
        }
         
        else
        if (map.GetPixel(x, y) == color_taggs[3])
        
            assets_holder[x, y] = Instantiate(grid_assets[3], grid[x, y].transform.position, Quaternion.identity);
        else
        if (map.GetPixel(x, y) == color_taggs[4])
            assets_holder[x, y] = Instantiate(grid_assets[4], grid[x, y].transform.position, Quaternion.identity);
        else
        if (map.GetPixel(x, y) == color_taggs[5])
            assets_holder[x, y] = Instantiate(grid_assets[5], grid[x, y].transform.position, Quaternion.identity);
        else
        if (map.GetPixel(x, y) == color_taggs[6])
        {
           /* assets_holder[x, y] = Instantiate(grid_assets[6], grid[x, y].transform.position, Quaternion.identity);
            assets_holder[x, y].GetComponent<Agent_behavour1>().currentLocation = new Point(x, y);
            assets_holder[x, y].GetComponent<Agent_behavour1>().Id = contor_blue;
            assets_holder[x, y].GetComponent<Agent_behavour1>().Team = "Blue";
            Debug.Log(x + " " + y + "Red" + contor_blue);
            contor_blue++;*/
        }
       // assets_holder[x, y] = Instantiate(grid_assets[6], grid[x, y].transform.position, Quaternion.identity);
        else
        if (map.GetPixel(x, y) == color_taggs[7])
        {
            assets_holder[x, y] = Instantiate(grid_assets[7], grid[x, y].transform.position, Quaternion.identity);
            BlueBaseLocation = new Point(x, y);
        }
          
        else
            assets_holder[x, y] = null;


        if (assets_holder[x, y] != null)
        {
            assets_holder[x, y].transform.localScale = new Vector3(0.5f, 1f, 0.5f);
            assets_holder[x, y].transform.SetParent(transform.GetChild(1));
            assets_holder[x, y].transform.position += new Vector3(0,0.5f,0);
        }


    }
    
    public void Arrange_cameras(Vector3 cc)//aranjele cele 4 camere
    {
        cams[0].transform.position = new Vector3(cc.x, n, -(cc.z + offset_cam / 2));
        cams[0].transform.rotation = Quaternion.Euler(cam_angle, 0, 0);

        cams[1].transform.position = new Vector3(cc.x, n, 2*(cc.z + offset_cam));
        //cams[1].transform.rotation = Quaternion.Euler(, 0, 0);
        cams[1].transform.localRotation= Quaternion.Euler(cam_angle, 180, 0);
    }


    public void PathToHouse(int x, int y, int dx, int dy)
    {

        // Debug.Log("Alo?");
        if (x == dx && y == dy)
        {
            grid[dx, dy].GetComponent<Renderer>().material = mat3;
            foundPath = true;
        }
           

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
        if (RightIndex(x, y) && !foundPath)
            if (GameManagement.assets_holder[x, y] != null)
            {
                if ((GameManagement.assets_holder[x, y].tag == "R_Agent" || GameManagement.assets_holder[x, y].tag == "B_Agent") && !agentMap[x, y])
                {
                    agentMap[x, y] = true;
                    pathIndex++;
                    pathList.Add(new Point(x, y));
                    Debug.Log("Prima oara cu + " + pathIndex);
                    countItems = pathList.Count;
                    return true;
                }
                return false;
            }
            else
            {
                if (!agentMap[x, y])
                {
                    agentMap[x, y] = true;
                    pathIndex++;
                    pathList.Add(new Point(x, y));
                    Debug.Log("A doua oara cu + "+pathIndex);
                    countItems = pathList.Count;
                    return true;
                }

                return false;
            }

        return false;
    }

    public void CheckIfHasMove(int dx, int dy)
    {
        Point t = pathList[pathList.Count-1];
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
                pathList.RemoveAt(pathIndex);
                countItems = pathList.Count;
                pathIndex--;
                Debug.Log("A treia oara cu - " + pathIndex);
                if (pathList.Count >= 0)
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

    public bool RightIndex(int x, int y)
    {
        if (x >= GameManagement.assets_holder.GetLength(0) || x < 0)
            return false;
        if (y >= GameManagement.assets_holder.GetLength(1) || y < 0)
            return false;

        return true;
    }

}
