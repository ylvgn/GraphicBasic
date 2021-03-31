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
                DrawRect(instance.tex, 30, 60, 256, 256, new Color(0, 1, 0, 1));
                break;
            case Task.RectOutLine:
                DrawRectOutLine(instance.tex, 30, 60, 256, 256, new Color(1, 1, 1, 1));
                break;
            case Task.RectOutLineAndRect:
                DrawRect(instance.tex, 30, 60, 256, 256, new Color(0, 1, 0, 1));
                DrawRectOutLine(instance.tex, 30, 60, 256, 256, new Color(1, 1, 1, 1));
                break;
            case Task.Line:
                break;
            case Task.Line_no_float:
                break;
            case Task.Circle:
                break;
            case Task.CircleOutLine:
                break;
            case Task.Triangle:
                break;
            case Task.TriangleOutLine:
                break;
            default:
                break;
        }

        tex.Apply(false);
    }

    private void DrawRectOutLine(Texture2D tex, int x, int y, int w, int h, Color color)
    {
        if (x < 0 || y < 0 || x  + w >= tex.width || y + h >= tex.height)
        {
            Debug.LogError("[DrawRectOutLine] args error");
            return;
        }

        for (int i = 0; i < w; i ++) // bottom
            tex.SetPixel(i + x, y, color);
        for (int j = 0; j < h; j ++) // left
            tex.SetPixel(x, j + y, color);
        for (int i = 0; i < w; i ++) // top
            tex.SetPixel(i + x, y + h, color);
        for (int j = 0; j < h; j++) // right
            tex.SetPixel(w + x, j + y, color);
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

    void OnGUI()
    {
        if (!tex) return;
        GUI.DrawTexture(new Rect(0, 0, tex.width, tex.height), tex);
    }
}
