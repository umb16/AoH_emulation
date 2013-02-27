using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Localisation : MonoBehaviour
{
	public static string currentLang = "ru";
	private static Dictionary<string,Dictionary<string,string>> uniDict = new Dictionary<string,Dictionary<string, string>> ();
	public TextAsset uniText;
	
	void getUniText ()
	{
		Dictionary<string,string> tempDict = new Dictionary<string, string> ();
		var asset = uniText;
		if (asset == null)
			Debug.LogError ("Could not find json unitext file");
		else {
			var jsonString = asset.text;
			var decodedHash = jsonString.hashtableFromJson ();
			if (decodedHash == null)
				Debug.LogError ("Is not json file");
			else {
				var unitext = (IDictionary)decodedHash ["unitext"];
				if (unitext == null)
					Debug.LogError ("Is not json unitext file");
				else
					foreach (System.Collections.DictionaryEntry item in unitext) {
						var lang = (IDictionary)((IDictionary)item.Value);
						if (lang == null)
							Debug.LogError ("Is not json unitext file");
						else if (item.Key.ToString () == currentLang) {
							tempDict.Clear ();
							foreach (System.Collections.DictionaryEntry phrase in lang) {
								tempDict.Add (phrase.Key.ToString (), phrase.Value.ToString ());
							}
							uniDict.Add (item.Key.ToString (), tempDict);
						}
						//Debug.Log("lang load:"+item.Key.ToString ());
					}
				asset = null;
				Resources.UnloadUnusedAssets ();
			}
		}
	}
	
	public static string getPhrase (string key)
	{
		//localisation.currentLang="ru";
		if (!uniDict.ContainsKey (Localisation.currentLang))
		if (!uniDict.ContainsKey ("en"))
			return key;
		else if (!uniDict ["en"].ContainsKey (key))
			return key;
		else
			return uniDict ["en"] [key];
		if (!Localisation.uniDict [Localisation.currentLang].ContainsKey (key))
			return key;
		return uniDict [Localisation.currentLang] [key];
	}
	
	void OnGUI()
	{
		//GUI.Label(new Rect(10,10,100,100),currentLang);
	}
	
	void Awake ()
	{
		
		

		#if UNITY_EDITOR
		currentLang="ru";
		#endif
		getUniText ();
		
	}
	// Use this for initialization
	void Start ()
	{
		
	}

	// Update is called once per frame
	void Update ()
	{
		
	}
}