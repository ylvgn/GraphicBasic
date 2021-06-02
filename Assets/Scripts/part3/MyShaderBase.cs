using UnityEngine;

// https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
public class ShaderParameters
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

    public ShaderParameters() { }
    public ShaderParameters(Transform objTransform, Camera camera)
    {
        var projectionMatrix = MyUtility.GetProjectMatrix(camera.fieldOfView, camera.aspect, camera.nearClipPlane, camera.farClipPlane);
        var viewMatrix       = MyUtility.GetViewMatrix(camera);
        var modelMatrix      = MyUtility.GetModelMatrix(objTransform);
        var time             = Time.time;
        var sinTime          = Mathf.Sin(time);
        var cosTime          = Mathf.Cos(time);
        var deltaTime        = Time.deltaTime;
        var smoothDeltaTime  = Time.smoothDeltaTime;

        // Built-in shader variables
        unity_ObjectToWorld  = modelMatrix;                           // M
        UNITY_MATRIX_V       = viewMatrix;                            // V
        UNITY_MATRIX_P       = projectionMatrix;                      // P
        unity_WorldToObject  = objTransform.worldToLocalMatrix;
        UNITY_MATRIX_MV      = UNITY_MATRIX_V * unity_ObjectToWorld;
        UNITY_MATRIX_MVP     = UNITY_MATRIX_P * UNITY_MATRIX_MV;
        UNITY_MATRIX_VP      = UNITY_MATRIX_P * UNITY_MATRIX_V;
        UNITY_MATRIX_T_MV    = UNITY_MATRIX_MV.transpose;
        UNITY_MATRIX_IT_MV   = UNITY_MATRIX_T_MV.inverse;

        // Camera and screen (TODO)

        // Time
        _Time                = new Vector4(time / 20.0f, time, time * 2.0f, time * 3.0f);                         // Time since level load (t/20, t, t*2, t*3), use to animate things inside the shaders.
        _SinTime             = new Vector4(sinTime / 8.0f, sinTime / 4.0f, sinTime / 2.0f, sinTime);              // Sine of time: (t/8, t/4, t/2, t).
        _CosTime             = new Vector4(cosTime / 8.0f, cosTime / 4.0f, cosTime / 2.0f, cosTime);              // Cosine of time: (t/8, t/4, t/2, t).
        unity_DeltaTime      = new Vector4(deltaTime, 1.0f / deltaTime, smoothDeltaTime, 1.0f / smoothDeltaTime); // Delta time: (dt, 1/dt, smoothDt, 1/smoothDt).
    }
}

public class ShaderSemantic
{
    public Vector4 POSITION;
    public Vector4 TEXCOORD0;
    public Vector4 TEXCOORD1;
    public Vector4 TEXCOORD2;
    public Vector4 TEXCOORD3;
    public Vector4 TEXCOORD4;
    public Vector4 TEXCOORD5;
    public Vector4 TEXCOORD6;
    public Vector4 TEXCOORD7;
    public Vector4 TEXCOORD8;
    public Vector4 NORMAL;
    public Vector4 TANGENT;
    public Vector4 COLOR;
    public Vector4 SV_POSITION; // pixel shader

    public void Lerp(ShaderSemantic start, ShaderSemantic end, float w)
    {
        POSITION = Vector4.Lerp(start.POSITION, end.POSITION, w);
        SV_POSITION = Vector4.Lerp(start.SV_POSITION, end.SV_POSITION, w);
        COLOR = Color.Lerp(start.COLOR, end.COLOR, w);
        NORMAL = Vector3.Lerp(start.NORMAL, end.NORMAL, w);
        TANGENT = Vector4.Lerp(start.TANGENT, end.TANGENT, w);
        TEXCOORD0 = Vector4.Lerp(start.TEXCOORD0, end.TEXCOORD0, w);
        TEXCOORD1 = Vector4.Lerp(start.TEXCOORD1, end.TEXCOORD1, w);
        TEXCOORD2 = Vector4.Lerp(start.TEXCOORD2, end.TEXCOORD2, w);
        TEXCOORD3 = Vector4.Lerp(start.TEXCOORD3, end.TEXCOORD3, w);
        TEXCOORD4 = Vector4.Lerp(start.TEXCOORD4, end.TEXCOORD4, w);
        TEXCOORD5 = Vector4.Lerp(start.TEXCOORD5, end.TEXCOORD5, w);
        TEXCOORD6 = Vector4.Lerp(start.TEXCOORD6, end.TEXCOORD6, w);
        TEXCOORD7 = Vector4.Lerp(start.TEXCOORD7, end.TEXCOORD7, w);
        TEXCOORD8 = Vector4.Lerp(start.TEXCOORD8, end.TEXCOORD8, w);
    }

    public static ShaderSemantic CreateInstance(System.Type type)
    {
        return (ShaderSemantic)System.Activator.CreateInstance(type);
    }
};

// https://docs.unity3d.com/2020.1/Documentation/Manual/SL-CullAndDepth.html
public enum ZWrite
{
    On,
    Off,
}

public enum ZTest
{
    Less,     // <
    LEqual,   // <= This is the default value.
    Equal,    // ==
    GEqual,   // >=
    Greater,  // >
    NotEqual, // !=
    Always,   // NONE
}

public enum Cull
{
    Back, // back-face culling.
    Front,
    Off,
}

public abstract class MyShaderBase : MonoBehaviour
{
    public class Command
    {
        public ZWrite ZWrite;
        public ZTest ZTest;
        public Cull Cull;

        public Command() // default
        {
            ZWrite = ZWrite.Off;
            ZTest = ZTest.LEqual;
            Cull = Cull.Back;    // Don’t render polygons that are facing away from the viewer
        }
    }

    public abstract GPUPipeline.VertexShader VertexShader { get; }
    public abstract GPUPipeline.PixelShader PixelShader { get; }
    public abstract System.Type CastType { get; }
    public Command Commands { get; protected set; }
    public virtual void Awake()
    {
        Commands = new Command();
    }
}
