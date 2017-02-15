using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> allEnemies;
    public GameManager GameManager;

    // Use this for initialization
    void Start ()
    {
        allEnemies = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    public void DestroyEnemy(GameObject target)
    {
        allEnemies.Remove(target);
        Destroy(target);
        GameManager.PlayerManager.ChangeMoney(20);
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
