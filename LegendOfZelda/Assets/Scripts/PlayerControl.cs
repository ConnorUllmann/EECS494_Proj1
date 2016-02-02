using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public enum Direction {NORTH, EAST, SOUTH, WEST};
public enum EntityState {NORMAL, ATTACKING, STUNNED};

public class PlayerControl : MonoBehaviour {

    public static PlayerControl S;

    public bool isCustomlevel = false;

    public bool twoDmovement = false;

    public float walking_velocity = 1.0f;
    public int rupee_count = 0;
    public int keys = 0; //Number of keys the player has.
    public int bombs = 0;
    public float maxhealth = 3.0f;
    public float health;
    public bool bInvincible = false;
    public float maxInvincibilityTimer = 3.0f;
    private float invincibiltyTimer = 0.0f;

    public float stunTimer = .5f;

    private float roomSize = 16f;
    public float doorMoveOffset = 0.7f;

    public GameObject shield;

    public Sprite[] link_run_down;
    public Sprite[] link_run_up;
    public Sprite[] link_run_right;
    public Sprite[] link_run_left;

    public Sprite[] link_run_down_invincible;
    public Sprite[] link_run_up_invincible;
    public Sprite[] link_run_right_invincible;
    public Sprite[] link_run_left_invincible;

    public Sprite[] down_invincible;
    public Sprite[] up_invincible;
    public Sprite[] right_invincible;
    public Sprite[] left_invincible;

    StateMachine animation_state_machine;
    StateMachine control_state_machine;

    public EntityState current_state = EntityState.NORMAL;
    public Direction current_direction = Direction.SOUTH;

    public GameObject selected_weapon_prefab_A_button;
    public GameObject selected_weapon_prefab_B_button;
    public bool canBoomerang = true;
    public bool hasBow = true;
    public bool canUseDoor = true;
    private bool lookBehind = false;
    public bool canLaserSword = true;
    private bool permanentInvincibility = false;

    public Vector3 start_point;
    // Use this for initialization
    void Awake() {
        if (S != null)
            Debug.LogError("Multiple players!");
        S = this;
        start_point = transform.position;
        health = maxhealth;

        animation_state_machine = new StateMachine();
        animation_state_machine.ChangeState(new StateIdleWithSprite(this, GetComponent<SpriteRenderer>(), link_run_down[0]));

        control_state_machine = new StateMachine();
        control_state_machine.ChangeState(new StateLinkNormalMovement(this));

        roomPos = new Vector3(-1000000, -1000000);
    }


    Vector3 roomPos;

    private bool alreadyDying = false;

