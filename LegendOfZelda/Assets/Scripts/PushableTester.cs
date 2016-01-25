using UnityEngine;
using System.Collections;

public class PushableTester : MonoBehaviour {

    GameObject parent;
    PushableBlock pb;

	// Use this for initialization
	void Start () {
	
	}

    public void SetParent(GameObject _parent)
    {
        parent = _parent;
        pb = parent.GetComponent<PushableBlock>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Hero")
        {
            if (pb.CheckContinueMoving())
            {
                pb.moving = true;
                pb.dTimer = pb.dTimerMax;
            }
        }
    }
    void OnTriggerStay(Collider c)
    {
        if (c.gameObject.tag == "Hero")
        {
            if (pb.CheckContinueMoving())
            {
                pb.moving = true;
                pb.dTimer = pb.dTimerMax;
            }
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag == "Hero")
            pb.moving = false;
    }
}
