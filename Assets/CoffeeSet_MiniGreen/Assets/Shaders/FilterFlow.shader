// Made with Amplify Shader Editor v1.9.7.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VRCCoffee/FilterFlow"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Flow("Flow", Range( 0 , 1)) = 0
		[Toggle]_FlowFlip("FlowFlip", Range( 0 , 1)) = 0
		_FlowSpeed("FlowSpeed", Float) = 0
		_Color("Color", Color) = (1,1,1)
		_ColorDetail1("ColorDetail1", Color) = (1,1,1)
		_ColorDetail2("ColorDetail2", Color) = (1,1,1)
		_DetailStretchX("Detail Stretch X", Float) = 0
		_DetailStretchY("Detail Stretch Y", Float) = 0
		_DetailScale("Detail Scale", Float) = 1
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_OffsetSpeed("Offset Speed", Float) = 0
		_OffsetScale("Offset Scale", Float) = 0
		_OffsetPower("Offset Power", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#define ASE_VERSION 19701
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform half _OffsetSpeed;
		uniform half _OffsetScale;
		uniform half _OffsetPower;
		uniform half3 _Color;
		uniform half3 _ColorDetail1;
		uniform half3 _ColorDetail2;
		uniform half _FlowSpeed;
		uniform half _DetailStretchX;
		uniform half _DetailStretchY;
		uniform half _DetailScale;
		uniform half _Metallic;
		uniform half _Smoothness;
		uniform half _Flow;
		uniform half _FlowFlip;
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


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			half3 ase_vertexNormal = v.normal.xyz;
			float3 ase_vertex3Pos = v.vertex.xyz;
			half mulTime67 = _Time.y * ( _OffsetSpeed / 10.0 );
			half3 appendResult63 = (half3(ase_vertex3Pos.x , ( ase_vertex3Pos.y + mulTime67 ) , ase_vertex3Pos.z));
			half simplePerlin3D54 = snoise( appendResult63*_OffsetScale );
			simplePerlin3D54 = simplePerlin3D54*0.5 + 0.5;
			v.vertex.xyz += ( ( ( float3( 0,0,0 ) - ( ase_vertexNormal * (-0.5 + (simplePerlin3D54 - 0.0) * (1.0 - -0.5) / (1.0 - 0.0)) ) ) * ( _OffsetPower / 10000.0 ) ) * ( 1.0 - v.texcoord.xy.y ) );
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			half mulTime36 = _Time.y * ( _FlowSpeed / 10.0 );
			half2 appendResult35 = (half2(0.0 , mulTime36));
			half2 appendResult22 = (half2(( abs( _DetailStretchX ) * 50.0 ) , ( abs( _DetailStretchY ) * 50.0 )));
			half2 temp_output_27_0 = ( ( i.uv_texcoord + appendResult35 ) / appendResult22 );
			half temp_output_45_0 = ( _DetailScale * 500.0 );
			half simplePerlin2D49 = snoise( ( temp_output_27_0 + float2( 0.001,0 ) )*temp_output_45_0 );
			simplePerlin2D49 = simplePerlin2D49*0.5 + 0.5;
			half3 lerpResult48 = lerp( _ColorDetail1 , _ColorDetail2 , simplePerlin2D49);
			half simplePerlin2D16 = snoise( temp_output_27_0*temp_output_45_0 );
			simplePerlin2D16 = simplePerlin2D16*0.5 + 0.5;
			half3 lerpResult37 = lerp( _Color , lerpResult48 , simplePerlin2D16);
			o.Albedo = lerpResult37;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
			half lerpResult11 = lerp( saturate( ( ( i.uv_texcoord.y - 0.5 ) + _Flow ) ) , ( 1.0 - saturate( ( ( i.uv_texcoord.y + 0.5 ) - _Flow ) ) ) , _FlowFlip);
			clip( lerpResult11 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19701
Node;AmplifyShaderEditor.RangedFloatNode;65;-1920,816;Inherit;False;Property;_OffsetSpeed;Offset Speed;12;0;Create;True;0;0;0;False;0;False;0;3.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-2304,-128;Inherit;False;Property;_FlowSpeed;FlowSpeed;3;0;Create;True;0;0;0;False;0;False;0;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;66;-1712,816;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1792,16;Inherit;False;Property;_DetailStretchY;Detail Stretch Y;8;0;Create;True;0;0;0;False;0;False;0;175;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-1792,-80;Inherit;False;Property;_DetailStretchX;Detail Stretch X;7;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;41;-2096,-128;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;51;-1600,656;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;67;-1584,816;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;25;-1616,16;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;43;-1616,-96;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;36;-1966,-136.5;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;64;-1328,784;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;35;-1744,-240;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;29;-2000,-304;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-1504,16;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;50;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-1504,-96;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;50;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-1168,848;Inherit;False;Property;_OffsetScale;Offset Scale;13;0;Create;True;0;0;0;False;0;False;0;25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;63;-1152,672;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;34;-1552,-304;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;22;-1344,-64;Inherit;False;FLOAT2;4;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-1376,176;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;54;-960,800;Inherit;False;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;4;-1072,288;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1232,384;Inherit;False;Property;_Flow;Flow;1;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-1184,-128;Inherit;False;Property;_DetailScale;Detail Scale;9;0;Create;True;0;0;0;False;0;False;1;30;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;27;-1168,-240;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NormalVertexDataNode;52;-928,656;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;68;-736,800;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.5;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;6;-944,288;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;12;-1104,192;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-992,-128;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;500;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;50;-992,-336;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.001,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-560,656;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-512,768;Inherit;False;Property;_OffsetPower;Offset Power;14;0;Create;True;0;0;0;False;0;False;0;13.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;13;-944,192;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;3;-800,288;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;15;-624,-448;Inherit;False;Property;_ColorDetail1;ColorDetail1;5;0;Create;True;0;0;0;False;0;False;1,1,1,1;0.9,0.45,0,1;True;False;0;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;47;-624,-304;Inherit;False;Property;_ColorDetail2;ColorDetail2;6;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,0.6925,0.55,1;True;False;0;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.NoiseGeneratorNode;49;-832,-304;Inherit;False;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;61;-384,656;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;60;-336,768;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;10000;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;69;-544,480;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;10;-800,368;Inherit;False;Property;_FlowFlip;FlowFlip;2;1;[Toggle];Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;14;-800,192;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;7;-656,288;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;9;-624,-592;Inherit;False;Property;_Color;Color;4;0;Create;True;0;0;0;False;0;False;1,1,1,1;0.5,0.04216865,0,1;True;False;0;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp;48;-384,-320;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;16;-832,-208;Inherit;False;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-224,656;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;71;-304,528;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;2;-704,48;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;11;-480,256;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-368,48;Inherit;False;Property;_Metallic;Metallic;11;0;Create;True;0;0;0;False;0;False;0;0.15;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-368,128;Inherit;False;Property;_Smoothness;Smoothness;10;0;Create;True;0;0;0;False;0;False;0;0.65;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;37;-192,-496;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-128,512;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Half;False;True;-1;2;ASEMaterialInspector;0;0;Standard;VRCCoffee/FilterFlow;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;66;0;65;0
WireConnection;41;0;40;0
WireConnection;67;0;66;0
WireConnection;25;0;20;0
WireConnection;43;0;42;0
WireConnection;36;0;41;0
WireConnection;64;0;51;2
WireConnection;64;1;67;0
WireConnection;35;1;36;0
WireConnection;31;0;25;0
WireConnection;44;0;43;0
WireConnection;63;0;51;1
WireConnection;63;1;64;0
WireConnection;63;2;51;3
WireConnection;34;0;29;0
WireConnection;34;1;35;0
WireConnection;22;0;44;0
WireConnection;22;1;31;0
WireConnection;54;0;63;0
WireConnection;54;1;55;0
WireConnection;4;0;1;2
WireConnection;27;0;34;0
WireConnection;27;1;22;0
WireConnection;68;0;54;0
WireConnection;6;0;4;0
WireConnection;6;1;5;0
WireConnection;12;0;1;2
WireConnection;45;0;26;0
WireConnection;50;0;27;0
WireConnection;56;0;52;0
WireConnection;56;1;68;0
WireConnection;13;0;12;0
WireConnection;13;1;5;0
WireConnection;3;0;6;0
WireConnection;49;0;50;0
WireConnection;49;1;45;0
WireConnection;61;1;56;0
WireConnection;60;0;59;0
WireConnection;14;0;13;0
WireConnection;7;0;3;0
WireConnection;48;0;15;0
WireConnection;48;1;47;0
WireConnection;48;2;49;0
WireConnection;16;0;27;0
WireConnection;16;1;45;0
WireConnection;58;0;61;0
WireConnection;58;1;60;0
WireConnection;71;0;69;2
WireConnection;11;0;14;0
WireConnection;11;1;7;0
WireConnection;11;2;10;0
WireConnection;37;0;9;0
WireConnection;37;1;48;0
WireConnection;37;2;16;0
WireConnection;70;0;58;0
WireConnection;70;1;71;0
WireConnection;0;0;37;0
WireConnection;0;3;39;0
WireConnection;0;4;38;0
WireConnection;0;10;11;0
WireConnection;0;11;70;0
ASEEND*/
//CHKSM=B31EB0247B705BFA848F0FB16142454DD327CD6D