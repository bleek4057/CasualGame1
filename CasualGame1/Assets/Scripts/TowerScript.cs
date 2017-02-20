using DigitalRuby.LightningBolt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerScript : MonoBehaviour {

    public GameManager gameManager;
    public List<EnemyScript> enemies = new List<EnemyScript>();

    private SphereCollider rangeSphere;
    private LightningBoltScript lightning;
    private float _timer = 0.0f;
    public float _fireRate = 0.333f;

    ParticleSystem ps;

	// Use this for initialization
	void Start () {
        rangeSphere = this.gameObject.GetComponent<SphereCollider>();
        lightning = gameObject.GetComponent<LightningBoltScript>();
        lightning.StartObject = transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
        ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        _timer -= Time.deltaTime;

        if(_timer <= 0 && enemies.Count > 0)
        {
            if (enemies[0] == null) enemies.RemoveAt(0);
            else Attack(enemies[0]);
        }

        if (gameManager.currentGame != GameManager.GameState.PlayPhase)
        {
            RaycastHit hit;
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layermask = ~(1 << 9);
            bool rayCast = Physics.Raycast(mouseRay, out hit, 1000, layermask);
            if (rayCast && hit.transform.gameObject == gameObject)
            {
                transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
                transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = Color.red;
                transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material.color = Color.red;
            }
            else
            {
                transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(34 / 255f, 34 / 255f, 34 / 255f, 1);
                transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = new Color(34 / 255f, 34 / 255f, 34 / 255f, 1);
                transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material.color = new Color(34 / 255f, 34 / 255f, 34 / 255f, 1);
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
        enemy.Damage(1);

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
