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

    public GPUPipeline.VertexShader vert;
    public GPUPipeline.PixelShader frag;

    void Awake()
    {
        // vertex shader
        vert = (appdata, shaderParams) =>
        {
            var o = new v2f();
            var v = appdata as appdata;
            o.pos = shaderParams.UNITY_MATRIX_MVP * v.pos;
            o.uv = v.uv;
            o.color = new Vector4(1, 0, 0, 1); // tmp
            return o;
        };

        // pixel shader
        frag = (v2f, shaderParams) =>
        {
            var o = v2f as v2f;
            return Color.red; // tmp
            var uv = o.uv;
            if (_MainTex)
                return _MainTex.GetPixel((int)(uv.x * _MainTex.width), (int)(uv.y * _MainTex.height));
            return o.color;
        };
    }

    public override GPUPipeline.VertexShader VertexShader
    {
        get => vert;
    }

    public override GPUPipeline.PixelShader PixelShader
    {
        get => frag;
    }

    public override Type GetCastType => typeof(appdata);
}
