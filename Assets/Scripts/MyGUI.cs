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
        RectAndOutLine,
        Line,
        Line_no_float,
        Circle,
        CircleOutLine,
        CircleAndOutline,
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
            case Task.RectAndOutLine:
                DrawRect(tex, 30, 60, 256, 256, new Color(0, 1, 0, 1));
                DrawRectOutLine(tex, 30, 60, 256, 256, new Color(1, 1, 1, 1));
                break;
            case Task.Line:
                DrawLine(tex, 340, 50, 340, 500, new Color(0, 1, 1, 1)); // k 不存在
                DrawLine(tex, 10, 270, 420, 270, new Color(0, 1, 0, 1));   // k == 0
                DrawLine(tex, 30, 30, 350, 100, new Color(1, 1, 0, 1));  // 0 < k < 1
                DrawLine(tex, 30, 30, 60, 300, new Color(0, 0, 1, 1));   // k > 1
                DrawLine(tex, 30, 400, 60, 60, new Color(1, 1, 1, 1));   // k < -1
                DrawLine(tex, 30, 400, 400, 350, new Color(1, 0, 1, 1)); // -1 < k < 0
                break;
            case Task.Line_no_float:
                //DrawLineNF(tex, 30, 400, 300, 60, new Color(1, 1, 1, 1)); // k < 0
                break;
            case Task.Circle:
                DrawCircle(tex, 256, 256, 100, new Color(0, 1, 0, 1));
                DrawCircle(tex, 400, 400, 100, new Color(0, 0, 1, 1));
                break;
            case Task.CircleOutLine:
                DrawCircleOutline(tex, 256, 256, 100, new Color(1, 1, 1, 1));
                DrawCircleOutline(tex, 400, 400, 100, new Color(0, 0, 1, 1));
                break;
            case Task.CircleAndOutline:
                // cirlr1
                DrawCircle(tex, 220, 200, 100, new Color(0, 1, 0, 1));
                DrawCircleOutline(tex, 220, 200, 100, new Color(1, 1, 1, 1));

                // circle2
                DrawCircle(tex, 400, 400, 100, new Color(0, 0, 1, 1));
                DrawCircleOutline(tex, 400, 400, 100, new Color(1, 1, 1, 1));
                break;
            case Task.Triangle:
                break;
            case Task.TriangleOutLine:
                DrawTriangleOutline(tex, 10, 10, 150, 150, 200, 10, new Color(0, 1, 0, 1));
                DrawTriangleOutline(tex, 10, 200, 10, 150, 200, 200, new Color(0, 1, 1, 1));
                break;
            default:
                break;
        }

        tex.Apply(false);
    }

    private void DrawCircleOutline(Texture2D tex, int x, int y, float r, Color color)
    {
        /* Solution1
        var a = MyUtility.RoundToInt(r);
        for (int i = x - a; i <= x + a; i ++)
            for (int j = y - a; j <= y + a; j ++)
            {
                if (i < 0 || y < 0 || x >= tex.width || y >= tex.height) continue;
                var vec2 = MyUtility.MakeVector(i, j, x, y);
                float d = MyUtility.Length(vec2);
                if (Mathf.Abs(d - r) <= 0.55) // ！！！
                    tex.SetPixel(i, j, color);
            }
        */

        // Solution2
        // (y - y0)² = r² - (x - x0)²
        var a = MyUtility.RoundToInt(r);
        for (int i = x - a; i <= x + a; i++)
        {
            float y_y0 = Mathf.Sqrt(r * r - (i - x) * (i - x));
            if (i < 0 || i >= tex.width ) continue;

            // 上半圆
            int j = MyUtility.RoundToInt(-(y_y0 - y));
            if (j > 0 && j < tex.height) tex.SetPixel(i, j, color);

            // 下半圆
            j = MyUtility.RoundToInt(y_y0 + y);
            if (j > 0 && j < tex.height) tex.SetPixel(i, j, color);
        }
    }

    private void DrawCircle(Texture2D tex, int x, int y, float r, Color color)
    {
        // (x - x0)² + (y - y0)² ≤ r²
        var a = MyUtility.RoundToInt(r);
        for (int i = x - a; i <= x + a; i ++)
            for (int j = y - a; j <= y + a; j ++)
            {
                if (i < 0 || j < 0 || i >= tex.width || j >= tex.height) continue;
                float d = (i - x) * (i - x) + (j - y) * (j - y);
                if (d <= r * r)
                    tex.SetPixel(i, j, color);
            }
    }

    private void DrawTriangleOutline(Texture2D tex, int x1, int y1, int x2, int y2, int x3, int y3, Color color)
    {
        // 可能是无效的三角形
        Vector2 a = MyUtility.MakeVector(x1, y1, x2, y2);
        Vector2 b = MyUtility.MakeVector(x1, y1, x3, y3);
        Vector2 c = MyUtility.MakeVector(x2, y2, x3, y3);
        float len_a = MyUtility.Length(a);
        float len_b = MyUtility.Length(b);
        float len_c = MyUtility.Length(c);
        if ((len_a + len_b < len_c) || (len_a + len_c < len_b) || (len_b + len_c < len_a))
        {
            Debug.LogError($"无效三角形:({x1}, {y1}), ({x2}, {y2}),({x3}, {y3})");
            return;
        }

        DrawLine(tex, x1, y1, x2, y2, color);
        DrawLine(tex, x1, y1, x3, y3, color);
        DrawLine(tex, x2, y2, x3, y3, color);
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

        if (Mathf.Abs(k) < 1)
        {
            // y = k * (x - x0) + y0
            for (int x = Mathf.Min(x1, x2); x <= Mathf.Max(x1, x2); x ++)
            {
                int y = MyUtility.RoundToInt(k * (x - x1) + y1);
                tex.SetPixel(x, y, color);
            }
        }
        else
        {
            // x = (y - y0) / k + x0
            for (int y = Mathf.Min(y1, y2); y <= Mathf.Max(y1, y2); y ++)
            {
                int x = MyUtility.RoundToInt((y - y1) / k + x1);
                tex.SetPixel(x, y, color);
            }
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
