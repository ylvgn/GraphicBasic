using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MyGUI : MonoBehaviour
{
    static MyGUI instance;
    public const int WIDTH = 512;
    public const int HEIGHT = 512;
    public enum TaskEnum
    {
        ALL,
        Rect,
        Line,
        Circle,
        Triangle,
    }

    public TaskEnum taskEnum;
    public DrawRect[] MyDrawRects;
    public DrawLine[] MyDrawLines;

    Texture2D tex;
    private bool isEnable;


    void Start() {
        tex = new Texture2D(WIDTH, HEIGHT);
    }

    void Update()
    {
        if (!instance) instance = this;
        if (!isEnable) return;
        if (!tex) tex = new Texture2D(WIDTH, HEIGHT);
        Clear();

        switch (taskEnum)
        {
            case TaskEnum.ALL:
                MyDrawRect();
                MyDrawLine();
                break;
            case TaskEnum.Rect:
                MyDrawRect();
                break;
            case TaskEnum.Line:
                MyDrawLine();
                break;
            case TaskEnum.Circle:
                break;
            case TaskEnum.Triangle:
                break;
            default:
                break;
        }

        tex.Apply(false);
    }

    void MyDrawRect()
    {
        if (MyDrawRects == null) return;
        for (int i = 0; i < MyDrawRects.Length; i++)
        {
            if (!MyDrawRects[i]) continue;
            MyDrawRects[i].MyDraw(tex);
        }
    }

    void MyDrawLine()
    {
        if (MyDrawLines == null) return;
        for (int i = 0; i < MyDrawLines.Length; i++)
        {
            if (!MyDrawLines[i]) continue;
            MyDrawLines[i].MyDraw(tex);
        }
    }

    [UnityEditor.MenuItem("MyTool/MyRender %F1")]
    static void MyRender()
    {
        if (!instance) return;
        instance.isEnable = !instance.isEnable;
        if (instance.isEnable) Debug.Log("<color=#FFFF00> Running </color>");
        else Debug.Log("<color=#FFFF00> Stoped </color>");
    }

    void Clear()
    {
        var c = new Color(0, 0, 0, 1);
        for (int i = 0; i < WIDTH; i++)
            for (int j = 0; j < HEIGHT; j++)
                tex.SetPixel(i, j, c);
        tex.Apply(false);
    }

    void OnGUI()
    {
        if (!tex) return;
        GUI.DrawTexture(new Rect(0, 0, tex.width, tex.height), tex);
    }
}
