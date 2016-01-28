using UnityEngine;
using System.Collections;

public class SwordExplosivePiece : MonoBehaviour {

    public int x_dir;
    public int y_dir;
    public float speed;
    public float time;

    float lastFrame;
    float dt;

	// Use this for initialization
	void Start () {
        lastFrame = Time.time;
	}

    void Update()
    {
        dt = Time.time - lastFrame;

        GetComponent<Rigidbody>().velocity = new Vector3(x_dir, y_dir, 0).normalized * speed;
        time -= dt;
        if (time <= 0)
            Destroy(this.gameObject);

        lastFrame = Time.time;
    }
}
