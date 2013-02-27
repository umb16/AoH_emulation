using UnityEngine;

 

class Debug 

{

	public static new void Log(object message)

    {

       UnityEngine.Debug.Log(message);
		GUI_log.logString+="\n"+message;

    }

 

    public static new void Log(object message, Object context)
    {

       UnityEngine.Debug.Log(message, context);

    }

 

    public static new void LogError(object message)
    {

        UnityEngine.Debug.LogError(message.ToString());

    }

    public static new void LogError(object message, Object context)

    {

        UnityEngine.Debug.LogError(message.ToString(), context);

    }

 

    public static new void LogWarning(object message)

    {

        UnityEngine.Debug.LogWarning(message.ToString());

    }

 

    public static new void LogWarning(object message, Object context)

    {

        UnityEngine.Debug.LogWarning(message.ToString(), context);

    }

}