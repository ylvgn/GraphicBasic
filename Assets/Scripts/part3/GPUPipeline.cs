using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUPipeline : MonoBehaviour
{
    public delegate ShaderSemantic VertexShader(ShaderSemantic appdata, ShaderParameters shaderParams);
    public delegate Vector4 PixelShader(ShaderSemantic v2f, ShaderParameters shaderParams);

    public Camera MyCamera;
    Texture2D tex;
    List<MyShaderBase> inputObj;

    public enum ShadingMode
    {
        Shaded,
        Wireframe,
    }

    public ShadingMode shadingMode = ShadingMode.Shaded;

    // debug
    public bool IsDebuging;
    public Color bgColor;
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
            tex = new Texture2D(Screen.width, Screen.height,TextureFormat.ARGB32, true, true);
            tex.name = "MyTexture";
        }

        // clean
        for (int i = 0; i < tex.height; i++)
            for (int j = 0; j < tex.width; j++)
                tex.SetPixel(j, i, bgColor);

        Run();
        tex.Apply(false);
    }

    void Run()
    {
        for (int i = 0; i < 1; i++)
        {
            var obj = inputObj[i];
            var meshFilter = obj.GetComponent<MeshFilter>();
            var mesh = meshFilter.sharedMesh;
            Draw(obj, mesh);
        }
    }

    void Draw(MyShaderBase obj, Mesh mesh)
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

        ShaderSemantic[] v2fs = new ShaderSemantic[vertices.Length];

        // gen data
        for (int i = 0; i < vertices.Length; i++)
        {
            var appdata = (ShaderSemantic)System.Activator.CreateInstance(obj.CastType);
            var p = vertices[i];
            appdata.POSITION = new Vector4(p.x, p.y, p.z, 1);
            appdata.NORMAL = mesh.normals[i];
            appdata.TANGENT = mesh.tangents[i];
            appdata.TEXCOORD0 = mesh.uv[i];
            appdata.COLOR = Color.blue;
            v2fs[i] = appdata;
        }

        // vertex shader
        for (int i = 0; i < vertices.Length; i++)
        {
            var appdata = v2fs[i];
            // vertex shader: local space -> world space -> view space -> clip space
            ShaderSemantic v2f = obj.VertexShader(appdata, shaderParams);
            var p = v2f.SV_POSITION;

            // homogeneous divide: clip space -> NDC[-1, 1]
            p = new Vector4(p.x / p.w, p.y / p.w, p.z / p.w, 1);

            // NDC -> screen space (window space) https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-semantics?redirectedfrom=MSDN#direct3d-9-vpos-and-direct3d-10-sv_position
            v2f.SV_POSITION = new Vector3(p.x * w + w, p.y * h + h, (far - near) * p.z / 2 + (far + near) / 2);
            v2fs[i] = v2f;
        }

        if (shadingMode == ShadingMode.Wireframe)
            Wireframe(obj, mesh, v2fs); // Wireframe
        else
            Shaded(obj, mesh, v2fs); // Shaded
    }

    void Wireframe(MyShaderBase obj, Mesh mesh, ShaderSemantic[] v2fs)
    {
        var triangles = mesh.triangles;
        int last_x = 0, last_y = 0;
        for (int i = 0; i < triangles.Length; i++)
        {
            var v2f = v2fs[triangles[i]];
            var p = v2f.SV_POSITION;
            int x = (int)p.x;
            int y = (int)p.y;
            if (i > 0 && i % 3 != 0)
            {
                DrawLineClamp(last_x, last_y, x, y, Color.white);
            }
            last_x = x;
            last_y = y;
        }
    }

    void Shaded(MyShaderBase obj, Mesh mesh, ShaderSemantic[] v2fs)
    {
        var triangles = mesh.triangles;
        for (int j = 0; j < triangles.Length; j += 3)
        {
            ShaderSemantic p1 = v2fs[triangles[j]];
            ShaderSemantic p2 = v2fs[triangles[j + 1]];
            ShaderSemantic p3 = v2fs[triangles[j + 2]];

            // 令 y1 ≥ y2 ≥ y3
            if (p2.SV_POSITION.y <= p3.SV_POSITION.y)
            {
                ShaderSemantic.Swap(ref p2, ref p3);
            }
            if (p1.SV_POSITION.y <= p2.SV_POSITION.y)
            {
                ShaderSemantic.Swap(ref p2, ref p1);
            }
            if (p2.SV_POSITION.y < p3.SV_POSITION.y)
            {
                ShaderSemantic.Swap(ref p2, ref p3);
            }
            // 令 y2 == y3 && x2 < x3
            if (p2.SV_POSITION.y == p3.SV_POSITION.y && p2.SV_POSITION.x > p3.SV_POSITION.x)
            {
                ShaderSemantic.Swap(ref p2, ref p3);
            }
            // 令 y1 == y2 && x2 < x1
            if (p2.SV_POSITION.y == p1.SV_POSITION.y && p2.SV_POSITION.x > p1.SV_POSITION.x)
            {
                ShaderSemantic.Swap(ref p2, ref p1);
            }

            float x1 = p1.SV_POSITION.x, y1 = p1.SV_POSITION.y;
            float x2 = p2.SV_POSITION.x, y2 = p2.SV_POSITION.y;
            float x3 = p3.SV_POSITION.x, y3 = p3.SV_POSITION.y;

            float M_x = (y2 - y1) / ((y1 - y3) / (x1 - x3)) + x1; // 直线AC y - y1 = m * (x - x1), m = (y1-y3)/(x1-x3)
            var M = (ShaderSemantic)System.Activator.CreateInstance(p1.GetType());
            M.SetData(p2);
            M.SV_POSITION = new Vector4(M_x, y2, p2.SV_POSITION.z, 1);

            var ABCDir = Vector3.Cross(p2.SV_POSITION - p1.SV_POSITION, p3.SV_POSITION - p1.SV_POSITION);
            var ABMDir = Vector3.Cross(p2.SV_POSITION - p1.SV_POSITION, M.SV_POSITION - p1.SV_POSITION);
            var Left = p2.SV_POSITION.x < M.SV_POSITION.x ? p2 : M;
            var Right = p2.SV_POSITION.x < M.SV_POSITION.x ? M : p2;
            if (ABCDir.z * ABMDir.z > 0) // ABC-ABM(SAME DIR) ABC-CBM(REVERSE DIR)
            {
                DrawTriangleClamp(p1, Left, Right, -1, obj);
                DrawTriangleClamp(p3, Left, Right, +1, obj);
            }
            else
            {
                DrawTriangleClamp(p1, Left, Right, +1, obj);
                DrawTriangleClamp(p3, Left, Right, +1, obj);
            }

            // 最后画BM, tmp
            DrawLineClamp((int)p2.SV_POSITION.x, (int)p2.SV_POSITION.y, (int)M.SV_POSITION.x, (int)M.SV_POSITION.y, M.COLOR);
        }
    }

    void DrawLineClamp(int x1, int y1, int x2, int y2, Color c)
    {
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
        for (int x = x1, y = y1; x <= x2; x++)
        {
            if (isSteep) {
                if (y > 0 && y < tex.width && x > 0 && x < tex.height)
                    tex.SetPixel(y, x, c);
            }
            else {
                if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                    tex.SetPixel(x, y, c);
            }
            slope_err += dy;
            if (slope_err >= 0)
            {
                slope_err -= dx;
                y += ystep;
            }
        }
    }

    void DrawTriangleClamp(ShaderSemantic p1, ShaderSemantic p2, ShaderSemantic p3, int yStep, MyShaderBase obj)
    {
        int x1 = (int)p1.SV_POSITION.x, y1 = (int)p1.SV_POSITION.y;
        int x2 = (int)p2.SV_POSITION.x, y2 = (int)p2.SV_POSITION.y;
        int x3 = (int)p3.SV_POSITION.x, y3 = (int)p3.SV_POSITION.y;
        if (y2 == y1 && y1 == y3) return;
        float m1 = (y1 - y2) * 1.0f / (x1 - x2); // 直线START-BOTTOM_LEFT: y - y1 = m1*(x - x1), m1 = (y1-y2)/(x1-x2)
        float m2 = (y1 - y3) * 1.0f / (x1 - x3); // 直线START-BOTTOM_RIGHT: y - y1 = m2*(x - x1), m2 = (y1-y3)/(x1-x3)
        for (int y = y1; y != y2;)
        {
            int xl = (int)((y - y1) / m1 + x1);
            int xr = (int)((y - y1) / m2 + x1);
            for (int x = xl; x <= xr; x++)
            {
                var v2f = (ShaderSemantic)System.Activator.CreateInstance(p1.GetType());
                v2f.SetData(p1);
                v2f.SV_POSITION = new Vector4(x, y, 1, 1); // tmp
                Color c = obj.PixelShader(v2f, null);
                tex.SetPixel(x, y, c);
            }
            y += yStep;
        }
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
}
