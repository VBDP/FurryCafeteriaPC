// Generated from template
// https://github.com/lilxyzw/Shader-MEMO/blob/main/Assets/ScriptTemplates/84-Shader__VR%20Unlit%20Shader-NewUnlitShader.shader.txt
// from
// https://github.com/lilxyzw/Shader-MEMO
Shader "Unlit/NightMode"
{
    Properties
    {
        _NightModeLevel ("Night Mode Level", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" "ForceNoShadowCasting"="True" "IgnoreProjector"="True"}
        LOD 100
        Cull Off
        ZTest Always
        ZWrite Off
        Blend Zero SrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float2 uv : TEXCOORD0;
                // single pass instanced rendering
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;

                // single pass instanced rendering
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float _NightModeLevel;

            v2f vert (appdata v)
            {
                v2f o;

                // single pass instanced rendering
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = float4(v.uv.x * 2 - 1, 1 - v.uv.y * 2, 0, 1);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // single pass instanced rendering
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                float4 col = float4(0, 0, 0, _NightModeLevel);
                return col;
            }
            ENDCG
        }
    }
}