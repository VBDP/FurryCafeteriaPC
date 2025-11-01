// Made with Amplify Shader Editor v1.9.7.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VRCCoffee/Latte_Iced"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Amount("Amount", Range( 0 , 1)) = 1
		_AmountCap("AmountCap", Vector) = (-6.4389,6.59976,0,0)
		[Header(Color)]_Color_Top1("Color_Top 1", Color) = (1,1,1,1)
		_Color_Top1Noise("Color_Top 1 Noise", Float) = 1
		_Color_Top1NoiseStr("Color_Top 1 Noise Str", Float) = 1
		_Color_Top2("Color_Top 2", Color) = (1,1,1,1)
		_Color_Top2Pos("Color_Top 2 Pos", Float) = 0
		_Color_Top2Blend("Color_Top 2 Blend", Range( 0 , 1)) = 0.5
		_Color_Botm("Color_Botm", Color) = (1,1,1,1)
		_Smooth("Smooth", Range( 0 , 1)) = 0
		[Header(Ice Contact)]_Contact_Distance("Contact_Distance", Float) = 1
		_Contact_Sharp("Contact_Sharp", Float) = 0.5
		[Header(Offset)]_Top_Pos("Top_Pos", Float) = 0
		_Top_Blend("Top_Blend", Range( 0 , 1)) = 0.5
		_Botm_Pos("Botm_Pos", Float) = 0
		_Botm_Blend("Botm_Blend", Range( 0 , 1)) = 0.5
		_BlendScale("Blend Scale", Float) = 0
		[Header(Blending)]_Dist_Offset("Dist_Offset", Vector) = (1,1,1,0)
		_Dist_Scale("Dist_Scale", Float) = 4.856915
		_Dist_Strength("Dist_Strength", Float) = 1
		_Dist_Lerp("Dist_Lerp", Float) = 0
		_Dist_Smooth("Dist_Smooth", Range( 0 , 1)) = 0.5
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#define ASE_VERSION 19701
		#pragma surface surf Standard keepalpha noshadow 
		struct Input
		{
			float4 screenPos;
			float3 worldPos;
			half3 worldNormal;
		};

		uniform half4 _Color_Botm;
		uniform half _Contact_Sharp;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform half _Contact_Distance;
		uniform half4 _Color_Top1;
		uniform half _Color_Top1Noise;
		uniform half _Color_Top1NoiseStr;
		uniform half4 _Color_Top2;
		uniform half _Color_Top2Pos;
		uniform half _Color_Top2Blend;
		uniform half _Top_Pos;
		uniform half _Top_Blend;
		uniform half _BlendScale;
		uniform half _Botm_Pos;
		uniform half _Botm_Blend;
		uniform half _Dist_Lerp;
		uniform half _Dist_Smooth;
		uniform half3 _Dist_Offset;
		uniform half _Dist_Scale;
		uniform half _Dist_Strength;
		uniform half _Smooth;
		uniform half _Amount;
		uniform half2 _AmountCap;
		uniform float _Cutoff = 0.5;


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


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			half4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth100 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			half distanceDepth100 = abs( ( screenDepth100 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( ( abs( _Contact_Distance ) / 5000.0 ) ) );
			half smoothstepResult115 = smoothstep( 0.0 , _Contact_Sharp , ( 1.0 - distanceDepth100 ));
			half clampResult107 = clamp( smoothstepResult115 , 0.0 , 1.0 );
			half Contact108 = clampResult107;
			half4 temp_cast_0 = (Contact108).xxxx;
			half4 Color_Botm117 = ( _Color_Botm - temp_cast_0 );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			half simplePerlin3D141 = snoise( ase_vertex3Pos*_Color_Top1Noise );
			simplePerlin3D141 = simplePerlin3D141*0.5 + 0.5;
			half4 temp_cast_1 = (( simplePerlin3D141 * ( _Color_Top1NoiseStr / 100.0 ) )).xxxx;
			half temp_output_125_0 = (_Color_Top2Blend + (( _Color_Top2Pos / 100.0 ) - 0.0) * (1.0 - _Color_Top2Blend) / (1.0 - 0.0));
			half smoothstepResult128 = smoothstep( temp_output_125_0 , ( temp_output_125_0 + -_Color_Top2Blend ) , ase_vertex3Pos.y);
			half4 lerpResult136 = lerp( ( _Color_Top1 - temp_cast_1 ) , ( _Color_Top2 * smoothstepResult128 ) , smoothstepResult128);
			half4 temp_cast_2 = (Contact108).xxxx;
			half4 Color_Top113 = ( lerpResult136 - temp_cast_2 );
			half simplePerlin3D76 = snoise( ase_vertex3Pos*_BlendScale );
			simplePerlin3D76 = simplePerlin3D76*0.5 + 0.5;
			half temp_output_90_0 = ( _Top_Blend * simplePerlin3D76 );
			half temp_output_29_0 = (-temp_output_90_0 + (( _Top_Pos / 100.0 ) - 0.0) * (1.0 - -temp_output_90_0) / (1.0 - 0.0));
			half smoothstepResult34 = smoothstep( temp_output_29_0 , ( temp_output_29_0 + temp_output_90_0 ) , ase_vertex3Pos.y);
			half temp_output_91_0 = ( _Botm_Blend * simplePerlin3D76 );
			half temp_output_71_0 = (temp_output_91_0 + (( _Botm_Pos / 100.0 ) - 0.0) * (1.0 - temp_output_91_0) / (1.0 - 0.0));
			half smoothstepResult77 = smoothstep( temp_output_71_0 , ( temp_output_71_0 + -temp_output_91_0 ) , ase_vertex3Pos.y);
			half temp_output_54_0 = (-_Dist_Smooth + (( _Dist_Lerp / 100.0 ) - 0.0) * (1.0 - -_Dist_Smooth) / (1.0 - 0.0));
			half simplePerlin3D45 = snoise( ( ase_vertex3Pos / _Dist_Offset )*_Dist_Scale );
			simplePerlin3D45 = simplePerlin3D45*0.5 + 0.5;
			half simplePerlin3D14 = snoise( ( ase_vertex3Pos * simplePerlin3D45 )*_Dist_Strength );
			simplePerlin3D14 = simplePerlin3D14*0.5 + 0.5;
			half smoothstepResult53 = smoothstep( temp_output_54_0 , ( temp_output_54_0 + _Dist_Smooth ) , simplePerlin3D14);
			half Blend109 = ( smoothstepResult34 + ( ( 1.0 - ( smoothstepResult34 + smoothstepResult77 ) ) * smoothstepResult53 ) );
			half4 lerpResult93 = lerp( Color_Botm117 , Color_Top113 , Blend109);
			o.Albedo = lerpResult93.rgb;
			o.Smoothness = _Smooth;
			o.Alpha = 1;
			half3 ase_worldNormal = i.worldNormal;
			half3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			ase_vertexNormal = normalize( ase_vertexNormal );
			half2 break155 = ( _AmountCap / float2( 100,100 ) );
			half lerpResult156 = lerp( break155.x , break155.y , _Amount);
			half Amount163 = ( _Amount == 0.0 ? 0.0 : ( ase_vertexNormal.y >= 0.999 ? 1.0 : step( ase_vertex3Pos.y , lerpResult156 ) ) );
			clip( Amount163 - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19701
Node;AmplifyShaderEditor.CommentaryNode;149;-2769.586,638.8273;Inherit;False;2281.584;1437.66;Blend;42;73;72;76;35;68;90;67;30;49;7;69;31;42;91;56;57;70;71;51;44;29;74;58;1;55;75;45;33;54;20;77;34;48;14;81;52;92;53;85;89;109;137;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;147;-2481.725,-696.5168;Inherit;False;1429.524;289.5157;Depth;9;99;115;101;100;98;97;96;108;107;;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;72;-2719.586,1010.682;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;73;-2698.686,1156.382;Inherit;False;Property;_BlendScale;Blend Scale;17;0;Create;True;0;0;0;False;0;False;0;17.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-2316.28,948.8137;Inherit;False;Property;_Top_Blend;Top_Blend;14;0;Create;True;0;0;0;False;0;False;0.5;0.162;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;76;-2530.025,1069.911;Inherit;False;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-2453.725,-632.7875;Inherit;False;Property;_Contact_Distance;Contact_Distance;11;1;[Header];Create;True;1;Ice Contact;0;0;False;0;False;1;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-2329.069,1280.493;Inherit;False;Property;_Botm_Blend;Botm_Blend;16;0;Create;True;0;0;0;False;0;False;0.5;0.086;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;-2038.391,954.4423;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;148;-2645.127,-362.7673;Inherit;False;1822.373;949.3693;Color;25;121;123;120;124;125;139;144;140;145;141;126;127;94;143;134;128;142;135;136;129;95;114;118;113;117;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-2045.203,809.191;Inherit;False;Property;_Top_Pos;Top_Pos;13;1;[Header];Create;True;1;Offset;0;0;False;0;False;0;5.91;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-2045.843,1184.021;Inherit;False;Property;_Botm_Pos;Botm_Pos;15;0;Create;True;0;0;0;False;0;False;0;-4.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;97;-2259.425,-633.7475;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-2043.099,1286.961;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;69;-1908.231,1190.507;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;7;-1904.14,813.9282;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;98;-2130.425,-629.7475;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;5000;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;31;-1908.58,915.8264;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;49;-2696.646,1854.104;Inherit;False;Property;_Dist_Offset;Dist_Offset;18;1;[Header];Create;True;1;Blending;0;0;False;0;False;1,1,1;1,2,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;121;-2484.05,253.9666;Inherit;False;Property;_Color_Top2Pos;Color_Top 2 Pos;7;0;Create;True;0;0;0;False;0;False;0;-8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;42;-2726.986,1613.122;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;120;-2595.127,395.5892;Inherit;False;Property;_Color_Top2Blend;Color_Top 2 Blend;8;0;Create;True;0;0;0;False;0;False;0.5;0.16;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;151;-2502.83,2186.422;Inherit;False;1190;461.0004;Amount;11;162;161;160;159;158;157;156;155;154;153;152;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TFHCRemapNode;29;-1767.106,843.5905;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;100;-2010.728,-629.7875;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;123;-2307.987,259.7039;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-2504.63,1809.67;Inherit;False;Property;_Dist_Scale;Dist_Scale;19;0;Create;True;0;0;0;False;0;False;4.856915;40;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;51;-2504.646,1701.104;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-1999.127,1960.105;Inherit;False;Property;_Dist_Smooth;Dist_Smooth;22;0;Create;True;0;0;0;False;0;False;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;70;-1907.37,1351.506;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-1864.9,1818.633;Inherit;False;Property;_Dist_Lerp;Dist_Lerp;21;0;Create;True;0;0;0;False;0;False;0;40;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;71;-1769.896,1216.27;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;152;-2452.83,2433.422;Inherit;False;Property;_AmountCap;AmountCap;2;0;Create;True;0;0;0;False;0;False;-6.4389,6.59976;-6.4389,6.59976;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PosVertexDataNode;1;-1764.441,688.8274;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;33;-1583.491,934.0167;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;139;-2260.633,-197.8435;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;75;-1586.282,1306.696;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;140;-2266.733,-51.14377;Inherit;False;Property;_Color_Top1Noise;Color_Top 1 Noise;4;0;Create;True;0;0;0;False;0;False;1;27.49;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;144;-2068.249,-37.60001;Inherit;False;Property;_Color_Top1NoiseStr;Color_Top 1 Noise Str;5;0;Create;True;0;0;0;False;0;False;1;15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;99;-1896.39,-502.001;Inherit;False;Property;_Contact_Sharp;Contact_Sharp;12;0;Create;True;0;0;0;False;0;False;0.5;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;45;-2327.042,1698.281;Inherit;False;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;125;-2170.953,289.3661;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;58;-1727.287,1825.119;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;101;-1774.269,-629.7875;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;55;-1730.427,1923.118;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;74;-1767.23,1061.507;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NegateNode;124;-2315.427,475.602;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;127;-2168.287,134.6029;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;145;-1855.249,-38.60001;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;153;-2246.83,2438.422;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;100,100;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SmoothstepOpNode;34;-1451.429,815.1956;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-2070.832,1620.825;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SmoothstepOpNode;77;-1454.219,1187.875;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;54;-1588.953,1850.882;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;126;-1987.338,379.7921;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-2071.648,1728.602;Inherit;False;Property;_Dist_Strength;Dist_Strength;20;0;Create;True;0;0;0;False;0;False;1;70;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;115;-1596.839,-639.5168;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;141;-2071.072,-138.6146;Inherit;False;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;128;-1855.276,260.9712;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;14;-1896.272,1644.099;Inherit;False;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;107;-1416.902,-624.3342;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-1742.249,-130.6002;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;134;-1836.322,73.64548;Inherit;False;Property;_Color_Top2;Color_Top 2;6;0;Create;True;0;0;0;False;0;False;1,1,1,1;0.9215686,0.8358143,0.7179019,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;94;-1814.342,-312.7674;Inherit;False;Property;_Color_Top1;Color_Top 1;3;1;[Header];Create;True;1;Color;0;0;False;0;False;1,1,1,1;0.672,0.3467005,0.136968,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleAddOpNode;81;-1189.73,1152.584;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;155;-2133.83,2438.422;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;52;-1405.337,1941.308;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;154;-2281.83,2531.422;Inherit;False;Property;_Amount;Amount;1;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;156;-2021.83,2439.422;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;53;-1273.276,1822.487;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;135;-1606.423,78.64462;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;92;-1183.937,1294.147;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;108;-1276.204,-626.3742;Inherit;False;Contact;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;157;-2045.363,2299.718;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;142;-1584.25,-223.6001;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;95;-1420.87,335.8885;Inherit;False;Property;_Color_Botm;Color_Botm;9;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode;129;-1401.089,190.146;Inherit;False;108;Contact;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;136;-1392.523,59.64473;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;159;-1863.83,2236.422;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-991.7941,1527.037;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;158;-1863.83,2378.422;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;89;-839.5386,1494.732;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;114;-1194.716,111.2327;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Compare;161;-1667.83,2279.422;Inherit;False;3;4;0;FLOAT;0;False;1;FLOAT;0.999;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;160;-1557.83,2424.422;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;118;-1199.06,334.1978;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Compare;162;-1492.83,2282.422;Inherit;False;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;113;-1046.755,109.6394;Inherit;False;Color_Top;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;109;-712.0019,1500.567;Inherit;False;Blend;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;117;-1060.06,335.1977;Inherit;False;Color_Botm;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;119;-509.0267,17.34888;Inherit;False;117;Color_Botm;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;110;-499.6864,176.6727;Inherit;False;109;Blend;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;163;-1299.891,2281.305;Inherit;False;Amount;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;112;-502.2252,99.08844;Inherit;False;113;Color_Top;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;93;-304.2771,78.98663;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;137;-1085.638,829.1786;Inherit;False;Blend_Botm;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;150;-300.8264,225.4763;Inherit;False;Property;_Smooth;Smooth;10;0;Create;True;0;0;0;False;0;False;0;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;164;-195.5476,305.7137;Inherit;False;163;Amount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Half;False;True;-1;2;ASEMaterialInspector;0;0;Standard;VRCCoffee/Latte_Iced;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;False;0;True;Opaque;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;2;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;76;0;72;0
WireConnection;76;1;73;0
WireConnection;90;0;35;0
WireConnection;90;1;76;0
WireConnection;97;0;96;0
WireConnection;91;0;68;0
WireConnection;91;1;76;0
WireConnection;69;0;67;0
WireConnection;7;0;30;0
WireConnection;98;0;97;0
WireConnection;31;0;90;0
WireConnection;29;0;7;0
WireConnection;29;3;31;0
WireConnection;100;0;98;0
WireConnection;123;0;121;0
WireConnection;51;0;42;0
WireConnection;51;1;49;0
WireConnection;70;0;91;0
WireConnection;71;0;69;0
WireConnection;71;3;91;0
WireConnection;33;0;29;0
WireConnection;33;1;90;0
WireConnection;75;0;71;0
WireConnection;75;1;70;0
WireConnection;45;0;51;0
WireConnection;45;1;44;0
WireConnection;125;0;123;0
WireConnection;125;3;120;0
WireConnection;58;0;56;0
WireConnection;101;0;100;0
WireConnection;55;0;57;0
WireConnection;124;0;120;0
WireConnection;145;0;144;0
WireConnection;153;0;152;0
WireConnection;34;0;1;2
WireConnection;34;1;29;0
WireConnection;34;2;33;0
WireConnection;48;0;42;0
WireConnection;48;1;45;0
WireConnection;77;0;74;2
WireConnection;77;1;71;0
WireConnection;77;2;75;0
WireConnection;54;0;58;0
WireConnection;54;3;55;0
WireConnection;126;0;125;0
WireConnection;126;1;124;0
WireConnection;115;0;101;0
WireConnection;115;2;99;0
WireConnection;141;0;139;0
WireConnection;141;1;140;0
WireConnection;128;0;127;2
WireConnection;128;1;125;0
WireConnection;128;2;126;0
WireConnection;14;0;48;0
WireConnection;14;1;20;0
WireConnection;107;0;115;0
WireConnection;143;0;141;0
WireConnection;143;1;145;0
WireConnection;81;0;34;0
WireConnection;81;1;77;0
WireConnection;155;0;153;0
WireConnection;52;0;54;0
WireConnection;52;1;57;0
WireConnection;156;0;155;0
WireConnection;156;1;155;1
WireConnection;156;2;154;0
WireConnection;53;0;14;0
WireConnection;53;1;54;0
WireConnection;53;2;52;0
WireConnection;135;0;134;0
WireConnection;135;1;128;0
WireConnection;92;0;81;0
WireConnection;108;0;107;0
WireConnection;142;0;94;0
WireConnection;142;1;143;0
WireConnection;136;0;142;0
WireConnection;136;1;135;0
WireConnection;136;2;128;0
WireConnection;85;0;92;0
WireConnection;85;1;53;0
WireConnection;158;0;157;2
WireConnection;158;1;156;0
WireConnection;89;0;34;0
WireConnection;89;1;85;0
WireConnection;114;0;136;0
WireConnection;114;1;129;0
WireConnection;161;0;159;2
WireConnection;161;3;158;0
WireConnection;160;0;154;0
WireConnection;118;0;95;0
WireConnection;118;1;129;0
WireConnection;162;0;160;0
WireConnection;162;3;161;0
WireConnection;113;0;114;0
WireConnection;109;0;89;0
WireConnection;117;0;118;0
WireConnection;163;0;162;0
WireConnection;93;0;119;0
WireConnection;93;1;112;0
WireConnection;93;2;110;0
WireConnection;137;0;34;0
WireConnection;0;0;93;0
WireConnection;0;4;150;0
WireConnection;0;10;164;0
ASEEND*/
//CHKSM=5FD33740E04C36E7C7DF33970CC219F47BC4B579