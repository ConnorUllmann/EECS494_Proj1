using UnityEngine;
using System.Collections;

public class Goriya : Enemy {
    public GameObject goriyaBoomerangPrefab;
    public bool boomerangReturned = false;

    public Sprite[] walkUp;
    public Sprite[] walkDown;
    public Sprite[] walkLeft;
    public Sprite[] walkRight;
    public float maxWalkTime = 4.0f; //The longest time it will go without randomizing direction again (in seconds)
    public float minWalkTime = 1.0f; //The shortest time it will go before randomizing direction again

    private float cooldownTimer;

    // Use this for initialization
    void Start() {
        GoToMiddleOfTile();
        state_machine = new StateMachine();
        state_machine.ChangeState(new StateGoriyaWalk(this, GetComponent<SpriteRenderer>()));
        cooldownTimer = (Random.value * maxWalkTime) + minWalkTime;
    }

    // Update is called once per frame
    public override void Update() {
        base.Update();
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0) {
            state_machine.ChangeState(new StateGoriyaThrowBoomerang(this));
            cooldownTimer = (Random.value * maxWalkTime) + minWalkTime;
        }

        if (boomerangReturned) {
            boomerangReturned = false;
            state_machine.ChangeState(new StateGoriyaWalk(this, GetComponent<SpriteRenderer>()));
        }

        state_machine.Update();
    }
}

public class StateGoriyaWalk : State {
    Goriya g;
    SpriteRenderer renderer;
    Sprite[] animation;
    Rigidbody rb;
    float current_frame_index = 0;

    Vector3 direction;
    Vector3 nextCell;

    public StateGoriyaWalk(Goriya _g, SpriteRenderer _renderer) {
        g = _g;
        renderer = _renderer;
        rb = g.GetComponent<Rigidbody>();

        g.GoToMiddleOfTile();

        do {
            direction = Utils.RandomDirection4();
            nextCell = new Vector3((int)g.transform.position.x + (direction.x), (int)g.transform.position.y + (direction.y), 0);
        } while (Tile.Unwalkable(nextCell) || Utils.CollidingWithAnyWall(nextCell));

        if(direction.x == 1) {
            g.current_direction = Direction.EAST;
            animation = g.walkRight;
        }
        else if(direction.x == -1) {
            g.current_direction = Direction.WEST;
            animation = g.walkLeft;
        }
        else if(direction.y == 1) {
            g.current_direction = Direction.NORTH;
            animation = g.walkUp;
        }
        else if(direction.y == -1) {
            g.current_direction = Direction.SOUTH;
            animation = g.walkDown;
        }
    }

    public override void OnUpdate(float time_delta_fraction) {
        rb.velocity = direction * g.speed_max;

        var v = 0.1f * time_delta_fraction * rb.velocity.magnitude / g.speed_max;
        current_frame_index += v;
        while (current_frame_index > animation.Length)
            current_frame_index -= animation.Length;
        renderer.sprite = animation[(int)current_frame_index];
        nextCell = new Vector3((int)g.transform.position.x + (direction.x), (int)g.transform.position.y + (direction.y), 0);
        if (Tile.Unwalkable(nextCell) || Utils.CollidingWithAnyWall(nextCell)) {
            state_machine.ChangeState(new StateGoriyaWalk(g, g.GetComponent<SpriteRenderer>()));
        }
    }
}

public class StateGoriyaThrowBoomerang : State {
    Goriya g;
    GameObject weapon_instance;

    public StateGoriyaThrowBoomerang(Goriya _g) {
        g = _g;
        g.GetComponent<Rigidbody>().velocity = Vector3.zero;

        weapon_instance = MonoBehaviour.Instantiate(g.goriyaBoomerangPrefab, g.transform.position, Quaternion.identity) as GameObject;
        weapon_instance.GetComponent<GoriyaBoomerang>().g = g;

        Vector3 direction_offset = Vector3.zero;
        Vector3 direction_eulerangle = Vector3.zero;

        if (g.current_direction == Direction.NORTH) {
            direction_offset = new Vector3(0, 1, 0);
            direction_eulerangle = new Vector3(0, 0, 90);
        } else if (g.current_direction == Direction.EAST) {
            direction_offset = new Vector3(1, 0, 0);
            direction_eulerangle = new Vector3(0, 0, 0);
        } else if (g.current_direction == Direction.SOUTH) {
            direction_offset = new Vector3(0, -1, 0);
            direction_eulerangle = new Vector3(0, 0, 270);
        } else if (g.current_direction == Direction.WEST) {
            direction_offset = new Vector3(-1, 0, 0);
            direction_eulerangle = new Vector3(0, 0, 180);
        }

        weapon_instance.transform.position += direction_offset;
        Quaternion new_weapon_rotation = new Quaternion();
        new_weapon_rotation.eulerAngles = direction_eulerangle;
        weapon_instance.transform.rotation = new_weapon_rotation;
        weapon_instance.GetComponent<BoxCollider>().transform.rotation = new_weapon_rotation;
    }

}
