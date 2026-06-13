Shader "TpLab/DrinkableAlcoholic/Ice"
{
    Properties
    {
        _Color("Color (Tint & Alpha)", Color) = (1, 1, 1, 0.5)
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Roughness("Roughness", Range(0,1)) = 0.3
        _MatCapTex("MatCap Texture", 2D) = "white" {}
        _MatCapIntensity("MatCap Intensity", Range(0, 1)) = 1.0
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200

        Blend One One
        ZWrite Off
        Cull Back
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MatCapTex;
            float _MatCapIntensity;

            fixed4 _Color;
            float _Metallic;
            float _Roughness;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 normal = normalize(i.worldNormal);
                float3 viewDir = normalize(i.viewDir);
                float3 r = reflect(-viewDir, normal);
                float2 matcapUV = r.xy * 0.5 + 0.5;
                fixed3 matcapCol = tex2D(_MatCapTex, matcapUV).rgb;

                fixed3 finalRGB = matcapCol * _Color.rgb * _MatCapIntensity;

                return fixed4(finalRGB, _Color.a);
            }
            ENDCG
        }
    }
}
