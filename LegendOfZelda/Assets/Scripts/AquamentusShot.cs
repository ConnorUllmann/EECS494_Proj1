using UnityEngine;
using System.Collections;

public class AquamentusShot : MonoBehaviour {

    public new Sprite[] animation;
    private int frame = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().color = (Time.time * 40) % 4 <= 2 ? Color.white : Color.green;

        if(transform.position.x < 66 ||
           transform.position.x >= 77 ||
           transform.position.y < 46 ||
           transform.position.y >= 52)
        {
            Destroy(this.gameObject);
        }
            //sprite = animation[(int)((Time.time * 40) % 4 <= 2 ? 1 : 0)];
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Shield")
        {
            var rb = GetComponent<Rigidbody>();
            var rbv = rb.velocity;
            rbv = PlayerControl.S.Bounce(rbv);
            rb.velocity = rbv;
        }
    }
}
