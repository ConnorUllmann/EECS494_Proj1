using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseMenu : MonoBehaviour {
    public static PauseMenu S; //Establish a singleton

    public GameObject pauseMenuPanel;
    public GameObject[] weaponPrefabs;
    public string[] weaponNames;
    public Text[] weaponTexts;

    public Text healthText;
    public Text rupeeText;

    public bool hasBow;
    public bool hasBoomerang;

    private RectTransform rt;
    private bool isPaused = false;
    private int currentMenuPointer = 0;
    private int usedAWeapon = 0;
    private int usedBWeapon = -1;
	// Use this for initialization
	void Awake () {
        if(S != null) {
            Debug.Log("Multiple instances of PauseMenu detected.");
        }
        S = this;

        rt = pauseMenuPanel.GetComponent<RectTransform>();
        hasBow = false;
        hasBoomerang = false;

        if(weaponPrefabs.Length != weaponTexts.Length || weaponTexts.Length!= weaponNames.Length) {
            Debug.LogError("Different number of weapon prefabs, names, and text objects assigned to PauseMenu in editor");
        }
	}
	
	// Update is called once per frame
	void Update () {
        rupeeText.text = "Rupees: " + PlayerControl.S.rupee_count.ToString();
        healthText.text = "Health:  " + PlayerControl.S.health.ToString();


        if (Input.GetKeyDown(KeyCode.Return)) {
            if (!isPaused) {
                isPaused = true;
                rt.localPosition = Vector3.zero;
            }
            else {
                isPaused = false;
                rt.localPosition = new Vector3(0, 245, 0);
                PlayerControl.S.pauseCurrentRoom(0.1f);
            }
        }

        if(isPaused) {
            PlayerControl.S.pauseCurrentRoom(1.0f);

            if(Input.GetKeyDown(KeyCode.DownArrow) && currentMenuPointer < (weaponTexts.Length-1)) {
                ++currentMenuPointer;
            }
            if(Input.GetKeyDown(KeyCode.UpArrow) && currentMenuPointer > 0) {
                --currentMenuPointer;
            }
            if(Input.GetKeyDown(KeyCode.A)) {
                PlayerControl.S.selected_weapon_prefab_A_button = weaponPrefabs[currentMenuPointer];
                usedAWeapon = currentMenuPointer;
                if (usedAWeapon == usedBWeapon) {
                    PlayerControl.S.selected_weapon_prefab_B_button = null;
                    usedBWeapon = -1;
                }
            }
            if (Input.GetKeyDown(KeyCode.S)) {
                PlayerControl.S.selected_weapon_prefab_B_button = weaponPrefabs[currentMenuPointer];
                usedBWeapon = currentMenuPointer;
                if(usedAWeapon == usedBWeapon) {
                    PlayerControl.S.selected_weapon_prefab_A_button = null;
                    usedAWeapon = -1;
                }
            }

            if(currentMenuPointer == 1 && !hasBoomerang) {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                    ++currentMenuPointer;
                else
                    --currentMenuPointer;
            }
            if(currentMenuPointer == 2 && !hasBow) {
                currentMenuPointer = 0;
            }

            for (int i=0; i<weaponTexts.Length; ++i) {
                string toDisplay = weaponNames[i];
                if ((i == 1 && !hasBoomerang) || (i == 2 && !hasBow)) {
                    toDisplay = " ";
                }

                if (i == usedAWeapon) {
                    toDisplay = "<color=#00ffffff>" + toDisplay + "</color>";
                }
                if(i == usedBWeapon) {
                    toDisplay = "<color=#ff00ffff>" + toDisplay + "</color>";
                }


                if(currentMenuPointer == i) {
                    weaponTexts[i].text = ">" + toDisplay;
                }
                else {
                    weaponTexts[i].text = toDisplay;
                }
            }

        }
        
	
	}
}
