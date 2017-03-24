using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerFakeScript : MonoBehaviour
{
    Color defaultMainColor;
    public List<GameObject> toBeColored;
    public List<Color> defaultColor;

    public float range = 0;

    // Use this for initialization
    void Start()
    {
        if (range > 0)
        {
            transform.FindChild("ShootingRange").transform.localScale = new Vector3(2 * range, 2 * range, 1);
        }
    }

    public void SetDefaultColors()
    {
        defaultColor = new List<Color>();
        defaultMainColor = GetComponent<Renderer>().material.color;
        foreach (GameObject child in toBeColored)
        {
            defaultColor.Add(child.GetComponent<Renderer>().material.color);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetColor(bool def)
    {
        if (!def)
        {
            GetComponent<Renderer>().material.color = new Color(1, 0, 0, 71 / 255f);
            for (int i = 0; i < toBeColored.Count; i++)
            {
                toBeColored[i].GetComponent<Renderer>().material.color = new Color(1, 0, 0, 71/255f);
            }
        }
        else
        {
            GetComponent<Renderer>().material.color = defaultMainColor;
            for (int i = 0; i < toBeColored.Count; i++)
            {
                toBeColored[i].GetComponent<Renderer>().material.color = defaultColor[i];
            }
        }
    }
}