using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

public class Serialize : MonoBehaviour {

	// Use this for initialization
	public static string serialization(object o)
	{
		string text="";
		TextWriter writer;
		XmlSerializer serialWrite = new XmlSerializer (o.GetType());
		writer = new StringWriter();
		serialWrite.Serialize (writer, o);
		text=writer.ToString();
		writer.Close ();
		return text;
	}
	
	public static object deserialization(object o,string xml_text)
	{
		TextReader reader;
		XmlSerializer serialRead = new XmlSerializer (o.GetType());
		reader = new StringReader(xml_text);
		o = serialRead.Deserialize (reader);
		reader.Close ();
		return o;
	}
	
	public static object deserialization (string path ,object o)
	{
		//players level = new players ();
		XmlSerializer preferences = new XmlSerializer (o.GetType());
		Stream reader;
		try {
			if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsPlayer)
				reader = new FileStream (path, FileMode.Open, FileAccess.Read);
			else
				reader = new FileStream (Application.persistentDataPath + "/" + path, FileMode.Open, FileAccess.Read);
		} catch (System.Exception ex) {
			serialization (path, o);
			if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsPlayer)
				reader = new FileStream (path, FileMode.Open, FileAccess.Read);
			else
				reader = new FileStream (Application.persistentDataPath + "/" + path, FileMode.Open, FileAccess.Read);
		}
		o = preferences.Deserialize (reader);
		reader.Close ();
		return o;
	}
	
	public static void serialization ( string path, object o)
	{
		Stream writer;
		XmlSerializer serialWrite = new XmlSerializer (o.GetType());
		if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsPlayer)
			writer = new FileStream (path, FileMode.Create, FileAccess.Write);
		else
			writer = new FileStream (Application.persistentDataPath + "/" + path, FileMode.Create, FileAccess.Write);
		serialWrite.Serialize (writer, o);
		writer.Close ();
	}
	
	
	
	
}
