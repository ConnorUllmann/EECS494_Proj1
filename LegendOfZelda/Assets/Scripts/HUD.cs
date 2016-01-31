using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUD : MonoBehaviour {

    public Text rupee_text;
    public Text health_text;
    public Text key_text;
    public Text weaponAText;
    public Text weaponBText;
    public Text bombText;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        rupee_text.text = "Rupees: " + PlayerControl.S.rupee_count.ToString();
        health_text.text = "Health:   " + PlayerControl.S.health.ToString() + "/" + PlayerControl.S.maxhealth.ToString();
        key_text.text = "Keys:     " + PlayerControl.S.keys.ToString();
        bombText.text = "Bombs:    " + PlayerControl.S.bombs.ToString();

        weaponAText.text = "<color=#00ffffff>A:" + PauseMenu.S.weaponNames[PauseMenu.S.usedAWeapon] + "</color>";
        weaponBText.text = "<color=#ff00ffff>B:" + ((PauseMenu.S.usedBWeapon == -1) ? "" : PauseMenu.S.weaponNames[PauseMenu.S.usedBWeapon]) + "</color>";

    }
}
