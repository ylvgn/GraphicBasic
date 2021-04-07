using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyUtility
{
    // 四舍五入
    public static int RoundToInt(float x)
    {
        int res = (int)x;
        float d = x - res;
        if (d * 10 >= 5) return res + 1;
        return res;
    }

    public static void Swap(ref float a, ref float b)
    {
        float tmp = a;
        a = b;
        b = tmp;
    }

    public static void Swap(ref int a, ref int b)
    {
        int tmp = a;
        a = b;
        b = tmp;
    }

    public static int Clamp(int value, int minv, int maxv)
    {
        if (minv > maxv)
        {
            Debug.LogError("minv > maxv");
            return value;
        }

        if (value < minv) value = minv;
        else if (value > maxv) value = maxv;
        return value;
    }
}
