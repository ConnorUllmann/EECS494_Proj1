using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

    public static List<List<Tile>> tiles = new List<List<Tile>>();

    public static List<string> opened = new List<string>();

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
        if (tag == "Door" && type == 'L')
        {
            //If we aren't open, but the static list of opened doors says this spot is open, then open up.
            if(!open && opened.Contains(gameObject.name))
                Open();

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
            if (open)
                bc.isTrigger = true;
        }
    }

    public void Open()
    {
        if (!open)
        {
            open = true;
            if (tileNum == 80 || tileNum == 81)
                tileNum += 12;
            else if (tileNum == 101)
                tileNum = 48;
            else if (tileNum == 106)
                tileNum = 51;
            sprend.sprite = spriteArray[tileNum];
            if(!opened.Contains(gameObject.name))
                opened.Add(gameObject.name);
            /*for(int i = 0; i < opened.Count; i++)
            {
                Debug.Log(opened[i]);
            }*/
            //Open up tiles to the left/right of us, to open up the other half of doors
            //Won't matter if it gets called on a non-locked door because open is already true by default.
            GetTile(transform.position + new Vector3(-1, 0, 0)).Open();
            GetTile(transform.position + new Vector3(1, 0, 0)).Open();
        }
    }

    //Returns the tile at the given position.
    public static Tile GetTile(Vector3 pos)
    {
        int x_ = (int)pos.x;
        int y_ = (int)pos.y;
        //Debug.Log("(" + x_ + ", " + y_ + ") " + pos);
        if (x_ >= tiles.Count || y_ >= tiles[x_].Count)
            return null;
        return tiles[x_][y_];
    }
    //Returns whether the tile at the given position is walkable.
    public static bool Unwalkable(Vector3 pos)
    {
        var tile = GetTile(pos);
        BoxCollider collider = null;
        if (tile != null)
            collider = tile.GetComponent<BoxCollider>();
        return tile != null && collider != null && collider.enabled;
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
            case 'W': //Water
                gameObject.tag = "Untagged";
                gameObject.layer = 4;
                bc.center = Vector3.zero;
                bc.size = Vector3.one;
                break;
            case '2'://2D
                gameObject.tag = "2D";
                bc.center = Vector3.zero;
                bc.size = new Vector3(.8f, .8f, .8f);
                bc.isTrigger = true;
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