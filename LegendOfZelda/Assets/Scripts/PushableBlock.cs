using UnityEngine;
using System.Collections;

public class PushableBlock : MonoBehaviour {

    private Vector3 start;
    public Vector3 dir;
    public GameObject testCube;
    public float t;
    public float tSpeed;
    public float tSpeedBack;
    public float dTimerMax;
    public float dTimer;
    public bool moving;

    private float timeLastFrame;

    // Use this for initialization
    void Start () {
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z);
        start = transform.position;

        testCube.GetComponent<PushableTester>().SetParent(this.gameObject);

        timeLastFrame = Time.time;
	}

	// Update is called once per frame
	void Update ()
    {
        var diff = Time.time - timeLastFrame;

        testCube.GetComponent<BoxCollider>().size = new Vector3(dir.x != 0 ? 0.2f : 1f, dir.y != 0 ? 0.2f : 1f, 0.2f);
        testCube.transform.position = transform.position - dir / 2;

        if (t >= 1)
        {

        }
        else if (moving)
        {
            moving = CheckContinueMoving();
            if (moving)
            {
                t = Mathf.Min(t + diff * tSpeed, 1);
                var p1 = transform.position;
                var p2 = NextPos;
                PlayerControl.S.transform.position += p2 - p1;

                if(t >= 1)
                {
                    
                }
            }
        }
        else if (dTimer > 0)
        {
            dTimer = Mathf.Max(dTimer - diff, 0);
        }
        else
        {
            t = Mathf.Max(t - diff * tSpeedBack, 0);
        }
        
        transform.position = NextPos;

        timeLastFrame = Time.time;
	}

    Vector3 NextPos
    {
        get
        {
            return start + dir * t;
        }
    }
    

    public bool CheckContinueMoving()
    {
        var g = PlayerControl.S;

        bool button_down = false;
        if (dir.x == 1)
            button_down = Input.GetKey(KeyCode.RightArrow);
        if (dir.x == -1)
            button_down = Input.GetKey(KeyCode.LeftArrow);
        if (dir.y == 1)
            button_down = Input.GetKey(KeyCode.UpArrow);
        if (dir.y == -1)
            button_down = Input.GetKey(KeyCode.DownArrow);

        var v0 = (Utils.DirectionToVector(g.GetComponent<PlayerControl>().current_direction).normalized - dir.normalized).magnitude < 0.1f;
        var v1 = Utils.Sign(transform.position.x - g.transform.position.x) == Utils.Sign(dir.x) || dir.x == 0;
        var v2 = Utils.Sign(transform.position.y - g.transform.position.y) == Utils.Sign(dir.y) || dir.y == 0;
        var v3 = button_down;
        var v = v0 && v1 && v2 && v3;
        return v;
    }
}
