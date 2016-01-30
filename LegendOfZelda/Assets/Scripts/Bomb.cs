using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bomb : MonoBehaviour {
    public float fuseTime = 2.0f;
    public float bombDamage = 2.0f;
    public float bombRadius = 1.0f;

    public GameObject explosionAnimationPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        fuseTime -= Time.deltaTime;
        if(fuseTime <= 0) {
            Explode();
            Destroy(gameObject);
        }
	}

    void Explode() {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, bombRadius);
        foreach(Collider c in hitEnemies) {
            if (c.gameObject.tag == "Enemy" && c.GetComponent<Enemy>() != null)
                c.GetComponent<Enemy>().health -= bombDamage;
        }

        List<GameObject> explosion = new List<GameObject>();
        explosion.Add(Instantiate(explosionAnimationPrefab, transform.position + new Vector3(0, 0, 0), new Quaternion()) as GameObject);
        explosion.Add(Instantiate(explosionAnimationPrefab, transform.position + new Vector3(0, 0, 0), new Quaternion()) as GameObject);
        explosion.Add(Instantiate(explosionAnimationPrefab, transform.position + new Vector3(0, 0, 0), new Quaternion()) as GameObject);
        explosion.Add(Instantiate(explosionAnimationPrefab, transform.position + new Vector3(0, 0, 0), new Quaternion()) as GameObject);

        explosion[0].GetComponent<Rigidbody>().velocity = new Vector3(2, 0, 0);
        explosion[1].GetComponent<Rigidbody>().velocity = new Vector3(-2, 0, 0);
        explosion[2].GetComponent<Rigidbody>().velocity = new Vector3(0, 2, 0);
        explosion[3].GetComponent<Rigidbody>().velocity = new Vector3(0, -2, 0);

    }
}
