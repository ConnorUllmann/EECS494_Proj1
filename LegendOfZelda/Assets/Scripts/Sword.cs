using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour {

    public Sprite[] flash;
    float lastFrame;
    float dt;
    float totalTime = 0;

    public GameObject explosivePrefab;
    public float speed;

    private bool shot = false;

	// Use this for initialization
	void Start () {
        lastFrame = Time.time;
	}
    
	void Update ()
    {
        dt = Time.time - lastFrame;
        totalTime += dt;

        if (shot)
        {
            GetComponent<SpriteRenderer>().sprite = flash[(int)(totalTime * 30 % 2)];

            if(Utils.CollidingWithAnyWall(transform.position))
            {
                BlowUp();
            }
        }
        
        lastFrame = Time.time;
	}

    public void Shoot()
    {
        PlayerControl.S.canLaserSword = false;
        shot = true;
        GetComponent<Rigidbody>().velocity = new Vector3(speed * Mathf.Cos(transform.eulerAngles.z * Mathf.PI / 180), speed * Mathf.Sin(transform.eulerAngles.z * Mathf.PI / 180), 0);
    }

    public void Hit()
    {
        if (shot)
            BlowUp();
    }

    public void BlowUp()
    {
        PlayerControl.S.canLaserSword = true;
        GameObject go = Instantiate(explosivePrefab, transform.position, Quaternion.identity) as GameObject;
        Destroy(this.gameObject);
    }
}
