using UnityEngine;

[System.Serializable]
public struct MyVector3
{
    public float x;
    public float y;
    public float z;

    public MyVector3(float x_, float y_, float z_)
    {
        x = x_;
        y = y_;
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

    public override string ToString()
    {
        return string.Format($"({x}, {y}, {z})");
    }
}
