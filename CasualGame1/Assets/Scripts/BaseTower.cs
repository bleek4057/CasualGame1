using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTower : MonoBehaviour {

    public int cost;

    public List<EnemyScript> enemies = new List<EnemyScript>();
    public GameObject fakeVersion;

    protected SphereCollider rangeSphere;
    protected float _timer = 0.0f;
    protected float _fireRate = 0.333f;

    public bool canAttack = true;

    Color defaultMainColor;
    public List<GameObject> toBeColored;
    List<Color> defaultColor;

    public Camera towerCamera;
    public Vector3 cameraPos;
    public Quaternion cameraAngle;
    public int damagePerHit;
    public GameObject cameraParent;

	// Use this for initialization
	protected virtual void Start () {
        rangeSphere = gameObject.GetComponent<SphereCollider>();

        defaultColor = new List<Color>();
        defaultMainColor = GetComponent<Renderer>().material.color;
        foreach(GameObject child in toBeColored)
        {
            defaultColor.Add(child.GetComponent<Renderer>().material.color);
        }
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if(canAttack && GameManager.Instance.currentGame == GameManager.GameState.PlayPhase)
        {
            _timer -= Time.deltaTime;
            if(enemies.Count > 0)
            {
                enemies.RemoveAll(enemy => enemy == null);

                if(_timer <= 0)
                {
                    Attack();
                }
            }
        }

        if(GameManager.Instance.currentGame == GameManager.GameState.BuildPhase)
        {
            RaycastHit hit;
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layermask = ~(1 << 9);
            bool rayCast = Physics.Raycast(mouseRay, out hit, 1000, layermask);
            if(rayCast && hit.transform.gameObject == gameObject)
            {
                GetComponent<Renderer>().material.color = Color.red;
                foreach(var go in toBeColored)
                {
                    go.GetComponent<Renderer>().material.color = Color.red;
                }
            }
            else
            {
                GetComponent<Renderer>().material.color = defaultMainColor;
                for(int i = 0; i < toBeColored.Count; i++)
                {
                    toBeColored[i].GetComponent<Renderer>().material.color = defaultColor[i];
                }
            }
        }
        else
        {
            GetComponent<Renderer>().material.color = defaultMainColor;
            for(int i = 0; i < toBeColored.Count; i++)
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

    protected virtual void Attack() { }
}