    public float timeToSpawnEnemyInRoom = 2.0f;
    private float enemySpawnTimer;
    private bool changingRoom = false;
    // Update is called once per frame
    void Update()
    {


        if (enemySpawnTimer > 0)
            enemySpawnTimer -= Time.deltaTime;
        if(changingRoom && enemySpawnTimer <= 0) {
            ActivateObjectsInRoom();
            changingRoom = false;
        }
        



        if(GetComponent<Rigidbody>().rotation.x != 0 ||
            GetComponent<Rigidbody>().rotation.y != 0 ||
            GetComponent<Rigidbody>().rotation.z != 0) {

            Quaternion rot = new Quaternion();
            rot.eulerAngles = new Vector3(0, 0, 0);
            GetComponent<Rigidbody>().rotation = rot;
        }

        if(health <= 0 && !alreadyDying) {
            alreadyDying = true;
            control_state_machine.ChangeState(new StateLinkDead(2.0f));
        }

        if(Input.GetKeyDown(KeyCode.I)) {
            permanentInvincibility = (permanentInvincibility ? false : true);
        }

        var roomPosNew = new Vector3(Utils.GetRoomI(transform.position.x), Utils.GetRoomJ(transform.position.y));
        if (roomPos != roomPosNew)
        {
            //ActivateObjectsInRoom();
            roomPos = roomPosNew;
        }

        animation_state_machine.Update();
        control_state_machine.Update();
        if (control_state_machine.IsFinished())
            control_state_machine.ChangeState(new StateLinkNormalMovement(this));
        if (bInvincible) {
            invincibiltyTimer -= Time.deltaTime;
            if (invincibiltyTimer <= 0) {
                bInvincible = false;
                invincibiltyTimer = maxInvincibilityTimer;
            }
        }


        //Debug.Log(Utils.GetTileInRoomI(transform.position.x) + ", " + Utils.GetTileInRoomJ(transform.position.y));


        switch (current_direction)
        {
            case Direction.NORTH:
                shield.transform.position = Vector3.zero + transform.position;
                shield.GetComponent<BoxCollider>().size = new Vector3(1, 0.2f, 0.2f);
                break;
            case Direction.EAST:
                shield.transform.position = new Vector3(0.5f, 0, 0) + transform.position;
                shield.GetComponent<BoxCollider>().size = new Vector3(0.2f, 1, 0.2f);
                break;
            case Direction.SOUTH:
                shield.transform.position = new Vector3(0, -0.5f, 0) + transform.position;
                shield.GetComponent<BoxCollider>().size = new Vector3(1, 0.2f, 0.2f);
                break;
            case Direction.WEST:
                shield.transform.position = new Vector3(-0.5f, 0, 0) + transform.position;
                shield.GetComponent<BoxCollider>().size = new Vector3(0.2f, 1, 0.2f);
                break;
        }

        if(!GetComponent<BoxCollider>().enabled) {

           // if (GetComponent<Rigidbody>().velocity != DontGetStuckInDoors)
             //   GetComponent<Rigidbody>().velocity = DontGetStuckInDoors;

            Vector3 nextCell = new Vector3((int)transform.position.x + (GetComponent<Rigidbody>().velocity.normalized.x), (int)transform.position.y + (GetComponent<Rigidbody>().velocity.normalized.y), 0);
            if(!Tile.Unwalkable(nextCell)) {
                lookBehind = true;
            } 
        }
        if(lookBehind && Tile.GetTile(transform.position).gameObject.tag == "Door") {
            GetComponent<BoxCollider>().enabled = true;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            lookBehind = false;
            enemySpawnTimer = timeToSpawnEnemyInRoom;
            changingRoom = true;

            /*
            if (!Tile.Unwalkable(transform.position)) {
                GetComponent<BoxCollider>().enabled = true;
                GetComponent<Rigidbody>().velocity = Vector3.zero;

                canUseDoor = Tile.GetTile(transform.position).gameObject.tag != "Door";

                lookBehind = false;
                //canUseDoor = true;
                ActivateObjectsInRoom();
                return;
            }

        */
            /*
            Vector3 prevCell = new Vector3((int)transform.position.x - (GetComponent<Rigidbody>().velocity.normalized.x), (int)transform.position.y - (GetComponent<Rigidbody>().velocity.normalized.y), 0);
            if(!Tile.Unwalkable(prevCell)) {
                GetComponent<BoxCollider>().enabled = true;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                lookBehind = false;
                //canUseDoor = true;
                ActivateObjectsInRoom();
            }
            */
        }

    }

