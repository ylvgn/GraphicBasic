using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUPipeline : MonoBehaviour
{
    public delegate ShaderSemantic VertexShader(ShaderSemantic appdata, ShaderParameters shaderParams);
    public delegate Vector4 PixelShader(ShaderSemantic v2f, ShaderParameters shaderParams);
    public enum ShadingMode
    {
        Shaded,
        Wireframe,
    }

    public Camera MyCamera;
    public Texture2D tex;
    public ShadingMode shadingMode;
    public RenderBuffer renderBuffer;
    public List<MyShaderBase> inputObj;

    // debug
    public bool IsDebuging;
    public Color textColor;

    void Start()
    {
        inputObj = new List<MyShaderBase>(GameObject.FindObjectsOfType<MyShaderBase>());
    }

    void Update()
    {
        if (!MyCamera) return;
        if (!tex || tex.width != Screen.width || tex.height != Screen.height)
        {
            MyUtility.MyDestroy(ref tex);
            tex = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, true, true);
            renderBuffer = new RenderBuffer(tex);
            tex.name = "MyTexture";
        }

        Clean();
        Run();
    }

    void Run()
    {
        for (int i = 0; i < 1; i++)
        {
            var obj = inputObj[i];
            var meshFilter = obj.GetComponent<MeshFilter>();
            var mesh = meshFilter.sharedMesh;
            DrawCall(obj, mesh);
        }

        for (int i = 0; i < tex.height; i++) // column
        {
            for (int j = 0; j < tex.width; j++) // row
            {
                Color c = renderBuffer.colorBuffer[j, i];
                tex.SetPixel(j, i, c);
            }
        }
        tex.Apply(false);
    }

    void DrawCall(MyShaderBase obj, Mesh mesh)
    {
        var vertices = mesh.vertices;
        var triangles = mesh.triangles;
        float screenWidth = MyCamera.pixelWidth;
        float screenHeight = MyCamera.pixelHeight;
        var far = MyCamera.farClipPlane;
        var near = MyCamera.nearClipPlane;
        float w = screenWidth / 2;
        float h = screenHeight / 2;
        var shaderParams = new ShaderParameters(obj.transform, MyCamera);

        ShaderSemantic[] appFullData = new ShaderSemantic[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            var appdata = (ShaderSemantic)System.Activator.CreateInstance(obj.CastType);
            var p = vertices[i];
            appdata.POSITION = new Vector4(p.x, p.y, p.z, 1);
            appdata.NORMAL = mesh.normals[i];
            appdata.TANGENT = mesh.tangents[i];
            appdata.TEXCOORD0 = mesh.uv[i];
            appFullData[i] = appdata;
        }

        // vertex shader
        for (int i = 0; i < vertices.Length; i++)
        {
            var appdata = appFullData[i];
            // vertex shader: local space -> world space -> view space -> clip space
            ShaderSemantic v2f = obj.VertexShader(appdata, shaderParams);
            var p = v2f.SV_POSITION;

            // homogeneous divide: clip space -> NDC[-1, 1]
            p = new Vector4(p.x / p.w, p.y / p.w, p.z / p.w, 1);
            v2f.POSITION = p;

            // NDC -> screen space (window space) https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-semantics?redirectedfrom=MSDN#direct3d-9-vpos-and-direct3d-10-sv_position
            v2f.SV_POSITION = new Vector3(p.x * w + w, p.y * h + h, (far - near) * p.z / 2 + (far + near) / 2);
            appFullData[i] = v2f;
        }

        // pixel shader
        if (shadingMode == ShadingMode.Wireframe)
            Wireframe(obj, shaderParams, mesh, appFullData); // Wireframe
        else
            Shaded(obj, shaderParams, mesh, appFullData);    // Shaded
    }

    void Wireframe(MyShaderBase obj, ShaderParameters shaderParams, Mesh mesh, ShaderSemantic[] appFullData)
    {
        var triangles = mesh.triangles;
        ShaderSemantic last = null;
        for (int i = 0; i < triangles.Length; i++)
        {
            var v2f = appFullData[triangles[i]];
            v2f.COLOR = Color.white;
            if (i > 0 && i % 3 != 0)
                DrawLine(obj, shaderParams, last, v2f);
            last = v2f;
        }
    }

    void Shaded(MyShaderBase obj, ShaderParameters shaderParams, Mesh mesh, ShaderSemantic[] appFullData)
    {
        var triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            ShaderSemantic p1 = appFullData[triangles[i]];
            ShaderSemantic p2 = appFullData[triangles[i + 1]];
            ShaderSemantic p3 = appFullData[triangles[i + 2]];

            //outline debug
            //DrawLine(obj, shaderParams, p1, p2);
            //DrawLine(obj, shaderParams, p2, p3);
            //DrawLine(obj, shaderParams, p3, p1);

            // debug color
            p1.COLOR = Color.red;
            p2.COLOR = Color.green;
            p3.COLOR = Color.blue;

            DrawTriangle(obj, shaderParams, p1, p2, p3);
            //break; // debug
        }
    }

    void DrawTriangle(MyShaderBase obj, ShaderParameters shaderParams, ShaderSemantic p1, ShaderSemantic p2, ShaderSemantic p3)
    {
        // 令 y1 ≥ y2 ≥ y3
        if (p2.SV_POSITION.y <= p3.SV_POSITION.y)
        {
            MyUtility.Swap(ref p2, ref p3);
        }
        if (p1.SV_POSITION.y <= p2.SV_POSITION.y)
        {
            MyUtility.Swap(ref p2, ref p1);
        }
        if (p2.SV_POSITION.y < p3.SV_POSITION.y)
        {
            MyUtility.Swap(ref p2, ref p3);
        }

        float y1 = p1.SV_POSITION.y; // A.y
        float y3 = p3.SV_POSITION.y; // C.y

        /*  将三角形在B点水平x轴切开上下两部分分开画：
         *  BM为水平直线, M在AC线上
         *  直线AC: y - y1 = m * (x - x1), 其中m = (y1-y3) / (x1-x3)
         *  将xy互换: x - x1 = m * (y - y1), 其中m = (x1-x3) / (y1-y3)
         *  已知: B.y == M.y(M_y), 求M.x 即(M_x)
         *      得: x = m * (y - y1) + x1
         */
        float M_y = p2.SV_POSITION.y;
        ShaderSemantic M = ShaderSemantic.CreateInstance(p1.GetType());
        float w = (M_y - y1) / (y3 - y1);
        M.Lerp(p1, p3, Mathf.Abs(w));

        // B和M的顺序 由x坐标确定
        ShaderSemantic Left = p2.SV_POSITION.x < M.SV_POSITION.x ? p2 : M;
        ShaderSemantic Right = p2.SV_POSITION.x < M.SV_POSITION.x ? M : p2;
        DrawTriangleInner(obj, shaderParams, p1, Left, Right, -1); // draw 🔺ABM
        DrawTriangleInner(obj, shaderParams, p3, Left, Right, +1); // draw 🔺CBM
        DrawLine(obj, shaderParams, Left, Right);                  // draw line BM

        // debug
        /*
        DrawPoint((int)p1.SV_POSITION.x, (int)p1.SV_POSITION.y, p1.COLOR);
        DrawPoint((int)p3.SV_POSITION.x, (int)p3.SV_POSITION.y, p3.COLOR);
        DrawPoint((int)M.SV_POSITION.x, (int)M.SV_POSITION.y, M.COLOR);
        DrawPoint((int)p2.SV_POSITION.x, (int)p2.SV_POSITION.y, p2.COLOR);
        */
    }

    // 在p1处开始画, p2p3是水平线, yStep为-1表示往下
    void DrawTriangleInner(MyShaderBase obj, ShaderParameters shaderParams, ShaderSemantic p1, ShaderSemantic p2, ShaderSemantic p3, int yStep)
    {
        float x1 = p1.SV_POSITION.x;
        int y1 = (int)p1.SV_POSITION.y;
        float x2 = p2.SV_POSITION.x;
        int y2 = (int)p2.SV_POSITION.y;
        float x3 = p3.SV_POSITION.x;
        float y3 = p3.SV_POSITION.y;

        float m1 = (y1 - y2) / (x1 - x2); // 直线p1p2: y - y1 = m1*(x - x1), m1 = (y1-y2)/(x1-x2)
        float m2 = (y1 - y3) / (x1 - x3); // 直线p1p3: y - y1 = m2*(x - x1), m2 = (y1-y3)/(x1-x3)

        var type = p1.GetType();
        for (int y = y1; y != y2;)
        {
            // scan line
            float xl = (y - y1) / m1 + x1;
            float leftWeight = (xl - x1) / (x2 - x1);
            ShaderSemantic left = ShaderSemantic.CreateInstance(type);
            left.Lerp(p1, p2, leftWeight);

            float xr = (y - y1) / m2 + x1;
            float rightWeight = (xr - x1) / (x3 - x1);
            ShaderSemantic right = ShaderSemantic.CreateInstance(type);
            right.Lerp(p1, p3, Mathf.Abs(rightWeight));

            DrawLine(obj, shaderParams, left, right);
            y += yStep;
        }
    }

    void DrawLine(MyShaderBase obj, ShaderParameters shaderParams, ShaderSemantic p1, ShaderSemantic p2)
    {
        int x1 = (int)p1.SV_POSITION.x;
        int y1 = (int)p1.SV_POSITION.y;
        int x2 = (int)p2.SV_POSITION.x;
        int y2 = (int)p2.SV_POSITION.y;

        // 令x1 < x2 && 0 < abs(dy) < dx
        bool isSteep = Mathf.Abs(y2 - y1) > Mathf.Abs(x2 - x1);
        if (isSteep)
        {
            MyUtility.Swap(ref x1, ref y1);
            MyUtility.Swap(ref x2, ref y2);
        }
        if (x1 > x2)
        {
            MyUtility.Swap(ref x1, ref x2);
            MyUtility.Swap(ref y1, ref y2);
        }

        int dy = System.Math.Abs(y2 - y1);
        int dx = x2 - x1;
        if (dy == 0 && dx == 0) return;
        int slope_err = dy - dx;
        int ystep = (y1 < y2) ? 1 : -1;
        var type = p1.GetType();
        for (int x = x1, y = y1; x <= x2; x++)
        {
            int _x = isSteep ? y : x;
            int _y = isSteep ? x : y;
            if (_x > 0 && _x < tex.width && _y > 0 && _y < tex.height)
            {
                ShaderSemantic v2f = ShaderSemantic.CreateInstance(type);
                float w = ((float)x - x1) / (x1 - x2);
                v2f.Lerp(p1, p2, Mathf.Abs(w));
                var color = obj.PixelShader(v2f, shaderParams);
                renderBuffer.Update(obj, _x, _y, v2f, color);
            }
            slope_err += dy;
            if (slope_err >= 0)
            {
                slope_err -= dx;
                y += ystep;
            }
        }
    }

    void Clean()
    {
        renderBuffer.Clean();
    }

    void OnGUI()
    {
        // output
        GUI.DrawTexture(new Rect(0, 0, tex.width, tex.height), tex);

        if (!IsDebuging) return;

        // debug
        MyUtility.LogPoint(new Rect(0, 0, 100, 20), $"Obj Count:{inputObj.Count}", textColor, 20);
        MyUtility.LogPoint(new Rect(0, 20, 100, 20), $"resolution:{tex.width}x{tex.height}", textColor, 20);

        var posY = 0;
        var offset = 120;
        posY -= offset;
        var str = $"==== unity_project matrix ====\n{MyCamera.projectionMatrix}";
        MyUtility.LogPoint(new Rect(0, MyCamera.pixelHeight + posY, 200, 150), str, textColor, 20);
        posY -= offset;
        var myProjectMatrix = MyUtility.GetProjectMatrix(MyCamera.fieldOfView, MyCamera.aspect, MyCamera.nearClipPlane, MyCamera.farClipPlane);
        str = $"==== my_project matrix ====\n{myProjectMatrix}";
        MyUtility.LogPoint(new Rect(0, MyCamera.pixelHeight + posY, 200, 150), str, textColor, 20);
        posY -= offset;
        str = $"==== unity_view matrix ====\n{MyCamera.worldToCameraMatrix}";
        MyUtility.LogPoint(new Rect(0, MyCamera.pixelHeight + posY, 200, 150), str, textColor, 20);
        posY -= offset;
        var myViewMatrix = MyUtility.GetViewMatrix(MyCamera);
        str = $"==== my_view matrix ====\n{myViewMatrix}";
        MyUtility.LogPoint(new Rect(0, MyCamera.pixelHeight + posY, 200, 150), str, textColor, 20);
        posY -= offset;

        /* debug model matrix
        var obj = inputObj[0];
        str = $"==== unity_local2World matrix ====\n{obj.transform.localToWorldMatrix}";
        MyUtility.LogPoint(new Rect(0, MyCamera.pixelHeight + posY, 200, 150), str, textColor, 20);
        posY -= offset;
        str = $"==== my_local2World matrix ====\n{MyUtility.GetModelMatrix(obj.transform)}";
        MyUtility.LogPoint(new Rect(0, MyCamera.pixelHeight + posY, 200, 150), str, textColor, 20);
        */
    }

    void DrawPoint(int x, int y, Color c)
    {
        int[] dx = { -1, 0, 1, 0, 0, -1, 1, -1, 1 };
        int[] dy = { 0, 1, 0, -1, 0, -1, -1, 1, 1 };
        for (int i = 0; i < dx.Length; i++)
        {
            int a = x + dx[i];
            int b = y + dy[i];
            if (a < 0 || a >= tex.width || b < 0 || b >= tex.height) continue;
            tex.SetPixel(a, b, c);
        }
    }
}
