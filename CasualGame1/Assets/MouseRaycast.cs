using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRaycast : MonoBehaviour
{
    //the prefab allowing new towers to be placed
    public GameObject towerPrefab;
    //the transparent tower object which is moved around with the mouse
    public GameObject fakeTower;

    //the size of each interval on the grid
    public int gridIntervalSize = 10;

    // Use this for initialization
    void Start ()
    {
		
	}

    //moves the transparent tower based on where the mouse is, to show the player where the tower would be placed
    void MoveFakeTower()
    {
        RaycastHit hit;
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool rayCast = Physics.Raycast(mouseRay, out hit);
        if (rayCast && hit.transform.tag == "Ground")
        {
            Vector2 target = new Vector2(gridIntervalSize * Mathf.Floor(hit.point.x / gridIntervalSize) + (gridIntervalSize/2), gridIntervalSize * Mathf.Floor(hit.point.z / gridIntervalSize) + (gridIntervalSize/2));
            fakeTower.transform.position = new Vector3(target.x, 5, target.y);
            fakeTower.SetActive(true);
        }
        else if(!rayCast)
        {
            fakeTower.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update ()
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
