using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyShader : MonoBehaviour
{
    public GPUPipeline.VertexShader vert = (appdata, shaderParams) =>
    {
        var o = new MyPixelData();
        o.vertex = shaderParams.UNITY_MATRIX_MVP * appdata.vertex;
        o.Color = Color.white;
        return o;
    };

    public GPUPipeline.PixelShader frag = (v2f, shaderParams) =>
    {
        return Color.red;
    };
}
