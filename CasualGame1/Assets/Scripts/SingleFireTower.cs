using DigitalRuby.LightningBolt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleFireTower : BaseTower {

    private LightningBoltScript lightning;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        lightning = gameObject.GetComponent<LightningBoltScript>();
        if (lightning != null)
            lightning.StartObject = transform.GetChild(0).GetChild(1).GetChild(0).gameObject;

        cameraAngle = transform.GetChild(0).GetChild(1).GetChild(1).transform.localRotation;
    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
        if(canAttack && GameManager.Instance.currentGame == GameManager.GameState.PlayPhase)
        {
            if(enemies.Count > 0)
            {
                transform.GetChild(0).GetChild(1).LookAt(enemies[0].transform.position);
                transform.GetChild(0).GetChild(1).eulerAngles = new Vector3(-90, transform.GetChild(0).GetChild(1).eulerAngles.y, 0);
            }
        }
	}

    protected override void Attack()
    {
        var enemy = enemies[0];
        
        transform.GetChild(0).GetChild(1).LookAt(enemy.transform.position);
        transform.GetChild(0).GetChild(1).eulerAngles = new Vector3(-90, transform.GetChild(0).GetChild(1).eulerAngles.y, 0);
        lightning.EndObject = enemy.gameObject;
        lightning.Trigger();
        enemy.Damage(damagePerHit);

        _timer = 1f * Mathf.Pow(2.0f, _fireRate);
    }
}
