using UnityEngine;
using System.Collections;


public class GUI_FPS : MonoBehaviour
{
	float timer;
	int frameCount = 0;
	int fps=0;
	Rect fpsPos;
	public static float screenK=1;
	public static int fontSize;
	void Start ()
	{
		
		screenK = (float)Screen.height/512;
		fpsPos = new Rect(Screen.width-30*screenK*2,0,30*screenK*2,(30*screenK));
		fontSize = (int)(13*screenK);
	}
	
	void OnGUI()
	{
		GUI.Label(fpsPos,string.Format("<size={1}> fps:{0} </size>",fps,fontSize));
	}
	
	// Update is called once per frame
	void Update ()
	{
		frameCount++;
		if (timer <= Time.realtimeSinceStartup) {
			timer = Time.realtimeSinceStartup + 1;
			fps=frameCount;
			//fpsText.text=fps.ToString();
			frameCount = 0;
		}
	}
}