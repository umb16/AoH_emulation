using UnityEngine;
using System.Collections;

public class rot : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.localEulerAngles+=Vector3.up*Time.deltaTime*10;
	}
}
