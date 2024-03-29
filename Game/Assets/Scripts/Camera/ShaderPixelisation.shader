﻿Shader "Hidden/ShaderPixelisation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelSize("Pixel Size", Range(0.001, 0.15)) = 0.001
        _FadeScale("Fade Scale", float) = 1

    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        Tags {"RenderType"="Opaque"}
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"


            sampler2D _MainTex;
            float _PixelSize;
            float _FadeScale;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float ratioX = (int)(i.uv.x / _PixelSize + 0.5) * _PixelSize;
                float ratioY = (int)(i.uv.y / _PixelSize + 0.5) * _PixelSize;

                
                fixed4 col;
                col = tex2D(_MainTex, float2(ratioX, ratioY));
                col = lerp(col, fixed4(0, 0, 0, 0), _PixelSize * _FadeScale);

                return col;
            }
            ENDCG
        }
    }
}
