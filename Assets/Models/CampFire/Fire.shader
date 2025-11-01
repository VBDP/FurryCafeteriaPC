// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "RED_SIM/Fire"
{
	Properties
	{
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 3
		_TessMin( "Tess Min Distance", Float ) = 2
		_TessMax( "Tess Max Distance", Float ) = 5
		_Speed2("Speed 2", Range( 0 , 10)) = 3.294118
		_Speed("Speed", Range( 0 , 10)) = 3.294118
		_Scale2("Scale 2", Range( 0 , 5)) = 5.882353
		_Scale("Scale", Range( 0 , 5)) = 5.882353
		[HDR]_ColorDown("Color Down", Color) = (1,0.3845955,0,0)
		[HDR]_ColorUp("Color Up", Color) = (1,0.3845955,0,0)
		_ConstantOffset("Constant Offset", Vector) = (0,0,0,0)
		_FlameSize("Flame Size", Range( 0 , 1)) = 0
		_VertexOffset("Vertex Offset", Range( 0 , 1)) = 0
		_CenterShrink("Center Shrink", Range( 0 , 2)) = 0
		_CenterRise("Center Rise", Range( 0 , 1)) = 0
		_Brightness("Brightness", Range( -3 , 0)) = 0
		_Wisth2("Wisth 2", Float) = 1
		_Width("Width", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Unlit alpha:fade keepalpha noshadow vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float2 uv_texcoord;
			float2 uv2_texcoord2;
		};

		uniform float _Wisth2;
		uniform float _Speed2;
		uniform float _Scale2;
		uniform float _VertexOffset;
		uniform float3 _ConstantOffset;
		uniform float _CenterShrink;
		uniform float _CenterRise;
		uniform float4 _ColorUp;
		uniform float4 _ColorDown;
		uniform float _Width;
		uniform float _Speed;
		uniform float _Scale;
		uniform float _Brightness;
		uniform float _FlameSize;
		uniform float _TessValue;
		uniform float _TessMin;
		uniform float _TessMax;


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


		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _TessValue );
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float3 ase_vertexNormal = v.normal.xyz;
			float2 appendResult56 = (float2(_Wisth2 , 1.0));
			float2 appendResult27 = (float2(0.0 , -( _Time.y * _Speed2 )));
			float2 uv2_TexCoord59 = v.texcoord1.xy * appendResult56 + appendResult27;
			float simplePerlin2D22 = snoise( ( uv2_TexCoord59 * _Scale2 ) );
			float smoothstepResult95 = smoothstep( 0.0 , 1.0 , v.texcoord1.xy.y);
			float3 ase_vertex3Pos = v.vertex.xyz;
			float2 appendResult108 = (float2(ase_vertex3Pos.x , ase_vertex3Pos.y));
			float3 appendResult99 = (float3(( -ase_vertex3Pos.x * _CenterShrink * smoothstepResult95 ) , ( -ase_vertex3Pos.y * _CenterShrink * smoothstepResult95 ) , ( ase_vertex3Pos.z * _CenterRise * saturate( ( 1.0 - length( appendResult108 ) ) ) )));
			v.vertex.xyz += ( ( ( ase_vertexNormal * simplePerlin2D22 ) * _VertexOffset * smoothstepResult95 ) + ( smoothstepResult95 * _ConstantOffset ) + appendResult99 );
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 appendResult20 = (float2(_Width , 1.0));
			float2 appendResult6 = (float2(0.0 , -( _Time.y * _Speed )));
			float2 uv_TexCoord3 = i.uv_texcoord * appendResult20 + appendResult6;
			float simplePerlin2D2 = snoise( ( uv_TexCoord3 * _Scale ) );
			float smoothstepResult70 = smoothstep( 0.0 , 1.0 , ( 1.0 - i.uv_texcoord.y ));
			float2 appendResult56 = (float2(_Wisth2 , 1.0));
			float2 appendResult27 = (float2(0.0 , -( _Time.y * _Speed2 )));
			float2 uv2_TexCoord59 = i.uv2_texcoord2 * appendResult56 + appendResult27;
			float simplePerlin2D22 = snoise( ( uv2_TexCoord59 * _Scale2 ) );
			float smoothstepResult87 = smoothstep( 0.0 , 1.0 , ( saturate( ( ( i.uv2_texcoord2.x + -0.7 ) * 4.0 ) ) + saturate( ( 1.0 - ( ( i.uv2_texcoord2.x + -0.05 ) * 4.0 ) ) ) ));
			float smoothstepResult69 = smoothstep( 0.0 , 1.0 , saturate( ( saturate( ( simplePerlin2D2 + smoothstepResult70 + _Brightness ) ) * smoothstepResult70 * saturate( ( saturate( ( 1.0 - ( simplePerlin2D22 * smoothstepResult87 ) ) ) - smoothstepResult87 ) ) ) ));
			float smoothstepResult86 = smoothstep( 0.0 , 1.0 , ( saturate( ( 1.0 - ( ( i.uv2_texcoord2.x * 2.0 ) + -1.0 ) ) ) * saturate( ( i.uv2_texcoord2.x * 2.0 ) ) ));
			float SideGradient88 = smoothstepResult86;
			float temp_output_89_0 = ( smoothstepResult69 * saturate( ( SideGradient88 - _FlameSize ) ) );
			float4 lerpResult61 = lerp( _ColorUp , _ColorDown , temp_output_89_0);
			o.Emission = lerpResult61.rgb;
			o.Alpha = temp_output_89_0;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
1999;72;1906;1010;-474.0449;-1357.79;1.043243;True;False
Node;AmplifyShaderEditor.RangedFloatNode;55;-1931.483,1784.218;Float;False;Constant;_Float2;Float 2;7;0;Create;True;0;0;False;0;-0.05;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-1877.035,1548.479;Float;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;24;-1762.694,1158.162;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-1862.188,1230.884;Float;False;Property;_Speed2;Speed 2;5;0;Create;True;0;0;False;0;3.294118;1.74;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;54;-1573.453,1751.899;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-1751.764,1438.032;Float;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;False;0;-0.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-1584.833,1169.697;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-1498.302,2012.385;Float;False;Constant;_Float3;Float 3;7;0;Create;True;0;0;False;0;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;34;-1366.782,1413.878;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-1306.027,1626.839;Float;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;False;0;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;26;-1452.334,1170.526;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-1275.705,1764.263;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-1483.131,1042.253;Float;False;Property;_Wisth2;Wisth 2;17;0;Create;True;0;0;False;0;1;1.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;56;-1295.17,1043.971;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;44;-1065.251,1769.199;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;4;-1994.7,140.2408;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-2094.193,212.9623;Float;False;Property;_Speed;Speed;6;0;Create;True;0;0;False;0;3.294118;2.39;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;27;-1307.334,1145.935;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-1119.027,1412.839;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;59;-1145.056,1104.316;Float;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;28;-1175.733,1242.426;Float;False;Property;_Scale2;Scale 2;7;0;Create;True;0;0;False;0;5.882353;1.25;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;46;-908.6025,1413.158;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-1816.838,151.7757;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;47;-891.3462,1769.736;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;76;-1499.321,2329.595;Float;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NegateNode;7;-1684.34,152.6052;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-701.1196,1557.147;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-1727.493,29.49221;Float;False;Property;_Width;Width;18;0;Create;True;0;0;False;0;1;20.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-882.7397,1165.098;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;-1226.717,2132.418;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;87;-484.6789,1564.024;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;22;-719.0068,1169.826;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;20;-1540.93,31.2103;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;80;-1000.717,2131.418;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;6;-1539.34,128.0141;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;82;-796.7169,2134.418;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-1233.717,2359.418;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1370.8,288.2;Float;False;Property;_Scale;Scale;8;0;Create;True;0;0;False;0;5.882353;3.68;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;14;-1367.28,362.8405;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-229.7352,1315.419;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-1339.8,19.2;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-1062.8,49.2;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;15;-1089.516,339.1779;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;84;-626.6329,2360.806;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;50;-29.72826,1312.143;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;83;-624.6339,2137.214;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;2;-872.9999,41.4;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;70;-914.7875,341.3868;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-422.7312,2207.075;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;53;176.7121,1323.743;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-1136.372,593.1385;Float;False;Property;_Brightness;Brightness;16;0;Create;True;0;0;False;0;0;0;-3;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;96;813.714,1776.779;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;86;-216.3641,2207.796;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;51;389.1525,1458.248;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;108;1023.834,1657.201;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;32;-566.6184,150.7587;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;30;-413.3332,149.8439;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;52;624.9187,1460.553;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;107;1153.196,1655.116;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;88;14.9444,2202.512;Float;False;SideGradient;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;90;1416.295,459.0239;Float;False;88;SideGradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;93;1248.59,601.1844;Float;False;Property;_FlameSize;Flame Size;12;0;Create;True;0;0;False;0;0;0.322;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;68;1197.565,803.6667;Float;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;109;1273.169,1654.071;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;700.7394,181.8118;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;94;1602.881,540.0994;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;95;1448.595,834.8368;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;98;1394.264,1868.505;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;62;1126.063,1072.162;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;101;1297.162,2213.614;Float;False;Property;_CenterShrink;Center Shrink;14;0;Create;True;0;0;False;0;0;0.69;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;110;1416.094,1710.406;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;103;1112.371,2061.883;Float;False;Property;_CenterRise;Center Rise;15;0;Create;True;0;0;False;0;0;0.167;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;97;1395.264,1804.505;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;11;1384.728,152.2408;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;69;1558.46,147.4883;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;73;1461.307,1568.214;Float;False;Property;_ConstantOffset;Constant Offset;11;0;Create;True;0;0;False;0;0,0,0;0.08,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;1691.102,1765.079;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;1396.91,1154.454;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;92;1725.05,466.7984;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;1261.775,1452.934;Float;False;Property;_VertexOffset;Vertex Offset;13;0;Create;True;0;0;False;0;0;0.148;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;1931.97,2043.382;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;1702.333,1898.554;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;60;1587.408,-91.08131;Float;False;Property;_ColorDown;Color Down;9;1;[HDR];Create;True;0;0;False;0;1,0.3845955,0,0;29.50885,4.634897,2.162952,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;1773.459,1507.264;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;10;1585.135,-263.462;Float;False;Property;_ColorUp;Color Up;10;1;[HDR];Create;True;0;0;False;0;1,0.3845955,0,0;22.62741,0.811169,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;1919.411,145.8265;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;1754.045,1142.377;Float;True;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;99;2133.651,1802.316;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;61;2112.661,-127.4132;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;74;1968.414,1400.771;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2460.948,-44.57562;Float;False;True;6;Float;ASEMaterialInspector;0;0;Unlit;RED_SIM/Fire;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;0;3;2;5;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;5;-1;-1;0;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;54;0;21;1
WireConnection;54;1;55;0
WireConnection;23;0;24;0
WireConnection;23;1;25;0
WireConnection;34;0;21;1
WireConnection;34;1;35;0
WireConnection;26;0;23;0
WireConnection;43;0;54;0
WireConnection;43;1;42;0
WireConnection;56;0;57;0
WireConnection;44;0;43;0
WireConnection;27;1;26;0
WireConnection;38;0;34;0
WireConnection;38;1;39;0
WireConnection;59;0;56;0
WireConnection;59;1;27;0
WireConnection;46;0;38;0
WireConnection;18;0;4;0
WireConnection;18;1;5;0
WireConnection;47;0;44;0
WireConnection;7;0;18;0
WireConnection;48;0;46;0
WireConnection;48;1;47;0
WireConnection;29;0;59;0
WireConnection;29;1;28;0
WireConnection;81;0;76;1
WireConnection;87;0;48;0
WireConnection;22;0;29;0
WireConnection;20;0;19;0
WireConnection;80;0;81;0
WireConnection;6;1;7;0
WireConnection;82;0;80;0
WireConnection;77;0;76;1
WireConnection;49;0;22;0
WireConnection;49;1;87;0
WireConnection;3;0;20;0
WireConnection;3;1;6;0
WireConnection;8;0;3;0
WireConnection;8;1;9;0
WireConnection;15;0;14;2
WireConnection;84;0;77;0
WireConnection;50;0;49;0
WireConnection;83;0;82;0
WireConnection;2;0;8;0
WireConnection;70;0;15;0
WireConnection;85;0;83;0
WireConnection;85;1;84;0
WireConnection;53;0;50;0
WireConnection;86;0;85;0
WireConnection;51;0;53;0
WireConnection;51;1;87;0
WireConnection;108;0;96;1
WireConnection;108;1;96;2
WireConnection;32;0;2;0
WireConnection;32;1;70;0
WireConnection;32;2;17;0
WireConnection;30;0;32;0
WireConnection;52;0;51;0
WireConnection;107;0;108;0
WireConnection;88;0;86;0
WireConnection;109;0;107;0
WireConnection;31;0;30;0
WireConnection;31;1;70;0
WireConnection;31;2;52;0
WireConnection;94;0;90;0
WireConnection;94;1;93;0
WireConnection;95;0;68;2
WireConnection;98;0;96;2
WireConnection;110;0;109;0
WireConnection;97;0;96;1
WireConnection;11;0;31;0
WireConnection;69;0;11;0
WireConnection;104;0;97;0
WireConnection;104;1;101;0
WireConnection;104;2;95;0
WireConnection;63;0;62;0
WireConnection;63;1;22;0
WireConnection;92;0;94;0
WireConnection;102;0;96;3
WireConnection;102;1;103;0
WireConnection;102;2;110;0
WireConnection;105;0;98;0
WireConnection;105;1;101;0
WireConnection;105;2;95;0
WireConnection;75;0;95;0
WireConnection;75;1;73;0
WireConnection;89;0;69;0
WireConnection;89;1;92;0
WireConnection;65;0;63;0
WireConnection;65;1;66;0
WireConnection;65;2;95;0
WireConnection;99;0;104;0
WireConnection;99;1;105;0
WireConnection;99;2;102;0
WireConnection;61;0;10;0
WireConnection;61;1;60;0
WireConnection;61;2;89;0
WireConnection;74;0;65;0
WireConnection;74;1;75;0
WireConnection;74;2;99;0
WireConnection;0;2;61;0
WireConnection;0;9;89;0
WireConnection;0;11;74;0
ASEEND*/
//CHKSM=7E80E5A909A28CE63AAC542A2E13CC78E324BFF5