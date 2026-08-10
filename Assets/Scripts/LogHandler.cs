using System.Collections;
using System.Collections.Generic;
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


    // LE SSERAFIM
    // MIFARESS EL
    // Overrides any normal debug.log with "LogHandler.Log" (switch 'debug' with 'LogHandler')
    public static void Log(object message, [CallerFilePath] string file = "")
    {
        // GET THE NAME OF THE CLASS THAT THE LOG IS BEING CALLED FROM.
        var className = Path.GetFileNameWithoutExtension(file);

        //assign determenistic color
        var color = GetColor(className);

        //add this custom color coded tag to any debug log
        Debug.Log($"<color=#{color}><b>[{className}]</b></color> {message}");
    }
}