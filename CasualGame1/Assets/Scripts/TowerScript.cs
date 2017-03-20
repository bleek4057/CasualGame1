using DigitalRuby.LightningBolt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerScript : MonoBehaviour
{
    public bool isBase;
    public int cost;

    public List<EnemyScript> enemies = new List<EnemyScript>();
    public GameObject fakeVersion;
    
    private LightningBoltScript lightning;
    private float _timer = 0.0f;
    public float _fireRate = 0.333f;

    public int range = 0;

    public bool canAttack = true;

    Color defaultMainColor;
    public List<GameObject> toBeColored;
    List<Color> defaultColor;

    ParticleSystem ps;
    
    public int damagePerHit;

    public bool controlled = false;

    // Use this for initialization
    void Start ()
    {
        lightning = gameObject.GetComponent<LightningBoltScript>();
        if (lightning != null)
        {
            lightning.StartObject = transform.GetChild(0).GetChild(0).gameObject;
        }
        ps = GetComponent<ParticleSystem>();

        defaultColor = new List<Color>();
        defaultMainColor = GetComponent<Renderer>().material.color;
        foreach (GameObject child in toBeColored)
        {
            defaultColor.Add(child.GetComponent<Renderer>().material.color);
        }

        if (!isBase)
        {
            transform.FindChild("Collider").GetComponent<CapsuleCollider>().radius = range;
            transform.FindChild("ShootingRange").transform.localScale = new Vector3(10 * range, 10 * range, 1);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (canAttack && GameManager.Instance.currentGame == GameManager.GameState.PlayPhase)
        {
            _timer -= Time.deltaTime;

            if (enemies.Count > 0 && !controlled)
            {
                if (enemies[0] == null)
                {
                    enemies.RemoveAt(0);
                }
                transform.LookAt(enemies[0].transform.position);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                if (_timer <= 0)
                {
                    Attack(enemies[0]);
                }
            }
        }

        SetColor(true);
        if (!isBase)
        {
            transform.FindChild("ShootingRange").gameObject.SetActive(false);
        }
        if (GameManager.Instance.currentGame == GameManager.GameState.BuildPhase)
        {
            if(GameManager.Instance.towerMouseOver == gameObject)
            {
                SetColor(false);
                if (!isBase)
                {
                    transform.FindChild("ShootingRange").gameObject.SetActive(true);
                }
            }
        }
    }
    public void SetColor(bool def)
    {
        if (!def)
        {
            GetComponent<Renderer>().material.color = Color.red;
            for (int i = 0; i < toBeColored.Count; i++)
            {
                toBeColored[i].GetComponent<Renderer>().material.color = Color.red;
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

    public float Timer
    {
        get
        {
            return _timer;
        }
    }

    public void Attack(EnemyScript enemy)
    {
        //ps.Play();
        lightning.EndPosition = new Vector3(0,0,0);
        lightning.EndObject = enemy.gameObject;
        lightning.Trigger();
        enemy.Damage(damagePerHit);

        _timer = 1f * Mathf.Pow(2.0f, _fireRate);
    }
    public void Attack(Vector3 position)
    {
        //ps.Play();
        lightning.EndPosition = position;
        lightning.EndObject = null;
        lightning.Trigger();

        _timer = 1f * Mathf.Pow(2.0f, _fireRate);
    }

    void OnTriggerEnter(Collider obj)
    {
        if (obj.tag == "Enemy")
        {
            var potentialEnemy = obj.gameObject.GetComponent<EnemyScript>();
            enemies.Add(potentialEnemy);
        }
    }

    void OnTriggerExit(Collider obj)
    {
        if(obj.tag == "Enemy")
        {
            var potentialEnemy = obj.gameObject.GetComponent<EnemyScript>();
            enemies.Remove(potentialEnemy);
        }
    }
}
