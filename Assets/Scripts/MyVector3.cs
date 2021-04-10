using UnityEngine;

[System.Serializable]
public struct MyVector3
{
    public float x;
    public float y;
    public float z;

    public MyVector3(float x_, float y_ = 0, float z_ = 0)
    {
        x = x_;
        y = y_;
        z = z_;
    }

    public MyVector3(MyVector2 v2, float z_ = 0)
    {
        x = v2.x;
        y = v2.y;
        z = z_;
    }

    public float Length()
    {
        return Mathf.Sqrt(Dot(this, this));
    }

    public static float Dot(MyVector3 a, MyVector3 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    public static MyVector3 Cross(MyVector3 a, MyVector3 b)
    {
        /*
            | x  y  z  |
            | x1 y1 z1 |
            | x2 y2 z2 |
        */
        float x = a.y * b.z - a.z * b.y;
        float y = -(a.x * b.z - a.z * b.x);
        float z = a.x * b.y - a.y * b.x;
        return new MyVector3(x, y, z);
    }

    public static MyVector3 Cross(MyVector2 a, MyVector2 b)
    {
        return Cross(new MyVector3(a), new MyVector3(b));
    }
    public static MyVector3 operator +(MyVector3 a, MyVector3 b)
    {
        MyVector3 v3;
        v3.x = a.x + b.x;
        v3.y = a.y + b.y;
        v3.z = a.z + b.z;
        return v3;
    }

    public static MyVector3 operator -(MyVector3 a, MyVector3 b)
    {
        MyVector3 v3;
        v3.x = a.x - b.x;
        v3.y = a.y - b.y;
        v3.z = a.z - b.z;
        return v3;
    }

    public override string ToString()
    {
        return string.Format($"({x}, {y}, {z})");
    }
}
