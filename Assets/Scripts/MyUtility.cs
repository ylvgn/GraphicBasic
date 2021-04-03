using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyUtility
{
    public static int RoundToInt(float x)
    {
        int res = (int)x;
        float d = x - res;
        if (d * 10 >= 5) return res + 1;
        return res;
    }

    public static float Dot(Vector2 a, Vector2 b)
    {
        return (a.x * b.x) + (a.y * b.y);
    }

    public static float Dot(Vector3 a, Vector3 b)
    {
        return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
    }

    public static float Length(Vector2 vec)
    {
        return Mathf.Sqrt(MyUtility.Dot(vec, vec));
    }
    public static Vector2 MakeVector(float x1, float y1, float x2, float y2)
    {
        return new Vector2(x2 - x1, y2 - y1);
    }

    public static Vector3 MakeVector(float x1, float y1, float z1, float x2, float y2, float z2)
    {
        return new Vector3(x2 - x1, y2 - y1, z2 - z1);
    }

    public static float Length(Vector3 vec)
    {
        return Mathf.Sqrt(MyUtility.Dot(vec, vec));
    }

    public static bool CheckIsValidTriangle(int x1, int y1, int x2, int y2, int x3, int y3)
    {
        Vector2 a = MyUtility.MakeVector(x1, y1, x2, y2);
        Vector2 b = MyUtility.MakeVector(x1, y1, x3, y3);
        Vector2 c = MyUtility.MakeVector(x2, y2, x3, y3);
        float len_a = MyUtility.Length(a);
        float len_b = MyUtility.Length(b);
        float len_c = MyUtility.Length(c);
        if ((len_a + len_b < len_c) || (len_a + len_c < len_b) || (len_b + len_c < len_a))
        {
            return false;
        }
        return true;
    }

    public static bool IsExistSlope(int x1, int y1, int x2, int y2)
    {
        return x1 != x2;
    }

    public static float GetSlope(int x1, int y1, int x2, int y2)
    {
        if (!IsExistSlope(x1, y1, x2, y2))
        {
            Debug.LogError("not exist");
            return 0;
        }
        return (y2 - y1) * 1.0f / (x2 - x1);
    }

    // k > 0 的右边, k < 0 的左边
    public static bool CheckIsOnLineRight(int x1, int y1, int x2, int y2, int x, int y)
    {
        if (!IsExistSlope(x1, y1, x2, y2))
        {
            return x > x1;
        }
        // y - y0 = k * (x - x0)
        float k = MyUtility.GetSlope(x1, y1, x2, y2);
        if (k == 0) return y <= y1;
        bool res = k * (x - x1) + y1 >= y;
        return k > 0 ? res : !res;
    }

    // K > 0 的左边, k < 0的右边
    public static bool CheckIsOnLineLeft(int x1, int y1, int x2, int y2, int x, int y)
    {
        if (!IsExistSlope(x1, y1, x2, y2))
        {
            return x > x1;
        }

        float k = MyUtility.GetSlope(x1, y1, x2, y2);
        if (k == 0) return y >= y1;
        bool res = k * (x - x1) + y1 <= y;
        return k > 0 ? res : !res;
    }
}
