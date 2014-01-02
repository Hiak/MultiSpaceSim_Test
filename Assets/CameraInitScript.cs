using UnityEngine;
using System.Collections;

public class CameraInitScript : MonoBehaviour {
	
	public GameObject[] cameras;
    public GameObject toSwitch;

	
	// Use this for initialization
	void Start () {

		System.Random r=new System.Random();
		
		int i=r.Next(cameras.Length);

        ChangeCamera(i);

	}

    void ChangeCamera(int cameraID) {

        foreach (GameObject c in cameras)
        {
            c.SetActive(false);
        }

        cameras[cameraID].SetActive(true);
        
    }

    int textIter = 0;
    bool switchState = true;
    int toolbarIter = 0;
    string[] toolbarStrings = { "0", "1", "2" };
    string textFieldString = "initialText";
    Vector2 scrollPos=Vector2.zero;

    void OnGUI()
    {

        GUI.Box(new Rect(10, 10, Screen.width - 20, 50), new GUIContent("Num=" + textIter.ToString(), "tooltip"));

        GUI.Label(new Rect(Input.mousePosition.x, Screen.height-Input.mousePosition.y-15, 100, 20), GUI.tooltip);

        scrollPos = GUI.BeginScrollView(new Rect(10, 110, 200, 200), scrollPos, new Rect(0, 0, 200, 300));

        if (GUI.Button(new Rect(10, 10, 100, 100), textIter.ToString()))
        {
            ChangeCamera(textIter);

            textIter++;
            textIter %= 3;
        }
        
        bool tmpS = switchState;
        switchState=GUI.Toggle(new Rect(120, 110, 100, 100),switchState, "light " + (switchState?"on":"off"));
        if(switchState!=tmpS)toSwitch.SetActive(switchState);

        toolbarIter=GUI.Toolbar(new Rect(10,210,100,50),toolbarIter,toolbarStrings);
        GUI.Label(new Rect(10, 265, 20, 20), toolbarIter.ToString());

        

        textFieldString = GUI.TextArea(new Rect(120, 0, 100, 50), textFieldString);
        GUI.Label(new Rect(320, 255, 100, 20), textFieldString);

        GUI.EndScrollView();
    }

	// Update is called once per frame
	void Update () {
	
	}
}
