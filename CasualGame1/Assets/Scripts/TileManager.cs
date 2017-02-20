using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathHeap;
using PathSortedList;


public class TileManager : MonoBehaviour
{
    //the path each enemy will follow
    public List<Vector2> enemyPath;
    private int x = 10;
    private int y = 11;
    
    //data on if a tile is occupied
    public bool[,] mapData;
    private Vector2 spawnLocation;
    private Vector2 baseLocation;

    public GameObject pathPrefab;

    // Use this for initialization
    void Start ()
    {
        mapData = new bool[x, y];
        enemyPath = new List<Vector2>();
        spawnLocation = new Vector2(4, 0);
        baseLocation = new Vector2(7, 9);

        for (int i = 0; i < 10; i++)
        {
            if(i != 4)
            {
                mapData[i, 0] = true;
            }
        }

        //Debug.Log(mapData);

        //creates the path the enemy will use
        CreatePath();
        
        
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void CreatePathIndicator()
    {
        foreach(Transform child in transform.GetComponentInChildren<Transform>())
        {
            Destroy(child.gameObject);
        }

        for (int i = 1; i < enemyPath.Count; i++)
        {
            if (enemyPath[i-1].x < enemyPath[i].x)
            {
                Quaternion rotation = Quaternion.Euler(90, 90, 0);
                Instantiate(pathPrefab, new Vector3(enemyPath[i].x - 5, 1, enemyPath[i].y), rotation, transform);
            }
            else if (enemyPath[i - 1].x > enemyPath[i].x)
            {
                Quaternion rotation = Quaternion.Euler(90, 90, 0);
                Instantiate(pathPrefab, new Vector3(enemyPath[i].x + 5, 1, enemyPath[i].y), rotation, transform);
            }
            else if (enemyPath[i - 1].y < enemyPath[i].y)
            {
                Quaternion rotation = Quaternion.Euler(90, 0, 0);
                Instantiate(pathPrefab, new Vector3(enemyPath[i].x, 1, enemyPath[i].y - 5), rotation, transform);
            }
            else
            {
                Quaternion rotation = Quaternion.Euler(90, 0, 0);
                Instantiate(pathPrefab, new Vector3(enemyPath[i].x, 1, enemyPath[i].y + 5), rotation, transform);
            }
        }
    }

    public void Reset()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int i2 = 0; i2 < 11; i2++)
            {
                mapData[i, i2] = false;
            }
            if (i != 4)
            {
                mapData[i, 0] = true;
            }
        }

        //creates the path each enemy will use
        if (CreatePath())
        {

        }
        else
        {
            enemyPath.Add(new Vector2(-5, 45));
            enemyPath.Add(new Vector2(-5, 35));
            enemyPath.Add(new Vector2(-5, 25));
            enemyPath.Add(new Vector2(-5, 15));
            enemyPath.Add(new Vector2(-5, 5));
            enemyPath.Add(new Vector2(-5, -5));
            enemyPath.Add(new Vector2(-5, -15));
            enemyPath.Add(new Vector2(-5, -25));
            enemyPath.Add(new Vector2(-5, -35));
            enemyPath.Add(new Vector2(5, -35));
            enemyPath.Add(new Vector2(15, -35));
            enemyPath.Add(new Vector2(25, -35));

            CreatePathIndicator();
        }
    }

    public bool CreatePath()
    {
        Vector2[,] pathParent = new Vector2[x,y];
        bool[,] availableTiles = new bool[x,y];
        
        //copy the map data for manipulation
        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            {
                availableTiles[i, j] = mapData[i, j];
                //Debug.Log(i + ", " + j + " " + availableTiles[i, j]);
            }
        }

        Heap prq = new Heap();

        int heur = 0;

        prq.Insert(0, spawnLocation);
        pathParent[(int)spawnLocation.x, (int)spawnLocation.y] = new Vector2(-1, -1);
        availableTiles[(int)spawnLocation.x, (int)spawnLocation.y] = true;

        while (true)
        {
            KeyValuePair<int, Vector2> currentTile = prq.Pop();

            if(currentTile.Value == baseLocation)
            {
                break;
            }

            //Debug.Log(currentTile);
            //Debug.Log(prq.GetSize());
            //Debug.Log(mapData[(int)currentTile.Value.x, (int)currentTile.Value.y]);

            //check the tile to the left
            if (currentTile.Value.x - 1 >= 0)
            {
                if (!availableTiles[(int)currentTile.Value.x - 1, (int)currentTile.Value.y])
                {
                    heur = (int)Vector2.Distance(new Vector2(currentTile.Value.x - 1, currentTile.Value.y), baseLocation);

                    //add the new tile to the heap
                    prq.Insert(currentTile.Key + 1 + heur, new Vector2(currentTile.Value.x - 1, currentTile.Value.y));

                    //add the location of he parent to the pathParent array
                    pathParent[(int)currentTile.Value.x - 1, (int)currentTile.Value.y] = currentTile.Value;

                    availableTiles[(int)currentTile.Value.x - 1, (int)currentTile.Value.y] = true;
                }
            }
            
            if(currentTile.Value.x + 1 < x)
            {
                if (!availableTiles[(int)currentTile.Value.x + 1, (int)currentTile.Value.y])
                {
                    heur = (int)Vector2.Distance(new Vector2(currentTile.Value.x + 1, currentTile.Value.y), baseLocation);

                    //add the new tile to the heap
                    prq.Insert(currentTile.Key + 1 + heur, new Vector2(currentTile.Value.x + 1, currentTile.Value.y));

                    //add the location of he parent to the pathParent array
                    pathParent[(int)currentTile.Value.x + 1, (int)currentTile.Value.y] = currentTile.Value;

                    availableTiles[(int)currentTile.Value.x + 1, (int)currentTile.Value.y] = true;
                }
            }
            
            if(currentTile.Value.y - 1 >= 0)
            {
                if (!availableTiles[(int)currentTile.Value.x, (int)currentTile.Value.y - 1])
                {
                    heur = (int)Vector2.Distance(new Vector2(currentTile.Value.x, currentTile.Value.y - 1), baseLocation);

                    //add the new tile to the heap
                    prq.Insert(currentTile.Key + 1 + heur, new Vector2(currentTile.Value.x, currentTile.Value.y - 1));

                    //add the location of he parent to the pathParent array
                    pathParent[(int)currentTile.Value.x, (int)currentTile.Value.y - 1] = currentTile.Value;

                    availableTiles[(int)currentTile.Value.x, (int)currentTile.Value.y - 1] = true;
                }
            }
            
            if(currentTile.Value.y + 1 < y)
            {
                if (!availableTiles[(int)currentTile.Value.x, (int)currentTile.Value.y + 1])
                {
                    heur = (int)Vector2.Distance(new Vector2(currentTile.Value.x, currentTile.Value.y + 1), baseLocation);

                    //add the new tile to the heap
                    prq.Insert(currentTile.Key + 1 + heur, new Vector2(currentTile.Value.x, currentTile.Value.y + 1));

                    //add the location of he parent to the pathParent array
                    pathParent[(int)currentTile.Value.x, (int)currentTile.Value.y + 1] = currentTile.Value;

                    availableTiles[(int)currentTile.Value.x, (int)currentTile.Value.y + 1] = true;
                }
            }
            

            

            if(prq.GetSize() == 0)
            {
                return false;
            }
        }

        //Debug.Log(pathParent);

        //return false;

        AssignPath(pathParent);

        CreatePathIndicator();
        return true;
    }

    void AssignPath(Vector2[,] pathParent)
    {
        enemyPath.Clear();

        Vector2 currentLocation = baseLocation;
        while (currentLocation != new Vector2(-1, -1))
        {
            //Debug.Log(currentLocation);

            enemyPath.Insert(0, new Vector2((currentLocation.x - 5) * 10 + 5, (currentLocation.y - 6) * -10 - 5));

            currentLocation = pathParent[(int)currentLocation.x,(int)currentLocation.y];
        }
    }
}
