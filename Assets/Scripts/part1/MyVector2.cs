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

    public static float Dot(MyVector2 a, MyVector2 b)
    {
        return a.x * b.x + a.y * b.y;
    }

    public static MyVector3 Cross(MyVector2 a, MyVector2 b)
    {
        return MyVector3.Cross(a, b);
    }

    public static MyVector2 operator *(MyVector2 a, float k)
    {
        return new MyVector2(a.x * k, a.y * k);
    }

    public static MyVector2 operator *(float k, MyVector2 a)
    {
        return a * k;
    }

    public static MyVector2 operator +(MyVector2 a, MyVector2 b)
    {
        MyVector2 v2;
        v2.x = a.x + b.x;
        v2.y = a.y + b.y;
        return v2;
    }

    public static MyVector2 operator -(MyVector2 a, MyVector2 b)
    {
        MyVector2 v2;
        v2.x = a.x - b.x;
        v2.y = a.y - b.y;
        return v2;
    }

    public override string ToString()
    {
        return string.Format($"({x}, {y})");
    }
}
