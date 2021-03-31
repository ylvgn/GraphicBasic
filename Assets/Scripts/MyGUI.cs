using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MyGUI : MonoBehaviour
{
    static MyGUI instance;
    public enum Task
    {
        Rect,
        RectOutLine,
        RectOutLineAndRect,
        Line,
        Line_no_float,
        Circle,
        CircleOutLine,
        Triangle,
        TriangleOutLine,
    }

    Texture2D tex;
    public Task MyEnum;

    void Update()
    {
        if (!instance)
            instance = this;
    }

    [UnityEditor.MenuItem("MyTool/MyRender %F1")]
    static void MyRender()
    {
        Debug.Log("MYRENDER");
        if (!instance) return;
        instance.MyRenderInner();
    }

    void MyRenderInner()
    {
        tex = new Texture2D(512, 512);
        var c = new Color(0, 0, 0, 1);
        for (int i = 0; i < 512; i++)
            for (int j = 0; j < 512; j++)
                tex.SetPixel(i, j, c);

        Debug.Log("Enum" + MyEnum);
        switch (MyEnum)
        {
            case Task.Rect:
                DrawRect(tex, 30, 60, 256, 256, new Color(0, 1, 0, 1));
                break;
            case Task.RectOutLine:
                DrawRectOutLine(tex, 30, 60, 256, 256, new Color(1, 1, 1, 1));
                break;
            case Task.RectOutLineAndRect:
                DrawRect(tex, 30, 60, 256, 256, new Color(0, 1, 0, 1));
                DrawRectOutLine(tex, 30, 60, 256, 256, new Color(1, 1, 1, 1));
                break;
            case Task.Line:
                DrawLine(tex, 10, 50, 10, 320, new Color(0, 1, 1, 1));  // k 不存在
                DrawLine(tex, 10, 20, 200, 20, new Color(0, 1, 0, 1));  // k == 0
                DrawLine(tex, 30, 30, 350, 100, new Color(1, 1, 0, 1)); // k > 0
                DrawLine(tex, 30, 400, 300, 60, new Color(1, 1, 1, 1)); // k < 0
                break;
            case Task.Line_no_float:
                DrawLineNF(tex, 30, 400, 300, 60, new Color(1, 1, 1, 1)); // k < 0
                break;
            case Task.Circle:
                break;
            case Task.CircleOutLine:
                break;
            case Task.Triangle:
                break;
            case Task.TriangleOutLine:
                DrawTriangle(tex, 10, 10, 150, 150, 200, 10, new Color(0, 1, 0, 1));
                break;
            default:
                break;
        }

        tex.Apply(false);
    }

    private void DrawTriangle(Texture2D tex, int x1, int y1, int x2, int y2, int x3, int y3, Color color)
    {
        // TODO 可能是无效的三角形
        DrawLine(tex, x1, y1, x2, y2, color);
        DrawLine(tex, x1, y1, x3, y3, new Color(1,1,1,1));
        DrawLine(tex, x2, y2, x3, y3, new Color(1, 0, 1, 1));
    }

    private void DrawRectOutLine(Texture2D tex, int x, int y, int w, int h, Color color)
    {
        if (x < 0 || y < 0 || x  + w >= tex.width || y + h >= tex.height)
        {
            Debug.LogError("[DrawRectOutLine] args error");
            return;
        }

        for (int i = 0; i < w; i ++) tex.SetPixel(i + x, y, color);     // bottom
        for (int j = 0; j < h; j ++) tex.SetPixel(x, j + y, color);     // left
        for (int i = 0; i < w; i ++) tex.SetPixel(i + x, y + h, color); // top
        for (int j = 0; j < h; j ++) tex.SetPixel(w + x, j + y, color); // right
    }

    void DrawRect(Texture2D tex, int x, int y, int w, int h, Color color)
    {
        if (x < 0 || y < 0 || x + w >= tex.width || y + h >= tex.height)
        {
            Debug.LogError("[DrawRect] args error");
            return;
        }

        for (int i = 0; i < w; i++)
            for (int j = 0; j < h; j++)
                tex.SetPixel(i + x, j + y, color);
    }

    void DrawLine(Texture2D tex, int x1, int y1, int x2, int y2, Color color)
    {
        if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 || x1 >= tex.width || y1 >= tex.height || x2 >= tex.width || y2 >= tex.height)
        {
            Debug.LogError("[DrawLine] args error");
            return;
        }

        if (x2 == x1)
        {
            int x = Mathf.Min(x1, x2);
            int y = Mathf.Min(y1, y2);
            DrawRect(tex, x, y, 1, Mathf.Max(y1, y2) - y, color);
            return;
        }

        float k = (y2 - y1) * 1.0f / (x2 - x1);
        if (k == 0)
        {
            int x = Mathf.Min(x1, x2);
            int y = Mathf.Min(y1, y2);
            DrawRect(tex, x, y, Mathf.Max(x1, x2) - x, 1, color);
            return;
        }

        // y - y0 = k * (x - x0)
        for (int x = x1; x <= x2; x ++)
        {
            int y = MyUtility.RoundToInt(k * (x - x1) + y1);
            tex.SetPixel(x, y, color);
        }
    }

    void DrawLineNF(Texture2D tex, int x1, int y1, int x2, int y2, Color color)
    {
        if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 || x1 >= tex.width || y1 >= tex.height || x2 >= tex.width || y2 >= tex.height)
        {
            Debug.LogError("[DrawLineNF] args error");
            return;
        }

    }

    void OnGUI()
    {
        if (!tex) return;
        GUI.DrawTexture(new Rect(0, 0, tex.width, tex.height), tex);
    }
}
