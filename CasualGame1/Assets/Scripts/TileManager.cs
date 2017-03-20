using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathHeap;
using PathSortedList;


public class TileManager : MonoBehaviour
{
    //the path each enemy will follow
    public List<Vector2> enemyPath;
    //map dimensions
    public int x = 10;
    public int y = 10;
    
    //data on if a tile is occupied
    public bool[,] mapData;
    //data on the contents and modular capacity of each tile
    public Tile[,] tileTowers;
    //location of the enemy spawn in mapData coordinates
    private Vector2 spawnLocation;
    //location of the player base in mapData coordinates
    private Vector2 baseLocation;

    public GameObject pathPrefab;

    public GameObject ground;
    public GameObject enemySpawn;
    public GameObject playerBase;

    //file path of the map data file
    public string mapFileName = "Assets\\MapData\\level1.txt";

    // Use this for initialization
    void Start ()
    {
        //read in the map data file and grab info
        ReadInFileData();

        tileTowers = new Tile[x, y];

        for (int i = 0; i < x; i++)
        {
            for (int i2 = 0; i2 < y; i2++)
            {
                tileTowers[i, i2] = new Tile(3);
            }
        }

        //adjust the map in accordance with the new mapData
        AdjustMap();

        //initialize the enemy path
        enemyPath = new List<Vector2>();

        //creates the path the enemy will use
        CreatePath(true);
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void CreateFakePathIndicator(Vector2[,] path)
    {
        foreach (Transform child in transform.GetChild(1).GetComponentInChildren<Transform>())
        {
            Destroy(child.gameObject);
        }

        List<Vector2> fakePath = new List<Vector2>();
        //set the current location to start at the player base
        Vector2 currentLocation = baseLocation;

        //while we do not run into the end signal, loop
        while (currentLocation != new Vector2(-1, -1))
        {
            //insert the new location into the beginning of the enemyPath
            //this is because we are moving from the end of the path to the beginning of the path and this will reverse it
            fakePath.Insert(0, new Vector2((currentLocation.x - 5) * 10 + 5, (currentLocation.y - 5) * -10 - 5));

            //grab the next location
            currentLocation = path[(int)currentLocation.x, (int)currentLocation.y];
        }

        for (int i = 1; i < fakePath.Count; i++)
        {
            if (fakePath[i - 1].x < fakePath[i].x)
            {
                Quaternion rotation = Quaternion.Euler(90, 90, 0);
                GameObject point = Instantiate(pathPrefab, new Vector3(fakePath[i].x - 5, .5f, fakePath[i].y), rotation, transform.GetChild(1));
                point.GetComponent<SpriteRenderer>().color = new Color(130 / 255f, 130 / 255f, 130 / 255f, 130 / 255f);
            }
            else if (fakePath[i - 1].x > fakePath[i].x)
            {
                Quaternion rotation = Quaternion.Euler(90, 90, 0);
                GameObject point = Instantiate(pathPrefab, new Vector3(fakePath[i].x + 5, .5f, fakePath[i].y), rotation, transform.GetChild(1));
                point.GetComponent<SpriteRenderer>().color = new Color(130 / 255f, 130 / 255f, 130 / 255f, 130 / 255f);
            }
            else if (fakePath[i - 1].y < fakePath[i].y)
            {
                Quaternion rotation = Quaternion.Euler(90, 0, 0);
                GameObject point = Instantiate(pathPrefab, new Vector3(fakePath[i].x, .5f, fakePath[i].y - 5), rotation, transform.GetChild(1));
                point.GetComponent<SpriteRenderer>().color = new Color(130 / 255f, 130 / 255f, 130 / 255f, 130 / 255f);
            }
            else
            {
                Quaternion rotation = Quaternion.Euler(90, 0, 0);
                GameObject point = Instantiate(pathPrefab, new Vector3(fakePath[i].x, .5f, fakePath[i].y + 5), rotation, transform.GetChild(1));
                point.GetComponent<SpriteRenderer>().color = new Color(130 / 255f, 130 / 255f, 130 / 255f, 130 / 255f);
            }
        }
    }
    public void CreatePathIndicator()
    {
        foreach(Transform child in transform.GetChild(0).GetComponentInChildren<Transform>())
        {
            Destroy(child.gameObject);
        }

        for (int i = 1; i < enemyPath.Count; i++)
        {
            if (enemyPath[i-1].x < enemyPath[i].x)
            {
                Quaternion rotation = Quaternion.Euler(90, 90, 0);
                Instantiate(pathPrefab, new Vector3(enemyPath[i].x - 5, 1, enemyPath[i].y), rotation, transform.GetChild(0));
            }
            else if (enemyPath[i - 1].x > enemyPath[i].x)
            {
                Quaternion rotation = Quaternion.Euler(90, 90, 0);
                Instantiate(pathPrefab, new Vector3(enemyPath[i].x + 5, 1, enemyPath[i].y), rotation, transform.GetChild(0));
            }
            else if (enemyPath[i - 1].y < enemyPath[i].y)
            {
                Quaternion rotation = Quaternion.Euler(90, 0, 0);
                Instantiate(pathPrefab, new Vector3(enemyPath[i].x, 1, enemyPath[i].y - 5), rotation, transform.GetChild(0));
            }
            else
            {
                Quaternion rotation = Quaternion.Euler(90, 0, 0);
                Instantiate(pathPrefab, new Vector3(enemyPath[i].x, 1, enemyPath[i].y + 5), rotation, transform.GetChild(0));
            }
        }
    }

    //reset the map data
    public void Reset()
    {
        //set mapData to default value
        for (int i = 0; i < x; i++)
        {
            for (int i2 = 0; i2 < y; i2++)
            {
                tileTowers[i, i2] = new Tile(3);
                mapData[i, i2] = false;
            }
        }

        //creates the path each enemy will use
        CreatePath(true);

    }

    public bool CreatePath(bool assign)
    {
        Vector2[,] pathParent = CreatePathPoints();
        if(pathParent == null)
        {
            return false;
        }
        //populate the enemyPath vector with the new path
        if (assign)
        {
            AssignPath(pathParent);

            //create indicators to show the path to the player
            CreatePathIndicator();
        }
        return true;
    }

    //calculate the path from the enemy spawn to the player base
    public Vector2[,] CreatePathPoints()
    {
        //2D array of Vector2a to hold the data of the parent node location to find the path after calculation
        Vector2[,] pathParent = new Vector2[x,y];

        //2D array of bools to keep track of visited nodes
        bool[,] availableTiles = new bool[x,y];
        
        //copy the map data for manipulation
        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            {
                availableTiles[i, j] = mapData[i, j];
            }
        }

        //create the priority queue
        Heap prq = new Heap();

        //int to keep track of the heuristic value
        int heur = 0;

        //insert the spawn location, this will be the starting location
        prq.Insert(0, spawnLocation);
        //place a signal value in the pathParent array
        pathParent[(int)spawnLocation.x, (int)spawnLocation.y] = new Vector2(-1, -1);
        //signal that this location has been visited
        availableTiles[(int)spawnLocation.x, (int)spawnLocation.y] = true;

        //loop forever, will be broken inside the loop
        while (true)
        {
            //get the next location from the priority queue
            KeyValuePair<int, Vector2> currentTile = prq.Pop();

            //if the location gotten from the priority queue is the base, the path has been found
            if(currentTile.Value == baseLocation)
            {
                //break out of the loop
                break;
            }

            //check the tile to the left
            if (currentTile.Value.x - 1 >= 0)
            {
                //if the tile is available
                if (!availableTiles[(int)currentTile.Value.x - 1, (int)currentTile.Value.y])
                {
                    //calculate the heuristic to the player base
                    heur = (int)Vector2.Distance(new Vector2(currentTile.Value.x - 1, currentTile.Value.y), baseLocation);

                    //add the new tile to the heap
                    //TYLER : changed (+ 1 +) to (+ 19 +), to go for the y direction first
                    prq.Insert(currentTile.Key + 19 + heur, new Vector2(currentTile.Value.x - 1, currentTile.Value.y));

                    //add the location of he parent to the pathParent array
                    pathParent[(int)currentTile.Value.x - 1, (int)currentTile.Value.y] = currentTile.Value;

                    availableTiles[(int)currentTile.Value.x - 1, (int)currentTile.Value.y] = true;
                }
            }
            
            if(currentTile.Value.x + 1 < x)
            {
                //if the tile is available
                if (!availableTiles[(int)currentTile.Value.x + 1, (int)currentTile.Value.y])
                {
                    //calculate the heuristic to the player base
                    heur = (int)Vector2.Distance(new Vector2(currentTile.Value.x + 1, currentTile.Value.y), baseLocation);

                    //add the new tile to the heap
                    //TYLER : changed (+ 1 +) to (+ 19 +), to go for the y direction first
                    prq.Insert(currentTile.Key + 19 + heur, new Vector2(currentTile.Value.x + 1, currentTile.Value.y));

                    //add the location of he parent to the pathParent array
                    pathParent[(int)currentTile.Value.x + 1, (int)currentTile.Value.y] = currentTile.Value;

                    availableTiles[(int)currentTile.Value.x + 1, (int)currentTile.Value.y] = true;
                }
            }
            
            if(currentTile.Value.y - 1 >= 0)
            {
                //if the tile is available
                if (!availableTiles[(int)currentTile.Value.x, (int)currentTile.Value.y - 1])
                {
                    //calculate the heuristic to the player base
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
                //if the tile is available
                if (!availableTiles[(int)currentTile.Value.x, (int)currentTile.Value.y + 1])
                {
                    //calculate the heuristic to the player base
                    heur = (int)Vector2.Distance(new Vector2(currentTile.Value.x, currentTile.Value.y + 1), baseLocation);

                    //add the new tile to the heap
                    prq.Insert(currentTile.Key + 1 + heur, new Vector2(currentTile.Value.x, currentTile.Value.y + 1));

                    //add the location of he parent to the pathParent array
                    pathParent[(int)currentTile.Value.x, (int)currentTile.Value.y + 1] = currentTile.Value;

                    availableTiles[(int)currentTile.Value.x, (int)currentTile.Value.y + 1] = true;
                }
            }

            //if the priority queue is empty, then there is no possible path to travel
            if(prq.GetSize() == 0)
            {
                //return false
                return null;
            }
        }
        return pathParent;
    }

    //assign the new path to the enemyPath List
    void AssignPath(Vector2[,] pathParent)
    {
        //clear the current path
        enemyPath.Clear();

        //set the current location to start at the player base
        Vector2 currentLocation = baseLocation;

        //while we do not run into the end signal, loop
        while (currentLocation != new Vector2(-1, -1))
        {
            //insert the new location into the beginning of the enemyPath
            //this is because we are moving from the end of the path to the beginning of the path and this will reverse it
            enemyPath.Insert(0, new Vector2((currentLocation.x - 5) * 10 + 5, (currentLocation.y - 5) * -10 - 5));

            //grab the next location
            currentLocation = pathParent[(int)currentLocation.x,(int)currentLocation.y];
        }
    }

    //read in data from the current map data file
    void ReadInFileData()
    {
        //open the file
        StreamReader sr = File.OpenText(mapFileName);

        string line = "";

        //read size of the map, the first and second line
        line = sr.ReadLine();
        x = Int32.Parse(line);

        line = sr.ReadLine();
        y = Int32.Parse(line);

        //resize the map array
        mapData = new bool[x, y];

        //holder variables
        int tempX = 0;
        int tempY = 0;

        while ((line = sr.ReadLine()) != null)
        {
            string[] lineData = line.Split(' ');

            if(lineData[0] == "S")
            {
                tempX = Int32.Parse(lineData[1]);
                tempY = Int32.Parse(lineData[2]);

                mapData[tempX, tempY] = true;

                enemySpawn.transform.position = new Vector3((tempX - x / 2) * 10 + 5, 0.001f, (tempY - y / 2) * -10 - 5);

                spawnLocation = new Vector2(tempX, tempY);
            }
            if(lineData[0] == "B")
            {
                tempX = Int32.Parse(lineData[1]);
                tempY = Int32.Parse(lineData[2]);

                playerBase.transform.position = new Vector3((tempX - x / 2) * 10 + 5, 0, (tempY - y / 2) * -10 - 5);

                baseLocation = new Vector2(tempX, tempY);
            }
        }
        sr.Close();
    }

    //adjust the size of the map
    void AdjustMap()
    {
        ground.transform.localScale = new Vector3(x, 1, y);
    }
}

public class Tile
{
    public int capacity;
    public List<GameObject> contents;

    public Tile(int cap)
    {
        capacity = cap;
        contents = new List<GameObject>();
    }
	
	public float Height()
	{
		float height = 0;
		for(int i = 0; i < contents.Count; i++)
		{
			height += contents[i].transform.localScale.y;
		}
		return height;
	}
	
	public GameObject TopTower()
	{
		if(contents.Count > 0)
		{
			return contents[contents.Count - 1];
		}
		return null;
	}
}
