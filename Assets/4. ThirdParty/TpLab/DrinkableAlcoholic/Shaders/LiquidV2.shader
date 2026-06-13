/*Please do support www.bitshiftprogrammer.com by joining the facebook page : fb.com/BitshiftProgrammer
Legal Stuff:
This code is free to use no restrictions but attribution would be appreciated.
Any damage caused either partly or completly due to usage this stuff is not my responsibility*/
Shader "TpLab/DrinkableAlcoholic/LiquidV2"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _AmountRate("Amount Rate", Range(0,1)) = 0.0
        _MinAmountRate("MinAmount Rate", Range(0,1)) = 0.0
        _LiquidHeight("Liquid Height", Float) = 0.0
        [HideInInspector] _WobbleX("WobbleX", Range(-1,1)) = 0.0
        [HideInInspector] _WobbleZ("WobbleZ", Range(-1,1)) = 0.0
        [Space(10)]
        _TopColor("Top Color", Color) = (1,1,1,1)
        [Enum(UnityEngine.Rendering.BlendMode)] _TopSrcBlend("Top Src Blend", Float) = 2 // DstColor
        [Enum(UnityEngine.Rendering.BlendMode)] _TopDstBlend("Top Dst Blend", Float) = 0 // Zero
        [Space(10)]
        _FoamColor("Foam Line Color", Color) = (1,1,1,1)
        _Rim("Foam Line Width", Range(0,0.1)) = 0.0
        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimPower("Rim Power", Range(0,10)) = 0.0
        [Space(10)]
        [HDR] _EmissionColor("Emission Color", Color) = (0,0,0)
        _ReflectionStrength("Reflection Strength", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        
        CGINCLUDE
        #include "UnityCG.cginc"

        float _AmountRate;
        float _MinAmountRate;
        float _LiquidHeight;
        float _WobbleX;
        float _WobbleZ;
        float4 _TopColor;
        float4 _RimColor;
        float4 _FoamColor;
        float4 _Color;
        float4 _EmissionColor;
        float _Rim;
        float _RimPower;
        float _ReflectionStrength;

        float4 RotateAroundYInDegrees(float4 vertex, float degrees)
        {
            float alpha = degrees * UNITY_PI / 180;
            float sina, cosa;
            sincos(alpha, sina, cosa);
            float2x2 m = float2x2(cosa, sina, -sina, cosa);
            return float4(vertex.yz , mul(m, vertex.xz)).xzyw;
        }

        float CalcLiquidFillEdge(float4 vertex)
        {
            float3 worldPos = mul(unity_ObjectToWorld, vertex.xyz);
            float3 worldPosX = RotateAroundYInDegrees(float4(worldPos,0),360);
            float3 worldPosZ = float3(worldPosX.y, worldPosX.z, worldPosX.x);
            float3 worldPosAdjusted = worldPos + (worldPosX * _WobbleX) + (worldPosZ * _WobbleZ);
            float fillBase = 0.5 + _LiquidHeight * 0.5;
            return worldPosAdjusted.y + fillBase - _LiquidHeight * _AmountRate + 0.0001;
        }
        ENDCG

        // backword pass
        Pass
        {
            Cull Front
            ZWrite Off
            ZTest LEqual
            Blend [_TopSrcBlend] [_TopDstBlend]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float fillEdge : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.fillEdge = CalcLiquidFillEdge(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex.xyz);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (_AmountRate <= _MinAmountRate) discard;

                float fillStep = step(i.fillEdge, 0.5);
                if (fillStep <= 0.0) discard;

                // refrection
                half3 worldViewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                half3 reflDir = reflect(-worldViewDir, i.worldNormal);
                half4 refColor = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflDir, 0);
                refColor.rgb = DecodeHDR(refColor, unity_SpecCube0_HDR);
                half3 finalColor = lerp(_TopColor.rgb + _EmissionColor, refColor.rgb, _ReflectionStrength);
                
                return fixed4(finalColor, _TopColor.a);
            }
            ENDCG
        }

        // foward pass
        Pass
        {
            Cull Back
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 viewDir : COLOR;
                float3 normal : COLOR2;
                float fillEdge : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.fillEdge = CalcLiquidFillEdge(v.vertex);
                o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
                o.normal = v.normal;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (_AmountRate <= _MinAmountRate) discard;

                // rim light
                float rimDot = dot(i.normal, i.viewDir);
                float rim = 1.0 - pow(rimDot, _RimPower);
                float4 rimStep = smoothstep(0.5, 1.0, rim);
                float3 rimResult = rimStep * _RimColor.rgb;

                // foam edge
                float fillStep = step(i.fillEdge, 0.5);
                if (fillStep <= 0.0) discard;

                float foamStep = fillStep - step(i.fillEdge, 0.5 - _Rim);
                float4 foamColor = foamStep * (_FoamColor * 0.75);

                // rest of the liquid
                float4 resultStep = fillStep - foamStep;
                float4 resultColored = resultStep * _Color;
                float4 emission = _EmissionColor * fixed4(1, 1, 1, 0);
                float4 finalResult = resultColored + foamColor + resultStep * emission;
                finalResult.rgb += rimResult;

                return finalResult;
            }
            ENDCG
        }
    }
}