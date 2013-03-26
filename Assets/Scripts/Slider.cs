using UnityEngine;
using System.Collections;

public class Slider : MonoBehaviour {
	
	float slider1=1;
	float slider2=1;
	int difference;
	int numberOfZones=26;
	float dmgFactor=1f;
	// Use this for initialization
	void Start () {
	
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(300,Screen.height/2-160,200,30),"Defence");
		GUI.Label(new Rect(300,Screen.height/2-130,200,30),slider1.ToString("#")+" "+BodyPart((int)slider1));
		slider1=Mathf.Floor( GUI.VerticalSlider(new Rect(300,Screen.height/2-100,30,200),slider1,1,numberOfZones));
		GUI.Label(new Rect(Screen.width -300,Screen.height/2-160,200,30),"Attack");
		GUI.Label(new Rect(Screen.width -300,Screen.height/2-130,200,30),slider2.ToString("#")+" "+BodyPart((int)slider2));
		slider2=Mathf.Floor(GUI.VerticalSlider(new Rect(Screen.width -300,Screen.height/2-100,30,200),slider2,1,numberOfZones));
		difference= (int)Mathf.Abs(slider1-slider2);
		if(difference>numberOfZones/2)
		difference=numberOfZones-difference;
		dmgFactor = calculateDmgFactor(difference);
		GUI.Label(new Rect(Screen.width/2 -50,Screen.height/2,200,50),difference.ToString()+"\n"+ (dmgFactor*100).ToString()+"%");
	}
	
	
	
	float calculateDmgFactor(int dif)
	{
		if(dif<=1)
			return 0;
		if(dif==2)
			return .25f;
		if(dif==3)
			return .50f;
		//if(dif>=4&&dif<=9)
		//	return 1.00f;
		if(dif==11)
			return 1.50f;
		if(dif==12)
			return 2.00f;
		if(dif>=13)
			return 2.50f;
		return 1.00f;
	}
	
	string BodyPart(int point)
	{
		if(point>=1&&point<=2)
			return "Head";
		if(point>=3&&point<=6)
			return "Right hand";
		if(point>=7&&point<=10)
			return "Body right side";
		if(point>=11&&point<=14)
			return "Right leg";
		if(point>=15&&point<=18)
			return "Left leg";
		if(point>=19&&point<=22)
			return "Body left side";
		if(point>=23&&point<=26)
			return "Left hand";
		return "Unknown";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
