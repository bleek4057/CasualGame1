using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerScript : MonoBehaviour {

    public List<EnemyScript> enemies = new List<EnemyScript>();

    private SphereCollider rangeSphere;
    private float _timer = 0.0f;
    public float _fireRate = 0.333f;

    public ParticleSystem ps;

	// Use this for initialization
	void Start () {
        rangeSphere = this.gameObject.GetComponent<SphereCollider>();
	}
	
	// Update is called once per frame
	void Update () {
        _timer -= Time.deltaTime;

        if(_timer <= 0 && enemies.Count > 0)
        {
            if (enemies[0] == null) enemies.RemoveAt(0);
            else Attack(enemies[0]);
        }
	}

    private void Attack(EnemyScript enemy)
    {
        ps.Play();
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
