// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VRCCoffee/Cafe Liquid"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Height("Height", Range( 0 , 1)) = 0.5
		_MinMax("MinMax", Vector) = (0,0,0,0)
		_ColorBase("ColorBase", Color) = (0,0,0,0)
		_ColorTint("ColorTint", Color) = (0,0,0,0)
		_Clearness("Clearness", Range( 0 , 1)) = 1
		_DepthLerp("Depth Lerp", Range( 0 , 1)) = 0
		_DepthSmooth("Depth Smooth", Range( 0 , 1)) = 0
		_FresnelScale("Fresnel Scale", Float) = 0
		_FresnelPower("Fresnel Power", Float) = 0
		_Texture("Texture", 2D) = "white" {}
		_UVSclaeOffset("UV Sclae/Offset", Vector) = (0,0,0,0)
		_SurfaceUVScale("Surface UV Scale", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent-2" "IgnoreProjector" = "True" "IsEmissive" = "true"   }
		Cull Back
		Stencil
		{
			Ref 174
			Comp Equal
		}
		Blend One Zero , SrcAlpha OneMinusSrcAlpha
		
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.5
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Standard keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float eyeDepth;
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float4 screenPos;
		};

		uniform sampler2D _Texture;
		uniform float2 _MinMax;
		uniform float _Height;
		uniform float _SurfaceUVScale;
		uniform float4 _UVSclaeOffset;
		uniform float4 _ColorTint;
		uniform float4 _ColorBase;
		uniform float _FresnelScale;
		uniform float _FresnelPower;
		uniform float _DepthLerp;
		uniform float _DepthSmooth;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _Clearness;
		uniform float _Cutoff = 0.5;


		float HeightDepth326( inout float3 worldPos, float3 worldCamPos, float3 transformPos, float camDist, float height, float heightMask )
		{
			if (heightMask > 0) return 0;
			if (camDist <= 0) return 0;
			float value;
			float step = 0.001 * camDist;
			float3 rayDir = worldPos - worldCamPos;
			if (rayDir.y > 0) return 0;
			if (step < 0.0001) step = 0.0001;
			for (int i = 0; i < 1/step; i++) {
				worldPos += rayDir * step / camDist;
				if (worldPos.y > height) value += step;
				else break;
			}
			return value;
		}


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 worldPos326 = ase_worldPos;
			float3 worldCamPos326 = _WorldSpaceCameraPos;
			float3 objToWorld352 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float3 transformPos326 = objToWorld352;
			float camDist326 = i.eyeDepth;
			float3 objToWorld55 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
			float2 break45 = ( _MinMax / ( 100.0 / ase_objectScale.y ) );
			float lerpResult44 = lerp( break45.x , break45.y , _Height);
			float HeightValue330 = ( objToWorld55.y + lerpResult44 );
			float height326 = HeightValue330;
			float HeightMask185 = step( ( ase_worldPos.y - objToWorld55.y ) , lerpResult44 );
			float heightMask326 = HeightMask185;
			float localHeightDepth326 = HeightDepth326( worldPos326 , worldCamPos326 , transformPos326 , camDist326 , height326 , heightMask326 );
			float3 WorldPosition399 = worldPos326;
			float3 objToWorld785 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float3 break782 = ( WorldPosition399 - objToWorld785 );
			float2 appendResult783 = (float2(break782.x , break782.z));
			float2 lerpResult788 = lerp( ( appendResult783 * _SurfaceUVScale ) , i.uv_texcoord , HeightMask185);
			float2 appendResult800 = (float2(_UVSclaeOffset.x , _UVSclaeOffset.y));
			float2 appendResult801 = (float2(_UVSclaeOffset.z , _UVSclaeOffset.w));
			float2 UV794 = ( ( lerpResult788 * appendResult800 ) + appendResult801 );
			float4 tex2DNode779 = tex2D( _Texture, UV794 );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float3 lerpResult379 = lerp( float3(0,1,0) , ase_worldNormal , HeightMask185);
			float fresnelNdotV376 = dot( normalize( lerpResult379 ), ase_worldViewDir );
			float fresnelNode376 = ( 0.0 + _FresnelScale * pow( max( 1.0 - fresnelNdotV376 , 0.0001 ), _FresnelPower ) );
			float temp_output_453_0 = (-_DepthSmooth + (_DepthLerp - 0.0) * (1.0 - -_DepthSmooth) / (1.0 - 0.0));
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth743 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth743 = abs( ( screenDepth743 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( 1.0 ) );
			float SurfaceGradation430 = localHeightDepth326;
			float smoothstepResult455 = smoothstep( temp_output_453_0 , ( temp_output_453_0 + _DepthSmooth ) , ( distanceDepth743 - SurfaceGradation430 ));
			float Depth732 = saturate( smoothstepResult455 );
			float4 lerpResult458 = lerp( ( ( tex2DNode779 * _ColorTint ) + _ColorBase ) , ( ( tex2DNode779 * _ColorBase ) + fresnelNode376 ) , Depth732);
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor810 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,ase_grabScreenPosNorm.xy);
			float4 lerpResult814 = lerp( float4( 1,1,1,1 ) , screenColor810 , _Clearness);
			float4 Albedo461 = ( lerpResult458 * lerpResult814 );
			o.Emission = Albedo461.rgb;
			float temp_output_375_0 = 0.0;
			o.Metallic = temp_output_375_0;
			o.Smoothness = temp_output_375_0;
			o.Alpha = 1;
			float screenDepth395 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth395 = saturate( abs( ( screenDepth395 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( 1.0 ) ) );
			float SurfaceOpacity336 = ( distanceDepth395 <= localHeightDepth326 ? 0.0 : 1.0 );
			float lerpResult322 = lerp( SurfaceOpacity336 , 1.0 , HeightMask185);
			clip( lerpResult322 - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "CafeLiquid_Editor"
}
/*ASEBEGIN
Version=18935
0;73;1400;808;2740.427;1384.143;1;True;False
Node;AmplifyShaderEditor.ObjectScaleNode;269;-2810.643,1359.143;Inherit;False;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector2Node;41;-2794.61,1231.876;Inherit;False;Property;_MinMax;MinMax;2;0;Create;True;0;0;0;False;0;False;0,0;-4.91,4.91;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleDivideOpNode;270;-2631.643,1326.143;Inherit;False;2;0;FLOAT;100;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;42;-2499.651,1234.204;Inherit;False;2;0;FLOAT2;500,0;False;1;FLOAT;100;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;45;-2388.813,1232.545;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;43;-2634.348,1447.282;Inherit;False;Property;_Height;Height;1;0;Create;True;0;0;0;False;0;False;0.5;0.571;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;53;-2486.385,938.5608;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformPositionNode;55;-2486.385,1079.562;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;98;-2274.26,1029.069;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;44;-2275.007,1233.215;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;232;-2083.963,1085.059;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;606;-2083.963,1181.059;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;185;-1971.962,1085.059;Inherit;False;HeightMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;330;-1971.962,1181.059;Inherit;False;HeightValue;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SurfaceDepthNode;728;-2949.906,765.1167;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;327;-2868.884,334.8203;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceCameraPos;328;-2934.726,477.263;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;332;-2885.906,845.1168;Inherit;False;330;HeightValue;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TransformPositionNode;352;-2896.067,615.0217;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;466;-2885.906,925.1168;Inherit;False;185;HeightMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;326;-2597.906,541.1167;Inherit;False;if (heightMask > 0) return 0@$if (camDist <= 0) return 0@$$float value@$float step = 0.001 * camDist@$float3 rayDir = worldPos - worldCamPos@$$if (rayDir.y > 0) return 0@$$if (step < 0.0001) step = 0.0001@$$for (int i = 0@ i < 1/step@ i++) {$	worldPos += rayDir * step / camDist@$	if (worldPos.y > height) value += step@$	else break@$}$$return value@;1;Create;6;False;worldPos;FLOAT3;0,0,0;InOut;;Inherit;False;False;worldCamPos;FLOAT3;0,0,0;In;;Inherit;False;False;transformPos;FLOAT3;0,0,0;In;;Inherit;False;False;camDist;FLOAT;0;In;;Inherit;False;False;height;FLOAT;0;In;;Inherit;False;False;heightMask;FLOAT;0;In;;Inherit;False;Height Depth;True;False;0;;False;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;2;FLOAT;0;FLOAT3;1
Node;AmplifyShaderEditor.RegisterLocalVarNode;399;-2165.905,621.1167;Inherit;False;WorldPosition;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;781;-2883.351,1809.406;Inherit;False;399;WorldPosition;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TransformPositionNode;785;-2883.351,1889.406;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;786;-2675.351,1809.406;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;782;-2531.35,1809.406;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;783;-2403.35,1809.406;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;791;-2275.35,1905.405;Inherit;False;Property;_SurfaceUVScale;Surface UV Scale;16;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;799;-2022.905,2016.116;Inherit;False;Property;_UVSclaeOffset;UV Sclae/Offset;15;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;790;-2275.35,1809.406;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;787;-2022.44,1925.623;Inherit;False;185;HeightMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;789;-2275.35,1985.405;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;800;-1829.904,2045.117;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;788;-2019.353,1809.406;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;450;-2609.096,-200.7043;Inherit;False;Property;_DepthSmooth;Depth Smooth;9;0;Create;True;0;0;0;True;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;801;-1701.904,2093.117;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NegateNode;451;-2322.569,-226.0425;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;430;-2165.905,541.1167;Inherit;False;SurfaceGradation;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;792;-1725.904,1860.117;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;452;-2461.538,-336.028;Inherit;False;Property;_DepthLerp;Depth Lerp;8;0;Create;True;0;0;0;True;0;False;0;40;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;802;-1541.904,1965.116;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DepthFade;743;-2725.906,-466.8831;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;453;-2188.595,-331.7789;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;412;-2693.906,-370.8831;Inherit;False;430;SurfaceGradation;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;443;-2469.906,-466.8831;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;794;-1429.904,1965.116;Inherit;False;UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;454;-1992.979,-221.3531;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;377;-2400.452,-1120.308;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;378;-2393.137,-981.9272;Inherit;False;185;HeightMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;380;-2370.306,-1265.493;Inherit;False;Constant;_Vector0;Vector 0;8;0;Create;True;0;0;0;False;0;False;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;795;-2217.406,-1615.276;Inherit;False;794;UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SmoothstepOpNode;455;-1877.904,-370.8831;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;379;-2181.81,-1089.08;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;1,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;779;-2048.28,-1639.597;Inherit;True;Property;_Texture;Texture;14;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;457;-1717.904,-370.8831;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;179;-1961.856,-1266.377;Inherit;False;Property;_ColorBase;ColorBase;3;0;Create;True;0;0;0;True;0;False;0,0,0,0;0.5882353,0.1686273,0.05490187,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;381;-2392.788,-836.4402;Inherit;False;Property;_FresnelScale;Fresnel Scale;10;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;405;-2392.701,-762.0512;Inherit;False;Property;_FresnelPower;Fresnel Power;11;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;459;-1963.208,-1442.167;Inherit;False;Property;_ColorTint;ColorTint;4;0;Create;True;0;0;0;True;0;False;0,0,0,0;0.5882353,0.1686273,0.05490187,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;797;-1712.03,-1422.319;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;796;-1715.904,-1528.883;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GrabScreenPosition;809;-1570.059,-1155.899;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;732;-1580.405,-376.0831;Inherit;False;Depth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;376;-1991.92,-1013.295;Inherit;False;Standard;WorldNormal;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;810;-1360.749,-1153.772;Inherit;False;Global;_GrabScreen0;Grab Screen 0;18;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DepthFade;395;-2597.906,445.1169;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;460;-1546.198,-1529.821;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;815;-1550.752,-1321.037;Inherit;False;732;Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;408;-1548.771,-1434.924;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;220;-1465.715,-974.1691;Inherit;False;Property;_Clearness;Clearness;5;0;Create;True;0;0;0;True;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Compare;398;-2309.906,445.1169;Inherit;False;5;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;458;-1356.566,-1496.851;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;814;-1137.387,-1050.722;Inherit;False;3;0;COLOR;1,1,1,1;False;1;COLOR;1,1,1,1;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;813;-988.5316,-1361.1;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;336;-2165.905,445.1169;Inherit;False;SurfaceOpacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;337;-126.0283,-22.6128;Inherit;False;336;SurfaceOpacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;316;-104.8296,50.83853;Inherit;False;185;HeightMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;461;-849.1583,-1364.976;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;383;-579.2322,538.366;Inherit;False;Property;_NoiseScale;Noise Scale;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;404;-521.1037,398.7669;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;375;151.2814,-177.0152;Inherit;False;Constant;_0;0;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;318;304,-496;Inherit;False;Property;_Metal;Metal;6;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinTimeNode;402;-734.1042,446.767;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;320;304,-336;Inherit;False;Property;_Layer;Layer;13;0;Create;True;0;0;0;False;0;False;0;106;0;255;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;355;-395.3381,462.4814;Inherit;False;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;401;-734.9233,356.4827;Inherit;False;399;WorldPosition;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;322;78.61646,-12.01519;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;319;304,-416;Inherit;False;Property;_Smooth;Smooth;7;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;462;93.31763,-281.2191;Inherit;False;461;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;304,-256;Float;False;True;-1;3;CafeLiquid_Editor;0;0;Standard;VRCCoffee/Cafe Liquid;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;1;Custom;0.5;True;False;-2;True;TransparentCutout;;Transparent;All;18;all;True;True;True;True;0;False;-1;True;174;False;320;255;False;-1;255;False;-1;5;False;235;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;15;10;25;False;0.5;False;0;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;1;=;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;270;1;269;2
WireConnection;42;0;41;0
WireConnection;42;1;270;0
WireConnection;45;0;42;0
WireConnection;98;0;53;2
WireConnection;98;1;55;2
WireConnection;44;0;45;0
WireConnection;44;1;45;1
WireConnection;44;2;43;0
WireConnection;232;0;98;0
WireConnection;232;1;44;0
WireConnection;606;0;55;2
WireConnection;606;1;44;0
WireConnection;185;0;232;0
WireConnection;330;0;606;0
WireConnection;326;0;327;0
WireConnection;326;1;328;0
WireConnection;326;2;352;0
WireConnection;326;3;728;0
WireConnection;326;4;332;0
WireConnection;326;5;466;0
WireConnection;399;0;326;1
WireConnection;786;0;781;0
WireConnection;786;1;785;0
WireConnection;782;0;786;0
WireConnection;783;0;782;0
WireConnection;783;1;782;2
WireConnection;790;0;783;0
WireConnection;790;1;791;0
WireConnection;800;0;799;1
WireConnection;800;1;799;2
WireConnection;788;0;790;0
WireConnection;788;1;789;0
WireConnection;788;2;787;0
WireConnection;801;0;799;3
WireConnection;801;1;799;4
WireConnection;451;0;450;0
WireConnection;430;0;326;0
WireConnection;792;0;788;0
WireConnection;792;1;800;0
WireConnection;802;0;792;0
WireConnection;802;1;801;0
WireConnection;453;0;452;0
WireConnection;453;3;451;0
WireConnection;443;0;743;0
WireConnection;443;1;412;0
WireConnection;794;0;802;0
WireConnection;454;0;453;0
WireConnection;454;1;450;0
WireConnection;455;0;443;0
WireConnection;455;1;453;0
WireConnection;455;2;454;0
WireConnection;379;0;380;0
WireConnection;379;1;377;0
WireConnection;379;2;378;0
WireConnection;779;1;795;0
WireConnection;457;0;455;0
WireConnection;797;0;779;0
WireConnection;797;1;179;0
WireConnection;796;0;779;0
WireConnection;796;1;459;0
WireConnection;732;0;457;0
WireConnection;376;0;379;0
WireConnection;376;2;381;0
WireConnection;376;3;405;0
WireConnection;810;0;809;0
WireConnection;460;0;796;0
WireConnection;460;1;179;0
WireConnection;408;0;797;0
WireConnection;408;1;376;0
WireConnection;398;0;395;0
WireConnection;398;1;326;0
WireConnection;458;0;460;0
WireConnection;458;1;408;0
WireConnection;458;2;815;0
WireConnection;814;1;810;0
WireConnection;814;2;220;0
WireConnection;813;0;458;0
WireConnection;813;1;814;0
WireConnection;336;0;398;0
WireConnection;461;0;813;0
WireConnection;404;0;401;0
WireConnection;404;1;402;1
WireConnection;355;0;404;0
WireConnection;355;1;383;0
WireConnection;322;0;337;0
WireConnection;322;2;316;0
WireConnection;0;2;462;0
WireConnection;0;3;375;0
WireConnection;0;4;375;0
WireConnection;0;10;322;0
ASEEND*/
//CHKSM=E2D3E8D6EEA7AC0266573F310DDD20CCDA6296A4