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
        unity_ObjectToWorld  = objTransform.localToWorldMatrix;       // M
        UNITY_MATRIX_V       = camera.worldToCameraMatrix;            // V
        UNITY_MATRIX_P       = projectionMatrix;                      // P
        unity_WorldToObject  = objTransform.worldToLocalMatrix;
        UNITY_MATRIX_MV      = UNITY_MATRIX_V * unity_ObjectToWorld;
        UNITY_MATRIX_MVP     = UNITY_MATRIX_P * UNITY_MATRIX_MV;
        UNITY_MATRIX_VP      = UNITY_MATRIX_P * UNITY_MATRIX_V;
        UNITY_MATRIX_T_MV    = UNITY_MATRIX_MV.transpose;
        UNITY_MATRIX_IT_MV   = UNITY_MATRIX_T_MV.inverse;
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
    //public Vector4 BINORMAL;  // unity shaderlab does not support 'BINORMAL' or 'BITANGENT' semantic
};

public abstract class MyShaderBase : MonoBehaviour
{
    public abstract GPUPipeline.VertexShader VertexShader { get; }
    public abstract GPUPipeline.PixelShader PixelShader { get; }
    public abstract System.Type GetCastType { get; }
}
