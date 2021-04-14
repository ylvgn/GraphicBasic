using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTriangle : MonoBehaviour
{
    public enum TriangleType
    {
        Triangle,
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

    bool IsSameSide(MyVector3 p1, MyVector3 p2, MyVector3 a, MyVector3 b)
    {
        var cp1 = MyVector3.Cross(b - a, p1 - a);
        var cp2 = MyVector3.Cross(b - a, p2 - a);
        return MyVector3.Dot(cp1, cp2) >= 0;
    }

    void MyDrawTriangle(Texture2D tex)
    {
        // https://blackpawn.com/texts/pointinpoly/
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
                MyVector3 P = new MyVector3(x, y); // 检查P 是否在 A->B && B->C && C->A 的内侧
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
