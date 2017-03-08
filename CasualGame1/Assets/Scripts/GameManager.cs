﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //current wave number
    public int waveNumber = 1;

    //the prefab allowing new towers to be placed
    public GameObject towerPrefab;
    //the transparent tower object which is moved around with the mouse
    public GameObject fakeTower;

    public GameObject playerBase;

    public TileManager TileManager;
    public EnemyManager EnemyManager;
    public PlayerManager PlayerManager;
    public DropDownMenuScript DropDownMenu;

    //the size of each interval on the grid
    public int gridIntervalSize = 10;

    public Canvas UI;
    public Canvas MainMenu;
    public Canvas Credits;
    public Canvas PauseMenu;

    public Camera playCamera;

    private Vector2 prevMousePosition;

    public Vector3 menuCameraPos;
    public Vector3 buildCameraPos;
    public Vector3 playCameraPos;
    public Quaternion playCameraAngles;
    public GameObject towerFollow;

    public static GameManager Instance;

    public enum GameState
    {
        MainMenu,
        BuildPhase,
        PlayPhase,
        WinPhase,
        LosePhase,
        Paused
    };

    public GameState currentGame = GameState.BuildPhase;
    private GameState pausedState;

    // Use this for initialization
    void Start ()
    {
        playCameraAngles = Quaternion.Euler(55, 0, 0);

        playCamera.transform.position = menuCameraPos;
        playCamera.transform.eulerAngles = new Vector3(90, 0, 0);

        UI.transform.FindChild("Start Wave").gameObject.SetActive(true);
        playerBase.transform.GetChild(1).gameObject.SetActive(false);

        PlayerManager.GameManager = this;
        EnemyManager.GameManager = this;

        //playCameraAngles = 

        Instance = this;
    }

    //moves the transparent tower based on where the mouse is, to show the player where the tower would be placed
    void MoveFakeTower()
    {
        RaycastHit hit;
        int layermask = ~(1 << 9);
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool rayCast = Physics.Raycast(mouseRay, out hit, 1000, layermask);
        if (rayCast)
        {
            if (hit.transform.tag == "Ground")
            {
                Vector2 target = new Vector2(gridIntervalSize * Mathf.Floor(hit.point.x / gridIntervalSize) + (gridIntervalSize / 2), gridIntervalSize * Mathf.Floor(hit.point.z / gridIntervalSize) + (gridIntervalSize / 2));

                //Debug.Log((int)Mathf.Floor(hit.point.x / 10) + (TileManager.x / 2) + " - " + (int)(((TileManager.y / 2) - 1) - Mathf.Floor(hit.point.z / 10)));
                fakeTower.transform.position = new Vector3(target.x, 5, target.y);
                fakeTower.SetActive(true);
                TileManager.mapData[(int)Mathf.Floor(hit.point.x / 10) + (TileManager.x / 2), (int)(((TileManager.y / 2) - 1) - Mathf.Floor(hit.point.z / 10))] = true;
                if (!PlayerManager.CanAffordTower(towerPrefab.GetComponent<TowerScript>().cost))
                {
                    fakeTower.GetComponent<TowerFakeScript>().SetColor(false);
                }
                else if (!TileManager.CreatePath(false))
                {
                    fakeTower.GetComponent<TowerFakeScript>().SetColor(false);
                }
                else
                {
                    fakeTower.GetComponent<TowerFakeScript>().SetColor(true);
                }
                TileManager.mapData[(int)Mathf.Floor(hit.point.x / 10) + (TileManager.x / 2), (int)(((TileManager.y / 2) - 1) - Mathf.Floor(hit.point.z / 10))] = false;
            }
            else if (hit.transform.tag == "Tower")
            {
                Vector2 target = new Vector2(hit.transform.position.x, hit.transform.position.z);
                Vector2 gridPos = new Vector2(((hit.transform.position.x - 5) / 10) + (TileManager.x / 2), (TileManager.y / 2 - 1) - ((hit.transform.position.z - 5) / 10));
                //Debug.Log("tile towers capacity " + TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].capacity);
            }
            else
            {
                fakeTower.SetActive(false);
            }
        }
        else
        {
            fakeTower.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        UI.transform.FindChild("NotEnoughMoney").gameObject.SetActive(false);
        fakeTower.SetActive(false);
        if (currentGame == GameState.MainMenu || currentGame == GameState.Paused)
        {

        }
        if (currentGame == GameState.PlayPhase)
        {
            if (towerFollow == null)
            {
                //LINEAR -- playCamera.transform.position = Vector3.Lerp(playCamera.transform.position, playCameraPos, 1/ Vector3.Distance(playCamera.transform.position, playCameraPos));
                //NONLINEAR -- 
                playCamera.transform.position = Vector3.Lerp(playCamera.transform.position, playCameraPos, .04f);
                //playCamera.transform.eulerAngles = Vector3.Lerp(playCamera.transform.eulerAngles, playCameraAngles, .04f);
                playCamera.transform.localRotation = Quaternion.RotateTowards(playCamera.transform.localRotation, playCameraAngles, 1f);
                if (Input.GetMouseButton(2) && Vector3.Distance(playCamera.transform.position, playCameraPos) < 5)
                {
                    playCamera.transform.RotateAround(Vector3.zero, Vector3.up, 3 * (Input.mousePosition.x - prevMousePosition.x) * Time.deltaTime);
                    playCameraPos = playCamera.transform.position;
                    playCameraAngles = playCamera.transform.rotation;
                }
            }
            else
            {
                playCamera.transform.localPosition = Vector3.Lerp(playCamera.transform.localPosition, towerFollow.GetComponent<TowerScript>().cameraPos, .04f);
                //for linear, use Quaternion.Angle(playCamera.transform.localRotation, towerFollow.GetComponent<TowerScript>().cameraAngle)
                playCamera.transform.localRotation = Quaternion.RotateTowards(playCamera.transform.localRotation, towerFollow.GetComponent<TowerScript>().cameraAngle, 1f);
            }
            //playCamera.fieldOfView -= Input.mouseScrollDelta.y;
            if (playCamera.fieldOfView < 20)
            {
                playCamera.fieldOfView = 20;
            }
            if (Input.GetMouseButtonUp(1))
            {
                if (towerFollow == null)
                {
                    RaycastHit hit;
                    Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    int layermask = ~(1 << 9);
                    bool rayCast = Physics.Raycast(mouseRay, out hit, 1000, layermask);
                    if (rayCast && hit.transform.tag == "Tower")
                    {
                        towerFollow = hit.transform.gameObject;
                        playCamera.transform.parent = hit.transform.GetComponent<TowerScript>().cameraParent.transform;
                    }
                }
                else
                {
                    towerFollow = null;
                    playCamera.transform.parent = null;
                }
            }
            prevMousePosition = Input.mousePosition;
        }
        else
        {
            towerFollow = null;
            playCamera.transform.parent = null;
        }
        if (currentGame == GameState.BuildPhase)
        {
            MoveFakeTower();

            playCamera.transform.position = Vector3.Lerp(playCamera.transform.position, buildCameraPos, .04f);
            playCamera.transform.eulerAngles = Vector3.Lerp(playCamera.transform.eulerAngles, new Vector3(90, 0, 0), .04f);

            //places a new tower where the player clicks, if there is nothing there
            RaycastHit hit;
            if (Input.GetMouseButtonDown(0) && PlayerManager.CanAffordTower(towerPrefab.GetComponent<TowerScript>().cost))
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                int layermask = ~(1 << 9);
                bool rayCast = Physics.Raycast(mouseRay, out hit, 1000, layermask);
                if (rayCast)
                {
                    if (hit.transform.tag == "Ground")
                    {
                        Vector2 target = new Vector2(10 * Mathf.Floor(hit.point.x / 10) + 5, 10 * Mathf.Floor(hit.point.z / 10) + 5);
                        Vector2 gridPos = new Vector2(((target.x - 5) / 10) + (TileManager.x / 2), (TileManager.y / 2 - 1) - ((target.y - 5) / 10));
                        TileManager.mapData[(int)gridPos.x, (int)gridPos.y] = true;
                        if (TileManager.enemyPath.Contains(target))
                        {
                            if (!TileManager.CreatePath(true))
                            {
                                TileManager.mapData[(int)Mathf.Floor(hit.point.x / 10) + (TileManager.x / 2), (int)(((TileManager.y / 2) - 1) - Mathf.Floor(hit.point.z / 10))] = false;
                                TileManager.CreatePath(true);
                            }
                            else
                            {
                                GameObject newTower = Instantiate(towerPrefab, new Vector3(target.x, 5, target.y), Quaternion.identity);
                                PlayerManager.ChangeMoney(-towerPrefab.GetComponent<TowerScript>().cost);
                                TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents.Add(newTower);
                            }
                        }
                        else
                        {
                            GameObject newTower = Instantiate(towerPrefab, new Vector3(target.x, 5, target.y), Quaternion.identity);
                            PlayerManager.ChangeMoney(-towerPrefab.GetComponent<TowerScript>().cost);
                            TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents.Add(newTower);
                        }

                        //UI.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Money " + PlayerManager.money;
                    }
                    else if (hit.transform.tag == "Tower")
                    {
                        Vector2 target = new Vector2(hit.transform.position.x, hit.transform.position.z);
                        Vector2 gridPos = new Vector2(((hit.transform.position.x - 5) / 10) + (TileManager.x / 2), (TileManager.y / 2 - 1) - ((hit.transform.position.z - 5) / 10));
                        Debug.Log(TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].capacity);
                        if(TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents.Count <= TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].capacity)
                        {
                            GameObject newTower = Instantiate(towerPrefab, new Vector3(target.x, 5, target.y), Quaternion.identity, hit.transform);
                            newTower.transform.localScale = new Vector3(newTower.transform.localScale.x / (hit.transform.localScale.x + 1), newTower.transform.localScale.y / (hit.transform.localScale.y + 1), newTower.transform.localScale.z / (hit.transform.localScale.z + 1));
                            TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents.Add(newTower);
                        }
                    }
                }
            }
            if (Input.GetMouseButtonUp(1))
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                int layermask = ~(1 << 9);
                bool rayCast = Physics.Raycast(mouseRay, out hit, 1000, layermask);
                if (rayCast && hit.transform.tag == "Tower")
                {
                    List<Vector2> savedPath = new List<Vector2>(TileManager.enemyPath);
                    TileManager.mapData[(int)(5 + (hit.transform.position.x - (TileManager.x / 2)) / 10), (int)(5 - (hit.transform.position.z - ((TileManager.y / 2) - 1)) / 10)] = false;
                    TileManager.CreatePath(true);
                    if (TileManager.enemyPath.Count == savedPath.Count)
                    {
                        TileManager.enemyPath = new List<Vector2>(savedPath);
                        TileManager.CreatePathIndicator();
                    }
                    //Debug.Log((5+(hit.transform.position.x - 5)/10) + " -- " + (5 - (hit.transform.position.z - 5) / 10));
                    PlayerManager.ChangeMoney(3 * hit.transform.gameObject.GetComponent<TowerScript>().cost / 5);
                    Destroy(hit.transform.gameObject);

                    //UI.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Money " + PlayerManager.money;
                }
            }
            if (Input.GetMouseButton(2))
            {
                playCamera.transform.Translate(5 * new Vector3(-(Input.mousePosition.x - prevMousePosition.x), -(Input.mousePosition.y - prevMousePosition.y), 0) * Time.deltaTime);
                buildCameraPos = playCamera.transform.position;
            }
            //playCamera.fieldOfView -= Input.mouseScrollDelta.y;
            if (playCamera.fieldOfView < 20)
            {
                playCamera.fieldOfView = 20;
            }
            prevMousePosition = Input.mousePosition;
            if (!PlayerManager.CanAffordTower(towerPrefab.GetComponent<TowerScript>().cost))
            {
                UI.transform.FindChild("NotEnoughMoney").gameObject.SetActive(true);
            }
        }
    }
    public void ToMainMenu()
    {
        currentGame = GameState.MainMenu;

        UI.gameObject.SetActive(false);
        MainMenu.gameObject.SetActive(true);
        Credits.gameObject.SetActive(false);

        playCamera.transform.position = menuCameraPos;
        playCamera.transform.eulerAngles = new Vector3(90, 0, 0);
    }

    public void StartGame()
    {
        Restart();
        currentGame = GameState.BuildPhase;

        playCamera.transform.position = buildCameraPos;
        playCamera.transform.eulerAngles = new Vector3(90, 0, 0);

        UI.gameObject.SetActive(true);
        MainMenu.gameObject.SetActive(false);
    }

    public void TogglePause()
    {
        switch(currentGame)
        {
            case GameState.BuildPhase:
            case GameState.PlayPhase:
                pausedState = currentGame;
                currentGame = GameState.Paused;
                UI.gameObject.SetActive(false);
                PauseMenu.gameObject.SetActive(true);
                break;
            case GameState.Paused:
                currentGame = pausedState;
                PauseMenu.gameObject.SetActive(false);
                UI.gameObject.SetActive(true);
                break;
        }
    }

    public void StartWave()
    {
        currentGame = GameState.PlayPhase;
        
        UI.transform.FindChild("Start Wave").gameObject.SetActive(false);

        playerBase.transform.GetChild(0).gameObject.SetActive(false);
        playerBase.transform.GetChild(1).gameObject.SetActive(true);

        UI.transform.FindChild("Help").GetChild(0).gameObject.SetActive(false);
        UI.transform.FindChild("Help").GetChild(1).gameObject.SetActive(false);
    }
    public void WinWave()
    {
        if (waveNumber == 10)
        {
            WinGame();
            return;
        }

        currentGame = GameState.BuildPhase;

        waveNumber += 1;
        UI.transform.FindChild("Wave UI").GetChild(0).GetComponent<Text>().text = "Wave " + waveNumber;
        
        UI.transform.FindChild("Start Wave").gameObject.SetActive(true);
        EnemyManager.DestroyAll();

        EnemyManager.NextWave();

        playerBase.transform.GetChild(0).gameObject.SetActive(true);
        playerBase.transform.GetChild(1).gameObject.SetActive(false);

        PlayerManager.ChangeMoney(20);

        UI.transform.FindChild("Help").GetChild(0).gameObject.SetActive(false);
        UI.transform.FindChild("Help").GetChild(1).gameObject.SetActive(false);
    }
    public void LoseWave()
    {
        currentGame = GameState.LosePhase;
        
        UI.transform.FindChild("Background").gameObject.SetActive(true);
        UI.transform.FindChild("Start Wave").gameObject.SetActive(false);
        UI.transform.FindChild("Lose").gameObject.SetActive(true);
        UI.transform.FindChild("Restart").gameObject.SetActive(true);
        UI.transform.FindChild("Quit2").gameObject.SetActive(true);
        EnemyManager.FreezeAll();
        
        UI.transform.FindChild("Help").GetChild(0).gameObject.SetActive(false);
        UI.transform.FindChild("Help").GetChild(1).gameObject.SetActive(false);
    }
    public void WinGame()
    {
        currentGame = GameState.WinPhase;
        
        UI.transform.FindChild("Background").gameObject.SetActive(true);
        UI.transform.FindChild("Start Wave").gameObject.SetActive(false);
        UI.transform.FindChild("Restart").gameObject.SetActive(true);
        UI.transform.FindChild("Win").gameObject.SetActive(true);
        UI.transform.FindChild("Quit2").gameObject.SetActive(true);
        EnemyManager.FreezeAll();
    }
    public void Restart()
    {
        currentGame = GameState.BuildPhase;

        waveNumber = 1;
        UI.transform.FindChild("Wave UI").GetChild(0).GetComponent<Text>().text = "Wave " + waveNumber;
        
        UI.transform.FindChild("Background").gameObject.SetActive(false);
        UI.transform.FindChild("Start Wave").gameObject.SetActive(true);
        UI.transform.FindChild("Restart").gameObject.SetActive(false);
        UI.transform.FindChild("Win").gameObject.SetActive(false);
        UI.transform.FindChild("Lose").gameObject.SetActive(false);
        UI.transform.FindChild("Quit2").gameObject.SetActive(false);
        EnemyManager.DestroyAll();
        TileManager.Reset();

        foreach(GameObject tower in GameObject.FindGameObjectsWithTag("Tower"))
        {
            Destroy(tower);
        }

        EnemyManager.RestartAll();
        PlayerManager.SetMoney(75);

        playerBase.transform.GetChild(0).gameObject.SetActive(true);
        playerBase.transform.GetChild(1).gameObject.SetActive(false);

        playerBase.GetComponent<BaseScript>().health = 4;

        UI.transform.FindChild("Help").GetChild(0).gameObject.SetActive(false);
        UI.transform.FindChild("Help").GetChild(1).gameObject.SetActive(false);

        LoadMapTowers();

        TileManager.CreatePath(true);
    }

    //read the current map file and place tiles that are there by default
    private void LoadMapTowers()
    {
        //open the file for the tower data
        StreamReader sr = File.OpenText(TileManager.mapFileName);

        string line = "";

        //get the map size
        int x = Int32.Parse(sr.ReadLine());
        int y = Int32.Parse(sr.ReadLine());
        //temp holders
        int tempX;
        int tempY;

        //Debug.Log(DropDownMenu.towerPrefabs[1]);

        //while there are lines to be read
        while ((line = sr.ReadLine()) != null)
        {
            //split the data to be read easier
            string[] lineData = line.Split(' ');

            //create a wall signal
            if (lineData[0] == "W")
            {
                //get the location data
                tempX = Int32.Parse(lineData[1]);
                tempY = Int32.Parse(lineData[2]);

                //update the map data based on the location
                TileManager.mapData[tempX, tempY] = true;

                //create the wall
                GameObject newTower = Instantiate(DropDownMenu.towerPrefabs[0], new Vector3((tempX - x / 2) * 10 + 5, 5, (tempY - y / 2) * -10 - 5), Quaternion.identity);
            }

            //create a tower signal
            if(lineData[0] == "BT")
            {
                //get the location data
                tempX = Int32.Parse(lineData[1]);
                tempY = Int32.Parse(lineData[2]);

                //update the map data based on the location
                TileManager.mapData[tempX, tempY] = true;

                //create the tower
                GameObject newTower = Instantiate(DropDownMenu.towerPrefabs[1], new Vector3((tempX - x / 2) * 10 + 5, 5, (tempY - y / 2) * -10 - 5), Quaternion.identity);
            }

            //signal a null space on the map
            if (lineData[0] == "N")
            {
                //get the location data
                tempX = Int32.Parse(lineData[1]);
                tempY = Int32.Parse(lineData[2]);

                //update the map data based on the location
                TileManager.mapData[tempX, tempY] = true;
            }
        }
        sr.Close();
    }

    public void ToggleHelp()
    {
        if (currentGame == GameState.BuildPhase)
        {
            UI.transform.FindChild("Help").GetChild(0).gameObject.SetActive(!UI.transform.FindChild("Help").GetChild(0).gameObject.activeSelf);
        }
        else if (currentGame == GameState.PlayPhase)
        {
            UI.transform.FindChild("Help").GetChild(1).gameObject.SetActive(!UI.transform.FindChild("Help").GetChild(0).gameObject.activeSelf);
        }
    }

    public void ToCredits()
    {
        MainMenu.gameObject.SetActive(false);
        Credits.gameObject.SetActive(true);
    }
}
