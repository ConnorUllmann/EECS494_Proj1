using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

    public static List<List<Tile>> tiles = new List<List<Tile>>();

    public static List<string> opened = new List<string>();
    public static List<Tile> redDoors = new List<Tile>();
    public static List<Tile> greenDoors = new List<Tile>();
    public static List<Tile> sealedDoors = new List<Tile>();

    static Sprite[]         spriteArray;

    public Texture2D        spriteTexture;
	public int				x, y;
	public int				tileNum;
	private BoxCollider		bc;
    private Material        mat;

    private char type; //The character that represents this tile in the collision map.
    public bool open = true; //Whether this tile is unlocked (if it is a door).

    private SpriteRenderer  sprend;

	void Awake() {
        if (spriteArray == null) {
            spriteArray = Resources.LoadAll<Sprite>(spriteTexture.name);
        }

		bc = GetComponent<BoxCollider>();

        sprend = GetComponent<SpriteRenderer>();
        //Renderer rend = gameObject.GetComponent<Renderer>();
        //mat = rend.material;
	}

    void Update()
    {
        if (tag == "Door" && (type == 'L' || type == 'R' || type == 'G'))
        {
            //If we aren't open, but the static list of opened doors says this spot is open, then open up.
            if(!open && opened.Contains(gameObject.name))
                Open();
            /*
            if (PlayerControl.S.keys > 0)
            {
                bc.center = Vector3.zero;
                bc.size = new Vector3(0.25f, 0.25f, 0.25f);
                bc.isTrigger = true;
            }
            else
            {
                bc.size = new Vector3(1, 1, 1);
                bc.isTrigger = false;
            }
            */
            if (open)
                bc.isTrigger = true;
        }
    }

    public void Open()
    {
        if (!open) {
            open = true;

            //Locked doors
            if (tileNum == 80 || tileNum == 81)
                tileNum += 12;
            else if (tileNum == 101 || tileNum == 98 || tileNum == 114)
                tileNum = 48;
            else if (tileNum == 106 || tileNum == 115 || tileNum == 99)
                tileNum = 51;
            else if (tileNum == 100)
            {
                tileNum = Utils.CollidingWithLeftWall(transform.position) ? 51 : 48;
            }

            //Red Doors
            else if (tileNum == 96 || tileNum == 97)
                tileNum -= 4;
            else if (tileNum == 144 || tileNum == 145)
                tileNum -= 118;

            //Green Doors
            else if (tileNum == 112 || tileNum == 113)
                tileNum -= 20;
            else if (tileNum == 128 || tileNum == 129)
                tileNum -= 102;

            //Non-locked sealed doors
            else if (tileNum == 100)
                tileNum = (Utils.CollidingWithLeftWall(transform.position) ? 51 : 48);

            sprend.sprite = spriteArray[tileNum];
            if(!opened.Contains(gameObject.name))
                opened.Add(gameObject.name);
            /*for(int i = 0; i < opened.Count; i++)
            {
                Debug.Log(opened[i]);
            }*/

            bc.center = Vector3.zero;
            bc.size = new Vector3(0.25f, 0.25f, 0.25f);
            bc.isTrigger = true;

            //Open up tiles to the left/right of us, to open up the other half of doors
            //Won't matter if it gets called on a non-locked door because open is already true by default.

            if (GetTile(transform.position + new Vector3(-1, 0, 0)) != null)
                GetTile(transform.position + new Vector3(-1, 0, 0)).Open();

            if (GetTile(transform.position + new Vector3(1, 0, 0)) != null)
                GetTile(transform.position + new Vector3(1, 0, 0)).Open();
        }
    }

    public void Close() {
        if(open) {
            open = false;

            if (opened.Contains(gameObject.name))
                opened.Remove(gameObject.name);

            bool isRedDoor = redDoors.Contains(this);//This function should only be called on red and green doors

            if (tileNum == 48)
                tileNum = (isRedDoor ? 98 : 114);
            else if (tileNum == 51)
                tileNum = (isRedDoor ? 99 : 115);
            else if (tileNum == 92 || tileNum == 93)
                tileNum += (isRedDoor ? 4 : 20);
            else if (tileNum == 26 || tileNum == 27)
                tileNum += (isRedDoor ? 118 : 102);


            sprend.sprite = spriteArray[tileNum];
            bc.center = Vector3.zero;
            bc.size = new Vector3(1, 1, 1);
            bc.isTrigger = false;
        }
    }

    //Returns the tile at the given position.
    public static Tile GetTile(Vector3 pos)
    {
        int x_ = (int)pos.x;
        int y_ = (int)pos.y;
        //Debug.Log("(" + x_ + ", " + y_ + ") " + tiles.Count + ", " + (tiles[x_] == null ? "null" : tiles[x_].Count.ToString()));
        if (x_ < 0 || x_ >= tiles.Count || y_ < 0 || y_ >= tiles[x_].Count)
            return null;
        return tiles[x_][y_];
    }
    //Returns whether the tile at the given position is walkable.
    public static bool Unwalkable(Vector3 pos)
    {
        var tile = GetTile(pos);
        if (tile != null)
        {
            var collider = tile.GetComponent<BoxCollider>();
            if (collider != null)
            {
                return collider.enabled;
            }
        }
        return false;
    }

	public void SetTile(int eX, int eY, int eTileNum = -1) {
		if (x == eX && y == eY) return; // Don't move this if you don't have to. - JB

        x = eX;
        y = eY;

        while (x >= tiles.Count)
        {
            tiles.Add(new List<Tile>());
        }
        while(y >= tiles[x].Count)
        {
            tiles[x].Add(null);
        }
        tiles[x][y] = this;

		transform.localPosition = new Vector3(x, y, 0);
        gameObject.name = x.ToString("D3")+"x"+y.ToString("D3");

		tileNum = eTileNum;
		if (tileNum == -1 && ShowMapOnCamera.S != null) {
			tileNum = ShowMapOnCamera.MAP[x,y];
			if (tileNum == 0) {
				ShowMapOnCamera.PushTile(this);
			}
		}

        sprend.sprite = spriteArray[tileNum];

		if (ShowMapOnCamera.S != null) SetCollider();
        //TODO: Add something for destructibility - JB

        gameObject.SetActive(true);
		if (ShowMapOnCamera.S != null) {
			if (ShowMapOnCamera.MAP_TILES[x,y] != null) {
				if (ShowMapOnCamera.MAP_TILES[x,y] != this) {
					ShowMapOnCamera.PushTile( ShowMapOnCamera.MAP_TILES[x,y] );
				}
			} else {
				ShowMapOnCamera.MAP_TILES[x,y] = this;
			}
		}
	}


	// Arrange the collider for this tile
	void SetCollider() {
        
        // Collider info from collisionData
        bc.enabled = true;
        char c = ShowMapOnCamera.S.collisionS[tileNum];
        type = c;
        switch (c) {
            case 'S': // Solid
                gameObject.tag = "Untagged";
                bc.center = Vector3.zero;
                bc.size = Vector3.one;
                break;
            case 'D': //Door
                gameObject.tag = "Door";
                bc.center = Vector3.zero;
                bc.size = new Vector3(.25f, .25f, .25f);
                bc.isTrigger = true;
                break;
            case 'L': //Locked Door
                gameObject.tag = "Door";
                open = false;
                break;
            case 'C'://Sealed doors that open on clear
                gameObject.tag = "Door";
                open = false;
                if (!sealedDoors.Contains(this))
                    sealedDoors.Add(this);
                break;
            case 'W': //Water
                gameObject.tag = "Untagged";
                gameObject.layer = 4;
                bc.center = Vector3.zero;
                bc.size = Vector3.one;
                break;
            case '2'://2D
                gameObject.tag = "2D";
                bc.center = Vector3.zero;
                bc.size = new Vector3(.9f, 1.1f, .8f);
                bc.isTrigger = true;
                break;
            case 'R'://Red Doors
                gameObject.tag = "Door";
                open = false;
                if(RedSwitch.areOpen && !opened.Contains(gameObject.name))
                    opened.Add(gameObject.name);
                if (!redDoors.Contains(this))
                    redDoors.Add(this);
                break;
            case 'G':
                gameObject.tag = "Door";
                open = false;
                if (GreenSwitch.areOpen && !opened.Contains(gameObject.name))
                    opened.Add(gameObject.name);
                if (!greenDoors.Contains(this))
                    greenDoors.Add(this);
                break;
            default:
                gameObject.tag = "Untagged";
                bc.enabled = false;
                break;
        }
	}	

   /* void OnTriggerEnter(Collider coll) {
        //Behavior for doors
        if(tag == "Door" && coll.gameObject.tag == "Hero") {
            GameObject otherSide = FindTileWithTag("Door", 3.0f);
            if(otherSide == null) { //if there is no door found do nothing
                return;
            }

            Vector3 directionOffset = otherSide.transform.position - transform.position;
            coll.transform.position = otherSide.transform.position + (directionOffset / directionOffset.magnitude); //place the hero 1 tile past the door
            
        }
    }


    //Searches for a tile within distance units of this one with the given tag
    //Returns null if no tile is found
    public GameObject FindTileWithTag(string tag, float distance) {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, distance);
        foreach(Collider c in hitColliders) {
            if (c.gameObject.tag == tag && c.gameObject != this.gameObject) return c.gameObject;
        }

        return null;
    }*/

}