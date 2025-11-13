Shader "Custom/UnderwaterEffectPolished"
{
    Properties
    {
        _TintColor("Water Tint", Color) = (0, 0.5, 1, 0.1) // Soft, transparent bluish tint
        _WaveFrequency("Wave Frequency", Float) = 10.0    // Ripple frequency
        _WaveAmplitude("Wave Amplitude", Float) = 0.02    // Ripple amplitude
        _WaveSpeed("Wave Speed", Float) = 1.0             // Speed of animation
        _DistortionTex("Distortion Texture", 2D) = "white" {} // Texture for distortion
        _DistortionTiling("Distortion Tiling", Vector) = (2, 2, 0, 0) // Texture tiling control
        _DistortionStrength("Distortion Strength", Float) = 0.05 // Ripple strength
        _DistortionAlpha("Distortion Alpha", Range(0, 1)) = 0.5 // Alpha for subtle distortion
        _VignetteIntensity("Vignette Intensity", Range(0, 1)) = 0.3 // Darken edges
    }

        SubShader
        {
            Tags { "Queue" = "Overlay+2" "RenderType" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha // Enable transparency blending
            Cull Front // Render the inside of the sphere
            ZTest Always
            ZWrite Off
            GrabPass { "_GrabTexture" }

            Pass
            {
                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float3 worldPos : TEXCOORD0; // World space position for proper UV
                    float2 uv : TEXCOORD1;
                };

                sampler2D _GrabTexture;
                sampler2D _DistortionTex;
                float4 _TintColor;
                float _WaveFrequency;
                float _WaveAmplitude;
                float _WaveSpeed;
                float4 _DistortionTiling; // Texture tiling control
                float _DistortionStrength;
                float _DistortionAlpha; // Controls distortion visibility
                float _VignetteIntensity;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    o.uv = v.vertex.xy * 0.5 + 0.5; // Basic UV mapping
                    return o;
                }

                float2 ApplyTextureDistortion(float2 uv)
                {
                    // Adjust the UVs based on tiling and animate them over time
                    float2 tiledUV = uv * _DistortionTiling.xy;
                    float2 ripple = tex2D(_DistortionTex, tiledUV + _Time.y * _WaveSpeed).rg * 2.0 - 1.0;

                    // Apply ripple with distortion strength
                    return uv + ripple * _WaveAmplitude * _DistortionStrength;
                }

                float4 frag(v2f i) : SV_Target
                {
                    // Calculate view direction to sample UVs correctly inside the sphere
                    float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos);
                    float2 distortedUV = ApplyTextureDistortion(viewDir.xy * 0.5 + 0.5);

                    // Sample the screen grab texture with distorted UVs
                    float4 screenColor = tex2Dproj(_GrabTexture, float4(distortedUV, 0, 1));

                    // Blend distortion effect using the alpha value
                    screenColor.rgb = lerp(screenColor.rgb, _TintColor.rgb, _DistortionAlpha);

                    // Apply vignette effect to darken the edges
                    float2 uvCentered = i.uv * 2.0 - 1.0;
                    float vignette = 1.0 - smoothstep(0.4, 1.0, length(uvCentered));
                    screenColor.rgb *= lerp(1.0, 0.8, vignette * _VignetteIntensity);

                    // Maintain transparency for subtle overlay
                    screenColor.a = _TintColor.a;

                    return screenColor;
                }
                ENDHLSL
            }
        }
            FallBack "Transparent/Diffuse"
}
