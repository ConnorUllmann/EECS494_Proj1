using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {
    
    public float speed;

	// Use this for initialization
	void Start ()
    {
        var direction_offset = new Vector3();
        var p = PlayerControl.S;
        if (p.current_direction == Direction.NORTH)
            direction_offset = new Vector3(0, 1, 0);
        else if (p.current_direction == Direction.EAST)
            direction_offset = new Vector3(1, 0, 0);
        else if (p.current_direction == Direction.SOUTH)
            direction_offset = new Vector3(0, -1, 0);
        else if (p.current_direction == Direction.WEST)
            direction_offset = new Vector3(-1, 0, 0);
        
        transform.position += new Vector3(0, -0.25f * (Utils.GetTileInRoomJ(p.transform.position.y) >= 9 ? 1 : 0));
        GetComponent<Rigidbody>().velocity = direction_offset * speed;
    }
	
	// Update is called once per frame
	void Update ()
    {
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.enabled &&
            (coll.gameObject.tag == "Enemy" ||
             coll.gameObject.tag == "Untagged"))
        {
            Destroy(this.gameObject);
        }
    }
}
