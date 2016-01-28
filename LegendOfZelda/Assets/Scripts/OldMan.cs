using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OldMan : MonoBehaviour {

    public Text script;
    private string text;

    private float timePerChar = 0.125f;
    private float timeStart;
    private int chars = 0;

	// Use this for initialization
	void Awake ()
    {
        text = (string)script.text.Clone();
        script.text = "";

        timeStart = Time.time;
    }
	
	// Update is called once per frame
	void Update ()
    {
        script.enabled = Utils.AreInSameRoom(PlayerControl.S.gameObject, gameObject);

        if (script.enabled)
        {
            if (Time.time - timeStart >= timePerChar)
            {
                timeStart += timePerChar;
                chars = Mathf.Min(chars + 1, text.Length);
            }
            script.text = text.Substring(0, chars);
        }
        else
        {
            timeStart = Time.time;
        }
    }
}
