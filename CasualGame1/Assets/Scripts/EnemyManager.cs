using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> allEnemies;
    public GameManager GameManager;

    public int enemiesToSpawn;
    private int enemiesSpawned;

    // enemy prefab and spawn point
    public Vector2 enemySpawnPoint;
    public GameObject enemyPrefab;
    
    public float spawnInterval = 5;
    private float interval;
    //starting time until next enemy spawns
    private float startInterval = 1;

    // Use this for initialization
    void Start ()
    {
        allEnemies = new List<GameObject>();

        enemiesSpawned = 0;

        interval = startInterval;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (GameManager.currentGame == GameManager.GameState.PlayPhase)
        {
            if (enemiesSpawned < enemiesToSpawn)
            {
                //counts down to when the next enemy appears
                interval -= Time.deltaTime;
                if (interval <= 0)
                {
                    SpawnEnemy();
                    interval = spawnInterval;
                }
            }
            else if (allEnemies.Count == 0)
            {
                GameManager.WinWave();
            }
        }
    }

    //spawns a new enemy and initializes its path
    void SpawnEnemy()
    {
        enemiesSpawned += 1;

        GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(enemySpawnPoint.x, 4.5f, enemySpawnPoint.y), Quaternion.identity);
        newEnemy.GetComponent<EnemyScript>().CopyList(GameManager.TileManager.GetComponent<TileManager>().enemyPath);
        newEnemy.GetComponent<EnemyScript>().playerBase = GameManager.playerBase;
        allEnemies.Add(newEnemy);

        newEnemy.transform.GetChild(0).GetComponent<LookAtCamera>().cameraToSee = GameManager.playCamera;

        //eh, this is for a slider health bar but i think the objects would be easier
        //GameObject enemyHealth = Instantiate(enemySliderPrefab);
        //enemyHealth.transform.SetParent(UI.transform, false);
    }

    public int EnemiesSpawned
    {
        get
        {
            return enemiesSpawned;
        }
        set
        {
            enemiesSpawned = value;
        }
    }

    public void RestartInterval()
    {
        interval = startInterval;
    }

    public void DestroyEnemy(GameObject target)
    {
        allEnemies.Remove(target);
        Destroy(target);
        GameManager.PlayerManager.ChangeMoney(3);
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
