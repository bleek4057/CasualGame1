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
        money = 75;

    }
	
	// Update is called once per frame
	void Update ()
    {
        GameManager.UI.transform.FindChild("Extra UI").GetChild(2).GetComponent<Text>().text = "Power: " + money;
    }

    public bool CanAffordTower(int cost)
    {
        if(money >= cost)
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
