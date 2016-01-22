using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

    public static List<List<Tile>> tiles = new List<List<Tile>>();


    static Sprite[]         spriteArray;

    public Texture2D        spriteTexture;
	public int				x, y;
	public int				tileNum;
	private BoxCollider		bc;
    private Material        mat;

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
        var collider = tile.GetComponent<BoxCollider>();
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
        switch (c) {
            case 'S': // Solid
                bc.center = Vector3.zero;
                bc.size = Vector3.one;
                break;
            case 'D': //Door
                tag = "Door";
                bc.center = Vector3.zero;
                bc.size = new Vector3(.25f, .25f, .25f);
                bc.isTrigger = true;
                break;
            default:
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