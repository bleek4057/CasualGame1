using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerFakeScript : MonoBehaviour
{
    Color defaultMainColor;
    public List<GameObject> toBeColored;
    public List<Color> defaultColor;

    // Use this for initialization
    void Start()
    {
        Debug.Log("START");
        //defaultColor = new List<Color>();
        defaultMainColor = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, GetComponent<Renderer>().material.color.a);
        foreach (GameObject child in toBeColored)
        {
            //defaultColor.Add(new Color(child.GetComponent<Renderer>().material.color.r, child.GetComponent<Renderer>().material.color.g, child.GetComponent<Renderer>().material.color.b, child.GetComponent<Renderer>().material.color.a));
        }
        //SetDefaultColors();
    }

    public void SetDefaultColors()
    {
        Debug.Log("DEFAULT");
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