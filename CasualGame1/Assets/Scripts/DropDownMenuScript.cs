using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownMenuScript : MonoBehaviour
{
    public GameManager GameManager;
    public List<GameObject> towerPrefabs;
    
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(transform.parent.GetComponent<RectTransform>().rect.width * -617f / 1920, -35); //134
    }

    public void ToggleDropdown()
    {
        transform.GetChild(1).gameObject.SetActive(!transform.GetChild(1).gameObject.activeInHierarchy);
        transform.GetChild(2).gameObject.SetActive(!transform.GetChild(2).gameObject.activeInHierarchy);
        transform.GetChild(3).gameObject.SetActive(!transform.GetChild(3).gameObject.activeInHierarchy);
    }
    public void SetActiveTower(int id)
    {
        ToggleDropdown();
        GameManager.towerPrefab = towerPrefabs[id];
        Destroy(GameManager.fakeTower);
        GameManager.fakeTower = Instantiate(towerPrefabs[id].GetComponent<BaseTower>().fakeVersion);
        GameManager.fakeTower.GetComponent<TowerFakeScript>().SetDefaultColors();
        GameManager.fakeTower.GetComponent<TowerFakeScript>().range = towerPrefabs[id].GetComponent<BaseTower>().range;
        gameObject.transform.GetChild(0).GetComponent<Text>().text = transform.GetChild(id + 1).gameObject.GetComponentInChildren<Text>().text;
        gameObject.transform.GetChild(4).GetComponent<Text>().text = "Cost:" + towerPrefabs[id].GetComponent<BaseTower>().cost;
    }
}
