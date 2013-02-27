
public class PushString
{

    public static string GetString(string key, string o)
    {
        try
        {
            int position = o.IndexOf("[" + key + ":") + key.Length + 2;
            return o.Substring(position, o.IndexOf(":/" + key + "]") - position);
        }
        catch
        {
            return null;
        }
       
    }

    public static int GetInt(string key, string o)
    {
        int result;
        try
        {
            int position = o.IndexOf("[" + key + ":") + key.Length + 2;
            if (int.TryParse(o.Substring(position, o.IndexOf(":/" + key + "]") - position), out result))
            {
                return result;
            }
        }
        catch
        {

        }
        return 0;
    }

    public static string SetTag(string tag)
    {
        return "[" + tag + "]";
    }

    public static bool ContainTag(string tag, string o)
    {
        return o.Contains("[" + tag + "]");
    }

    public static string SetValue(string key, string value)
    {

        return "[" + key + ":" + value + ":/" + key + "]";
    }
    public static string SetValue(string key, int value)
    {

        return "[" + key + ":" + value.ToString() + ":/" + key + "]";
    }
}