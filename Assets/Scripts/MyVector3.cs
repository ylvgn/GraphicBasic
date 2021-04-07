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

    public static void Swap(ref MyVector3 a, ref MyVector3 b)
    {
        MyUtility.Swap(ref a.x, ref b.x);
        MyUtility.Swap(ref a.y, ref b.y);
        MyUtility.Swap(ref a.z, ref b.z);
    }
}
