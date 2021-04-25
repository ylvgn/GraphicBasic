using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRect : MonoBehaviour
{
    public enum RectType
    {
        Rect,
        RectOutLine,
        RectAndOutLine,
    }
    public RectType type;

    public MyVector2 center;
    [Range(0,  MyGUI.WIDTH)] public float width;
    [Range(0, MyGUI.HEIGHT)] public float height;
    public Color colorRect;
    public Color colorOutLine;

    public void MyDraw(Texture2D tex)
    {
        switch (type)
        {
            case RectType.Rect:
                MyDrawRect(tex);
                break;
            case RectType.RectOutLine:
                MyDrawRectOutLine(tex);
                break;
            case RectType.RectAndOutLine:
                MyDrawRect(tex);
                MyDrawRectOutLine(tex);
                break;
            default:
                break;
        }
    }

    void MyDrawRect(Texture2D tex)
    {
        float left_bottom_x = center.x - width / 2;
        float right_top_x = center.x + height / 2;
        float left_bottom_y = center.y - width / 2;
        float right_top_y = center.y + height / 2;

        int min_x = (int)left_bottom_x;
        int max_x = (int)right_top_x;
        int min_y = (int)left_bottom_y;
        int max_y = (int)right_top_y;

        for (int j = min_y; j <= max_y; j++)
            for (int i = min_x; i <= max_x; i++)
            {
                if (i < 0 || j < 0 || i >= tex.width || j >= tex.height) continue;
                tex.SetPixel(i, j, colorRect);
            }
    }

    void MyDrawRectOutLine(Texture2D tex)
    {
        float left_bottom_x = center.x - width / 2;
        float right_top_x = center.x + height / 2;
        float left_bottom_y = center.y - width / 2;
        float right_top_y = center.y + height / 2;

        int min_x = (int)left_bottom_x;
        int max_x = (int)right_top_x;
        int min_y = (int)left_bottom_y;
        int max_y = (int)right_top_y;

        min_x = MyUtility.Clamp(min_x, 0, tex.width);
        max_x = MyUtility.Clamp(max_x, 0, tex.width);
        min_y = MyUtility.Clamp(min_y, 0, tex.height);
        max_y = MyUtility.Clamp(max_y, 0, tex.height);

        for (int x = min_x; x <= max_x; x ++)
            tex.SetPixel(x, max_y, colorOutLine);

        for (int y = min_y; y <= max_y; y ++) {
            tex.SetPixel(min_x, y, colorOutLine);
            tex.SetPixel(max_x, y, colorOutLine);
        }

        for (int x = min_x; x <= max_x; x ++)
            tex.SetPixel(x, min_y, colorOutLine);
    }
}
