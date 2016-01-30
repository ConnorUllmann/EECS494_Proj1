using UnityEngine;
using System.Collections;

public class ParticleEffect : MonoBehaviour {
    public float particleDuration = 1f;

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().velocity = new Vector3(0.5f -  Random.value, 0.5f - Random.value, 0.5f - Random.value);
	}
	
	// Update is called once per frame
	void Update () {
        particleDuration -= Time.deltaTime;
        if(particleDuration <= 0) {
            Destroy(gameObject);
        }
	}
}
