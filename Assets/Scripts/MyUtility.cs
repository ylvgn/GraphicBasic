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
}
