using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParseEntities : MonoBehaviour {

    public TextAsset level;

    public List<string> names;
    public List<GameObject> prefabs;

	// Use this for initialization
	void Start () {
        Parse();
	}

    public int space;
    public string entName;
    public int tagStart;
    public int tagEnd;
    public int lastQuotationMark;
    public int equals;
    public string attrName;
    public string attrVal;
	
	void Parse () {
        var entStart = level.text.IndexOf("<Entities", 0);
        var entEnd = level.text.IndexOf("</Entities", 0);

        tagStart = entStart;
        do
        {
            tagStart = level.text.IndexOf("<", tagStart + 1);
            if (tagStart >= entEnd)
                break;
            space = level.text.IndexOf(" ", tagStart);
            entName = level.text.Substring(tagStart + 1, space - (tagStart + 1));
            //Debug.Log("Name: " + entName);
            tagEnd = level.text.IndexOf(" />", tagStart);
            Dictionary<string, string> attr = new Dictionary<string, string>();
            while (space < tagEnd)
            {
                lastQuotationMark = level.text.IndexOf("\"", level.text.IndexOf("\"", space)+1);
                equals = level.text.IndexOf("=", space);
                attrName = level.text.Substring(space + 1, equals - (space + 1));
                attrVal = level.text.Substring(equals + 2, lastQuotationMark - (equals + 2));
                attr.Add(attrName, attrVal);
                //Debug.Log("    - " + attrName + ": " + attrVal);
                space = lastQuotationMark + 1;
            }
            Create(entName, attr);
        } while (true);
	}

    public static int count = 0;
    GameObject Create(string objName, Dictionary<string, string> objAttr)
    {
        int ind = -1;
        for(int i = 0; i < names.Count; i++)
        {
            if(names[i] == objName)
            {
                ind = i;
                break;
            }
        }

        if (ind < 0 || ind >= prefabs.Count || objName == "Gel")
            return null;

        //if (count >= 5)
       //     return null;

        GameObject o = null;
        switch(names[ind])
        {
            default:
                float x = float.Parse(objAttr["x"]);
                float y = float.Parse(objAttr["y"]);
                Debug.Log(objName + ": (" + x + ", " + y + ")");
                o = Instantiate(prefabs[ind], new Vector3(x/16f, (704-y)/16f, 0), Quaternion.identity) as GameObject;
                count++;
                break;
        }

        return o;
    }
}
