﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //current wave number
    private int waveNumber = 1;

    // enemy prefab and spawn point
    public Vector2 enemySpawnPoint;
    public GameObject enemyPrefab;
    public GameObject enemySliderPrefab;

    //starting time until next enemy spawns
    private float spawnInterval = 1;

    //the prefab allowing new towers to be placed
    public GameObject towerPrefab;
    //the transparent tower object which is moved around with the mouse
    public GameObject fakeTower;

    public GameObject playerBase;

    public TileManager TileManager;
    public EnemyManager EnemyManager;

    //the size of each interval on the grid
    public int gridIntervalSize = 10;

    public Canvas UI;

    public Camera playCamera;
    public Camera buildCamera;

    public int enemiesToSpawn;
    private int enemiesSpawned;

    private enum GameState
    {
        BuildPhase,
        PlayPhase,
        WinPhase,
        LosePhase,
    };

    GameState currentGame = GameState.BuildPhase;

    // Use this for initialization
    void Start ()
    {
        playCamera.gameObject.SetActive(false);
        buildCamera.gameObject.SetActive(true);
        UI.transform.FindChild("Start Wave").gameObject.SetActive(true);
        playerBase.transform.GetChild(1).gameObject.SetActive(false);

        enemiesSpawned = 0;
    }

    //spawns a new enemy and initializes its path
    void SpawnEnemy()
    {
        enemiesSpawned += 1;

        GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(enemySpawnPoint.x, 4.5f, enemySpawnPoint.y), Quaternion.identity);
        newEnemy.GetComponent<EnemyScript>().CopyList(TileManager.GetComponent<TileManager>().enemyPath);
        newEnemy.GetComponent<EnemyScript>().playerBase = playerBase;
        EnemyManager.GetComponent<EnemyManager>().allEnemies.Add(newEnemy);

        //eh, this is for a slider health bar but i think the objects would be easier
        //GameObject enemyHealth = Instantiate(enemySliderPrefab);
        //enemyHealth.transform.SetParent(UI.transform, false);
    }
    //moves the transparent tower based on where the mouse is, to show the player where the tower would be placed
    void MoveFakeTower()
    {
        RaycastHit hit;
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool rayCast = Physics.Raycast(mouseRay, out hit);
        if (rayCast && hit.transform.tag == "Ground")
        {
            Vector2 target = new Vector2(gridIntervalSize * Mathf.Floor(hit.point.x / gridIntervalSize) + (gridIntervalSize / 2), gridIntervalSize * Mathf.Floor(hit.point.z / gridIntervalSize) + (gridIntervalSize / 2));
            fakeTower.transform.position = new Vector3(target.x, 5, target.y);
            fakeTower.SetActive(true);
        }
        else
        {
            fakeTower.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (currentGame == GameState.PlayPhase)
        {
            if (enemiesSpawned <= enemiesToSpawn)
            {
                //counts down to when the next enemy appears
                spawnInterval -= Time.deltaTime;
                if (spawnInterval <= 0)
                {
                    SpawnEnemy();
                    spawnInterval = 5;
                }
            }
            else if(EnemyManager.allEnemies.Count == 0)
            {
                WinWave();
            }
        }
        if(currentGame == GameState.BuildPhase)
        {
                
            MoveFakeTower();

            //places a new tower where the player clicks, if there is nothing there
            RaycastHit hit;
            if (Input.GetMouseButtonDown(0))
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                bool rayCast = Physics.Raycast(mouseRay, out hit);
                if (rayCast && hit.transform.tag == "Ground")
                {
                    Vector2 target = new Vector2(10 * Mathf.Floor(hit.point.x / 10) + 5, 10 * Mathf.Floor(hit.point.z / 10) + 5);
                    Instantiate(towerPrefab, new Vector3(target.x, 5, target.y), Quaternion.identity);
                }
            }
        }
    }

    public void StartWave()
    {
        currentGame = GameState.PlayPhase;

        playCamera.gameObject.SetActive(true);
        buildCamera.gameObject.SetActive(false);
        UI.transform.FindChild("Start Wave").gameObject.SetActive(false);

        playerBase.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void WinWave()
    {
        currentGame = GameState.BuildPhase;

        waveNumber += 1;
        UI.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Wave " + waveNumber;

        playCamera.gameObject.SetActive(false);
        buildCamera.gameObject.SetActive(true);
        UI.transform.FindChild("Start Wave").gameObject.SetActive(true);
        EnemyManager.DestroyAll();

        enemiesSpawned = 0;

        playerBase.transform.GetChild(1).gameObject.SetActive(false);
    }
    public void LoseWave()
    {
        currentGame = GameState.LosePhase;

        playCamera.gameObject.SetActive(true);
        buildCamera.gameObject.SetActive(false);
        UI.transform.FindChild("Start Wave").gameObject.SetActive(false);
        UI.transform.FindChild("Lose").gameObject.SetActive(true);
        UI.transform.FindChild("Restart").gameObject.SetActive(true);
        EnemyManager.FreezeAll();
    }
    public void WinGame()
    {
        currentGame = GameState.WinPhase;

        playCamera.gameObject.SetActive(true);
        buildCamera.gameObject.SetActive(false);
        UI.transform.FindChild("Start Wave").gameObject.SetActive(false);
        UI.transform.FindChild("Restart").gameObject.SetActive(true);
        UI.transform.FindChild("Win").gameObject.SetActive(true);
        EnemyManager.FreezeAll();
    }
    public void Restart()
    {
        currentGame = GameState.BuildPhase;

        waveNumber = 1;
        UI.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Wave " + waveNumber;

        playCamera.gameObject.SetActive(false);
        buildCamera.gameObject.SetActive(true);
        UI.transform.FindChild("Start Wave").gameObject.SetActive(true);
        UI.transform.FindChild("Restart").gameObject.SetActive(false);
        UI.transform.FindChild("Win").gameObject.SetActive(false);
        UI.transform.FindChild("Lose").gameObject.SetActive(false);
        EnemyManager.DestroyAll();

        enemiesSpawned = 0;

        playerBase.transform.GetChild(1).gameObject.SetActive(false);
    }
}