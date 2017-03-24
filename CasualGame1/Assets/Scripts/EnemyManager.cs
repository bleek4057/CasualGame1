using System;
using System.IO;
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

    //data of different enemies to spawn in a wave
    public List<List<GameObject>> enemyTypesToSpawn;
    //number of enemies to spawn, corresponds with enemyTypesToSpawn
    private List<List<int>> enemyNumberToSpawn;
    //time for enemies to spawn on a wave, corresponds with enemyTypesToSpawn
    private List<float> enemySpawnTime;
    //List of enemy prefabs
    public List<GameObject> enemyPrefabs;
    //int representing section of enemy wave that we are on
    private int currentEnemySection;


    // Use this for initialization
    void Start ()
    {
        allEnemies = new List<GameObject>();

        //initialize enemy Lists
        enemyTypesToSpawn = new List<List<GameObject>>();
        enemyNumberToSpawn = new List<List<int>>();
        enemySpawnTime = new List<float>();
        for(int i = 0; i < 10; i++)
        {
            enemyNumberToSpawn.Add(new List<int>());
            enemyTypesToSpawn.Add(new List<GameObject>());
            //enemyNumberToSpawn.Add();
        }

        currentEnemySection = 0;

        LoadEnemyData("level1");

        enemiesSpawned = 0;

        spawnInterval = enemySpawnTime[0];

        interval = startInterval;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (GameManager.currentGame == GameManager.GameState.PlayPhase)
        {
            if (enemiesSpawned < enemiesToSpawn)
            {
                //Debug.Log("Enemies in current section: " + enemyNumberToSpawn[GameManager.waveNumber - 1][currentEnemySection]);

                //Debug.Log("Enemies to spawn: " + enemiesToSpawn + ", Enemies spawned: " + enemiesSpawned);

                //if we have spawned enough of the type of enemies in this section of a wave
                if(enemiesSpawned == enemyNumberToSpawn[GameManager.waveNumber - 1][currentEnemySection])
                {
                    enemiesSpawned -= enemyNumberToSpawn[GameManager.waveNumber - 1][currentEnemySection];
                    enemiesToSpawn -= enemyNumberToSpawn[GameManager.waveNumber - 1][currentEnemySection];

                    currentEnemySection++;
                }

                //counts down to when the next enemy appears
                interval -= Time.deltaTime;
                if (interval <= 0)
                {
                    SpawnEnemy(currentEnemySection);
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
    void SpawnEnemy(int waveSection)
    {
        //GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(enemySpawnPoint.x, 4.5f, enemySpawnPoint.y), Quaternion.identity);
        //Debug.Log("Wave #" + (GameManager.waveNumber - 1));

        GameObject newEnemy = Instantiate(enemyTypesToSpawn[GameManager.waveNumber - 1][waveSection], new Vector3(enemySpawnPoint.x, 4.5f, enemySpawnPoint.y), Quaternion.identity);
        newEnemy.GetComponent<EnemyScript>().CopyList(GameManager.TileManager.GetComponent<TileManager>().enemyPath);
        newEnemy.GetComponent<EnemyScript>().maxHealth += 5 * (GameManager.waveNumber - 1);
        newEnemy.GetComponent<EnemyScript>().playerBase = GameManager.playerBase;
        allEnemies.Add(newEnemy);

        enemiesSpawned += 1;

        //Debug.Log("Enemies Spawned " + enemiesSpawned);

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

    public void NextWave()
    {
        enemiesSpawned = 0;
        currentEnemySection = 0;
        RestartInterval();
        spawnInterval = enemySpawnTime[GameManager.waveNumber - 1];

        enemiesToSpawn = 0;

        //Debug.Log("wave " + (GameManager.waveNumber - 1));

        for (int i = 0; i < enemyNumberToSpawn[GameManager.waveNumber - 1].Count; i++)
        {
            //Debug.Log(enemyNumberToSpawn[GameManager.waveNumber - 1][i]);
            enemiesToSpawn += enemyNumberToSpawn[GameManager.waveNumber - 1][i];
        }

        //Debug.Log("enemies to spawn: " + enemiesToSpawn);
    }
    public void RestartAll()
    {
        enemiesSpawned = 0;
        enemiesToSpawn = enemyNumberToSpawn[0][0];
        spawnInterval = enemySpawnTime[0];
        currentEnemySection = 0;
        RestartInterval();
        ResetEnemyData();
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

    public void LoadEnemyData(string level)
    {
        StreamReader sr = File.OpenText("Assets\\WaveData\\" + level + ".txt");
        //StreamReader sr = File.OpenText("Assets\\MapData\\level1.txt");

        string line = "";
        int wave = 0;

        while ((line = sr.ReadLine()) != null)
        {
            string[] lineData = line.Split(' ');
            //Debug.Log("HI - " + line);
            
            if (lineData[0] == "BE")
            {
                wave = Int32.Parse(lineData[2]);
                //Debug.Log("Wave # " + wave);
                enemyTypesToSpawn[wave].Add(enemyPrefabs[0]);
                enemyNumberToSpawn[wave].Add(Int32.Parse(lineData[1]));

                if(enemySpawnTime.Count <= wave)
                {
                    //Debug.Log("wave" + wave);
                    enemySpawnTime.Add(float.Parse(lineData[3]));
                }
                
            }
            if(lineData[0] == "FE")
            {
                wave = Int32.Parse(lineData[2]);
                //Debug.Log("Wave # " + wave);
                enemyTypesToSpawn[wave].Add(enemyPrefabs[1]);
                enemyNumberToSpawn[wave].Add(Int32.Parse(lineData[1]));
                if (enemySpawnTime.Count <= wave)
                {
                    //Debug.Log("wave" + wave);
                    enemySpawnTime.Add(float.Parse(lineData[3]));
                }
            }
            if(lineData[0] == "SE")
            {
                wave = Int32.Parse(lineData[2]);
                //Debug.Log("Wave # " + wave);
                enemyTypesToSpawn[wave].Add(enemyPrefabs[2]);
                enemyNumberToSpawn[wave].Add(Int32.Parse(lineData[1]));
                if (enemySpawnTime.Count <= wave)
                {
                    //Debug.Log("wave" + wave);
                    enemySpawnTime.Add(float.Parse(lineData[3]));
                }
            }
        }

        enemiesToSpawn = 0;

        for (int i = 0; i < enemyNumberToSpawn[0].Count; i++)
        {
            //Debug.Log(enemyNumberToSpawn[0][i]);
            enemiesToSpawn += enemyNumberToSpawn[0][i];
        }


        //Debug.Log("spawn time count " + enemySpawnTime.Count);
        //
        //for(int i = 0; i < enemySpawnTime.Count; i++)
        //{
        //    Debug.Log(enemySpawnTime[i]);
        //}
    }

    private void ResetEnemyData()
    {

        enemyTypesToSpawn = new List<List<GameObject>>();
        enemyNumberToSpawn = new List<List<int>>();
        enemySpawnTime = new List<float>();
        for (int i = 0; i < 10; i++)
        {
            enemyNumberToSpawn.Add(new List<int>());
            enemyTypesToSpawn.Add(new List<GameObject>());
            //enemyNumberToSpawn.Add();
        }
    }
}
