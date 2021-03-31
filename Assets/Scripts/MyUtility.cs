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
}
