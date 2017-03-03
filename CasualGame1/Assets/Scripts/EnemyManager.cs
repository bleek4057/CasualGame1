﻿using System;
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

        GetEnemyData("level1");

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
        //GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(enemySpawnPoint.x, 4.5f, enemySpawnPoint.y), Quaternion.identity);
        Debug.Log(GameManager.waveNumber - 1);
        Debug.Log(enemiesSpawned);
        GameObject newEnemy = Instantiate(enemyTypesToSpawn[GameManager.waveNumber - 1][0], new Vector3(enemySpawnPoint.x, 4.5f, enemySpawnPoint.y), Quaternion.identity);
        newEnemy.GetComponent<EnemyScript>().CopyList(GameManager.TileManager.GetComponent<TileManager>().enemyPath);
        newEnemy.GetComponent<EnemyScript>().playerBase = GameManager.playerBase;
        allEnemies.Add(newEnemy);

        enemiesSpawned += 1;

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
        RestartInterval();
        spawnInterval *= .9f;
        enemiesToSpawn = enemyNumberToSpawn[GameManager.waveNumber - 1][0];
    }
    public void RestartAll()
    {
        enemiesSpawned = 0;
        enemiesToSpawn = enemyNumberToSpawn[0][0];
        spawnInterval = 5;
        RestartInterval();
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

    private void GetEnemyData(string level)
    {
        //StreamReader sr = File.OpenText("Assets\\WaveData\\" + level + ".txt");
        StreamReader sr = File.OpenText("Assets\\MapData\\level1.txt");

        string line = "";
        int wave = 0;

        line = sr.ReadLine();
        while (line != null)
        {
            //enemySpawnTime.Add(float.Parse(line));

            //while((line = sr.ReadLine()) != null && line != "")
            //{
                string[] lineData = line.Split(' ');
                Debug.Log("HI - " + line);
            
            if (lineData[0] == "BE")
                {
                    wave = Int32.Parse(lineData[2]);
                    Debug.Log("HIIII - " + wave);
                    enemyTypesToSpawn[wave].Add(enemyPrefabs[Int32.Parse(lineData[3])]);
                    enemyNumberToSpawn[wave].Add(Int32.Parse(lineData[1]));
            }
            line = sr.ReadLine();
            //}

            //wave++;

        }

        //sr.Close();

        for(int i = 0; i < enemySpawnTime.Count; i++)
        {
            Debug.Log(enemySpawnTime[i]);
        }
    }
}
