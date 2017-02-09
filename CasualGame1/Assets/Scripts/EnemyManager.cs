using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> allEnemies;

    // Use this for initialization
    void Start ()
    {
        allEnemies = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    public void FreezeAll()
    {
        foreach (GameObject enemy in allEnemies)
        {
            enemy.GetComponent<EnemyScript>().enabled = false;
        }
    }
    public void UnfreezeAll()
    {
        foreach (GameObject enemy in allEnemies)
        {
            enemy.GetComponent<EnemyScript>().enabled = true;
        }
    }
    public void DestroyAll()
    {
        foreach(GameObject enemy in allEnemies)
        {
            Destroy(enemy);
        }

        allEnemies = new List<GameObject>();
    }
}
