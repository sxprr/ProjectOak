using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;


public static class LogHandler
{
    private static string GetColor(string name)
    {
        var hue = (uint)name.GetHashCode() / (float)uint.MaxValue;
        var color = Color.HSVToRGB(hue, 0.6f, 1f);
        return ColorUtility.ToHtmlStringRGB(color);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    // Overrides any normal debug.log with "LogHandler.Log" (switch 'debug' with 'LogHandler')
    public static void Log(object message, Object context = null, [CallerFilePath] string file = "")
    {
        // GET THE NAME OF THE CLASS THAT THE LOG IS BEING CALLED FROM.
        var className = Path.GetFileNameWithoutExtension(file);

        //assign determenistic color
        var color = GetColor(className);

        //add this custom color coded tag to any debug log
        Debug.Log($"<color=#{GetColor(className)}><b>[{className}]</b></color> {message}", context);
    }

    public static void LogWarning(object message, [CallerFilePath] string file = "")
    {
        // GET THE NAME OF THE CLASS THAT THE LOG IS BEING CALLED FROM.
        var className = Path.GetFileNameWithoutExtension(file);

        //assign determenistic color
        var color = GetColor(className);

        //add this custom color coded tag to any debug log
        Debug.LogWarning($"<color=#{color}><b>[{className}]</b></color> {message}");
    }

    public static void LogError(object message, [CallerFilePath] string file = "")
    {
        // GET THE NAME OF THE CLASS THAT THE LOG IS BEING CALLED FROM.
        var className = Path.GetFileNameWithoutExtension(file);

        //assign determenistic color
        var color = GetColor(className);

        //add this custom color coded tag to any debug log
        Debug.LogError($"<color=#{color}><b>[{className}]</b></color> {message}");
    }
}