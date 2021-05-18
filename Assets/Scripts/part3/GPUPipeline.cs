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
    public Color bgColor;
    public Color textColor;
    Vector3 mouseLeft;
    Vector3 mouseRight;
    Vector3 mouseMiddle;

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

        // debug
        MoveCamera();
    }

    void Run()
    {
        for (int i = 0; i < 1; i ++)
        {
            var obj = inputObj[i];
            var meshFilter = obj.GetComponent<MeshFilter>();
            var mesh = meshFilter.sharedMesh;
            var vertices = mesh.vertices;
            var triangles = mesh.triangles;

            if (shadingMode == ShadingMode.Wireframe)
            {
                int last_x = 0, last_y = 0;
                var M = MyUtility.GetModelMatrix(obj.transform);
                for (int j = 0; j < triangles.Length; j ++)
                {
                    var index = triangles[j];
                    var world_pos = obj.transform.localToWorldMatrix.MultiplyPoint(vertices[index]);
                    Vector3 screenPos = MyCamera.WorldToScreenPoint(world_pos);
                    int x = (int)screenPos.x;
                    int y = (int)screenPos.y;
                    if (j > 0 && j % 3 != 0)
                    {
                        DrawLineClamp(last_x, last_y, x, y, Color.white);
                    }
                    last_x = x;
                    last_y = y;
                }
                continue;
            }

            var shaderParams = new ShaderParameters(obj.transform, MyCamera);
            ShaderSemantic[] v2fs = new ShaderSemantic[vertices.Length];
            float screenWidth = MyCamera.pixelWidth;
            float screenHeight = MyCamera.pixelHeight;
            var far = MyCamera.farClipPlane;
            var near = MyCamera.nearClipPlane;
            float w = screenWidth / 2;
            float h = screenHeight / 2;
            for (int j = 0; j < vertices.Length; j ++)
            {
                var appdata = (ShaderSemantic)System.Activator.CreateInstance(obj.GetCastType);
                appdata.POSITION = new Vector4(vertices[j].x, vertices[j].y, vertices[j].z, 1);
                appdata.NORMAL = mesh.normals[j];
                appdata.TANGENT = mesh.tangents[j];
                appdata.TEXCOORD0 = mesh.uv[j];

                // vertex shader: local space -> world space -> view space -> clip space
                var v2f = obj.VertexShader(appdata, shaderParams);

                var p = v2f.SV_POSITION;
                // homogeneous divide: clip space -> Canonical View Volume(CVV) <==> NDC
                v2f.SV_POSITION = new Vector4(p.x / p.w, p.y / p.w, p.z / p.w, 1);

                // NDC -> screen space (window space)
                p = v2f.SV_POSITION;
                Vector3 screenPos = new Vector3(p.x * w + w, p.y * h + h, (far - near) * p.z / 2 + (far + near) / 2);
                tex.SetPixel((int)screenPos.x, (int)screenPos.y, Color.red);
                v2fs[j] = v2f;
            }
        }
    }

    void MoveCamera()
    {
        // offset
        if (Input.GetMouseButtonDown(2))
            mouseMiddle = MyCamera.transform.position;
        if (Input.GetMouseButton(2))
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            mouseMiddle.x += x;
            mouseMiddle.y += y;
            MyCamera.transform.position = Vector3.Lerp(MyCamera.transform.position, mouseMiddle, Time.deltaTime * 10);
        }

        // Rotate
        if (Input.GetMouseButtonDown(1))
            mouseRight = MyCamera.transform.eulerAngles;
        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            mouseRight.y += x;
            mouseRight.x += -y;
            MyCamera.transform.rotation = Quaternion.Lerp(MyCamera.transform.rotation, Quaternion.Euler(mouseRight), Time.deltaTime * 20);
        }

        // FOV
        if (Input.GetMouseButtonDown(0))
            mouseLeft = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            var dir = Input.mousePosition - mouseLeft;
            var speed = Mathf.Clamp(dir.y, -10, 10);
            var fov = MyCamera.fieldOfView + speed;
            MyCamera.fieldOfView = Mathf.Lerp(MyCamera.fieldOfView, fov, Time.deltaTime);
            mouseLeft = Input.mousePosition;
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

    void OnGUI()
    {
        // output
        GUI.DrawTexture(new Rect(0, 0, tex.width, tex.height), tex);

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
        var obj = inputObj[0];
        str = $"==== unity_local2World matrix ====\n{obj.transform.localToWorldMatrix}";
        MyUtility.LogPoint(new Rect(0, MyCamera.pixelHeight + posY, 200, 150), str, textColor, 20);
        posY -= offset;
        var tt = MyUtility.GetModelMatrix(obj.transform);
        str = $"==== my_local2World matrix ====\n{tt}";
        MyUtility.LogPoint(new Rect(0, MyCamera.pixelHeight + posY, 200, 150), str, textColor, 20);
    }
}
