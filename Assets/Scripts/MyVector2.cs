using UnityEngine;

[System.Serializable]
public struct MyVector2
{
    public float x;
    public float y;

    public MyVector2(float x_, float y_)
    {
        x = x_;
        y = y_;
    }

    public float Length()
    {
        return Mathf.Sqrt(Dot(this, this));
    }

    public static float Dot(MyVector2 a, MyVector2 b) {
        return a.x * b.x + a.y * b.y;
    }

    public static void Swap(ref MyVector2 a, ref MyVector2 b)
    {
        MyUtility.Swap(ref a.x, ref b.x);
        MyUtility.Swap(ref a.y, ref b.y);
    }
}
