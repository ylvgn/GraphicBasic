using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBizierCurve : MonoBehaviour
{
    public MyVector2[] Points;
    public Color color;

    public enum BizierCurveType
    {
        Quad_th_BizierCurve,
        MyNth_BizierCurve,
        Nth_BizierCurve,
    }

    public BizierCurveType type;

    public void MyDraw(Texture2D tex)
    {
        if (Points == null || Points.Length < 1) return;

        switch (type)
        {
            case BizierCurveType.Quad_th_BizierCurve:
                Quad_th_BizierCurve(tex);
                break;
            case BizierCurveType.MyNth_BizierCurve:
                MyNth_BizierCurve(tex);
                break;
            case BizierCurveType.Nth_BizierCurve:
                Nth_BizierCurve(tex);
                break;
            default:
                break;
        }

        // debug
        int n = Points.Length;
        MyUtility.DrawPoint(tex, (int)Points[0].x, (int)Points[0].y, Color.red);
        for (int i = 0, j = 1; j < n; i++, j++)
        {
            var pre = Points[i];
            var cur = Points[j];
            int x0 = (int)pre.x, y0 = (int)pre.y;
            int x1 = (int)cur.x, y1 = (int)cur.y;
            DrawLine.Draw(tex, x0, y0, x1, y1, Color.white, DrawLine.LineType.Bresenham);
            MyUtility.DrawPoint(tex, x1, y1, Color.red);
        }
    }

    // https://www.jasondavies.com/animated-bezier/
    void Quad_th_BizierCurve(Texture2D tex, float t = 0)
    {
        /* t 在[0, 1]
         * n = 2 -> B1(t) = P0 + (P1 - p0)t, 从P0起点沿P0->P1直线方向移动, 当t = 1时到达P1
         * n = 3 -> B2(t) = (1-t)²*P0 + (1-t)*t*P1 + t²*P2
         */
        if (Points == null || Points.Length < 3) return;
        if (t >= 1) return;

        MyVector2 p1 = Points[0];
        MyVector2 p2 = Points[1];
        MyVector2 p3 = Points[2];

        int x1 = (int)p1.x, y1 = (int)p1.y;
        int x2 = (int)p2.x, y2 = (int)p2.y;
        int x3 = (int)p3.x, y3 = (int)p3.y;
        DrawLine.Draw(tex, x1, y1, x2, y2, Color.white, DrawLine.LineType.Bresenham);
        DrawLine.Draw(tex, x2, y2, x3, y3, Color.white, DrawLine.LineType.Bresenham);
        MyUtility.DrawPoint(tex, x1, y1, Color.red);
        MyUtility.DrawPoint(tex, x2, y2, Color.green);
        MyUtility.DrawPoint(tex, x3, y3, Color.blue);

        // p' = (1-t)*p0 + t*p1
        var pa = (1 - t) * p1 + t * p2;
        var pb = (1 - t) * p2 + t * p3;
        var o = (1 - t) * pa + t * pb;
        tex.SetPixel((int)o.x, (int)o.y, Color.red);
        Quad_th_BizierCurve(tex, t + 0.01f);
    }

    MyVector2 Get_B(MyVector2[] p, int n, float t)
    {
        if (n == 1) return p[0];
        MyVector2[] backup = new MyVector2[n - 1];
        for (int i = 0; i < n - 1; i ++) {
            backup[i] = (1 - t) *p[i] + t * p[i + 1];
        }
        return Get_B(backup, n - 1, t);
    }

    [Range(0, 1)] public float dt = 1;
    void MyNth_BizierCurve(Texture2D tex, float t = 0)
    {
        if (t >= 1 || t >= dt) return;
        int n = Points.Length;
        var o = Get_B(Points, n, t);
        tex.SetPixel((int)o.x, (int)o.y, color);
        MyNth_BizierCurve(tex, t + 0.01f);
    }
    
    // https://en.wikipedia.org/wiki/B%C3%A9zier_curve#Constructing_B.C3.A9zier_curves
    void Nth_BizierCurve(Texture2D tex, float t = 0)
    {
        if (Points == null || Points.Length < 1) return;
        if (t >= 1 || t >= dt) return;
        int n = Points.Length;

        MyVector2 o = new MyVector2(0, 0);
        // C[a][b]: 组合数, 从a个里面选b个
        float[][] C = new float[n+1][];
        for (int i = 0; i <= n; i++)
        {
            C[i] = new float[n + 1];
            for (int j = 0; j <= i; j++)
            {
                if (j == 0) C[i][j] = 1;
                else C[i][j] = C[i - 1][j - 1] + C[i - 1][j];
            }
        }

        for (int i = 0; i < n; i ++)
        {
            var pi = Points[i];
            var k = C[n-1][i] * Mathf.Pow(t, i) * Mathf.Pow(1 - t, n - 1 - i);
            o += k * pi;
        }
        tex.SetPixel((int)o.x, (int)o.y, color);
        Nth_BizierCurve(tex, t + 0.01f);
    }
}
