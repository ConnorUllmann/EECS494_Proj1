using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseMenu : MonoBehaviour {
    public GameObject pauseMenuPanel;

    private RectTransform rt;
    private bool isPaused = false;
	// Use this for initialization
	void Awake () {
        rt = pauseMenuPanel.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Return)) {
            if (!isPaused) {
                Time.timeScale = 0.0f;
                isPaused = true;

                rt.localPosition = Vector3.zero;
            }
            else {
                Time.timeScale = 1.0f;
                isPaused = false;
                rt.localPosition = new Vector3(0, 245, 0);
            }
        }
        
	
	}
}
