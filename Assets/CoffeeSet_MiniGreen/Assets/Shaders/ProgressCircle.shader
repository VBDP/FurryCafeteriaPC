// Made with Amplify Shader Editor v1.9.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VRCCoffee/ProgressCircle"
{
	Properties
	{
		[HDR]_Color("Color", Color) = (1,1,1,1)
		_Progress("Progress", Range( 0 , 1)) = 1
		[Toggle]_Counterclockwise("Counterclockwise", Float) = 0
		_Radius("Radius", Range( 0 , 1)) = 1
		_Thickness("Thickness", Range( 0 , 1)) = 0.25
		_Rotation("Rotation", Range( 0 , 360)) = 0
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.5
		#define ASE_VERSION 19800
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform half4 _Color;
		uniform half _Radius;
		uniform half _Thickness;
		uniform half _Rotation;
		uniform half _Progress;
		uniform half _Counterclockwise;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Emission = _Color.rgb;
			half temp_output_42_0 = 0.0;
			o.Metallic = temp_output_42_0;
			o.Smoothness = temp_output_42_0;
			o.Alpha = _Color.a;
			float2 uv_TexCoord14 = i.uv_texcoord + float2( -0.5,-0.5 );
			half temp_output_15_0 = distance( uv_TexCoord14 , float2( 0,0 ) );
			half temp_output_45_0 = (0.5 + (_Radius - 0.0) * (0.0 - 0.5) / (1.0 - 0.0));
			float2 uv_TexCoord23 = i.uv_texcoord + float2( -0.5,-0.5 );
			float cos22 = cos( ( ( (0.0 + (_Rotation - 0.0) * (1.0 - 0.0) / (360.0 - 0.0)) * 6.28318548202515 ) + (0.0 + (( 180.0 * 6.28318548202515 ) - 0.0) * (1.0 - 0.0) / (360.0 - 0.0)) ) );
			float sin22 = sin( ( ( (0.0 + (_Rotation - 0.0) * (1.0 - 0.0) / (360.0 - 0.0)) * 6.28318548202515 ) + (0.0 + (( 180.0 * 6.28318548202515 ) - 0.0) * (1.0 - 0.0) / (360.0 - 0.0)) ) );
			half2 rotator22 = mul( uv_TexCoord23 - float2( 0,0 ) , float2x2( cos22 , -sin22 , sin22 , cos22 )) + float2( 0,0 );
			half2 temp_output_34_0_g1 = ( rotator22 - float2( 0,0 ) );
			half2 break39_g1 = temp_output_34_0_g1;
			half2 appendResult50_g1 = (half2(( 1.0 * ( length( temp_output_34_0_g1 ) * 2.0 ) ) , ( ( atan2( break39_g1.x , break39_g1.y ) * ( 1.0 / 6.28318548202515 ) ) * 1.0 )));
			half2 break53_g1 = appendResult50_g1;
			half temp_output_1_56 = break53_g1.y;
			half temp_output_7_0 = (-0.5 + (_Progress - 0.0) * (0.5 - -0.5) / (1.0 - 0.0));
			half lerpResult39 = lerp( step( temp_output_1_56 , temp_output_7_0 ) , step( (0.5 + (temp_output_1_56 - -0.5) * (-0.5 - 0.5) / (0.5 - -0.5)) , temp_output_7_0 ) , _Counterclockwise);
			clip( ( ( step( ( temp_output_15_0 + temp_output_45_0 ) , 0.5 ) - step( temp_output_15_0 , ( (0.5 + (_Thickness - 0.0) * (0.0 - 0.5) / (1.0 - 0.0)) - temp_output_45_0 ) ) ) * lerpResult39 ) - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.5
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19800
Node;AmplifyShaderEditor.RangedFloatNode;32;-4336,608;Inherit;False;Property;_Rotation;Rotation;5;0;Create;True;0;0;0;False;0;False;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.TauNode;31;-4048,832;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-3760,976;Inherit;False;2;2;0;FLOAT;180;False;1;FLOAT;180;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;34;-4048,608;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;360;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-3760,752;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;53;-3536,976;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;360;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-3472,432;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;-0.5,-0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;51;-3520,752;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;22;-3216,432;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0.07;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;14;-3904,-656;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;-0.5,-0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;43;-3904,-400;Inherit;False;Property;_Radius;Radius;3;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-3904,-192;Inherit;False;Property;_Thickness;Thickness;4;0;Create;True;0;0;0;False;0;False;0.25;0.2282609;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-3216,688;Inherit;False;Property;_Progress;Progress;1;0;Create;True;0;0;0;False;0;False;1;0.4021739;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1;-2912,432;Inherit;False;Polar Coordinates;-1;;1;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT;1;False;4;FLOAT;1;False;3;FLOAT2;0;FLOAT;55;FLOAT;56
Node;AmplifyShaderEditor.DistanceOpNode;15;-3616,-656;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;19;-3616,-192;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.5;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;45;-3616,-400;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.5;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;7;-2912,688;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.5;False;4;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;37;-2592,816;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-0.5;False;2;FLOAT;0.5;False;3;FLOAT;0.5;False;4;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;48;-3280,-400;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;46;-3280,-656;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;38;-2288,672;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;5;-2288,464;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-2288,880;Inherit;False;Property;_Counterclockwise;Counterclockwise;2;1;[Toggle];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;17;-3056,-400;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;16;-3056,-656;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;39;-2032,640;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;20;-2800,-528;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-192,80;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;41;-605.069,-452.8967;Inherit;False;Property;_Color;Color;0;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-1552,-32;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Half;False;True;-1;3;ASEMaterialInspector;0;0;Standard;VRCCoffee/ProgressCircle;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;False;TransparentCutout;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;6;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;52;1;31;0
WireConnection;34;0;32;0
WireConnection;33;0;34;0
WireConnection;33;1;31;0
WireConnection;53;0;52;0
WireConnection;51;0;33;0
WireConnection;51;1;53;0
WireConnection;22;0;23;0
WireConnection;22;2;51;0
WireConnection;1;1;22;0
WireConnection;15;0;14;0
WireConnection;19;0;18;0
WireConnection;45;0;43;0
WireConnection;7;0;6;0
WireConnection;37;0;1;56
WireConnection;48;0;19;0
WireConnection;48;1;45;0
WireConnection;46;0;15;0
WireConnection;46;1;45;0
WireConnection;38;0;37;0
WireConnection;38;1;7;0
WireConnection;5;0;1;56
WireConnection;5;1;7;0
WireConnection;17;0;15;0
WireConnection;17;1;48;0
WireConnection;16;0;46;0
WireConnection;39;0;5;0
WireConnection;39;1;38;0
WireConnection;39;2;40;0
WireConnection;20;0;16;0
WireConnection;20;1;17;0
WireConnection;21;0;20;0
WireConnection;21;1;39;0
WireConnection;0;2;41;5
WireConnection;0;3;42;0
WireConnection;0;4;42;0
WireConnection;0;9;41;4
WireConnection;0;10;21;0
ASEEND*/
//CHKSM=BDE3224F76A363CFE5024A111C6C545D7EA30052