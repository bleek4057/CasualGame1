using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int health;
    public int speed;
    public Queue<Vector2> path = new Queue<Vector2>();

	// Use this for initialization
	void Start ()
    {
	}

    // Update is called once per frame
    void Update()
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
        }
        //destroy the object if it reached the end of its path (the base)
        if(path.Count == 0)
        {
            Destroy(gameObject);
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
}
