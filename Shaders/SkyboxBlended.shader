﻿// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Skybox/SkyboxBlended"
{
Properties
{
    _Tint ("Tint Color", Color) = (.5, .5, .5, .5)
    [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
    _Rotation ("Rotation", Range(0, 360)) = 0
	_Blend ("Skybox blend", Range(0.0, 1.0)) = 0
    [NoScaleOffset] _FrontTex ("Front 1 [+Z]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _BackTex ("Back 1 [-Z]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _LeftTex ("Left 1 [+X]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _RightTex ("Right 1 [-X]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _UpTex ("Up 1 [+Y]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _DownTex ("Down 1 [-Y]   (HDR)", 2D) = "grey" {}
	[NoScaleOffset] _FrontTex2 ("Front 2 [+Z]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _BackTex2 ("Back 2 [-Z]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _LeftTex2 ("Left 2 [+X]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _RightTex2 ("Right 2 [-X]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _UpTex2 ("Up 2 [+Y]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _DownTex2 ("Down 2 [-Y]   (HDR)", 2D) = "grey" {}
}

SubShader
{
    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
    Cull Off ZWrite Off

    CGINCLUDE
    #include "UnityCG.cginc"

    half4 _Tint;
    half _Exposure;
    float _Rotation;
	float _Blend;

    float3 RotateAroundYInDegrees (float3 vertex, float degrees)
    {
        float alpha = degrees * UNITY_PI / 180.0;
        float sina, cosa;
        sincos(alpha, sina, cosa);
        float2x2 m = float2x2(cosa, -sina, sina, cosa);
        return float3(mul(m, vertex.xz), vertex.y).xzy;
    }

    struct appdata_t
	{
        float4 vertex : POSITION;
        float2 texcoord : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };
    struct v2f
	{
        float4 vertex : SV_POSITION;
        float2 texcoord : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

	v2f vert (appdata_t v)
    {
        v2f o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        float3 rotated = RotateAroundYInDegrees(v.vertex, _Rotation);
        o.vertex = UnityObjectToClipPos(rotated);
        o.texcoord = v.texcoord;
        return o;
    }

	half4 skybox_frag (v2f i, sampler2D smp, half4 smpDecode)
    {
        half4 tex = tex2D (smp, i.texcoord);
        half3 c = DecodeHDR (tex, smpDecode);
        c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb;
        c *= _Exposure;
        return half4(c, 1);
    }
    ENDCG

    Pass
	{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _FrontTex;
		sampler2D _FrontTex2;
        half4 _FrontTex_HDR;
        half4 _FrontTex2_HDR;
		half4 frag (v2f i) : SV_Target
		{
			half4 tex1Frag = skybox_frag(i, _FrontTex, _FrontTex_HDR);
			half4 tex2Frag = skybox_frag(i, _FrontTex2, _FrontTex2_HDR);

			return lerp(tex1Frag, tex2Frag, _Blend);
		}
        ENDCG
    }
    Pass
	{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _BackTex;
		sampler2D _BackTex2;
        half4 _BackTex_HDR;
		half4 _BackTex2_HDR;
        half4 frag (v2f i) : SV_Target
		{
			half4 tex1Frag = skybox_frag(i, _BackTex, _BackTex_HDR);
			half4 tex2Frag = skybox_frag(i, _BackTex2, _BackTex2_HDR);

			return lerp(tex1Frag, tex2Frag, _Blend);
		}
        ENDCG
    }
    Pass
	{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _LeftTex;
        sampler2D _LeftTex2;
		half4 _LeftTex_HDR;
        half4 _LeftTex2_HDR;
        half4 frag (v2f i) : SV_Target
		{
			half4 tex1Frag = skybox_frag(i, _LeftTex, _LeftTex_HDR);
			half4 tex2Frag = skybox_frag(i, _LeftTex2, _LeftTex2_HDR);

			return lerp(tex1Frag, tex2Frag, _Blend);
		}
        ENDCG
    }
    Pass
	{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _RightTex;
        sampler2D _RightTex2;
        half4 _RightTex_HDR;
		half4 _RightTex2_HDR;
        half4 frag (v2f i) : SV_Target
		{
			half4 tex1Frag = skybox_frag(i, _RightTex, _RightTex_HDR);
			half4 tex2Frag = skybox_frag(i, _RightTex2, _RightTex2_HDR);

			return lerp(tex1Frag, tex2Frag, _Blend);
		}
        ENDCG
    }
    Pass
	{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _UpTex;
        sampler2D _UpTex2;
        half4 _UpTex_HDR;
		half4 _UpTex2_HDR;
        half4 frag (v2f i) : SV_Target
		{
			half4 tex1Frag = skybox_frag(i, _UpTex, _UpTex_HDR);
			half4 tex2Frag = skybox_frag(i, _UpTex2, _UpTex2_HDR);

			return lerp(tex1Frag, tex2Frag, _Blend);
		}
        ENDCG
    }
    Pass
	{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _DownTex;
        sampler2D _DownTex2;
        half4 _DownTex_HDR;
		half4 _DownTex2_HDR;
        half4 frag (v2f i) : SV_Target
		{
			half4 tex1Frag = skybox_frag(i, _DownTex, _DownTex_HDR);
			half4 tex2Frag = skybox_frag(i, _DownTex2, _DownTex2_HDR);

			return lerp(tex1Frag, tex2Frag, _Blend);
		}
        ENDCG
    }
}

Fallback "Skybox/6 Sided", 1
}
