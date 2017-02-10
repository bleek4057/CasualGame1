using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerScript : MonoBehaviour {

    public List<EnemyScript> enemies = new List<EnemyScript>();

    private SphereCollider rangeSphere;
    private float _timer = 0;

    public ParticleSystem ps;

	// Use this for initialization
	void Start () {
        ps = this.GetComponent<ParticleSystem>();
        rangeSphere = this.gameObject.GetComponent<SphereCollider>();
	}
	
	// Update is called once per frame
	void Update () {
        _timer -= Time.deltaTime;

        if(_timer <= 0 && enemies.Count > 0)
        {
            
        }
	}

    void OnTriggerEnter(Collider obj)
    {
        if(obj.GetComponent<EnemyScript>() != null)
        {
            enemies.Add(obj.GetComponent<EnemyScript>());
        }
    }

    void OnTriggerExit(Collider obj)
    {
        if(obj.GetComponent<EnemyScript>() != null)
        {
            enemies.Remove(obj.GetComponent<EnemyScript>());
        }
    }
}
