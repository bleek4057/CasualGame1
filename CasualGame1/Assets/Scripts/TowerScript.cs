using DigitalRuby.LightningBolt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerScript : MonoBehaviour {
    public int cost;

    public List<EnemyScript> enemies = new List<EnemyScript>();
    public GameObject fakeVersion;

    private SphereCollider rangeSphere;
    private LightningBoltScript lightning;
    private float _timer = 0.0f;
    public float _fireRate = 0.333f;

    public bool canAttack = true;

    Color defaultMainColor;
    public List<GameObject> toBeColored;
    List<Color> defaultColor;

    ParticleSystem ps;

    public Camera towerCamera;
    public Vector3 cameraPos;
    public Quaternion cameraAngle;
    public int damagePerHit;
    public GameObject cameraParent;

    // Use this for initialization
    void Start () {
        rangeSphere = this.gameObject.GetComponent<SphereCollider>();
        lightning = gameObject.GetComponent<LightningBoltScript>();
        if(lightning != null)
            lightning.StartObject = transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
        ps = GetComponent<ParticleSystem>();

        defaultColor = new List<Color>();
        defaultMainColor = GetComponent<Renderer>().material.color;
        foreach (GameObject child in toBeColored)
        {
            defaultColor.Add(child.GetComponent<Renderer>().material.color);
        }

        cameraAngle = transform.GetChild(0).GetChild(1).GetChild(1).transform.localRotation;
    }
	
	// Update is called once per frame
	void Update () {
        if (canAttack && GameManager.Instance.currentGame == GameManager.GameState.PlayPhase)
        {
            _timer -= Time.deltaTime;

            if (enemies.Count > 0)
            {
                if (enemies[0] == null)
                {
                    enemies.RemoveAt(0);
                }
                transform.GetChild(0).GetChild(1).LookAt(enemies[0].transform.position);
                transform.GetChild(0).GetChild(1).eulerAngles = new Vector3(-90, transform.GetChild(0).GetChild(1).eulerAngles.y, 0);

                if (_timer <= 0)
                {
                    Attack(enemies[0]);
                }
            }
        }

        if (GameManager.Instance.currentGame == GameManager.GameState.BuildPhase)
        {
            RaycastHit hit;
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layermask = ~(1 << 9);
            bool rayCast = Physics.Raycast(mouseRay, out hit, 1000, layermask);
            if (rayCast && hit.transform.gameObject == gameObject)
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
        else
        {
            GetComponent<Renderer>().material.color = defaultMainColor;
            for (int i = 0; i < toBeColored.Count; i++)
            {
                toBeColored[i].GetComponent<Renderer>().material.color = defaultColor[i];
            }
        }
    }

    private void Attack(EnemyScript enemy)
    {
        //ps.Play();
        transform.GetChild(0).GetChild(1).LookAt(enemy.transform.position);
        transform.GetChild(0).GetChild(1).eulerAngles = new Vector3(-90, transform.GetChild(0).GetChild(1).eulerAngles.y, 0);
        lightning.EndObject = enemy.gameObject;
        lightning.Trigger();
        enemy.Damage(damagePerHit);

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
