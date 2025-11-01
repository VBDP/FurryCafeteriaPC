// Made with Amplify Shader Editor v1.9.5.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VRCCoffee/Americano"
{
	Properties
	{
		_Amount("Amount", Range( 0 , 1)) = 1
		_AmountCap("AmountCap", Vector) = (-6.4389,6.59976,0,0)
		_BaseColor("Base Color", Color) = (1,1,1,0)
		_ContactColor("Contact Color", Color) = (1,1,1,0)
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Smooth("Smooth", Range( 0 , 1)) = 1
		_Distance("Distance", Float) = 1
		_EdgeSharp("Edge Sharp", Float) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" }
		Cull Off
		Blend One Zero , SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 2.0
		#pragma surface surf Standard keepalpha noshadow 
		struct Input
		{
			float4 screenPos;
			half3 worldNormal;
			float3 worldPos;
		};

		uniform half4 _BaseColor;
		uniform half4 _ContactColor;
		uniform half _EdgeSharp;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform half _Distance;
		uniform half _Smooth;
		uniform half _Amount;
		uniform half2 _AmountCap;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			half4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth17 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			half distanceDepth17 = abs( ( screenDepth17 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( ( abs( _Distance ) / 5000.0 ) ) );
			half smoothstepResult43 = smoothstep( 0.0 , ( _EdgeSharp / 10.0 ) , ( 1.0 - distanceDepth17 ));
			half4 lerpResult38 = lerp( _BaseColor , _ContactColor , smoothstepResult43);
			o.Albedo = lerpResult38.rgb;
			o.Smoothness = _Smooth;
			o.Alpha = 1;
			half3 ase_worldNormal = i.worldNormal;
			half3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			ase_vertexNormal = normalize( ase_vertexNormal );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			half2 break52 = ( _AmountCap / float2( 100,100 ) );
			half lerpResult49 = lerp( break52.x , break52.y , _Amount);
			clip( ( _Amount == 0.0 ? 0.0 : ( ase_vertexNormal.y >= 0.999 ? 1.0 : step( ase_vertex3Pos.y , lerpResult49 ) ) ) - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19501
Node;AmplifyShaderEditor.CommentaryNode;60;-1775.878,469.2949;Inherit;False;1190;461.0004;Amount;11;48;51;52;47;49;45;57;46;59;58;55;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1489.69,172.8239;Inherit;False;Property;_Distance;Distance;6;0;Create;True;0;0;0;False;0;False;1;75;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;48;-1725.878,716.2952;Inherit;False;Property;_AmountCap;AmountCap;1;0;Create;True;0;0;0;False;0;False;-6.4389,6.59976;-6.4389,6.59976;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.AbsOpNode;34;-1338.39,172.8639;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;51;-1519.878,721.2952;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;100,100;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;30;-1223.39,176.8639;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;5000;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-1554.878,814.2952;Inherit;False;Property;_Amount;Amount;0;0;Create;True;0;0;0;False;0;False;1;0.8;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;52;-1406.878,721.2952;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DepthFade;17;-1103.693,176.8239;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-1023.355,283.6104;Inherit;False;Property;_EdgeSharp;Edge Sharp;7;0;Create;True;0;0;0;False;0;False;1;15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;49;-1294.878,722.2952;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;45;-1318.411,582.5907;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;44;-844.5198,256.7563;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;18;-867.234,176.8239;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;46;-1136.878,661.2952;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;57;-1136.878,519.2949;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;43;-714.5199,180.7563;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;37;-758.0769,-0.948122;Inherit;False;Property;_ContactColor;Contact Color;3;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.539,0.1911347,0,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;36;-763.7207,-179.2816;Inherit;False;Property;_BaseColor;Base Color;2;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.2879996,0.05279975,0,0.5607843;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.WireNode;59;-830.8782,707.2952;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Compare;58;-940.8782,562.2952;Inherit;False;3;4;0;FLOAT;0;False;1;FLOAT;0.999;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Compare;55;-765.8782,565.2952;Inherit;False;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-382.0767,157.5693;Inherit;False;Property;_Smooth;Smooth;5;0;Create;True;0;0;0;False;0;False;1;0.6;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;38;-506.3781,-59.64012;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-5,8;Half;False;True;-1;0;ASEMaterialInspector;0;0;Standard;VRCCoffee/Americano;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;False;0;True;Opaque;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;0;5;False;;10;False;;2;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;4;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;34;0;16;0
WireConnection;51;0;48;0
WireConnection;30;0;34;0
WireConnection;52;0;51;0
WireConnection;17;0;30;0
WireConnection;49;0;52;0
WireConnection;49;1;52;1
WireConnection;49;2;47;0
WireConnection;44;0;42;0
WireConnection;18;0;17;0
WireConnection;46;0;45;2
WireConnection;46;1;49;0
WireConnection;43;0;18;0
WireConnection;43;2;44;0
WireConnection;59;0;47;0
WireConnection;58;0;57;2
WireConnection;58;3;46;0
WireConnection;55;0;59;0
WireConnection;55;3;58;0
WireConnection;38;0;36;0
WireConnection;38;1;37;0
WireConnection;38;2;43;0
WireConnection;0;0;38;0
WireConnection;0;4;8;0
WireConnection;0;10;55;0
ASEEND*/
//CHKSM=5112FEFEC1B3295C0FC326491C147CB5574DAD62