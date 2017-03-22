using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScript : MonoBehaviour
{
    public int health;

    // Use this for initialization
    void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < health; i++)
        {
            //transform.GetChild(0).GetChild(i).GetComponent<SpriteRenderer>().color = Color.black;
            //transform.GetChild(1).GetChild(i).GetComponent<SpriteRenderer>().color = Color.black;
            transform.GetChild(2).GetChild(i).GetComponent<MeshRenderer>().materials[0].color = new Color(34 / 255f, 34 / 255f, 34 / 255f, 1);
            transform.GetChild(2).GetChild(i).GetComponent<MeshRenderer>().materials[1].color = new Color(204 / 255f, 204 / 255f, 204 / 255f, 1);
        }
        for (int i = health; i < 4; i++)
        {
            //transform.GetChild(0).GetChild(i).GetComponent<SpriteRenderer>().color = Color.black;
            //transform.GetChild(1).GetChild(i).GetComponent<SpriteRenderer>().color = Color.black;
            transform.GetChild(2).GetChild(i).GetComponent<MeshRenderer>().materials[0].color = Color.black;
            transform.GetChild(2).GetChild(i).GetComponent<MeshRenderer>().materials[1].color = Color.red;
        }
	}

    public void LoseHealth()
    {
        health--;
        if (health == 0)
        {
            GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>().LoseWave();
        }
    }
}
