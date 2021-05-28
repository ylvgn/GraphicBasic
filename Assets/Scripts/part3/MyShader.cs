using System;
using UnityEngine;

public class MyShader : MyShaderBase
{
    // Properties
    public Texture2D _MainTex;

    // SubShader
    public class appdata : ShaderSemantic
    {
        public Vector4 pos
        {
            get => POSITION;
            set => POSITION = value;
        }
        public Vector2 uv
        {
            get => TEXCOORD0;
            set => TEXCOORD0 = value;
        }
    }

    public class v2f : ShaderSemantic
    {
        public Vector4 pos
        {
            get => SV_POSITION;
            set => SV_POSITION = value;
        }
        public Vector4 color
        {
            get => COLOR;
            set => COLOR = value;
        }
        public Vector4 uv
        {
            get => TEXCOORD0;
            set => TEXCOORD0 = value;
        }
    }

    public override GPUPipeline.VertexShader VertexShader => (appdata, shaderParams) =>
    {
        var o = new v2f();
        var v = appdata as appdata;
        o.pos = shaderParams.UNITY_MATRIX_MVP * v.pos;
        o.uv = v.uv;
        o.color = v.COLOR;
        return o;
    };

    public override GPUPipeline.PixelShader PixelShader => (v2f, shaderParams) =>
    {
        var o = v2f as v2f;
        var uv = o.uv;
        if (_MainTex)
            return _MainTex.GetPixel((int)(uv.x * _MainTex.width), (int)(uv.y * _MainTex.height));
        return o.color;
    };

    public override Type CastType => typeof(appdata);
}
