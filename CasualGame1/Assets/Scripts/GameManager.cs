using System.Collections;
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
    public PlayerManager PlayerManager;

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

        PlayerManager.GameManager = this;
        EnemyManager.GameManager = this;

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

        newEnemy.transform.GetChild(0).GetComponent<LookAtCamera>().cameraToSee = playCamera;

        //eh, this is for a slider health bar but i think the objects would be easier
        //GameObject enemyHealth = Instantiate(enemySliderPrefab);
        //enemyHealth.transform.SetParent(UI.transform, false);
    }

    //moves the transparent tower based on where the mouse is, to show the player where the tower would be placed
    void MoveFakeTower()
    {
        RaycastHit hit;
        int layermask = 1 << 8;
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool rayCast = Physics.Raycast(mouseRay, out hit, 1000, layermask);
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
            if (enemiesSpawned < enemiesToSpawn)
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

            if (Input.GetKey(KeyCode.D))
            {
                playCamera.transform.RotateAround(Vector3.zero, Vector3.up, -20 * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A))
            {
                playCamera.transform.RotateAround(Vector3.zero, Vector3.up, 20 * Time.deltaTime);
            }
        }
        if(currentGame == GameState.BuildPhase)
        {
            MoveFakeTower();

            //places a new tower where the player clicks, if there is nothing there
            RaycastHit hit;
            if (Input.GetMouseButtonDown(0) && PlayerManager.CanAffordTower(25))
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                bool rayCast = Physics.Raycast(mouseRay, out hit);
                if (rayCast && hit.transform.tag == "Ground")
                {
                    Vector2 target = new Vector2(10 * Mathf.Floor(hit.point.x / 10) + 5, 10 * Mathf.Floor(hit.point.z / 10) + 5);
                    GameObject newTower = Instantiate(towerPrefab, new Vector3(target.x, 5, target.y), Quaternion.identity);
                    TileManager.mapData[(int)Mathf.Floor(hit.point.x / 10)+5, (int)(5 - Mathf.Floor(hit.point.z / 10))] = true;
                    PlayerManager.ChangeMoney(-25);
                    if(!TileManager.CreatePath())
                    {
                        Destroy(newTower);
                        TileManager.mapData[(int)Mathf.Floor(hit.point.x / 10) + 5, (int)(5 - Mathf.Floor(hit.point.z / 10))] = false;
                        PlayerManager.ChangeMoney(+25);
                        TileManager.CreatePath();
                    }
                    
                    //UI.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Money " + PlayerManager.money;
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                bool rayCast = Physics.Raycast(mouseRay, out hit);
                if (rayCast && hit.transform.tag == "Tower")
                {
                    Destroy(hit.transform.gameObject);
                    Debug.Log((5+(hit.transform.position.x - 5)/10) + " -- " + (5 - (hit.transform.position.z - 5) / 10));
                    TileManager.mapData[(int)(5 + (hit.transform.position.x - 5) / 10), (int)(5 - (hit.transform.position.z - 5) / 10)] = false;
                    PlayerManager.ChangeMoney(+15);
                    TileManager.CreatePath();

                    //UI.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Money " + PlayerManager.money;
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
        if (waveNumber == 10)
        {
            WinGame();
            return;
        }

        currentGame = GameState.BuildPhase;

        waveNumber += 1;
        UI.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Wave " + waveNumber;

        playCamera.gameObject.SetActive(false);
        buildCamera.gameObject.SetActive(true);
        UI.transform.FindChild("Start Wave").gameObject.SetActive(true);
        EnemyManager.DestroyAll();

        enemiesSpawned = 0;

        playerBase.transform.GetChild(1).gameObject.SetActive(false);

        PlayerManager.ChangeMoney(50);
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
        TileManager.Reset();

        foreach(GameObject tower in GameObject.FindGameObjectsWithTag("Tower"))
        {
            Destroy(tower);
        }

        enemiesSpawned = 0;
        PlayerManager.SetMoney(100);

        playerBase.transform.GetChild(1).gameObject.SetActive(false);
    }
}
