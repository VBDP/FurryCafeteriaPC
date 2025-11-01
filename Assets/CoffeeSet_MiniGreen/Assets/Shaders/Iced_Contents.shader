// Made with Amplify Shader Editor v1.9.7.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VRCCoffee/Iced Contents"
{
	Properties
	{
		_Amount("Amount", Range( 0 , 1)) = 1
		_AmountCap("AmountCap", Vector) = (-6.4389,6.59976,0,0)
		_BaseColor("Base Color", Color) = (1,1,1,1)
		_Metalic("Metalic", Range( 0 , 1)) = 0
		_Smooth("Smooth", Range( 0 , 1)) = 0
		_DepthColor("Depth Color", Color) = (1,1,1,1)
		_Depth("Depth", Float) = 0
		[Toggle]_ShowDepth("Show Depth", Float) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+1" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 2.0
		#define ASE_VERSION 19701
		#pragma surface surf Standard alpha:fade keepalpha noshadow 
		struct Input
		{
			float4 screenPos;
			half3 worldNormal;
			float3 worldPos;
		};

		uniform half4 _DepthColor;
		uniform half4 _BaseColor;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform half _Depth;
		uniform half _ShowDepth;
		uniform half _Metalic;
		uniform half _Smooth;
		uniform half _Amount;
		uniform half2 _AmountCap;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			half4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth64 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			half distanceDepth64 = saturate( abs( ( screenDepth64 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( ( _Depth / 100.0 ) ) ) );
			half3 lerpResult68 = lerp( _DepthColor.rgb , _BaseColor.rgb , distanceDepth64);
			half3 lerpResult72 = lerp( lerpResult68 , float3( 1,1,1 ) , _ShowDepth);
			o.Albedo = lerpResult72;
			half lerpResult70 = lerp( 0.0 , distanceDepth64 , _ShowDepth);
			half3 temp_cast_0 = (lerpResult70).xxx;
			o.Emission = temp_cast_0;
			o.Metallic = _Metalic;
			o.Smoothness = _Smooth;
			half3 ase_worldNormal = i.worldNormal;
			half3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			ase_vertexNormal = normalize( ase_vertexNormal );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			half2 break52 = ( _AmountCap / float2( 100,100 ) );
			half lerpResult49 = lerp( break52.x , break52.y , _Amount);
			half temp_output_55_0 = ( _Amount == 0.0 ? 0.0 : ( ase_vertexNormal.y >= 0.999 ? 1.0 : step( ase_vertex3Pos.y , lerpResult49 ) ) );
			half lerpResult69 = lerp( _DepthColor.a , _BaseColor.a , distanceDepth64);
			half lerpResult73 = lerp( saturate( ( temp_output_55_0 * lerpResult69 ) ) , temp_output_55_0 , _ShowDepth);
			o.Alpha = lerpResult73;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19701
Node;AmplifyShaderEditor.CommentaryNode;60;-1775.878,469.2949;Inherit;False;1190;461.0004;Amount;11;48;51;52;47;49;45;57;46;59;58;55;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;48;-1725.878,716.2952;Inherit;False;Property;_AmountCap;AmountCap;1;0;Create;True;0;0;0;False;0;False;-6.4389,6.59976;-6.4389,6.59976;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleDivideOpNode;51;-1519.878,721.2952;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;100,100;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-1554.878,814.2952;Inherit;False;Property;_Amount;Amount;0;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;52;-1406.878,721.2952;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.LerpOp;49;-1294.878,722.2952;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;45;-1318.411,582.5907;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;65;-1440,128;Inherit;False;Property;_Depth;Depth;6;0;Create;True;0;0;0;False;0;False;0;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;57;-1136.878,519.2949;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;46;-1136.878,661.2952;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;66;-1264,128;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.Compare;58;-940.8782,562.2952;Inherit;False;3;4;0;FLOAT;0;False;1;FLOAT;0.999;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;59;-830.8782,707.2952;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;36;-1088,-64;Inherit;False;Property;_BaseColor;Base Color;2;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.DepthFade;64;-1120,128;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;67;-1088,-256;Inherit;False;Property;_DepthColor;Depth Color;5;0;Create;True;0;0;0;False;0;False;1,1,1,1;0.9,0.9,0.9,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.Compare;55;-765.8782,565.2952;Inherit;False;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;69;-656,80;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-480,224;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;68;-651.9663,-120.9481;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-480,-64;Inherit;False;Property;_ShowDepth;Show Depth;7;1;[Toggle];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;63;-320,224;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-411.6213,41.87253;Inherit;False;Property;_Metalic;Metalic;3;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-413.0767,114.5693;Inherit;False;Property;_Smooth;Smooth;4;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;72;-288,-240;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;1,1,1;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;70;-288,-112;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;73;-176,288;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,16;Half;False;True;-1;0;ASEMaterialInspector;0;0;Standard;VRCCoffee/Iced Contents;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;1;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;2;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;51;0;48;0
WireConnection;52;0;51;0
WireConnection;49;0;52;0
WireConnection;49;1;52;1
WireConnection;49;2;47;0
WireConnection;46;0;45;2
WireConnection;46;1;49;0
WireConnection;66;0;65;0
WireConnection;58;0;57;2
WireConnection;58;3;46;0
WireConnection;59;0;47;0
WireConnection;64;0;66;0
WireConnection;55;0;59;0
WireConnection;55;3;58;0
WireConnection;69;0;67;4
WireConnection;69;1;36;4
WireConnection;69;2;64;0
WireConnection;62;0;55;0
WireConnection;62;1;69;0
WireConnection;68;0;67;5
WireConnection;68;1;36;5
WireConnection;68;2;64;0
WireConnection;63;0;62;0
WireConnection;72;0;68;0
WireConnection;72;2;71;0
WireConnection;70;1;64;0
WireConnection;70;2;71;0
WireConnection;73;0;63;0
WireConnection;73;1;55;0
WireConnection;73;2;71;0
WireConnection;0;0;72;0
WireConnection;0;2;70;0
WireConnection;0;3;61;0
WireConnection;0;4;8;0
WireConnection;0;9;73;0
ASEEND*/
//CHKSM=BB6613855CF329BDBEF13BB30D9EA3E5EA8CFA56