// Made with Amplify Shader Editor v1.9.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VRCCoffee/Ade"
{
	Properties
	{
		_Amount("Amount", Range( 0 , 1)) = 1
		_AmountCap("AmountCap", Vector) = (-6.4389,6.59976,0,0)
		_TopColor("Top Color", Color) = (1,1,1,1)
		_BottomColor("Bottom Color", Color) = (1,1,1,1)
		_Smoothness("Smoothness", Range( 0 , 1)) = 1
		_Depth("Depth", Float) = 0
		_DepthSmooth("Depth Smooth", Float) = 0
		_FresnelScale("Fresnel Scale", Float) = 0
		_FresnelPower("Fresnel Power", Float) = 0
		[Header(Blend)][Toggle]_ViewBlend("View Blend", Float) = 0
		_Pos("Pos", Float) = 0
		_Smooth("Smooth", Float) = 0
		_NoiseStrength("Noise Strength", Range( 0 , 1)) = 0
		_NoiseScale("Noise Scale", Float) = 1
		_DistortionAmount("Distortion Amount", Float) = 0
		_DistortionScale("Distortion Scale", Float) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+1" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#define ASE_VERSION 19800
		#pragma surface surf Standard alpha:fade keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float4 ase_positionOS4f;
			float4 screenPos;
			float3 worldPos;
			half3 worldNormal;
		};

		uniform half _ViewBlend;
		uniform half _Pos;
		uniform half _Smooth;
		uniform half _NoiseStrength;
		uniform half _DistortionAmount;
		uniform half _DistortionScale;
		uniform half _NoiseScale;
		uniform half4 _TopColor;
		uniform half4 _BottomColor;
		uniform half _Smoothness;
		uniform half _DepthSmooth;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform half _Depth;
		uniform half _FresnelScale;
		uniform half _FresnelPower;
		uniform half _Amount;
		uniform half2 _AmountCap;


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_positionOS4f = v.vertex;
			o.ase_positionOS4f = ase_positionOS4f;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_positionOS = i.ase_positionOS4f.xyz;
			half simplePerlin3D25 = snoise( ase_positionOS*_DistortionAmount );
			simplePerlin3D25 = simplePerlin3D25*0.5 + 0.5;
			half simplePerlin3D18 = snoise( ( ase_positionOS * ( simplePerlin3D25 * _DistortionScale ) )*_NoiseScale );
			simplePerlin3D18 = simplePerlin3D18*0.5 + 0.5;
			half smoothstepResult22 = smoothstep( -_NoiseStrength , _NoiseStrength , simplePerlin3D18);
			half temp_output_9_0 = ( ( ( 1.0 - _Smooth ) * smoothstepResult22 ) / 100.0 );
			half temp_output_3_0 = (-temp_output_9_0 + (( _Pos / 100.0 ) - 0.0) * (1.0 - -temp_output_9_0) / (1.0 - 0.0));
			half smoothstepResult5 = smoothstep( temp_output_3_0 , ( temp_output_3_0 + temp_output_9_0 ) , ase_positionOS.y);
			half clampResult70 = clamp( smoothstepResult5 , 0.0 , 1.0 );
			half ColorBlend41 = clampResult70;
			half4 temp_cast_0 = (ColorBlend41).xxxx;
			half4 lerpResult54 = lerp( _TopColor , _BottomColor , ColorBlend41);
			half4 ifLocalVar73 = 0;
			if( _ViewBlend > 0.5 )
				ifLocalVar73 = temp_cast_0;
			else if( _ViewBlend < 0.5 )
				ifLocalVar73 = lerpResult54;
			o.Albedo = ifLocalVar73.rgb;
			o.Smoothness = _Smoothness;
			half lerpResult68 = lerp( _TopColor.a , _BottomColor.a , ColorBlend41);
			float4 ase_positionSS = float4( i.screenPos.xyz , i.screenPos.w + 1e-7 );
			half4 ase_positionSSNorm = ase_positionSS / ase_positionSS.w;
			ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
			float screenDepth43 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_positionSSNorm.xy ));
			half distanceDepth43 = abs( ( screenDepth43 - LinearEyeDepth( ase_positionSSNorm.z ) ) / ( ( abs( _Depth ) / 5000.0 ) ) );
			half smoothstepResult51 = smoothstep( 0.0 , ( _DepthSmooth / 10.0 ) , distanceDepth43);
			float3 ase_positionWS = i.worldPos;
			half3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - ase_positionWS );
			half3 ase_viewDirWS = normalize( ase_viewVectorWS );
			half3 ase_normalWS = i.worldNormal;
			half fresnelNdotV52 = dot( ase_normalWS, ase_viewDirWS );
			half fresnelNode52 = ( 0.0 + _FresnelScale * pow( max( 1.0 - fresnelNdotV52 , 0.0001 ), _FresnelPower ) );
			half temp_output_129_0 = saturate( ( smoothstepResult51 + fresnelNode52 ) );
			half ifLocalVar75 = 0;
			if( _ViewBlend > 0.5 )
				ifLocalVar75 = 1.0;
			else if( _ViewBlend < 0.5 )
				ifLocalVar75 = ( lerpResult68 * temp_output_129_0 );
			half3 ase_normalOS = mul( unity_WorldToObject, float4( ase_normalWS, 0 ) );
			ase_normalOS = normalize( ase_normalOS );
			half2 break115 = ( _AmountCap / float2( 100,100 ) );
			half lerpResult116 = lerp( break115.x , break115.y , _Amount);
			half Amount123 = ( _Amount == 0.0 ? 0.0 : ( ase_normalOS.y >= 0.999 ? 1.0 : step( ase_positionOS.y , lerpResult116 ) ) );
			o.Alpha = ( ifLocalVar75 * Amount123 );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19800
