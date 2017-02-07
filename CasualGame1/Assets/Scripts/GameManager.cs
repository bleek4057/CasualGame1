﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //current wave number
    private int waveNumber = 1;

    // enemy prefab and spawn point
    public Vector2 enemySpawnPoint;
    public GameObject enemyPrefab;

    //starting time until next enemy spawns
    private float spawnInterval = 1;

    //the prefab allowing new towers to be placed
    public GameObject towerPrefab;
    //the transparent tower object which is moved around with the mouse
    public GameObject fakeTower;
    
    public GameObject TileManager;

    //the size of each interval on the grid
    public int gridIntervalSize = 10;

    public Canvas UI;

    public Camera playCamera;
    public Camera buildCamera;

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
    }

    //spawns a new enemy and initializes its path
    void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(enemySpawnPoint.x, 4.5f, enemySpawnPoint.y), Quaternion.identity);
        newEnemy.GetComponent<EnemyScript>().CopyList(TileManager.GetComponent<TileManager>().enemyPath);
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
            //counts down to when the next enemy appears
            spawnInterval -= Time.deltaTime;
            if (spawnInterval <= 0)
            {
                SpawnEnemy();
                spawnInterval = 5;
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
    }

    public void WinWave()
    {
        currentGame = GameState.BuildPhase;

        waveNumber += 1;

        playCamera.gameObject.SetActive(false);
        buildCamera.gameObject.SetActive(true);
        UI.transform.FindChild("Start Wave").gameObject.SetActive(true);
    }
    public void LoseWave()
    {
        currentGame = GameState.LosePhase;

        playCamera.gameObject.SetActive(true);
        buildCamera.gameObject.SetActive(false);
        UI.transform.FindChild("Start Wave").gameObject.SetActive(false);
        UI.transform.FindChild("Lose").gameObject.SetActive(true);
        UI.transform.FindChild("Restart").gameObject.SetActive(true);
    }
    public void WinGame()
    {
        currentGame = GameState.WinPhase;

        playCamera.gameObject.SetActive(true);
        buildCamera.gameObject.SetActive(false);
        UI.transform.FindChild("Start Wave").gameObject.SetActive(false);
        UI.transform.FindChild("Restart").gameObject.SetActive(true);
        UI.transform.FindChild("Win").gameObject.SetActive(true);
    }
    public void Restart()
    {
        currentGame = GameState.BuildPhase;

        waveNumber = 1;

        playCamera.gameObject.SetActive(false);
        buildCamera.gameObject.SetActive(true);
        UI.transform.FindChild("Start Wave").gameObject.SetActive(true);
        UI.transform.FindChild("Restart").gameObject.SetActive(false);
        UI.transform.FindChild("Win").gameObject.SetActive(false);
        UI.transform.FindChild("Lose").gameObject.SetActive(false);
    }
}
