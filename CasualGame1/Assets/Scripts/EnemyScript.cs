using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int maxHealth;
    private int health;
    public int speed;
    public Queue<Vector2> path = new Queue<Vector2>();
    public GameObject playerBase;

    public EnemyManager manager;

	// Use this for initialization
	void Start ()
    {
        health = maxHealth;
        manager = GameObject.FindGameObjectWithTag("Enemy Manager").GetComponent<EnemyManager>();
	}

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.currentGame == GameManager.GameState.PlayPhase)
        {
            //moves the enemy to its next point (VERY simple and not optimized)
            if (path.Peek().x < transform.position.x)
            {
                this.transform.Translate(-speed * Time.deltaTime, 0, 0);
            }
            if (path.Peek().x > transform.position.x)
            {
                this.transform.Translate(speed * Time.deltaTime, 0, 0);
            }
            if (path.Peek().y < transform.position.z)
            {
                this.transform.Translate(0, 0, -speed * Time.deltaTime);
            }
            if (path.Peek().y > transform.position.z)
            {
                this.transform.Translate(0, 0, speed * Time.deltaTime);
            }
            if (Vector2.Distance(path.Peek(), new Vector2(transform.position.x, transform.position.z)) <= .5f)
            {
                path.Dequeue();
                //Damage(1);
            }
            //destroy the object if it reached the end of its path (the base)
            if (path.Count == 0)
            {
                playerBase.GetComponent<BaseScript>().LoseHealth();
                manager.DestroyEnemy(gameObject);
            }
            if (health <= 0)
            {
                manager.DestroyEnemy(gameObject);
            }
        }
    }

    //copies a list into a queue
    public void CopyList(List<Vector2> incoming)
    {
        for(int i = 0; i < incoming.Count; i++)
        {
            path.Enqueue(incoming[i]);
        }
    }

    public void Damage(int damage)
    {
        health -= damage;

        transform.GetChild(0).GetChild(0).localScale = new Vector3(1f * health / maxHealth, 1, 1);
    }
}
