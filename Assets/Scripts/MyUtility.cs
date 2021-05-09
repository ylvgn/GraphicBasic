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

    // (同一个平面上)直线ab和直线cd的交点o
    public static Vector3 LineLineInterSect(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        // 原理：cross<a, b> = S平行四边形, https://www.cnblogs.com/xiangtingshen/p/12329951.html
        // 作CE//AB && CE == AB, 然后SACD:SCDE = ao:ab
        var cd = d - c;
        var ab = b - a;
        var ca = a - c;
        var Sacd = Vector3.Cross(cd, ca); // Sparallelogram_acd (ad是对角线, cd是底)
        var Scde = Vector3.Cross(ab, cd); // Sparallelogram_cde (de是对角线, cd是底)
        var aoOverab = Vector3.Dot(Sacd, Scde) / Vector3.Dot(Scde, Scde); // p1.x/p2.x + p1.y/p2.y + p1.z/p2.z
        return a + aoOverab * ab;

        /*
        // 或者作AE//CD, AE==CD, 然后SABE:SABC = co:cd
        var S_abe = Vector3.Cross(b - a, d - c);
        var S_abc = Vector3.Cross(c - a, b - a); //ca,bc
        var coOvercd = Vector3.Dot(S_abc, S_abe) / Vector3.Dot(S_abe, S_abe); // Sacb面积: Scde = co:cd
        var o = c + (d - c) * coOvercd;
        return o;
        */
    }

    // p投影到平面o (平面normal是nl) 得到p'
    public static Vector3 PointCast2Plane(Vector3 o, Vector3 nl, Vector3 p)
    {
        nl = nl.normalized;
        return p + Vector3.Dot(o - p, nl) * nl;
    }

    public static void MyDestroy(ref Texture2D o)
    {
        if (o == null) return;
        GameObject.Destroy(o);
        o = null;
    }

    // http://www.songho.ca/opengl/gl_projectionmatrix.html
    public static Matrix4x4 GetProjectMatrix(float left, float right, float bottom, float top, float near, float far)
    {
        Matrix4x4 m = new Matrix4x4();
        m[0] = (2 * near) / (right - left);
        m[5] = (2 * near) / (top - bottom);
        m[8] = (right + left) / (right - left);
        m[9] = (top + bottom) / (top - bottom);
        m[10] = -(far + near) / (far - near);
        m[11] = -1;
        m[14] = -(2 * far * near) / (far - near);
        return m;
    }

    public static Matrix4x4 GetProjectMatrix(float fov, float aspect, float near, float far)
    {
        var tan = Mathf.Tan(fov / 2 * Mathf.Deg2Rad);
        var h = tan * near;
        var w = h * aspect;
        return GetProjectMatrix(-w, w, -h, h, near, far);
    }
}
