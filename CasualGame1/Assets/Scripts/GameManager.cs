using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //current wave number
    public int waveNumber = 1;

    public int levelNumber = 1;

    //the prefab allowing new towers to be placed
    public GameObject towerPrefab;
    //the transparent tower object which is moved around with the mouse
    public GameObject fakeTower;

    public GameObject baseTowerPrefab;
    public GameObject baseTowerFake;

    public GameObject playerBase;

    public TileManager TileManager;
    public EnemyManager EnemyManager;
    public PlayerManager PlayerManager;
    public DropDownMenuScript DropDownMenu;

    //the size of each interval on the grid
    public int gridIntervalSize = 10;

    public Canvas UI;
    public Canvas MainMenu;
    public Canvas LevelMenu;
    public Canvas Credits;
    public Canvas PauseMenu;

    public Camera playCamera;

    private Vector2 prevMousePosition;

    public Vector3 menuCameraPos;
    public Vector3 buildCameraPos;
    private Vector3 updateBuildCameraPos;
    public Vector3 playCameraPos;
    private Vector3 updatePlayCameraPos;
    public Quaternion playCameraAngles;
    private Quaternion updatePlayCameraAngles;
    private Quaternion followAngles;
    public GameObject towerFollow;

    public GameObject towerMouseOver;

    public static GameManager Instance;

    public enum GameState
    {
        MainMenu,
        LevelMenu,
        BuildPhase,
        PlayPhase,
        WinPhase,
        LosePhase,
        Paused
    };

    public GameState currentGame = GameState.BuildPhase;
    private GameState pausedState;

    private GameObject pastLine;

    // Use this for initialization
    void Start ()
    {
        playCameraAngles = Quaternion.Euler(playCameraAngles.x, playCameraAngles.y, playCameraAngles.z);

        playCamera.transform.position = menuCameraPos;
        playCamera.transform.eulerAngles = new Vector3(90, 0, 0);

        UI.transform.FindChild("Start Wave").gameObject.SetActive(true);
        //playerBase.transform.GetChild(1).gameObject.SetActive(false);

        PlayerManager.GameManager = this;
        EnemyManager.GameManager = this;

        Instance = this;

        ToMainMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
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
                baseTowerFake.SetActive(false);
                Vector2 target = new Vector2(gridIntervalSize * Mathf.Floor(hit.point.x / gridIntervalSize) + (gridIntervalSize / 2), gridIntervalSize * Mathf.Floor(hit.point.z / gridIntervalSize) + (gridIntervalSize / 2));

                //Debug.Log((int)Mathf.Floor(hit.point.x / 10) + (TileManager.x / 2) + " - " + (int)(((TileManager.y / 2) - 1) - Mathf.Floor(hit.point.z / 10)));

                if (!towerPrefab.GetComponent<BaseTower>().isBase)
                {
                    baseTowerFake.transform.position = new Vector3(target.x, 5, target.y);
                    baseTowerFake.SetActive(true);
                    fakeTower.transform.position = new Vector3(target.x, 11, target.y);
                }
                else
                {
                    fakeTower.transform.position = new Vector3(target.x, 5, target.y);
                }
                fakeTower.SetActive(true);
                TileManager.mapData[(int)Mathf.Floor(hit.point.x / 10) + (TileManager.x / 2), (int)(((TileManager.y / 2) - 1) - Mathf.Floor(hit.point.z / 10))] = true;
                if (TileManager.CreatePath(false))
                {
                    TileManager.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                    TileManager.CreateFakePathIndicator(TileManager.CreatePathPoints());
                }
                else
                {
                    TileManager.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                }

                if (!PlayerManager.CanAffordTower(towerPrefab.GetComponent<BaseTower>().cost) || !towerPrefab.GetComponent<BaseTower>().isBase && !PlayerManager.CanAffordTower(baseTowerPrefab.GetComponent<BaseTower>().cost + towerPrefab.GetComponent<BaseTower>().cost))
                {
                    fakeTower.GetComponent<TowerFakeScript>().SetColor(false);
                    baseTowerFake.GetComponent<TowerFakeScript>().SetColor(false);
                    UI.transform.FindChild("NotEnoughMoney").gameObject.SetActive(true);
                }
                else if (!TileManager.CreatePath(false))
                {
                    fakeTower.GetComponent<TowerFakeScript>().SetColor(false);
                    baseTowerFake.GetComponent<TowerFakeScript>().SetColor(false);
                }
                else
                {
                    fakeTower.GetComponent<TowerFakeScript>().SetColor(true);
                    baseTowerFake.GetComponent<TowerFakeScript>().SetColor(true);
                }
                TileManager.mapData[(int)Mathf.Floor(hit.point.x / 10) + (TileManager.x / 2), (int)(((TileManager.y / 2) - 1) - Mathf.Floor(hit.point.z / 10))] = false;
            }
            else if (hit.transform.tag == "Tower")
            {
                baseTowerFake.SetActive(false);
                Vector2 target = new Vector2(hit.transform.position.x, hit.transform.position.z);
                Vector2 gridPos = new Vector2(((hit.transform.position.x - 5) / 10) + (TileManager.x / 2), (TileManager.y / 2 - 1) - ((hit.transform.position.z - 5) / 10));
                if ((!towerPrefab.GetComponent<BaseTower>().isBase || TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].TopTower().GetComponent<BaseTower>().isBase) && TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents.Count <= TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].capacity)
                {
                    fakeTower.transform.position = new Vector3(target.x, TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].Height() + (fakeTower.transform.localScale.y / 2), target.y);
                    fakeTower.SetActive(true);
                }
                else
                {
                    fakeTower.SetActive(false);
                }
                if (!PlayerManager.CanAffordTower(towerPrefab.GetComponent<BaseTower>().cost))
                {
                    fakeTower.GetComponent<TowerFakeScript>().SetColor(false);
                    UI.transform.FindChild("NotEnoughMoney").gameObject.SetActive(true);
                }
                else
                {
                    fakeTower.GetComponent<TowerFakeScript>().SetColor(true);
                }
            }
            else
            {
                fakeTower.SetActive(false);
                baseTowerFake.SetActive(false);
                TileManager.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
        else
        {
            fakeTower.SetActive(false);
            baseTowerFake.SetActive(false);
            TileManager.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        UI.transform.FindChild("NotEnoughMoney").gameObject.SetActive(false);
        if (currentGame == GameState.MainMenu || currentGame == GameState.Paused)
        {
            fakeTower.SetActive(false);
        }
        if (currentGame == GameState.PlayPhase)
        {
            fakeTower.SetActive(false);
            TileManager.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            //handles moving the camera to its desired location
            if (towerFollow == null)
            {
                //LINEAR -- playCamera.transform.position = Vector3.Lerp(playCamera.transform.position, playCameraPos, 1/ Vector3.Distance(playCamera.transform.position, updatePlayCameraPos));
                //NONLINEAR -- 
                playCamera.transform.position = Vector3.Lerp(playCamera.transform.position, updatePlayCameraPos, .04f);
                playCamera.transform.localRotation = Quaternion.RotateTowards(playCamera.transform.localRotation, updatePlayCameraAngles, 1f);

                if (Input.GetMouseButton(2) && Mathf.Abs(Vector3.Distance(playCamera.transform.position, Vector3.zero) - Vector3.Distance(updatePlayCameraPos, Vector3.zero)) < 5)
                {
                    playCamera.transform.RotateAround(Vector3.zero, Vector3.up, 3 * (Input.mousePosition.x - prevMousePosition.x) * Time.deltaTime);
                    updatePlayCameraPos = playCamera.transform.position;
                    updatePlayCameraAngles = playCamera.transform.rotation;
                }
            }
            else
            {
                Vector2 gridPos = new Vector2(((towerFollow.transform.position.x - 5) / 10) + (TileManager.x / 2), (TileManager.y / 2 - 1) - ((towerFollow.transform.position.z - 5) / 10));
                Vector3 followPos = new Vector3(towerFollow.transform.position.x, TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].Height() + (TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].Height()*(2.5f/12f)), towerFollow.transform.position.z);

                playCamera.transform.localPosition = Vector3.Lerp(playCamera.transform.localPosition, followPos, .04f);
                //for linear, use Quaternion.Angle(playCamera.transform.localRotation, towerFollow.GetComponent<TowerScript>().cameraAngle)
                playCamera.transform.localRotation = Quaternion.RotateTowards(playCamera.transform.localRotation, followAngles, 1f);
                if (!Input.GetMouseButton(2))
                    if (Mathf.Abs(Vector3.Distance(playCamera.transform.position, followPos)) < 1)
                    {
                        playCamera.transform.Rotate(0, 10 * (Input.mousePosition.x - prevMousePosition.x) * Time.deltaTime, 0, Space.World);
                        followAngles = playCamera.transform.rotation;
                    }
                foreach (GameObject shooter in TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents)
                {
                    if (!shooter.GetComponent<BaseTower>().isBase && shooter.GetComponent<BaseTower>().controlled)
                    {
                        shooter.transform.eulerAngles = new Vector3(0, playCamera.transform.eulerAngles.y, 0);
                    }
                }

                //DO MANUALLY SHOOTING TOWERS HERE
                if (UI.transform.FindChild("LeaveControl").gameObject.activeSelf)
                {
                    int layerMask = (1 << 10);
                    RaycastHit hit;
                    if (Physics.Raycast(new Vector3(towerFollow.transform.position.x, 1, towerFollow.transform.position.z), new Vector3(Mathf.Sin(Mathf.PI * playCamera.transform.eulerAngles.y / 180), 0, Mathf.Cos(Mathf.PI * playCamera.transform.eulerAngles.y / 180)), out hit, 1000, layerMask))
                    {
                        if (pastLine != null)
                        {
                            Destroy(pastLine);
                        }
                        GameObject myLine = new GameObject();
                        myLine.AddComponent<LineRenderer>();
                        myLine.GetComponent<LineRenderer>().material = new Material(Shader.Find("Sprites/Default"));
                        myLine.GetComponent<LineRenderer>().startWidth = .2f;
                        myLine.GetComponent<LineRenderer>().startColor = Color.red;
                        myLine.GetComponent<LineRenderer>().endColor = Color.red;
                        myLine.GetComponent<LineRenderer>().SetPosition(0, towerFollow.transform.position);
                        myLine.GetComponent<LineRenderer>().SetPosition(1, hit.collider.transform.position);
                        pastLine = myLine;

                        if (Input.GetMouseButton(0))
                        {
                            foreach (GameObject shooter in TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents)
                            {
                                if (shooter.GetComponent<BaseTower>().canAttack && shooter.GetComponent<BaseTower>().Timer <= 0)
                                {
                                    shooter.GetComponent<BaseTower>().Attack(hit.collider.gameObject.GetComponent<EnemyScript>());
                                }
                            }
                        }
                    }
                    else
                    {
                        if (pastLine != null)
                        {
                            Destroy(pastLine);
                        }
                        Vector3 endPos = new Vector3(towerFollow.transform.position.x, 1, towerFollow.transform.position.z) + (300 * new Vector3(Mathf.Sin(Mathf.PI * playCamera.transform.eulerAngles.y / 180), 0, Mathf.Cos(Mathf.PI * playCamera.transform.eulerAngles.y / 180)));
                        GameObject myLine = new GameObject();
                        myLine.AddComponent<LineRenderer>();
                        myLine.GetComponent<LineRenderer>().material = new Material(Shader.Find("Sprites/Default"));
                        myLine.GetComponent<LineRenderer>().startWidth = .2f;
                        myLine.GetComponent<LineRenderer>().startColor = Color.blue;
                        myLine.GetComponent<LineRenderer>().endColor = Color.blue;
                        myLine.GetComponent<LineRenderer>().SetPosition(0, towerFollow.transform.position);
                        myLine.GetComponent<LineRenderer>().SetPosition(1, endPos);
                        pastLine = myLine;
                        /*if (Input.GetMouseButton(0))
                        {
                            foreach (GameObject shooter in TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents)
                            {
                                if (shooter.GetComponent<TowerScript>().canAttack && shooter.GetComponent<TowerScript>().Timer <= 0)
                                {
                                    shooter.GetComponent<TowerScript>().Attack(endPos);
                                }
                            }
                        }*/
                    }
                }
            }
            //handles right clicking, which will either zoom the camera to a tower or back the camera out to the main view
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
                        Vector2 gridPos = new Vector2(((hit.transform.position.x - 5) / 10) + (TileManager.x / 2), (TileManager.y / 2 - 1) - ((hit.transform.position.z - 5) / 10));
                        if(TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents.Count > 1)
                        {
                            followAngles = Quaternion.Euler((20f/12)* TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].Height(), 0, 0);
                            if((20f / 12) * TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].Height() > 40)
                            {
                                followAngles = Quaternion.Euler(40, 0, 0);
                            }
                            //Debug.Log(TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].Height());
                            //followAngles = Quaternion.Euler(20, 0, 0);
                            towerFollow = hit.transform.parent.gameObject;
                            bool canControlTower = false;
                            foreach(GameObject towers in TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents)
                            {
                                if(towers.GetComponent<BaseTower>().canBeControlled)
                                {
                                    canControlTower = true;
                                }
                            }
                            if (canControlTower)
                            {
                                UI.transform.FindChild("TakeControl").gameObject.SetActive(true);
                            }
                        }
                    }
                }
                else
                {
                    Vector2 gridPos = new Vector2(((towerFollow.transform.position.x - 5) / 10) + (TileManager.x / 2), (TileManager.y / 2 - 1) - ((towerFollow.transform.position.z - 5) / 10));
                    foreach (GameObject shooter in TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents)
                    {
                        if (!shooter.GetComponent<BaseTower>().isBase)
                        {
                            shooter.GetComponent<BaseTower>().controlled = false;
                        }
                    }
                    UI.transform.FindChild("TakeControl").gameObject.SetActive(false);
                    UI.transform.FindChild("LeaveControl").gameObject.SetActive(false);
                    towerFollow = null;
                }
            }
            prevMousePosition = Input.mousePosition;
        }
        else if(towerFollow != null)
        {
            Vector2 gridPos = new Vector2(((towerFollow.transform.position.x - 5) / 10) + (TileManager.x / 2), (TileManager.y / 2 - 1) - ((towerFollow.transform.position.z - 5) / 10));
            foreach (GameObject shooter in TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents)
            {
                if (!shooter.GetComponent<BaseTower>().isBase)
                {
                    shooter.GetComponent<BaseTower>().controlled = false;
                }
            }
            UI.transform.FindChild("TakeControl").gameObject.SetActive(false);
            UI.transform.FindChild("LeaveControl").gameObject.SetActive(false);
            towerFollow = null;
        }
        if (currentGame == GameState.BuildPhase)
        {
            if (pastLine != null)
            {
                Destroy(pastLine);
                pastLine = null;
            }
            MoveFakeTower();

            playCamera.transform.position = Vector3.Lerp(playCamera.transform.position, updateBuildCameraPos, .04f);
            playCamera.transform.eulerAngles = Vector3.Lerp(playCamera.transform.eulerAngles, new Vector3(90, 0, 0), .04f);

            towerMouseOver = null;
            RaycastHit hit2;
            Ray mouseRay2 = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layermask2 = ~(1 << 9);
            bool rayCast2 = Physics.Raycast(mouseRay2, out hit2, 1000, layermask2);
            if (rayCast2 && hit2.transform.tag == "Tower")
            {
                Vector2 gridPos = new Vector2(((hit2.transform.position.x - 5) / 10) + (TileManager.x / 2), (TileManager.y / 2 - 1) - ((hit2.transform.position.z - 5) / 10));
                towerMouseOver = TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents[TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents.Count - 1];
                if (TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents.Count == 1)
                {
                    TileManager.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                    TileManager.mapData[(int)gridPos.x, (int)gridPos.y] = false;
                    TileManager.CreateFakePathIndicator(TileManager.CreatePathPoints());
                    TileManager.mapData[(int)gridPos.x, (int)gridPos.y] = true;
                }
                else
                {
                    TileManager.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                }
            }

            //places a new tower where the player clicks, if there is nothing there
            RaycastHit hit;
            if (Input.GetMouseButtonDown(0) && PlayerManager.CanAffordTower(towerPrefab.GetComponent<BaseTower>().cost))
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
                        if (!TileManager.CreatePath(true))
                        {
                            TileManager.mapData[(int)Mathf.Floor(hit.point.x / 10) + (TileManager.x / 2), (int)(((TileManager.y / 2) - 1) - Mathf.Floor(hit.point.z / 10))] = false;
                            TileManager.CreatePath(true);
                        }
                        else
                        {
                            GameObject tower = new GameObject("Tower");
                            tower.tag = "Tower Parent";
                            tower.transform.position = new Vector3(target.x, 5, target.y);
                            GameObject newTower;
                            if (!towerPrefab.GetComponent<BaseTower>().isBase)
                            {
                                if (PlayerManager.CanAffordTower(towerPrefab.GetComponent<BaseTower>().cost + baseTowerPrefab.GetComponent<BaseTower>().cost))
                                {
                                    GameObject newBaseTower = Instantiate(baseTowerPrefab, new Vector3(target.x, 5, target.y), Quaternion.identity, tower.transform);
                                    PlayerManager.ChangeMoney(-baseTowerPrefab.GetComponent<BaseTower>().cost);
                                    TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents.Add(newBaseTower);
                                    newTower = Instantiate(towerPrefab, new Vector3(target.x, TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].Height() + (towerPrefab.transform.localScale.y / 2), target.y), Quaternion.identity, tower.transform);
                                    PlayerManager.ChangeMoney(-towerPrefab.GetComponent<BaseTower>().cost);
                                    TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents.Add(newTower);
                                }
                            }
                            else
                            {
                                newTower = Instantiate(towerPrefab, new Vector3(target.x, 5, target.y), Quaternion.identity, tower.transform);
                                PlayerManager.ChangeMoney(-towerPrefab.GetComponent<BaseTower>().cost);
                                TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents.Add(newTower);
                            }
                        }

                        //UI.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Money " + PlayerManager.money;
                    }
                    else if (hit.transform.tag == "Tower")
                    {
                        Vector2 target = new Vector2(hit.transform.position.x, hit.transform.position.z);
                        Vector2 gridPos = new Vector2(((hit.transform.position.x - 5) / 10) + (TileManager.x / 2), (TileManager.y / 2 - 1) - ((hit.transform.position.z - 5) / 10));
                        if(TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents.Count <= TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].capacity && (!towerPrefab.GetComponent<BaseTower>().isBase || TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].TopTower().GetComponent<BaseTower>().isBase))
                        {
                            GameObject newTower = Instantiate(towerPrefab, new Vector3(target.x, TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].Height() + (towerPrefab.transform.localScale.y / 2), target.y), Quaternion.identity, hit.transform.parent);
                            PlayerManager.ChangeMoney(-towerPrefab.GetComponent<BaseTower>().cost);
                            TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents.Add(newTower);
                        }
                    }
                }
            }
            if (Input.GetMouseButtonUp(1) && towerMouseOver != null) //use towerMouseOver
            {
                Vector2 gridPos = new Vector2(((towerMouseOver.transform.position.x - 5) / 10) + (TileManager.x / 2), (TileManager.y / 2 - 1) - ((towerMouseOver.transform.position.z - 5) / 10));
                PlayerManager.ChangeMoney(TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents[TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents.Count - 1].GetComponent<BaseTower>().cost);
                if (TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents.Count == 1)
                {
                    TileManager.mapData[(int)gridPos.x, (int)gridPos.y] = false;
                    TileManager.CreatePath(true);
                }
                TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents.RemoveAt(TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents.Count - 1);
                if(TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents.Count == 0)
                {
                    Destroy(towerMouseOver.transform.parent.gameObject, .1f);
                }
                Destroy(towerMouseOver);
            }
            if (Input.GetMouseButton(2))
            {
                playCamera.transform.Translate(5 * new Vector3(-(Input.mousePosition.x - prevMousePosition.x), -(Input.mousePosition.y - prevMousePosition.y), 0) * Time.deltaTime);
                updateBuildCameraPos = playCamera.transform.position;
                playCamera.transform.position = new Vector3(playCamera.transform.position.x, buildCameraPos.y, playCamera.transform.position.z);
            }
            //playCamera.fieldOfView -= Input.mouseScrollDelta.y;
            if (playCamera.fieldOfView < 20)
            {
                playCamera.fieldOfView = 20;
            }
            prevMousePosition = Input.mousePosition;
            /*if (!PlayerManager.CanAffordTower(towerPrefab.GetComponent<BaseTower>().cost))
            {
                UI.transform.FindChild("NotEnoughMoney").gameObject.SetActive(true);
            }*/
        }
    }

    public void ChangeControl(bool control)
    {
        if(towerFollow != null)
        {
            Vector2 gridPos = new Vector2(((towerFollow.transform.position.x - 5) / 10) + (TileManager.x / 2), (TileManager.y / 2 - 1) - ((towerFollow.transform.position.z - 5) / 10));
            foreach (GameObject shooter in TileManager.tileTowers[(int)gridPos.x, (int)gridPos.y].contents)
            {
                if (shooter.GetComponent <BaseTower>().canBeControlled)
                {
                    shooter.GetComponent<BaseTower>().controlled = control;
                }
            }
        }

        if(control)
        {
            UI.transform.FindChild("TakeControl").gameObject.SetActive(false);
            UI.transform.FindChild("LeaveControl").gameObject.SetActive(true);
        }
        else
        {
            UI.transform.FindChild("TakeControl").gameObject.SetActive(true);
            UI.transform.FindChild("LeaveControl").gameObject.SetActive(false);
        }
    }

    public void ToMainMenu()
    {
        currentGame = GameState.MainMenu;

        UI.gameObject.SetActive(false);
        MainMenu.gameObject.SetActive(true);
        LevelMenu.gameObject.SetActive(false);
        Credits.gameObject.SetActive(false);

        playCamera.transform.position = menuCameraPos;
        playCamera.transform.eulerAngles = new Vector3(90, 0, 0);
    }

    public void ToLevelMenu()
    {
        currentGame = GameState.LevelMenu;

        UI.gameObject.SetActive(false);
        MainMenu.gameObject.SetActive(false);
        LevelMenu.gameObject.SetActive(true);
        Credits.gameObject.SetActive(false);

        playCamera.transform.position = menuCameraPos;
        playCamera.transform.eulerAngles = new Vector3(90, 0, 0);

    }

    public void Level(int level)
    {
        levelNumber = level;

        StartGame();
    }

    public void StartGame()
    {
        Restart();
        
        currentGame = GameState.BuildPhase;

        playCamera.transform.position = buildCameraPos;
        playCamera.transform.eulerAngles = new Vector3(90, 0, 0);

        UI.gameObject.SetActive(true);
        MainMenu.gameObject.SetActive(false);
        LevelMenu.gameObject.SetActive(false);
    }

    //loads in all data from files for the current level
    private void LoadLevelData()
    {
        TileManager.LoadLevelData("level" + levelNumber);
        LoadMapTowers("level" + levelNumber);
        EnemyManager.LoadEnemyData("level" + levelNumber);
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

        UI.transform.FindChild("LeftClickBuild").gameObject.SetActive(false);
        UI.transform.FindChild("RightClickBuild").gameObject.SetActive(false);
        UI.transform.FindChild("MiddleClickBuild").gameObject.SetActive(false);
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
        UI.transform.FindChild("TakeControl").gameObject.SetActive(false);
        UI.transform.FindChild("LeaveControl").gameObject.SetActive(false);
        EnemyManager.DestroyAll();

        EnemyManager.NextWave();

        PlayerManager.ChangeMoney(20);

        UI.transform.FindChild("LeftClickPlay").gameObject.SetActive(false);
        UI.transform.FindChild("RightClickPlay").gameObject.SetActive(false);
        UI.transform.FindChild("MiddleClickPlay").gameObject.SetActive(false);
    }

    public void LoseWave()
    {
        currentGame = GameState.LosePhase;
        
        UI.transform.FindChild("Background").gameObject.SetActive(true);
        UI.transform.FindChild("Start Wave").gameObject.SetActive(false);
        UI.transform.FindChild("Lose").gameObject.SetActive(true);
        UI.transform.FindChild("Restart").gameObject.SetActive(true);
        UI.transform.FindChild("Level2").gameObject.SetActive(true);
        UI.transform.FindChild("TakeControl").gameObject.SetActive(false);
        UI.transform.FindChild("LeaveControl").gameObject.SetActive(false);
        EnemyManager.FreezeAll();
        
        UI.transform.FindChild("LeftClickPlay").gameObject.SetActive(false);
        UI.transform.FindChild("RightClickPlay").gameObject.SetActive(false);
        UI.transform.FindChild("MiddleClickPlay").gameObject.SetActive(false);
    }

    public void WinGame()
    {
        currentGame = GameState.WinPhase;
        
        UI.transform.FindChild("Background").gameObject.SetActive(true);
        UI.transform.FindChild("Start Wave").gameObject.SetActive(false);
        UI.transform.FindChild("Restart").gameObject.SetActive(true);
        UI.transform.FindChild("Win").gameObject.SetActive(true);
        UI.transform.FindChild("Level2").gameObject.SetActive(true);
        UI.transform.FindChild("TakeControl").gameObject.SetActive(false);
        UI.transform.FindChild("LeaveControl").gameObject.SetActive(false);
        EnemyManager.FreezeAll();
        
        UI.transform.FindChild("LeftClickPlay").gameObject.SetActive(false);
        UI.transform.FindChild("RightClickPlay").gameObject.SetActive(false);
        UI.transform.FindChild("MiddleClickPlay").gameObject.SetActive(false);
    }

    public void Restart()
    {
        currentGame = GameState.BuildPhase;

        updateBuildCameraPos = buildCameraPos;
        updatePlayCameraPos = playCameraPos;
        updatePlayCameraAngles = playCameraAngles;

        waveNumber = 1;
        UI.transform.FindChild("Wave UI").GetChild(0).GetComponent<Text>().text = "Wave " + waveNumber;
        
        UI.transform.FindChild("Background").gameObject.SetActive(false);
        UI.transform.FindChild("Start Wave").gameObject.SetActive(true);
        UI.transform.FindChild("Restart").gameObject.SetActive(false);
        UI.transform.FindChild("Win").gameObject.SetActive(false);
        UI.transform.FindChild("Lose").gameObject.SetActive(false);
        UI.transform.FindChild("Level2").gameObject.SetActive(false);
        UI.transform.FindChild("TakeControl").gameObject.SetActive(false);
        UI.transform.FindChild("LeaveControl").gameObject.SetActive(false);
        EnemyManager.DestroyAll();
        TileManager.Reset();

        foreach(GameObject tower in GameObject.FindGameObjectsWithTag("Tower"))
        {
            Destroy(tower);
        }
        foreach (GameObject tower in GameObject.FindGameObjectsWithTag("Tower Parent"))
        {
            Destroy(tower);
        }

        EnemyManager.RestartAll();
        //PlayerManager.SetMoney(75);

        //playerBase.transform.GetChild(0).gameObject.SetActive(true);
        //playerBase.transform.GetChild(1).gameObject.SetActive(false);

        playerBase.GetComponent<BaseScript>().health = 4;

        UI.transform.FindChild("Help").GetChild(0).gameObject.SetActive(false);
        UI.transform.FindChild("Help").GetChild(1).gameObject.SetActive(false);

        LoadLevelData();

        TileManager.CreatePath(true);
    }

    //read the current map file and place tiles that are there by default
    private void LoadMapTowers(string level)
    {
        //open the file for the tower data
        StreamReader sr = File.OpenText("Assets\\MapData\\" + level + ".txt");

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


            /*if (lineData[0] == "S")
            {
                tempX = Int32.Parse(lineData[1]);
                tempY = Int32.Parse(lineData[2]);

                EnemyManager.enemySpawnPoint = new Vector2();
            }*/

            if (lineData[0] == "P")
            {
                PlayerManager.SetMoney(Int32.Parse(lineData[1]));
            }

            //create a wall signal
            if (lineData[0] == "W")
            {
                //get the location data
                tempX = Int32.Parse(lineData[1]);
                tempY = Int32.Parse(lineData[2]);

                //update the map data based on the location
                TileManager.mapData[tempX, tempY] = true;

                //create the wall
                //GameObject newTower = Instantiate(DropDownMenu.towerPrefabs[0], new Vector3((tempX - x / 2) * 10 + 5, 5, (tempY - y / 2) * -10 - 5), Quaternion.identity);
                
                if (TileManager.tileTowers[(int)tempX, (int)tempY].contents.Count == 0)
                {
                    GameObject tower = new GameObject("Tower");
                    tower.tag = "Tower Parent";
                    tower.transform.position = new Vector3((tempX - x / 2) * 10 + 5, 5, (tempY - y / 2) * -10 - 5);
                    GameObject newTower = Instantiate(DropDownMenu.towerPrefabs[0], new Vector3((tempX - x / 2) * 10 + 5, 5, (tempY - y / 2) * -10 - 5), Quaternion.identity, tower.transform);
                    TileManager.tileTowers[(int)tempX, (int)tempY].contents.Add(newTower);
                }
                else
                {
                    GameObject newTower = Instantiate(DropDownMenu.towerPrefabs[0], new Vector3((tempX - x / 2) * 10 + 5, TileManager.tileTowers[(int)tempX, (int)tempY].Height() + (DropDownMenu.towerPrefabs[0].transform.localScale.y / 2), (tempY - y / 2) * -10 - 5), Quaternion.identity, TileManager.tileTowers[(int)tempX, (int)tempY].contents[0].transform.parent);
                    TileManager.tileTowers[(int)tempX, (int)tempY].contents.Add(newTower);
                }
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
                //GameObject newTower = Instantiate(DropDownMenu.towerPrefabs[1], new Vector3((tempX - x / 2) * 10 + 5, 5, (tempY - y / 2) * -10 - 5), Quaternion.identity);
                //Debug.Log(TileManager.tileTowers[(int)tempX, (int)tempY].contents.Count);
                if (TileManager.tileTowers[(int)tempX, (int)tempY].contents.Count == 0)
                {
                    GameObject tower = new GameObject("Tower");
                    tower.tag = "Tower Parent";
                    tower.transform.position = new Vector3((tempX - x / 2) * 10 + 5, 5, (tempY - y / 2) * -10 - 5);
                    GameObject newBaseTower = Instantiate(DropDownMenu.towerPrefabs[0], new Vector3((tempX - x / 2) * 10 + 5, 5, (tempY - y / 2) * -10 - 5), Quaternion.identity, tower.transform);
                    TileManager.tileTowers[(int)tempX, (int)tempY].contents.Add(newBaseTower);
                    GameObject newTower = Instantiate(DropDownMenu.towerPrefabs[1], new Vector3((tempX - x / 2) * 10 + 5, TileManager.tileTowers[(int)tempX, (int)tempY].Height() + (DropDownMenu.towerPrefabs[1].transform.localScale.y/2), (tempY - y / 2) * -10 - 5), Quaternion.identity, tower.transform);
                    TileManager.tileTowers[(int)tempX, (int)tempY].contents.Add(newTower);
                }
                else
                {
                    GameObject newTower = Instantiate(DropDownMenu.towerPrefabs[1], new Vector3((tempX - x / 2) * 10 + 5, TileManager.tileTowers[(int)tempX, (int)tempY].Height() + (DropDownMenu.towerPrefabs[1].transform.localScale.y / 2), (tempY - y / 2) * -10 - 5), Quaternion.identity, TileManager.tileTowers[(int)tempX, (int)tempY].contents[0].transform.parent);
                    TileManager.tileTowers[(int)tempX, (int)tempY].contents.Add(newTower);
                }
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
//Debug.Log(TileManager.mapData[1, 1]);
    }

    public void ToggleHelp()
    {
        if (currentGame == GameState.BuildPhase)
        {
            //UI.transform.FindChild("Help").GetChild(0).gameObject.SetActive(!UI.transform.FindChild("Help").GetChild(0).gameObject.activeSelf);
            UI.transform.FindChild("LeftClickBuild").gameObject.SetActive(!UI.transform.FindChild("LeftClickBuild").gameObject.activeSelf);
            UI.transform.FindChild("RightClickBuild").gameObject.SetActive(!UI.transform.FindChild("RightClickBuild").gameObject.activeSelf);
            UI.transform.FindChild("MiddleClickBuild").gameObject.SetActive(!UI.transform.FindChild("MiddleClickBuild").gameObject.activeSelf);
        }
        else if (currentGame == GameState.PlayPhase)
        {
            //UI.transform.FindChild("Help").GetChild(1).gameObject.SetActive(!UI.transform.FindChild("Help").GetChild(1).gameObject.activeSelf);
            UI.transform.FindChild("LeftClickPlay").gameObject.SetActive(!UI.transform.FindChild("LeftClickPlay").gameObject.activeSelf);
            UI.transform.FindChild("RightClickPlay").gameObject.SetActive(!UI.transform.FindChild("RightClickPlay").gameObject.activeSelf);
            UI.transform.FindChild("MiddleClickPlay").gameObject.SetActive(!UI.transform.FindChild("MiddleClickPlay").gameObject.activeSelf);
        }
    }

    public void ToCredits()
    {
        MainMenu.gameObject.SetActive(false);
        Credits.gameObject.SetActive(true);
    }
}