    public List<string> OffscreenStopNames; //Names for objects that will be disabled when offscreen
    private List<GameObject> deactivated = new List<GameObject>(); //Objects that have been deactivated by the player for not being in the same room.
    //Activates objects in the same room as the player and deactivates those that aren't.
    void ActivateObjectsInRoom()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (!TestDeactivateObject(go))
            {
                deactivated.Add(go);
            }
        }
        List<GameObject> newlyActive = new List<GameObject>();
        foreach (GameObject go in deactivated)
        {
            if (TestDeactivateObject(go))
            {
                newlyActive.Add(go);
            }
        }
        for (int i = 0; i < newlyActive.Count; i++)
        {
            deactivated.Remove(newlyActive[i]);
        }
        //Debug.Log("Deactive objects: " + deactivated.Count);
    }
    //Activates/deactivates an individual objects based on whether or not it is in the same room as the player.
    bool TestDeactivateObject(GameObject go)
    {
        for (int i = 0; i < OffscreenStopNames.Count; i++)
        {
            if (go.name.Contains(OffscreenStopNames[i]))
            {
                bool active = Utils.AreInSameRoom(go, PlayerControl.S.gameObject);
                go.SetActive(active);
                return active;
            }
        }
        return go.activeSelf;
    }

    public Vector3 ShieldVector()
    {
        return Utils.DirectionToVector(current_direction);
    }

    public Vector3 Bounce(Vector3 v)
    {
        switch(current_direction)
        {
            case Direction.NORTH:
                return new Vector3(v.x, Mathf.Abs(v.y), v.z);
            case Direction.EAST:
                return new Vector3(Mathf.Abs(v.x), v.y, v.z);
            case Direction.SOUTH:
                return new Vector3(v.x, -Mathf.Abs(v.y), v.z);
            case Direction.WEST:
                return new Vector3(-Mathf.Abs(v.x), v.y, v.z);
        }
        return Vector3.zero;
    }

    void OnTriggerEnter(Collider coll) {
        switch (coll.gameObject.tag) {
            case "Rupee":
                Destroy(coll.gameObject);
                rupee_count++;
                break;
            case "Heart":
                Destroy(coll.gameObject);
                if (health < maxhealth) {
                    ++health;
                }
                break;
            case "Key":
                Destroy(coll.gameObject);
                ++keys;
                break;
            case "BombPickup":
                Destroy(coll.gameObject);
                ++bombs;
                break;
            case "Triforce":
                Destroy(coll.gameObject);
                health = maxhealth;

                control_state_machine.ChangeState(new StateLinkVictory(3.0f));

                break;
            case "HeartPiece":
                Destroy(coll.gameObject);
                ++maxhealth;
                break;

            case "BowPickup":
                Destroy(coll.gameObject);
                PauseMenu.S.hasBow = true;

                if(isCustomlevel && PauseMenu.S.hasBoomerang) {
                    PauseMenu.S.hasBoomerang = false;
                    selected_weapon_prefab_B_button = null;
                    PauseMenu.S.usedBWeapon = -1;
                }

                break;
            case "BoomerangPickup":
                Destroy(coll.gameObject);
                PauseMenu.S.hasBoomerang = true;

                if (isCustomlevel && PauseMenu.S.hasBow) {
                    PauseMenu.S.hasBow = false;
                    selected_weapon_prefab_B_button = null;
                    PauseMenu.S.usedBWeapon = -1;
                }

                break;
            case "MapPickup":
                Destroy(coll.gameObject);
                PauseMenu.S.hasMap = true;
                break;
            case "CompassPickup":
                Destroy(coll.gameObject);
                PauseMenu.S.hasCompass = true;
                break;


            case "Enemy":
            case "EnemyProjectile":
                if (!bInvincible && !permanentInvincibility)
                {
                    --health;
                    bInvincible = true;

                    current_state = EntityState.STUNNED;
                    control_state_machine.ChangeState(new StateLinkStunned(this, stunTimer));
                }

                break;
            case "Door":
                if (canUseDoor) {

                    if (!coll.gameObject.GetComponent<Tile>().open)
                    {
                        if (keys > 0)
                        {
                            keys--;
                            Debug.Log("Keys: " + keys);
                            coll.gameObject.GetComponent<Tile>().Open();
                        }
                    }

                    if (coll.gameObject.GetComponent<Tile>().open)
                    {
                        canUseDoor = false;
                        if (!MoveToNextRoom(coll))
                            canUseDoor = true;
                    }
                }
                break;
            case "Wallmaster":
                transform.position = start_point;
                break;

            case "2D":
                GoToMiddleOfTileYPosition();
                twoDmovement = true;
                break;

            case "BowRoomEntrance":
                control_state_machine.ChangeState(new StateLinkStunned(this, 1.0f));
                transform.position = coll.GetComponent<TeleportToBowRoom>().target.transform.position;
                break;
        }
    }

    void OnCollisionEnter(Collision coll) {
        if(coll.gameObject.tag == "Door") {
            if (!coll.collider.GetComponent<Tile>().open &&
                !Tile.redDoors.Contains(coll.collider.GetComponent<Tile>()) &&
                !Tile.greenDoors.Contains(coll.collider.GetComponent<Tile>()) &&
                keys > 0) {
                --keys;
                coll.collider.GetComponent<Tile>().Open();
            }
        }
    }

    public void GoToMiddleOfTileYPosition() {
        var temp = transform.position;
        temp.y = (int)Mathf.Round(temp.y);
        transform.position = temp;
    }

    void OnTriggerExit(Collider coll) {
        if(coll.gameObject.tag == "2D") {
            twoDmovement = false;
        }

        if (coll.gameObject.tag == "Door")
            canUseDoor = true;
    }


    private Vector3 DontGetStuckInDoors;
    bool MoveToNextRoom(Collider thisDoor) {
        //Check if there is another door to go to
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 6.0f);
        GameObject nextDoor = null;

        foreach (Collider c in hitColliders) {
            if (c.gameObject.tag == "Door" && Vector3.Distance(thisDoor.gameObject.transform.position, c.transform.position) > 2.0f && Vector3.Distance(thisDoor.gameObject.transform.position, c.transform.position) < 5.0f) {
                nextDoor = c.gameObject;
                
            }
        }

        if (nextDoor == null) {
            Debug.Log("No door to go to");
            return false;
        }


        control_state_machine.ChangeState(new StateLinkStunned(this, 1.5f));
        //pauseCurrentRoom(4.0f);

        Vector3 door_direction = Vector3.zero;

        if (Utils.CollidingWithTopWall(transform.position) || Utils.CollidingWithTopWall(transform.position + new Vector3(0, .5f, 0))) {
            door_direction = new Vector3(0, 1, 0);
        } else if (Utils.CollidingWithBottomWall(transform.position) || Utils.CollidingWithBottomWall(transform.position + new Vector3(0, -.5f, 0))) {
            door_direction = new Vector3(0, -1, 0);
        } else if (Utils.CollidingWithRightWall(transform.position) || Utils.CollidingWithRightWall(transform.position + new Vector3(.5f, 0, 0))) {
            door_direction = new Vector3(1, 0, 0);
        } else if (Utils.CollidingWithLeftWall(transform.position) || Utils.CollidingWithLeftWall(transform.position + new Vector3(-.5f, 0, 0))) {
            door_direction = new Vector3(-1, 0, 0);
        }

        if(door_direction == Vector3.zero) {
            return false;
        }

        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().velocity = door_direction * walking_velocity;
        DontGetStuckInDoors = GetComponent<Rigidbody>().velocity;
        return true;

        /*Collider[] hitColliders = Physics.OverlapSphere(transform.position, 6.0f);
        GameObject nextDoor = null;

        foreach (Collider c in hitColliders) {
            if (c.gameObject.tag == "Door" && Vector3.Distance(thisDoor.gameObject.transform.position, c.transform.position) > 2.0f && Vector3.Distance(thisDoor.gameObject.transform.position, c.transform.position) < 5.0f) {
                if (nextDoor == null) {
                    nextDoor = c.gameObject;
                } 
            }
        }

        if (nextDoor == null) {
            Debug.Log("No door to go to");
            return;
        }

        Vector3 destinationLocation = nextDoor.transform.position;

        Vector3 sourceLocation = thisDoor.transform.position;

        Vector3 directionOffset = destinationLocation - sourceLocation;


        Vector3 newPos = destinationLocation + ((directionOffset.normalized) * doorMoveOffset);
        transform.position = newPos;

        pauseCurrentRoom(2.0f);
        */

    }



    public void pauseCurrentRoom(float pauseDuration) {
        control_state_machine.ChangeState(new StateLinkStunned(this, pauseDuration));
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, roomSize);
        foreach(Collider c in hitColliders) {
            if (c.gameObject.tag == "Enemy") {
                c.gameObject.GetComponent<Enemy>().state_machine.ChangeState(new StateEnemyStunned(c.gameObject.GetComponent<Enemy>(), pauseDuration));
            }
        }
    }
}
