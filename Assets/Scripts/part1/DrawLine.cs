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
        Bresenham,  // Bresenham 写法1
        Bresenham2, // Bresenham 写法2
        Bresenham3, // Bresenham 写法3
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
            case LineType.Bresenham2:
                MyDrawLine_Bresenham2(tex);
                break;
            case LineType.Bresenham3:
                MyDrawLine_Bresenham2(tex);
                MyDrawLine_Bresenham3(tex);
                break;
            case LineType.XiaolinWu:
                MyDrawLine_XiaolinWu(tex);
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
        DrawNormalLine(tex, x1, y1, x2, y2, color);
    }

    void MyDrawLine_Bresenham(Texture2D tex)
    {
        int x1 = (int)p1.x;
        int y1 = (int)p1.y;
        int x2 = (int)p2.x;
        int y2 = (int)p2.y;
        DrawBresenhamLine(tex, x1, y1, x2, y2, color);
    }

    void MyDrawLine_Bresenham2(Texture2D tex)
    {
        int x1 = (int)p1.x;
        int y1 = (int)p1.y;
        int x2 = (int)p2.x;
        int y2 = (int)p2.y;
        DrawBresenhamLine2(tex, x1, y1, x2, y2, color);
    }

    void MyDrawLine_Bresenham3(Texture2D tex)
    {
        int x1 = (int)p1.x;
        int y1 = (int)p1.y;
        int x2 = (int)p2.x;
        int y2 = (int)p2.y;
        DrawBresenhamLine3(tex, x1, y1, x2, y2, Color.green);
    }

    void MyDrawLine_XiaolinWu(Texture2D tex)
    {
        int x1 = (int)p1.x;
        int y1 = (int)p1.y;
        int x2 = (int)p2.x;
        int y2 = (int)p2.y;
        DrawXiaolinWuLine(tex, x1, y1, x2, y2, color);
    }

    public static void Draw(Texture2D tex, int x1, int y1, int x2, int y2, Color c, LineType type)
    {
        switch (type)
        {
            case LineType.Line:
                DrawNormalLine(tex, x1, y1, x2, y2, c);
                break;
            case LineType.Bresenham:
                DrawBresenhamLine(tex, x1, y1, x2, y2, c);
                break;
            case LineType.Bresenham2:
                DrawBresenhamLine2(tex, x1, y1, x2, y2, c);
                break;
            case LineType.Bresenham3:
                DrawBresenhamLine3(tex, x1, y1, x2, y2, c);
                break;
            case LineType.XiaolinWu:
                DrawXiaolinWuLine(tex, x1, y1, x2, y2, c);
                break;
            default:
                break;
        }
    }

    // https://www.dgp.toronto.edu/~hertzman/418notes.pdf
    static void DrawNormalLine(Texture2D tex, int x1, int y1, int x2, int y2, Color c)
    {
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
                tex.SetPixel(x, y + y0, c);
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
                tex.SetPixel(x + x0, y, c);
            }
        }
    }

    // https://unionassets.com/blog/algorithm-brezenhema-and-wu-s-line-299
    static void DrawBresenhamLine(Texture2D tex, int x1, int y1, int x2, int y2, Color c)
    {
        // 令 x1 < x2 && y1 < y2
        bool isSteep = Mathf.Abs(y2 - y1) > Mathf.Abs(x2 - x1);
        if (isSteep)
        {
            MyUtility.Swap(ref x1, ref y1);
            MyUtility.Swap(ref x2, ref y2);
        }

        if (x1 > x2)
        {
            MyUtility.Swap(ref x1, ref x2);
            MyUtility.Swap(ref y1, ref y2);
        }

        // 0 < abs(dy) < dx
        int dy = System.Math.Abs(y2 - y1);
        int dx = x2 - x1;

        if (dx == 0 && dy == 0) return;
        int slope_error = dy - dx;
        int ystep = (y1 < y2) ? 1 : -1;
        for (int x = x1, y = y1; x <= x2; x++)
        {
            tex.SetPixel(isSteep ? y : x, isSteep ? x : y, c);
            slope_error += dy;
            if (slope_error >= 0)
            {
                y += ystep;
                slope_error -= dx;
            }
        }
    }

    // https://csustan.csustan.edu/~tom/Lecture-Notes/Graphics/Bresenham-Line/Bresenham-Line.pdf
    static void DrawBresenhamLine2(Texture2D tex, int x1, int y1, int x2, int y2, Color c)
    {
        int xStep = x1 < x2 ? 1 : -1;
        int yStep = y1 < y2 ? 1 : -1;
        int dx = System.Math.Abs(x1 - x2);
        int dy = System.Math.Abs(y1 - y2);

        dy <<= 1; // dy = 2▲y
        dx <<= 1; // dx = 2▲x
        tex.SetPixel(x1, y1, c);

        // DV(k) = ((Y0 + (k + 1) ∗ m) mod 1) − 0.5f
        // 将两边乘slope的分母: slope_error = 2*▲x * DV(k) = ((k ∗ 2∗▲y + 2∗▲y) mod (2*▲x)) − (▲x)
        if (dx > dy)
        {
            int slope_error = dy - (dx >> 1); // 2▲y - ▲x
            while( x1 != x2 )
            {
                x1 += xStep;
                if (slope_error >= 0) // [-0.5, 0.5] -> [2▲x * -0.5, 2▲x * 0.5]
                {
                    y1 += yStep;
                    slope_error -= dx;
                }
                slope_error += dy;
                tex.SetPixel(x1, y1, c);
            }
        } else
        {
            int slope_error = dx - (dy >> 1); // 2▲x - ▲y
            while (y1 != y2)
            {
                y1 += yStep;
                if (slope_error >= 0) // [-0.5, 0.5] -> [2▲y * -0.5, 2▲y * 0.5]
                {
                    x1 += xStep;
                    slope_error -= dy;
                }
                slope_error += dx;
                tex.SetPixel(x1, y1, c);
            }
        }

    }

    // http://members.chello.at/~easyfilter/Bresenham.pdf
    static void DrawBresenhamLine3(Texture2D tex, int x1, int y1, int x2, int y2, Color c)
    {
        /*
            f(x, y) = (y-y1)(x2-x1) - (x-x1)(y2-y1), f(x,y)=0就是直线方程
            令e = f(x,y) = (y-y1)dx - (x-x1)dy
            e_x, e_y, e_xy ：表示当前点预测下一个点三个方向的误差:
                e_xy  =  (y+1-y1)dx - (x+1-x1)dy
                      =  e + dx - dy
                e_x   =  (y+1-y1)dx - (x-x1)dy
                      =  e + dx
                      =  e_xy + dy
                e_y   =  (y-y1)dx - (x+1-x1)dy
                      =  e - dy
                      =  e_xy - dx
            // 初始e = 0, 可直接画(x1, y1)
            e_xy = (y1 + 1 - y1)dx - (x1+1-x1)dy = dx - dy 
         */
        int xStep = x1 < x2 ? 1 : -1;
        int yStep = y1 < y2 ? 1 : -1;
        int dx = System.Math.Abs(x1 - x2);
        int dy = System.Math.Abs(y1 - y2);
        int e_xy = dx - dy;
        int e_xy2; // e_xy2 = 2 * e_xy

        while(true)
        {
            tex.SetPixel(x1, y1, c);
            e_xy2 = e_xy + e_xy;
            if (e_xy2 + dy >= 0) // e_xy + e_x > 0  -> e_xy + e_xy + dy > 0
            {
                if (x1 == x2) break;
                x1 += xStep;
                e_xy -= dy;
            }
            if (e_xy2 - dx <= 0) // e_xy + e_y < 0 -> e_xy + e_xy - dx < 0
            {
                if (y1 == y2) break;
                y1+=yStep;
                e_xy += dx;
            }
        }
    }

    // https://www.geeksforgeeks.org/anti-aliased-line-xiaolin-wus-algorithm/
    static void DrawXiaolinWuLine(Texture2D tex, int x1, int y1, int x2, int y2, Color c)
    {
        bool isSteep = Mathf.Abs(y2 - y1) > Mathf.Abs(x2 - x1);
        if (isSteep)
        {
            MyUtility.Swap(ref x1, ref y1);
            MyUtility.Swap(ref x2, ref y2);
        }

        if (x1 > x2)
        {
            MyUtility.Swap(ref x1, ref x2);
            MyUtility.Swap(ref y1, ref y2);
        }

        // 0 < abs(dy) < dx
        int dy = System.Math.Abs(y2 - y1);
        int dx = x2 - x1;

        int ystep = y1 < y2 ? 1 : -1;
        int slope_error = dy - dx;
        float scope = dx + dy;
        for (int x = x1, y = y1; x <= x2; x ++)
        {
            float brightness = (scope + slope_error) / scope;
            tex.SetPixel(isSteep ? y : x, isSteep ? x : y, c * brightness); // 和 Bresenham line 不同的地方
            slope_error += dy;
            if (slope_error >= 0)
            {
                y += ystep;
                slope_error -= dx;
            } else
            {
                tex.SetPixel(isSteep ? y + ystep : x, isSteep ? x : y + ystep, c * (1 - brightness)); // next_ystep = y + ystep
            }
        }
    }
}