Node;AmplifyShaderEditor.RangedFloatNode;30;-4159.741,1213.79;Inherit;False;Property;_DistortionAmount;Distortion Amount;14;0;Create;True;0;0;0;False;0;False;0;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;19;-4133.581,1065.218;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;31;-3946.742,1230.79;Inherit;False;Property;_DistortionScale;Distortion Scale;15;0;Create;True;0;0;0;False;0;False;0;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;25;-3946.562,1133.295;Inherit;False;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-3769.005,1130.351;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-3621.526,1163.235;Inherit;False;Property;_NoiseScale;Noise Scale;13;0;Create;True;0;0;0;False;0;False;1;12.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-3613.526,1245.235;Inherit;False;Property;_NoiseStrength;Noise Strength;12;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-3620.681,1067.141;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NegateNode;24;-3351.141,1167.18;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;18;-3434.681,1063.141;Inherit;False;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-3201.213,969.7738;Inherit;False;Property;_Smooth;Smooth;11;0;Create;True;0;0;0;False;0;False;0;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;71;-3206.136,1038.347;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;22;-3225.681,1140.141;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-3063.68,1034.141;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;9;-2939.213,1033.774;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-2952.213,906.7739;Inherit;False;Property;_Pos;Pos;10;0;Create;True;0;0;0;False;0;False;0;-6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;133;-1792,16;Inherit;False;1172;515;Depth & Frensel;12;45;48;46;49;50;43;64;65;51;52;132;129;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;6;-2826.213,911.7739;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;10;-2832.213,1012.774;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;111;-1708.05,1083.094;Inherit;False;1190;461.0004;Amount;11;122;121;120;119;118;117;116;115;114;113;112;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TFHCRemapNode;3;-2707.213,909.7739;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;112;-1658.05,1330.095;Inherit;False;Property;_AmountCap;AmountCap;1;0;Create;True;0;0;0;False;0;False;-6.4389,6.59976;-6.4389,6.59976;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;45;-1728,80;Inherit;False;Property;_Depth;Depth;5;0;Create;True;0;0;0;False;0;False;0;50;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;17;-2530.68,1008.141;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;1;-2704.413,759.6741;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;113;-1452.05,1335.095;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;100,100;False;1;FLOAT2;0
Node;AmplifyShaderEditor.AbsOpNode;48;-1536,80;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;5;-2414.813,886.7739;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;115;-1339.05,1335.095;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;114;-1487.05,1428.095;Inherit;False;Property;_Amount;Amount;0;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;46;-1440,80;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;5000;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-1376,192;Inherit;False;Property;_DepthSmooth;Depth Smooth;6;0;Create;True;0;0;0;False;0;False;0;12.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;70;-2264.587,891.256;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;117;-1250.583,1196.39;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;116;-1227.05,1336.095;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;50;-1216,192;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;43;-1312,64;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-1248,336;Inherit;False;Property;_FresnelScale;Fresnel Scale;7;0;Create;True;0;0;0;False;0;False;0;1.75;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-1248,416;Inherit;False;Property;_FresnelPower;Fresnel Power;8;0;Create;True;0;0;0;False;0;False;0;3.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;41;-2125.833,884.3637;Inherit;False;ColorBlend;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;118;-1069.05,1133.094;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;119;-1069.05,1275.095;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;51;-1088,64;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;52;-1088,272;Inherit;False;Standard;TangentNormal;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;56;-873.0434,-482.8015;Inherit;False;Property;_TopColor;Top Color;2;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,0.9,0.7490196;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.WireNode;121;-763.0497,1321.095;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Compare;120;-873.0497,1176.095;Inherit;False;3;4;0;FLOAT;0;False;1;FLOAT;0.999;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;132;-896,144;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;-869.0433,-116.8013;Inherit;False;41;ColorBlend;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;53;-874.7787,-307.8589;Inherit;False;Property;_BottomColor;Bottom Color;3;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,0.8313254,0,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.Compare;122;-698.0497,1179.095;Inherit;False;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;129;-784,144;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;68;-627.5872,-197.744;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-186.3186,-0.2434998;Inherit;False;Constant;_Opacityconst1;Opacity const 1;16;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;123;-510.5378,1175.575;Inherit;False;Amount;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-242.8745,-254.9095;Inherit;False;Property;_ViewBlend;View Blend;9;2;[Header];[Toggle];Create;True;1;Blend;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-176,80;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;54;-627.8801,-325.0515;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ConditionalIfNode;75;5.681396,-34.2435;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;124;16,144;Inherit;False;123;Amount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;74;-254.8745,-403.9095;Inherit;False;41;ColorBlend;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;125;192,0;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;101;-425.8652,-189.942;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;0,-144;Inherit;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;73;-51.87451,-470.9095;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;326.8508,-211.1415;Half;False;True;-1;2;ASEMaterialInspector;0;0;Standard;VRCCoffee/Ade;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;1;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;1;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;2;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;25;0;19;0
WireConnection;25;1;30;0
WireConnection;29;0;25;0
WireConnection;29;1;31;0
WireConnection;26;0;19;0
WireConnection;26;1;29;0
WireConnection;24;0;23;0
WireConnection;18;0;26;0
WireConnection;18;1;20;0
WireConnection;71;0;8;0
WireConnection;22;0;18;0
WireConnection;22;1;24;0
WireConnection;22;2;23;0
WireConnection;21;0;71;0
WireConnection;21;1;22;0
WireConnection;9;0;21;0
WireConnection;6;0;7;0
WireConnection;10;0;9;0
WireConnection;3;0;6;0
WireConnection;3;3;10;0
WireConnection;17;0;3;0
WireConnection;17;1;9;0
WireConnection;113;0;112;0
WireConnection;48;0;45;0
WireConnection;5;0;1;2
WireConnection;5;1;3;0
WireConnection;5;2;17;0
WireConnection;115;0;113;0
WireConnection;46;0;48;0
WireConnection;70;0;5;0
WireConnection;116;0;115;0
WireConnection;116;1;115;1
WireConnection;116;2;114;0
WireConnection;50;0;49;0
WireConnection;43;0;46;0
WireConnection;41;0;70;0
WireConnection;119;0;117;2
WireConnection;119;1;116;0
WireConnection;51;0;43;0
WireConnection;51;2;50;0
WireConnection;52;2;64;0
WireConnection;52;3;65;0
WireConnection;121;0;114;0
WireConnection;120;0;118;2
WireConnection;120;3;119;0
WireConnection;132;0;51;0
WireConnection;132;1;52;0
WireConnection;122;0;121;0
WireConnection;122;3;120;0
WireConnection;129;0;132;0
WireConnection;68;0;56;4
WireConnection;68;1;53;4
WireConnection;68;2;55;0
WireConnection;123;0;122;0
WireConnection;69;0;68;0
WireConnection;69;1;129;0
WireConnection;54;0;56;0
WireConnection;54;1;53;0
WireConnection;54;2;55;0
WireConnection;75;0;72;0
WireConnection;75;2;76;0
WireConnection;75;4;69;0
WireConnection;125;0;75;0
WireConnection;125;1;124;0
WireConnection;101;0;68;0
WireConnection;101;1;129;0
WireConnection;73;0;72;0
WireConnection;73;2;74;0
WireConnection;73;4;54;0
WireConnection;0;0;73;0
WireConnection;0;4;78;0
WireConnection;0;9;125;0
ASEEND*/
//CHKSM=DDB81DBA7A54E40253C17F9D0990AD03EF1CBD5C