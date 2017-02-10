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



        return true;
    }

    void AssignPath()
    {

    }
}
