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
    private Heap h;

    // Use this for initialization
    void Start ()
    {
        bool[,] mapData = new bool[x, y];

        for(int i = 0; i < 10; i++)
        {
            if(i != 5)
            {
                mapData[i, 0] = true;
            }
        }

        //creates the path each enemy will use
        enemyPath = new List<Vector2>();
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
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
