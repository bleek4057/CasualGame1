using DigitalRuby.LightningBolt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleFireTower : BaseTower
{

    private LightningBoltScript lightning;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        lightning = gameObject.GetComponent<LightningBoltScript>();
        if (lightning != null)
            lightning.StartObject = transform.GetChild(0).GetChild(0).gameObject;

        transform.FindChild("Collider").GetComponent<CapsuleCollider>().radius = range;
        transform.FindChild("ShootingRange").transform.localScale = new Vector3(10 * range, 10 * range, 1);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (canAttack && GameManager.Instance.currentGame == GameManager.GameState.PlayPhase)
        {
            if (enemies.Count > 0)
            {
                transform.LookAt(enemies[0].transform.position);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
        }
        transform.FindChild("ShootingRange").gameObject.SetActive(false);

        if(GameManager.Instance.currentGame == GameManager.GameState.BuildPhase && GameManager.Instance.towerMouseOver == gameObject)
        {
            transform.FindChild("ShootingRange").gameObject.SetActive(true);
        }
    }

    protected override void Attack()
    {
        var enemy = enemies[0];

        transform.LookAt(enemy.transform.position);
        transform.eulerAngles = new Vector3(-90, transform.eulerAngles.y, 0);
        lightning.EndObject = enemy.gameObject;
        lightning.Trigger();
        enemy.Damage(damagePerHit);

        _timer = 1f * Mathf.Pow(2.0f, _fireRate);
    }

    public void Attack(EnemyScript enemy)
    {
        //ps.Play();
        lightning.EndPosition = new Vector3(0, 0, 0);
        lightning.EndObject = enemy.gameObject;
        lightning.Trigger();
        enemy.Damage(damagePerHit);

        _timer = 1f * Mathf.Pow(2.0f, _fireRate);
    }

    public void Attack(Vector3 target)
    {
        lightning.EndPosition = target;
        lightning.EndObject = null;
        lightning.Trigger();

        _timer = 1f * Mathf.Pow(2.0f, _fireRate);
    }
}
