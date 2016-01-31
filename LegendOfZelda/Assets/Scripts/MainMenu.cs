using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {
    public Text[] menuTexts;
    public string[] menuWords;

    public string classicLevel;
    public string customLevel;

    private int menuPointer = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //Moving menu pointer
	    if(Input.GetKeyDown(KeyCode.DownArrow) && menuPointer != menuTexts.Length-1)
            ++menuPointer;
        if (Input.GetKeyDown(KeyCode.UpArrow) && menuPointer != 0)
            --menuPointer;

        if(Input.GetKeyDown(KeyCode.A)) {
            switch(menuPointer) {
                case 0:
                    SceneManager.LoadScene(classicLevel);
                    break;
                case 1:
                    SceneManager.LoadScene(customLevel);
                    break;
                case 2:
                    Application.Quit();
                    break;
            }
        }

        for(int i=0; i<menuTexts.Length; ++i) {
            menuTexts[i].text = (i == menuPointer ? ">" : "") + menuWords[i];
        }


	}
}
