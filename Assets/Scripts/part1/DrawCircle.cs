using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCircle : MonoBehaviour
{
    public MyVector2 center;
    public float radius;
    public Color circleColor;
    public Color outLineCircle;

    public enum CircleType
    {
        Circle,
        CircleOutLine,
        CircleAndOutLine,
    }

    public CircleType type;

    public void MyDraw(Texture2D tex)
    {
        switch (type)
        {
            case CircleType.Circle:
                MyDrawCircle(tex);
                break;
            case CircleType.CircleOutLine:
                MyDrawCircleOutLine(tex);
                break;
            case CircleType.CircleAndOutLine:
                MyDrawCircle(tex);
                MyDrawCircleOutLine(tex);
                break;
            default:
                break;
        }
    }

    void MyDrawCircle(Texture2D tex)
    {
        // 半圆 * 2
        int x0 = (int)center.x;
        int y0 = (int)center.y;
        int r = (int)radius;

        for (int y = 0; y <= r; y ++)
        {
            int dx = (int)Mathf.Sqrt(r * r - y * y);
            for (int x = -dx; x <= dx; x++)
            {
                tex.SetPixel(x0 + x, y0 + y, circleColor); // 下半圆
                tex.SetPixel(x0 + x, y0 - y, circleColor); // 上半圆
            }
        }

        // 圆心
        tex.SetPixel(x0, y0, Color.red);
        tex.SetPixel(x0 + 1, y0, Color.red);
        tex.SetPixel(x0, y0 + 1, Color.red);
        tex.SetPixel(x0 + 1, y0 + 1, Color.red);
    }

    public int t = 10;
    void MyDrawCircleOutLine(Texture2D tex)
    {
        // 右上角1/4圆 * 4
        int x0 = (int)center.x;
        int y0 = (int)center.y;
        int r = (int)radius;

        // 圆心
        tex.SetPixel(x0, y0, Color.red);
        tex.SetPixel(x0 + 1, y0, Color.red);
        tex.SetPixel(x0, y0 + 1, Color.red);
        tex.SetPixel(x0+1, y0 + 1, Color.red);

        for (int dx = 0, dy = r; ;) // 从顶开始画
        {
            //if (dx > t) break; // debug

            // 右上角1/8(上) 
            tex.SetPixel(x0 - dx, y0 + dy, outLineCircle); // 左上
            tex.SetPixel(x0 + dx, y0 + dy, outLineCircle); // 右上
            tex.SetPixel(x0 - dx, y0 - dy, outLineCircle); // 左下
            tex.SetPixel(x0 + dx, y0 - dy, outLineCircle); // 右下

            // 右上角1/8(下)
            tex.SetPixel(x0 + dy, y0 - dx, outLineCircle); // 左上(对称y=x)
            tex.SetPixel(x0 + dy, y0 + dx, outLineCircle); // 右上(对称y=x)
            tex.SetPixel(x0 - dy, y0 - dx, outLineCircle); // 左下(对称y=x)
            tex.SetPixel(x0 - dy, y0 + dx, outLineCircle); // 右下(对称y=x)

            int new_dx = dx + 1;
            int new_dy = dy - 1;
            float a = new_dx * new_dx + dy * dy;         // →
            float b = dx * dx + new_dy * new_dy;         // ↓
            float c = new_dx * new_dx + new_dy * new_dy; // ↘

            a = Mathf.Abs(Mathf.Sqrt(a) - r);
            b = Mathf.Abs(Mathf.Sqrt(b) - r);
            c = Mathf.Abs(Mathf.Sqrt(c) - r);
            if (a < b && a < c) // → 更接近r
            {
                dx = new_dx;
            }
            else if (b < a && b < c) // ↓ 更接近r
            {
                dy = new_dy;
            }
            else // ↘ 更接近r
            {
                dx = new_dx;
                dy = new_dy;
            }
            if (dx > dy) break;
        }
    }
}
