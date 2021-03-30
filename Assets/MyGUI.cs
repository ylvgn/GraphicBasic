using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MyGUI : MonoBehaviour
{

    Texture2D tex;
    static MyGUI instance;

    [UnityEditor.MenuItem("MyTool/MyRender %F1")]
    static void MyRender()
    {
        if (!instance) return;
        instance.tex = new Texture2D(512, 512);

        var c = new Color(0, 0, 0, 1);
        for (int i = 0; i < 512; i++)
            for (int j = 0; j < 512; j++)
                instance.tex.SetPixel(i, j, c);

        instance.tex.Apply(false);
    }

    private void Start()
    {
        instance = this;
    }

    void OnGUI()
    {
        if (!tex) return;

        GUI.DrawTexture(new Rect(0, 0, tex.width, tex.height), tex);
    }
}
