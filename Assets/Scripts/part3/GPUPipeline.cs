using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
public class ShaderParams
{
    // Built-in shader variables
    public Matrix4x4 UNITY_MATRIX_MVP;
    public Matrix4x4 UNITY_MATRIX_MV;
    public Matrix4x4 UNITY_MATRIX_V;
    public Matrix4x4 UNITY_MATRIX_P;
    public Matrix4x4 UNITY_MATRIX_VP;
    public Matrix4x4 UNITY_MATRIX_T_MV;
    public Matrix4x4 UNITY_MATRIX_IT_MV;
    public Matrix4x4 unity_ObjectToWorld;
    public Matrix4x4 unity_WorldToObject;

    // Camera and screen
    public Vector3 _WorldSpaceCameraPos;
    public Vector4 _ProjectionParams;
    public Vector4 _ScreenParams;
    public Vector4 _ZBufferParams;
    public Vector4 unity_OrthoParams;
    public Matrix4x4 unity_CameraProjection;
    public Matrix4x4 unity_CameraInvProjection;
    public Vector4 unity_CameraWorldClipPlanes; // in this order: left, right, bottom, top, near, far

    // Time
    public Vector4 _Time;
    public Vector4 _SinTime;
    public Vector4 _CosTime;
    public Vector4 unity_DeltaTime;
}

public struct MyVertexData
{
    public Vector4 vertex;
    public Vector2 uv;
    public Vector2 uv2;
    public Vector3 normal;
    public Vector4 Color;
};

public struct MyPixelData
{
    public Vector4 vertex;
    public Vector2 uv;
    public Vector2 uv2;
    public Vector4 Color;
};

public class GPUPipeline : MonoBehaviour
{
    public Camera MyCamera;
    Texture2D tex;
    MyShader[] inputObj; // tmp

    public delegate MyPixelData VertexShader(MyVertexData appdata, ShaderParams shaderParams);
    public delegate Color PixelShader(MyPixelData v2f, ShaderParams shaderParams);

    private void Start()
    {
        inputObj = GameObject.FindObjectsOfType<MyShader>();
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

        var c = new Color(0, 0, 0, 1);
        for (int i = 0; i < tex.height; i++)
            for (int j = 0; j < tex.width; j++)
                tex.SetPixel(j, i, c);

        Test();
        tex.Apply(false);
    }

    ShaderParams PackBuiltInShaderVariables(Transform objTransform, Camera camera)
    {
        var projectionMatrix = GetMyProjectMatrix(MyCamera.fieldOfView, MyCamera.aspect, MyCamera.nearClipPlane, MyCamera.farClipPlane);
        ShaderParams param = new ShaderParams();
        param.unity_ObjectToWorld  = objTransform.localToWorldMatrix;       // M
        param.UNITY_MATRIX_V       = camera.worldToCameraMatrix;            // V
        param.UNITY_MATRIX_P       = projectionMatrix;                      // P
        param.unity_WorldToObject  = objTransform.worldToLocalMatrix;
        param.UNITY_MATRIX_MV      = param.UNITY_MATRIX_V * param.unity_ObjectToWorld;
        param.UNITY_MATRIX_MVP     = param.UNITY_MATRIX_P * param.UNITY_MATRIX_MV;
        param.UNITY_MATRIX_VP      = param.UNITY_MATRIX_P * param.UNITY_MATRIX_V;
        param.UNITY_MATRIX_T_MV    = param.UNITY_MATRIX_MV.transpose;
        param.UNITY_MATRIX_IT_MV   = param.UNITY_MATRIX_T_MV.inverse;
        return param;
    }

    void Test()
    {
        for (int i = 0; i < inputObj.Length; i ++)
        {
            var obj = inputObj[i];
            var meshFilter = obj.GetComponent<MeshFilter>();
            var mesh = meshFilter.mesh;
            var vertices = mesh.vertices;
            var shaderParams = PackBuiltInShaderVariables(obj.transform, MyCamera);

            for (int j = 0; j < vertices.Length; j++)
            {
                var appdata = new MyVertexData();
                appdata.vertex = new Vector4(vertices[j].x, vertices[j].y, vertices[j].z, 1);
                //appdata.uv = Vector2.zero;

                // vertex shader
                MyPixelData o = obj.vert(appdata, shaderParams);

                // pixel shader
                Color c = obj.frag(o, shaderParams); // tmp
                var pos = o.vertex;

                // ndc
                o.vertex = new Vector4(pos.x / pos.w, pos.y / pos.w, pos.z / pos.w, 1);

                // screen space
                float screenWidth = MyCamera.pixelWidth;
                float screenHeight = MyCamera.pixelHeight;
                var far = MyCamera.farClipPlane;
                var near = MyCamera.nearClipPlane;
                float w = screenWidth / 2;
                float h = screenHeight / 2;
                Vector3 screenPos = new Vector3(o.vertex.x * w + w, o.vertex.y * h + h, (far - near) * o.vertex.z / 2 + (far + near) / 2);
                tex.SetPixel((int)screenPos.x, (int)screenPos.y, c);
            }
        }
    }

    // http://www.songho.ca/opengl/gl_projectionmatrix.html
    Matrix4x4 GetMyProjectMatrix(float left, float right, float bottom, float top, float near, float far)
    {
        Matrix4x4 m = new Matrix4x4();
        m[0] = (2 * near) / (right - left);
        m[5] = (2 * near) / (top - bottom);
        m[8] = (right + left) / (right - left);
        m[9] = (top + bottom) / (top - bottom);
        m[10] = -(far + near) / (far - near);
        m[11] = -1;
        m[14] = -(2 * far * near) / (far - near);
        return m;
    }

    Matrix4x4 GetMyProjectMatrix(float fov, float aspect, float near, float far)
    {
        var tan = Mathf.Tan(fov / 2 * Mathf.Deg2Rad);
        var h = tan * near;
        var w = h * aspect;
        return GetMyProjectMatrix(-w, w, -h, h, near, far);
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, tex.width, tex.height), tex);
        // debug
        MyUtility.LogPoint(new Rect(0, 0, 100, 20), $"Obj Count:{inputObj.Length}", Color.white, 20);
        MyUtility.LogPoint(new Rect(0, 20, 100, 20), $"resolution:{tex.width}x{tex.height}", Color.white, 20);

        var str = $"==== unity_project matrix ====\n{MyCamera.projectionMatrix}";
        MyUtility.LogPoint(new Rect(0, MyCamera.pixelHeight - 300, 200, 150), str, Color.white, 20);
        var myProjectMatrix = GetMyProjectMatrix(MyCamera.fieldOfView, MyCamera.aspect, MyCamera.nearClipPlane, MyCamera.farClipPlane);
        str = $"==== my_project matrix ====\n{myProjectMatrix}";
        MyUtility.LogPoint(new Rect(0, MyCamera.pixelHeight - 150, 200, 150), str, Color.white, 20);
    }
}
