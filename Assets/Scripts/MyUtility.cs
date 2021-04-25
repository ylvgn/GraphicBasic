using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyUtility
{
    // 四舍五入
    public static int RoundToInt(float x)
    {
        int res = (int)x;
        float d = x - res;
        if (d * 10 >= 5) return res + 1;
        return res;
    }

    public static void Swap(ref float a, ref float b)
    {
        float tmp = a;
        a = b;
        b = tmp;
    }

    public static void Swap(ref int a, ref int b)
    {
        int tmp = a;
        a = b;
        b = tmp;
    }

    public static int Clamp(int value, int minv, int maxv)
    {
        if (minv > maxv)
        {
            Debug.LogError("minv > maxv");
            return value;
        }

        if (value < minv) value = minv;
        else if (value > maxv) value = maxv;
        return value;
    }

    public static void DrawPoint(Texture2D tex, int x, int y, Color c)
    {
        tex.SetPixel(x, y, c);
        tex.SetPixel(x-1, y, c);
        tex.SetPixel(x+1, y, c);
        tex.SetPixel(x, y+1, c);
        tex.SetPixel(x, y-1, c);
    }

    // world-space
    public static void LogPoint(Vector3 pos, string text, Color c, int fontSize = 30)
    {
        var oldColor = GUI.color;
        var oldFontSize = GUI.skin.label.fontSize;
        UnityEditor.Handles.BeginGUI();
        GUI.color = c;
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(pos);
        GUI.skin.label.fontSize = fontSize;
        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        //GUI.Label(new Rect(screenPos.x, view.camera.pixelHeight -screenPos.y, size.x, size.y), text);
        GUI.Label(new Rect(screenPos.x - 120, view.camera.pixelHeight - screenPos.y - 100, size.x, size.y), text);
        UnityEditor.Handles.EndGUI();
        GUI.color = oldColor;
        GUI.skin.label.fontSize = oldFontSize;
    }

    // screen-space
    public static void LogPoint(Rect printRect, string text, Color c, int fontSize = 40)
    {
        var oldColor = GUI.color;
        var oldFontSize = GUI.skin.label.fontSize;
        UnityEditor.Handles.BeginGUI();
        GUI.color = c;
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        GUI.skin.label.fontSize = fontSize;
        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        printRect.width += size.x;
        printRect.height += size.y;
        GUI.Label(printRect, text);
        UnityEditor.Handles.EndGUI();
        GUI.color = oldColor;
        GUI.skin.label.fontSize = oldFontSize;
    }

    public static float Clamp(float v, float min, float max)
    {
        if (min > max)
        {
            Debug.LogError($"clamp min={min} > {max}=max");
        }
        if (v < min) return min;
        if (v > max) return max;
        return v;
    }

    public static int ClampInt(int v, int min, int max)
    {
        if (min > max)
        {
            Debug.LogError($"clamp min={min} > {max}=max");
        }
        if (v < min) return min;
        if (v > max) return max;
        return v;
    }
}
