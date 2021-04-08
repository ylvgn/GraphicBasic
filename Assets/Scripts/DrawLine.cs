using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    public MyVector2 p1;
    public MyVector2 p2;
    public Color color;

    public enum LineType
    {
        Line,
        Bresenham,
        XiaolinWu,
    }

    public LineType type;

    public void MyDraw(Texture2D tex)
    {
        switch (type)
        {
            case LineType.Line:
                MyDrawLine(tex);
                break;
            case LineType.Bresenham:
                MyDrawLine_Bresenham(tex);
                break;
            case LineType.XiaolinWu:
                break;
            default:
                break;
        }
    }

    void MyDrawLine(Texture2D tex)
    {
        int x1 = (int)p1.x;
        int y1 = (int)p1.y;
        int x2 = (int)p2.x;
        int y2 = (int)p2.y;

        float dx = x2 - x1;
        float dy = y2 - y1;
        if (dx == 0 && dy == 0) return;

        if (Mathf.Abs(dx) < Mathf.Abs(dy))
        {
            float m = dx / dy;
            int y0 = System.Math.Min(y1, y2);
            int x0 = y1 == y0 ? x1 : x2;
            int n = (int)Mathf.Abs(dy);
            for (int y = 0; y <= n; y++)   // [0, n(dy)] + min(y1, y2) -> [min_y, max_y]
            {
                int x = (int)(m * y + x0); // m = (x - x0) / (y - y0)
                if (x < 0 || y + y0 < 0 || x >= tex.width || y + y0 >= tex.height) continue;
                tex.SetPixel(x, y + y0, color);
            }
        }
        else
        {
            float m = dy / dx;
            int x0 = System.Math.Min(x1, x2);
            int y0 = x1 == x0 ? y1 : y2;
            int n = (int)Mathf.Abs(dx);
            for (int x = 0; x <= n; x++)   // [0, n(dx)] + min(x1, x2) -> [min_x, max_x]
            {
                int y = (int)(m * x + y0); // m = (y - y0) / (x - x0)
                if (x + x0 < 0 || y < 0 || x + x0 >= tex.width || y >= tex.height) continue;
                tex.SetPixel(x + x0, y, color);
            }
        }
    }

    void MyDrawLine_Bresenham(Texture2D tex)
    {

    }
}
