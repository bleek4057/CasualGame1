using System.Collections;
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
    private float spawnInterval = 0;

    //the path each enemy will follow
    private List<Vector2> enemyPath;

    //the prefab allowing new towers to be placed
    public GameObject towerPrefab;
    //the transparent tower object which is moved around with the mouse
    public GameObject fakeTower;

    //the size of each interval on the grid
    public int gridIntervalSize = 10;

    // Use this for initialization
    void Start ()
    {
        //creates the path each enemy will use
        enemyPath = new List<Vector2>();
        enemyPath.Add(new Vector2(-5, 45));
        enemyPath.Add(new Vector2(-5, 35));
        enemyPath.Add(new Vector2(-5, 25));
        enemyPath.Add(new Vector2(-5, 15));
        enemyPath.Add(new Vector2(-5, 5));
        enemyPath.Add(new Vector2(-5, -5));
        enemyPath.Add(new Vector2(-5, -15));
        enemyPath.Add(new Vector2(-5, -25));
        enemyPath.Add(new Vector2(-5, -35));
        enemyPath.Add(new Vector2(5, -35));
        enemyPath.Add(new Vector2(15, -35));
        enemyPath.Add(new Vector2(25, -35));
    }

    //spawns a new enemy and initializes its path
    void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(enemySpawnPoint.x, 4.5f, enemySpawnPoint.y), Quaternion.identity);
        newEnemy.GetComponent<EnemyScript>().CopyList(enemyPath);
    }

    //moves the transparent tower based on where the mouse is, to show the player where the tower would be placed
    void MoveFakeTower()
    {
        RaycastHit hit;
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool rayCast = Physics.Raycast(mouseRay, out hit);
        if (rayCast && hit.transform.tag == "Ground")
        {
            Debug.Log("Hello2");
            Vector2 target = new Vector2(gridIntervalSize * Mathf.Floor(hit.point.x / gridIntervalSize) + (gridIntervalSize / 2), gridIntervalSize * Mathf.Floor(hit.point.z / gridIntervalSize) + (gridIntervalSize / 2));

            Debug.Log(target);
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
        //counts down to when the next enemy appears
        spawnInterval -= Time.deltaTime;
        if(spawnInterval <= 0)
        {
            SpawnEnemy();
            spawnInterval = 5;
        }

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
