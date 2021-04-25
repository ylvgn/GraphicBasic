using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTriangle : MonoBehaviour
{
    public enum TriangleType
    {
        Triangle,
        TriangleWithRect,
        TriangleOutLine,
        TriangleAndOutLine,
    }

    public MyVector2 p1;
    public MyVector2 p2;
    public MyVector2 p3;

    public TriangleType type;
    public Color triangleColor;
    public Color outLineColor;

    public void MyDraw(Texture2D tex)
    {
        switch (type)
        {
            case TriangleType.Triangle:
                MyDrawTriangle(tex);
                break;
            case TriangleType.TriangleWithRect:
                MyDrawTriangleWithRect(tex);
                break;
            case TriangleType.TriangleOutLine:
                MyDrawTriangleOutLine(tex);
                break;
            case TriangleType.TriangleAndOutLine:
                MyDrawTriangle(tex);
                MyDrawTriangleOutLine(tex);
                break;
            default:
                break;
        }
    }

    void MyDrawTriangle(Texture2D tex)
    {
        float x1 = p1.x, y1 = p1.y;
        float x2 = p2.x, y2 = p2.y;
        float x3 = p3.x, y3 = p3.y;

        // 令 y1 ≥ y2 ≥ y3
        if (y2 <= y3)
        {
            MyUtility.Swap(ref y2, ref y3);
            MyUtility.Swap(ref x2, ref x3);
        }
        if (y1 <= y2)
        {
            MyUtility.Swap(ref y2, ref y1);
            MyUtility.Swap(ref x2, ref x1);
        }
        if (y2 < y3)
        {
            MyUtility.Swap(ref y2, ref y3);
            MyUtility.Swap(ref x2, ref x3);
        }
        // 令 y2 == y3 && x2 < x3
        if (y2 == y3 && x2 > x3)
        {
            MyUtility.Swap(ref y2, ref y3);
            MyUtility.Swap(ref x2, ref x3);
        }
        // 令 y1 == y2 && x2 < x1
        if (y2 == y1 && x2 > x1)
        {
            MyUtility.Swap(ref y2, ref y1);
            MyUtility.Swap(ref x2, ref x1);
        }

        var A = new MyVector3(x1, y1);
        var B = new MyVector3(x2, y2);
        var C = new MyVector3(x3, y3);

        // 直线AC y - y1 = m * (x - x1), m = (y1-y3)/(x1-x3)
        float M_x = (y2 - y1) / ((y1 - y3) / (x1 - x3)) + x1;
        MyVector3 M = new MyVector3(M_x, y2);
        var ABCDir = MyVector3.Cross(B - A, C - A);
        var ABMDir = MyVector3.Cross(B - A, M - A);

        var Left = B.x < M.x ? B : M;
        var Right = B.x < M.x ? M : B;
        if (ABCDir.z * ABMDir.z > 0) // ABC-ABM(SAME DIR) ABC-CBM(REVERSE DIR)
        {
            DrawTriangleInner(tex, A, Left, Right, -1);
            DrawTriangleInner(tex, C, Left, Right, +1);
        } else
        {
            DrawTriangleInner(tex, A, Left, Right, +1);
            DrawTriangleInner(tex, C, Left, Right, +1);
        }

        // 最后画BM
        DrawLine.Draw(tex, (int)B.x, (int)B.y, (int)M.x, (int)M.y, triangleColor, DrawLine.LineType.Line);

        // 4个顶点
        MyUtility.DrawPoint(tex, (int)A.x, (int)A.y, Color.red);
        MyUtility.DrawPoint(tex, (int)B.x, (int)B.y, Color.green);
        MyUtility.DrawPoint(tex, (int)C.x, (int)C.y, Color.blue);
        MyUtility.DrawPoint(tex, (int)M.x, (int)M.y, Color.yellow);
    }

    void DrawTriangleInner(Texture2D tex, MyVector3 START, MyVector3 BOTTOM_LEFT, MyVector3 BOTTOM_RIGHT, int yStep)
    {
        int x1 = (int)START.x, y1 = (int)START.y;
        int x2 = (int)BOTTOM_LEFT.x, y2 = (int)BOTTOM_LEFT.y;
        int x3 = (int)BOTTOM_RIGHT.x, y3 = (int)BOTTOM_RIGHT.y;
        if (y2 == y1 && y1 == y3) return;
        float m1 = (y1 - y2) *1.0f / (x1 - x2); // 直线START-BOTTOM_LEFT: y - y1 = m1*(x - x1), m1 = (y1-y2)/(x1-x2)
        float m2 = (y1 - y3) *1.0f / (x1 - x3); // 直线START-BOTTOM_RIGHT: y - y1 = m2*(x - x1), m2 = (y1-y3)/(x1-x3)
        //Debug.Log($"y1={y1}, y2={y2}, yStep={yStep} m1={m1}, m2={m2}");
        for (int y = y1; y != y2;)
        {
            //求直线y = c 和 两直线的交点
            int xl = (int)((y - y1) / m1 + x1);
            int xr = (int)((y - y1) / m2 + x1);
            //Debug.Log($"{y} : xl={xl}, xr={xr}");
            for (int x = xl; x <= xr; x ++)
                tex.SetPixel(x, y, triangleColor);
            y += yStep;
        }
    }

    // https://blackpawn.com/texts/pointinpoly/
    bool IsSameSide(MyVector3 p1, MyVector3 p2, MyVector3 a, MyVector3 b)
    {
        var cp1 = MyVector3.Cross(b - a, p1 - a);
        var cp2 = MyVector3.Cross(b - a, p2 - a);
        return MyVector3.Dot(cp1, cp2) >= 0;
    }

    void MyDrawTriangleWithRect(Texture2D tex)
    {
        MyVector3 A = new MyVector3(p1);
        MyVector3 B = new MyVector3(p2);
        MyVector3 C = new MyVector3(p3);

        int min_x = (int)Mathf.Min(p1.x, p2.x, p3.x);
        int max_x = (int)Mathf.Max(p1.x, p2.x, p3.x);
        int min_y = (int)Mathf.Min(p1.y, p2.y, p3.y);
        int max_y = (int)Mathf.Max(p1.y, p2.y, p3.y);

        for (int y = min_y; y <= max_y; y ++)
            for(int x = min_x; x <= max_x; x ++)
            {
                MyVector3 P = new MyVector3(x, y); // 检查P 是否在 A->B && B->C && C->A 的内侧(内侧即 向量顺时针摆动能碰到点P)
                if (IsSameSide(P, A, B, C) && IsSameSide(P, B, C, A) && IsSameSide(P, C, A, B))
                    tex.SetPixel(x, y, triangleColor);
            }
    }

    void MyDrawTriangleOutLine(Texture2D tex)
    {
        int x1 = (int)p1.x, y1 = (int)p1.y;
        int x2 = (int)p2.x, y2 = (int)p2.y;
        int x3 = (int)p3.x, y3 = (int)p3.y;
        DrawLine.Draw(tex, x1, y1, x2, y2, outLineColor, DrawLine.LineType.Line);
        DrawLine.Draw(tex, x2, y2, x3, y3, outLineColor, DrawLine.LineType.Line);
        DrawLine.Draw(tex, x3, y3, x1, y1, outLineColor, DrawLine.LineType.Line);
    }
}
