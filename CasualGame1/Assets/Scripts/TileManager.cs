using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    //the path each enemy will follow
    public List<Vector2> enemyPath;

    // Use this for initialization
    void Start ()
    {
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
