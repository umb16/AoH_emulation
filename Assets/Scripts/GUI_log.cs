using UnityEngine;
using System.Collections;

public class GUI_log : MonoBehaviour {
	
	public GUISkin guiSkin;
	public static string logString = "";
	public bool log=true;
	Vector2 logScroll = new Vector2(0,0);
	string oldString = "";
	
	// Use this for initialization
	void Start () {
	
	}
	
	void OnGUI()
	{
		GUI.skin=guiSkin;
		GUILayout.BeginArea (new Rect (10, Screen.height - 100, Screen.width - 20, 100));
		if (oldString != logString) {
			logScroll.y = Mathf.Infinity;
			oldString = logString;
		}
		logScroll = GUILayout.BeginScrollView (new Vector2 (logScroll.x, logScroll.y), GUILayout.Width (Screen.width - 20), GUILayout.Height (100));
		if (log)
		GUILayout.Label (logString);
		GUILayout.EndScrollView ();
		GUILayout.EndArea ();
	}
	// Update is called once per frame
	void Update () {
	
	}
}
