using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeTower : BaseTower {

    private ParticleSystem ps;

	// Use this for initialization
	protected override void Start () {
        base.Start();
        ps = gameObject.GetComponent<ParticleSystem>();

        cameraAngle = transform.localRotation;
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

    protected override void Attack()
    {
        ps.Play();
        foreach(var enemy in enemies)
        {
            enemy.Damage(damagePerHit);
        }
        _timer = 1f * Mathf.Pow(2.0f, _fireRate);
    }
}
