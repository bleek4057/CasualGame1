using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTower : MonoBehaviour
{

    public int cost;

    public List<EnemyScript> enemies = new List<EnemyScript>();
    public GameObject fakeVersion;

    protected SphereCollider rangeSphere;
    public float _timer = 0.0f;

    public float Timer { get { return _timer; } }

    public float _fireRate = 0.333f;
    public float range = 0.0f;

    public bool canAttack = true;

    Color defaultMainColor;
    public List<GameObject> toBeColored;
    List<Color> defaultColor;

    public int damagePerHit;

    public bool canBeControlled = true;
    public bool controlled = false;
    public bool isBase = false;

    // Use this for initialization
    protected virtual void Start()
    {
        rangeSphere = gameObject.GetComponent<SphereCollider>();

        defaultColor = new List<Color>();
        defaultMainColor = GetComponent<Renderer>().material.color;
        foreach (GameObject child in toBeColored)
        {
            defaultColor.Add(child.GetComponent<Renderer>().material.color);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (canAttack && GameManager.Instance.currentGame == GameManager.GameState.PlayPhase)
        {
            _timer -= Time.deltaTime;
            if (enemies.Count > 0 && !controlled)
            {
                enemies.RemoveAll(enemy => enemy == null);

                if (_timer <= 0)
                {
                    Attack();
                }
            }
        }
        SetColor(true);
        if (GameManager.Instance.currentGame == GameManager.GameState.BuildPhase)
        {
            if (GameManager.Instance.towerMouseOver == gameObject)
            {
                SetColor(false);
            }
        }
    }

    public void SetColor(bool def)
    {
        if (!def)
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
        if (obj.tag == "Enemy")
        {
            var potentialEnemy = obj.gameObject.GetComponent<EnemyScript>();
            enemies.Remove(potentialEnemy);
        }
    }

    public virtual void Attack() { }
    public virtual void Attack(EnemyScript enemy) { }
    public virtual void Attack(Vector3 position) { }
}
