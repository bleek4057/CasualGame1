using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //current wave number
    private int waveNumber = 1;

    //the prefab allowing new towers to be placed
    public GameObject towerPrefab;
    //the transparent tower object which is moved around with the mouse
    public GameObject fakeTower;

    public GameObject playerBase;

    public TileManager TileManager;
    public EnemyManager EnemyManager;
    public PlayerManager PlayerManager;

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
    public Vector3 playCameraAngles;
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
        playCamera.transform.position = menuCameraPos;
        playCamera.transform.eulerAngles = new Vector3(90, 0, 0);

        UI.transform.FindChild("Start Wave").gameObject.SetActive(true);
        playerBase.transform.GetChild(1).gameObject.SetActive(false);

        PlayerManager.GameManager = this;
        EnemyManager.GameManager = this;

        Instance = this;
    }

    //moves the transparent tower based on where the mouse is, to show the player where the tower would be placed
    void MoveFakeTower()
    {
        RaycastHit hit;
        int layermask = ~(1 << 9);
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool rayCast = Physics.Raycast(mouseRay, out hit, 1000, layermask);
        if (rayCast && hit.transform.tag == "Ground")
        {
            Vector2 target = new Vector2(gridIntervalSize * Mathf.Floor(hit.point.x / gridIntervalSize) + (gridIntervalSize / 2), gridIntervalSize * Mathf.Floor(hit.point.z / gridIntervalSize) + (gridIntervalSize / 2));
            fakeTower.transform.position = new Vector3(target.x, 5, target.y);
            fakeTower.SetActive(true);
            if (PlayerManager.CanAffordTower(towerPrefab.GetComponent<TowerScript>().cost))
            {
                fakeTower.GetComponent<TowerFakeScript>().SetColor(true);
            }
            else
            {
                fakeTower.GetComponent<TowerFakeScript>().SetColor(false);
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
        if (currentGame == GameState.MainMenu || currentGame == GameState.Paused)
        {

        }
        if (currentGame == GameState.PlayPhase)
        {
            if (Input.GetMouseButton(1))
            {
                playCamera.transform.RotateAround(Vector3.zero, Vector3.up, 3 * (Input.mousePosition.x - prevMousePosition.x) * Time.deltaTime);
                /*playCamera.transform.RotateAround(Vector3.zero, Vector3.left, 3 * (Input.mousePosition.y - prevMousePosition.y) * Time.deltaTime);
                playCamera.transform.Rotate(0, 0, -playCamera.transform.eulerAngles.z);
                if (playCamera.transform.eulerAngles.x < 50 || playCamera.transform.eulerAngles.x > 70)
                {
                    playCamera.transform.RotateAround(Vector3.zero, Vector3.left, -3 * (Input.mousePosition.y - prevMousePosition.y) * Time.deltaTime);
                }*/
            }
            playCamera.fieldOfView -= Input.mouseScrollDelta.y;
            if (playCamera.fieldOfView < 20)
            {
                playCamera.fieldOfView = 20;
            }
            RaycastHit hit;
            /*if (Input.GetMouseButtonUp(1))
            {
                if (playCamera.gameObject.activeSelf)
                {
                    Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    int layermask = ~(1 << 9);
                    bool rayCast = Physics.Raycast(mouseRay, out hit, 1000, layermask);
                    if (rayCast && hit.transform.tag == "Tower" && hit.transform.gameObject.GetComponent<TowerScript>().camera != null)
                    {
                        hit.transform.gameObject.GetComponent<TowerScript>().camera.gameObject.SetActive(true);
                        playCamera.gameObject.SetActive(false);
                        currentCamera = hit.transform.gameObject.GetComponent<TowerScript>().camera.gameObject;
                    }
                }
                else
                {
                    currentCamera.gameObject.SetActive(false);
                    playCamera.gameObject.SetActive(true);
                }
            }*/
            prevMousePosition = Input.mousePosition;
        }
        if(currentGame == GameState.BuildPhase)
        {
            MoveFakeTower();

            //places a new tower where the player clicks, if there is nothing there
            RaycastHit hit;
            if (Input.GetMouseButtonDown(0) && PlayerManager.CanAffordTower(towerPrefab.GetComponent<TowerScript>().cost))
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                int layermask = ~(1 << 9);
                bool rayCast = Physics.Raycast(mouseRay, out hit, 1000, layermask);
                if (rayCast && hit.transform.tag == "Ground")
                {
                    Vector2 target = new Vector2(10 * Mathf.Floor(hit.point.x / 10) + 5, 10 * Mathf.Floor(hit.point.z / 10) + 5);
                    TileManager.mapData[(int)Mathf.Floor(hit.point.x / 10) + (TileManager.x/2), (int)(((TileManager.y / 2)-1) - Mathf.Floor(hit.point.z / 10))] = true;
                    if (TileManager.enemyPath.Contains(target))
                    {
                        if (!TileManager.CreatePath())
                        {
                            TileManager.mapData[(int)Mathf.Floor(hit.point.x / 10) + (TileManager.x / 2), (int)(((TileManager.y / 2)-1) - Mathf.Floor(hit.point.z / 10))] = false;
                            TileManager.CreatePath();
                        }
                        else
                        {
                            GameObject newTower = Instantiate(towerPrefab, new Vector3(target.x, 5, target.y), Quaternion.identity);
                            PlayerManager.ChangeMoney(-towerPrefab.GetComponent<TowerScript>().cost);
                        }
                    }
                    else
                    {
                        GameObject newTower = Instantiate(towerPrefab, new Vector3(target.x, 5, target.y), Quaternion.identity);
                        PlayerManager.ChangeMoney(-towerPrefab.GetComponent<TowerScript>().cost);
                    }

                    //UI.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Money " + PlayerManager.money;
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
                    TileManager.CreatePath();
                    if (TileManager.enemyPath.Count == savedPath.Count)
                    {
                        TileManager.enemyPath = new List<Vector2>(savedPath);
                        TileManager.CreatePathIndicator();
                    }
                    //Debug.Log((5+(hit.transform.position.x - 5)/10) + " -- " + (5 - (hit.transform.position.z - 5) / 10));
                    PlayerManager.ChangeMoney(3*hit.transform.gameObject.GetComponent<TowerScript>().cost/5);
                    Destroy(hit.transform.gameObject);

                    //UI.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Money " + PlayerManager.money;
                }
            }
            if (Input.GetMouseButton(1))
            {
                playCamera.transform.Translate(5 * new Vector3(-(Input.mousePosition.x - prevMousePosition.x), -(Input.mousePosition.y - prevMousePosition.y), 0) * Time.deltaTime);
            }
            //playCamera.fieldOfView -= Input.mouseScrollDelta.y;
            if(playCamera.fieldOfView < 20)
            {
                playCamera.fieldOfView = 20;
            }
            prevMousePosition = Input.mousePosition;
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
        
        playCamera.transform.position = playCameraPos;
        playCamera.transform.eulerAngles = playCameraAngles;
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
        
        playCamera.transform.position = buildCameraPos;
        playCamera.transform.eulerAngles = new Vector3(90, 0, 0);
        UI.transform.FindChild("Start Wave").gameObject.SetActive(true);
        EnemyManager.DestroyAll();

        EnemyManager.EnemiesSpawned = 0;
        EnemyManager.RestartInterval();
        EnemyManager.spawnInterval *= .9f;
        EnemyManager.enemiesToSpawn += 1;

        playerBase.transform.GetChild(0).gameObject.SetActive(true);
        playerBase.transform.GetChild(1).gameObject.SetActive(false);

        PlayerManager.ChangeMoney(20);

        UI.transform.FindChild("Help").GetChild(0).gameObject.SetActive(false);
        UI.transform.FindChild("Help").GetChild(1).gameObject.SetActive(false);
    }
    public void LoseWave()
    {
        currentGame = GameState.LosePhase;
        
        playCamera.transform.position = playCameraPos;
        playCamera.transform.eulerAngles = playCameraAngles;
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
        
        playCamera.transform.position = playCameraPos;
        playCamera.transform.eulerAngles = playCameraAngles;
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
        
        playCamera.transform.position = buildCameraPos;
        playCamera.transform.eulerAngles = new Vector3(90, 0, 0);
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

        EnemyManager.EnemiesSpawned = 0;
        EnemyManager.enemiesToSpawn = 3;
        EnemyManager.spawnInterval = 5;
        EnemyManager.RestartInterval();
        PlayerManager.SetMoney(125);

        playerBase.transform.GetChild(0).gameObject.SetActive(true);
        playerBase.transform.GetChild(1).gameObject.SetActive(false);

        playerBase.GetComponent<BaseScript>().health = 4;

        UI.transform.FindChild("Help").GetChild(0).gameObject.SetActive(false);
        UI.transform.FindChild("Help").GetChild(1).gameObject.SetActive(false);
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
