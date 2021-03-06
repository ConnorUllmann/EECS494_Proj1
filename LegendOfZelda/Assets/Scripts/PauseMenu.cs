﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour {
    public static PauseMenu S; //Establish a singleton

    public GameObject pauseMenuPanel;
    public GameObject[] weaponPrefabs;
    public string[] weaponNames;
    public Text[] weaponTexts;

    public Text healthText;
    public Text rupeeText;
    public Text keysText;
    public Text BombCountText;

    public GameObject MapImage;
    public bool hasMap = false;

    public GameObject CompassController;
    public bool hasCompass = false;

    public bool hasBow;
    public bool hasBoomerang;

    private RectTransform rt;
    private bool isPaused = false;
    private int currentMenuPointer = 0;
    public int usedAWeapon = 0;
    public int usedBWeapon = -1;

    private bool selectPaused;

    public Text returningToLevelSelect;
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
        healthText.text = "Health:  " + PlayerControl.S.health.ToString() + "/" + PlayerControl.S.maxhealth.ToString();
        keysText.text = "Keys:    " + PlayerControl.S.keys.ToString();
        BombCountText.text = "Bombs:    " + PlayerControl.S.bombs.ToString();


        if (hasMap != MapImage.GetComponent<Image>().enabled) {
            MapImage.GetComponent<Image>().enabled = hasMap;
        }

        if(hasCompass != CompassController.activeSelf) {
            CompassController.SetActive(hasCompass);
        }

        if (Input.GetKeyDown(KeyCode.Return) && PlayerControl.S.GetComponent<BoxCollider>().enabled && !selectPaused) {
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

        if(!isPaused && Input.GetKeyDown(KeyCode.RightShift) && PlayerControl.S.GetComponent<BoxCollider>().enabled) {

            if (!selectPaused) {
                selectPaused = true;
                returningToLevelSelect.text = "PAUSED";
            }
            else {
                selectPaused = false;
                returningToLevelSelect.text = " ";
            }
        }

        if (selectPaused)
            PlayerControl.S.pauseCurrentRoom(.5f);


        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            SceneManager.LoadScene("Dungeon");

        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            SceneManager.LoadScene("CustomLevel");


        if(isPaused) {
            PlayerControl.S.pauseCurrentRoom(1.0f);

            if(Input.GetKeyDown(KeyCode.DownArrow) && currentMenuPointer < (weaponTexts.Length-1)) {
                ++currentMenuPointer;
            }
            if(Input.GetKeyDown(KeyCode.UpArrow) && currentMenuPointer > 0) {
                --currentMenuPointer;
            }


            //Turns out you can't have anything other than sword on A
            /*
            if(Input.GetKeyDown(KeyCode.A)) {
                PlayerControl.S.selected_weapon_prefab_A_button = weaponPrefabs[currentMenuPointer];
                usedAWeapon = currentMenuPointer;
                if (usedAWeapon == usedBWeapon) {
                    PlayerControl.S.selected_weapon_prefab_B_button = null;
                    usedBWeapon = -1;
                }
            }
            */
            if (Input.GetKeyDown(KeyCode.S)) {
                if(currentMenuPointer == 0) {
                    return;
                }

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
                if (Input.GetKeyDown(KeyCode.DownArrow))
                    ++currentMenuPointer;
                else
                    currentMenuPointer = 0;
            }

            for (int i=0; i<weaponTexts.Length; ++i) {
                string toDisplay = weaponNames[i];
                if ((i == 1 && !hasBoomerang) || (i == 2 && !hasBow)) {
                    toDisplay = " ";
                }

                if (i == usedAWeapon) {
                    toDisplay = "<color=#00ffffff>A:" + toDisplay + "</color>";
                }
                if(i == usedBWeapon) {
                    toDisplay = "<color=#ff00ffff>B:" + toDisplay + "</color>";
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
