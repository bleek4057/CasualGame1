using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public int money;
    public GameManager GameManager;
    public GameObject selectedTowerPrefab;

	// Use this for initialization
	void Start ()
    {
        money = 100;

    }
	
	// Update is called once per frame
	void Update ()
    {
        GameManager.UI.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = "Money: " + money;
    }

    public bool CanAffordTower(int cost)
    {
        if(money >= 25)
        {
            return true;
        }
        return false;
    }

    public void ChangeMoney(int mon)
    {
        money += mon;
    }
    public void SetMoney(int mon)
    {
        money = mon;
    }
}
