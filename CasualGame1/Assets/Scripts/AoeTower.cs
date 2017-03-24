using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeTower : BaseTower
{
    private ParticleSystem ps;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        ps = gameObject.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Attack()
    {
        ps.Play();
        foreach (var enemy in enemies)
        {
            enemy.Damage(damagePerHit);
        }
        _timer = 1f * Mathf.Pow(2.0f, _fireRate);
    }
    public override void Attack(EnemyScript enemy)
    {
        ps.Play();
        foreach (var enem in enemies)
        {
            enem.Damage(damagePerHit);
        }
        _timer = 1f * Mathf.Pow(2.0f, _fireRate);
    }
    public override void Attack(Vector3 position)
    {
        ps.Play();
        foreach (var enemy in enemies)
        {
            enemy.Damage(damagePerHit);
        }
        _timer = 1f * Mathf.Pow(2.0f, _fireRate);
    }
}