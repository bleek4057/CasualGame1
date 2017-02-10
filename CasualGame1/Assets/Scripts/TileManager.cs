using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathHeap;


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

    // Use this for initialization
    void Start ()
    {
        mapData = new bool[x, y];
        enemyPath = new List<Vector2>();
        spawnLocation = new Vector2(4, 0);
        baseLocation = new Vector2(7, 9);

        for (int i = 0; i < 10; i++)
        {
            if(i != 5)
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
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    bool CreatePath()
    {
        Vector2[,] pathParent = new Vector2[x,y];
        int[,] pathDist = new int[x, y];
        Heap prq = new Heap();

        prq.Insert(0, spawnLocation);
        pathParent[(int)spawnLocation.x, (int)spawnLocation.y] = new Vector2(-1, -1);

        while(true)
        {
            KeyValuePair<int, Vector2> currentTileData = prq.Pop();

            if(currentTileData.Value == baseLocation)
            {
                break;
            }


            //check that the tile to the left can be moved too
            if(currentTileData.Value.x - 1 >= 0 && !mapData[(int)currentTileData.Value.x - 1, (int)currentTileData.Value.y])
            {
                //add the new tile data to the heap
                prq.Insert(currentTileData.Key + 1, new Vector2(currentTileData.Value.x - 1, currentTileData.Value.y));

                //pathDist[(int)currentTileData.Value.x - 1, (int)currentTileData.Value.y] = currentTileData.Key + 1;

                //add the location of the parent of the path to the pathParent array
                pathParent[(int)currentTileData.Value.x - 1, (int)currentTileData.Value.y] = currentTileData.Value;
            }

            if (currentTileData.Value.x + 1 < x && !mapData[(int)currentTileData.Value.x + 1, (int)currentTileData.Value.y])
            {
                prq.Insert(currentTileData.Key + 1, new Vector2(currentTileData.Value.x + 1, currentTileData.Value.y));

                //add the location of the parent of the path to the pathParent array
                pathParent[(int)currentTileData.Value.x + 1, (int)currentTileData.Value.y] = currentTileData.Value;
            }

            if (currentTileData.Value.y - 1 >= 0 && !mapData[(int)currentTileData.Value.x, (int)currentTileData.Value.y - 1])
            {
                prq.Insert(currentTileData.Key + 1, new Vector2(currentTileData.Value.x, currentTileData.Value.y - 1));

                //add the location of the parent of the path to the pathParent array
                pathParent[(int)currentTileData.Value.x, (int)currentTileData.Value.y - 1] = currentTileData.Value;

            }
            if (currentTileData.Value.y + 1 < y && !mapData[(int)currentTileData.Value.x, (int)currentTileData.Value.y + 1])
            { 
                prq.Insert(currentTileData.Key + 1, new Vector2(currentTileData.Value.x, currentTileData.Value.y + 1));

                //add the location of the parent of the path to the pathParent array
                pathParent[(int)currentTileData.Value.x, (int)currentTileData.Value.y + 1] = currentTileData.Value;
            }

            if(prq.GetSize() == 0)
            {
                return false;
            }
        }

        AssignPath(pathParent);

        return true;
    }

    void AssignPath(Vector2[,] pathParent)
    {
        enemyPath.Clear();

        Vector2 currentLocation = pathParent[(int)baseLocation.x, (int)baseLocation.y];

        while(currentLocation != new Vector2(-1, -1))
        {

        }
    }
}
