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
        var c = new Color(0, 0, 0, 1);
        for (int i = 0; i < tex.height; i++)
            for (int j = 0; j < tex.width; j++)
                tex.SetPixel(j, i, c);

        Run();
        tex.Apply(false);
    }

    void Run()
    {
        for (int i = 0; i < inputObj.Count; i ++)
        {
            var obj = inputObj[i];
            var meshFilter = obj.GetComponent<MeshFilter>();
            var mesh = meshFilter.mesh;
            var vertices = mesh.vertices;
            var triangles = mesh.triangles;

            if (shadingMode == ShadingMode.Wireframe)
            {
                int last_x = 0, last_y = 0;
                for (int j = 0; j < triangles.Length; j ++)
                {
                    var index = triangles[j];
                    var world_pos = obj.transform.localToWorldMatrix.MultiplyPoint(vertices[index]);
                    Vector3 screenPos = MyCamera.WorldToScreenPoint(world_pos);
                    int x = (int)screenPos.x;
                    int y = (int)screenPos.y;
                    if (j > 0 && j % 3 != 0)
                        DrawLine.Draw(tex, last_x, last_y, x, y, Color.white, DrawLine.LineType.Bresenham);
                    last_x = x;
                    last_y = y;
                }
                continue;
            }

            var shaderParams = new ShaderParameters(obj.transform, MyCamera);
            for (int j = 0; j < vertices.Length; j ++)
            {
                var appdata = (ShaderSemantic)System.Activator.CreateInstance(obj.GetCastType);
                appdata.POSITION = new Vector4(vertices[j].x, vertices[j].y, vertices[j].z, 1);
                appdata.NORMAL = mesh.normals[j];
                appdata.TANGENT = mesh.tangents[j];
                appdata.TEXCOORD0 = mesh.uv[j];

                // vertex shader: local space -> world space -> view space -> clip space
                ShaderSemantic v2f = obj.VertexShader(appdata, shaderParams);

                // clip TODO

                // interpolation: lerp TODO

                // pixel shader TODO
                Color c = obj.PixelShader(v2f, shaderParams);
                var t = v2f.SV_POSITION;

                // homogeneous divide: clip space -> Canonical View Volume(CVV) <==> NDC
                v2f.SV_POSITION = new Vector4(t.x / t.w, t.y / t.w, t.z / t.w, 1);

                // NDC -> screen space (window space)
                float screenWidth = MyCamera.pixelWidth;
                float screenHeight = MyCamera.pixelHeight;
                var far = MyCamera.farClipPlane;
                var near = MyCamera.nearClipPlane;
                float w = screenWidth / 2;
                float h = screenHeight / 2;
                t = v2f.SV_POSITION;
                Vector3 screenPos = new Vector3(t.x * w + w, t.y * h + h, (far - near) * t.z / 2 + (far + near) / 2);

                // apply
                tex.SetPixel((int)screenPos.x, (int)screenPos.y, c);
            }
        }
    }

    void OnGUI()
    {
        // output
        GUI.DrawTexture(new Rect(0, 0, tex.width, tex.height), tex);

        // debug
        MyUtility.LogPoint(new Rect(0, 0, 100, 20), $"Obj Count:{inputObj.Count}", Color.white, 20);
        MyUtility.LogPoint(new Rect(0, 20, 100, 20), $"resolution:{tex.width}x{tex.height}", Color.white, 20);

        var str = $"==== unity_project matrix ====\n{MyCamera.projectionMatrix}";
        MyUtility.LogPoint(new Rect(0, MyCamera.pixelHeight - 300, 200, 150), str, Color.white, 20);
        var myProjectMatrix = MyUtility.GetProjectMatrix(MyCamera.fieldOfView, MyCamera.aspect, MyCamera.nearClipPlane, MyCamera.farClipPlane);
        str = $"==== my_project matrix ====\n{myProjectMatrix}";
        MyUtility.LogPoint(new Rect(0, MyCamera.pixelHeight - 150, 200, 150), str, Color.white, 20);
    }
}
