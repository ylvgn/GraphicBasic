using UnityEngine;

[System.Serializable]
public class Buffer<T>
{
    [SerializeField] public T[] data;
    private int width;
    private int height;

    public Buffer(Texture2D canvas)
    {
        // index = x + y * tex.height
        width = canvas.width;
        height = canvas.height;
        data = new T[width * height]; // size = tex.width * tex.height.
    }

    public void Clean(T val = default(T))
    {
        for (int i = 0; i < data.Length; i++)
            data[i] = val;
    }

    public int GetIndex(int x, int y)
    {
        return x + y * height;
    }

    void safeCheck(int index)
    {
        if (index < 0 || index >= data.Length)
            throw new System.IndexOutOfRangeException("Invalid index!");
    }

    public T this[int index]
    {
        get
        {
            safeCheck(index);
            return data[index];
        }

        set
        {
            safeCheck(index);
            data[index] = value;
        }
    }

    public T this[int x, int y]
    {
        get
        {
            int index = GetIndex(x, y);
            return this[index];
        }

        set
        {
            int index = GetIndex(x, y);
            this[index] = value;
        }
    }
}

[System.Serializable]
public class RenderBuffer
{
    [SerializeField] public Buffer<float> depthBuffer;
    [SerializeField] public Buffer<Color> colorBuffer;

    public RenderBuffer(Texture2D canvas)
    {
        depthBuffer = new Buffer<float>(canvas);
        colorBuffer = new Buffer<Color>(canvas);
    }

    public float GetDepth(int x, int y)
    {
        return depthBuffer[x, y];
    }

    public Color GetColor(int x, int y)
    {
        return colorBuffer[x, y];
    }

    private void SetColor(int x, int y, Color c)
    {
        colorBuffer[x, y] = c;
    }

    private void SetDepth(int x, int y, float d)
    {
        depthBuffer[x, y] = d;
    }

    private bool DepthTest(int x, int y, ShaderSemantic v2f)
    {
        var depth = v2f.POSITION.z;
        return depth >= 0 && depth <= 1;
    }

    public void Update(MyShaderBase obj, int x, int y, ShaderSemantic v2f, Color outputColor)
    {
        if (!DepthTest(x, y, v2f)) return;
        
        var newDepth = v2f.POSITION.z;
        var oldDepth = GetDepth(x, y);
        switch (obj.Commands.ZWrite)
        {
            case ZWrite.On:
                switch (obj.Commands.ZTest)
                {
                    case ZTest.Less:
                        if (newDepth < oldDepth)
                        {
                            SetDepth(x, y, newDepth);
                            SetColor(x, y, outputColor);
                        }
                        break;
                    case ZTest.LEqual:
        
                        if (newDepth <= oldDepth)
                        {
                            SetDepth(x, y, newDepth);
                            SetColor(x, y, outputColor);
                        }
                        break;
                    case ZTest.Equal:
                        if (newDepth == oldDepth)
                        {
                            SetDepth(x, y, newDepth);
                            SetColor(x, y, outputColor);
                        }
                        break;
                    case ZTest.GEqual:
                        if (newDepth >= oldDepth)
                        {
                            SetDepth(x, y, newDepth);
                            SetColor(x, y, outputColor);
                        }
                        break;
                    case ZTest.Greater:
                        if (newDepth > oldDepth)
                        {
                            SetDepth(x, y, newDepth);
                            SetColor(x, y, outputColor);
                        }
                        break;
                    case ZTest.NotEqual:
                        if (newDepth != oldDepth)
                        {
                            SetDepth(x, y, newDepth);
                            SetColor(x, y, outputColor);
                        }
                        break;
                    case ZTest.Always:
                        SetDepth(x, y, newDepth);
                        SetColor(x, y, outputColor);
                        break;
                }
                break;
            case ZWrite.Off:
                break;
            default:
                break;
        }
    }

    public void Clean()
    {
        colorBuffer.Clean(Color.black);
        depthBuffer.Clean(1); // test tmp
    }
}