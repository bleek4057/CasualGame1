using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScript : MonoBehaviour
{
    public int health;
    public GameObject healthDisplay;

    // Use this for initialization
    void Start ()
    {
        healthDisplay = transform.GetChild(1).gameObject;
	}
	
	// Update is called once per frame
	void Update ()
    {
	}

    public void LoseHealth()
    {
        health--;
        for(int i = health; i < 4; i++)
        {
            transform.GetChild(0).GetChild(i).GetComponent<SpriteRenderer>().color = Color.black;
            transform.GetChild(1).GetChild(i).GetComponent<SpriteRenderer>().color = Color.black;
        }
        if (health == 0)
        {
            GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>().LoseWave();
        }
    }
}
