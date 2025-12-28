// Made with Amplify Shader Editor v1.9.7.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VRCCoffee/Iced_Espresso"
{
	Properties
	{
		_Amount("Amount", Range( 0 , 1)) = 1
		_AmountCap("AmountCap", Vector) = (-6.4389,6.59976,0,0)
		_TopTexture("Top Texture", 2D) = "white" {}
		_ColorBase("Color Base", Color) = (0,0,0,0)
		_ColorBaseMix("Color Base Mix", Color) = (0,0,0,0)
		_Mixpos("Mix pos", Float) = 0
		_MixGrad("Mix Grad", Float) = 0
		_MixNoisePower("Mix Noise Power", Float) = 0
		_MixNoiseScale("Mix Noise Scale", Float) = 0
		[Header(Rim)]_ColorRim1("Color Rim 1", Color) = (0,0,0,0)
		_ColorRim2("Color Rim 2", Color) = (0,0,0,0)
		_RimNoiseScale("Rim Noise Scale", Float) = 0
		_RimNoiseStrength("Rim Noise Strength", Float) = 0
		_RimThickness("Rim Thickness", Float) = 0
		_RimGrad("Rim Grad", Float) = 0
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#define ASE_VERSION 19701
		#pragma surface surf Standard keepalpha noshadow 
		struct Input
		{
			float3 worldPos;
			float2 uv2_texcoord2;
			half3 worldNormal;
		};

		uniform half4 _ColorBase;
		uniform half4 _ColorBaseMix;
		uniform half _MixGrad;
		uniform half _MixNoiseScale;
		uniform half _MixNoisePower;
		uniform half2 _AmountCap;
		uniform half _Amount;
		uniform half _Mixpos;
		uniform half4 _ColorRim1;
		uniform half4 _ColorRim2;
		uniform half _RimNoiseScale;
		uniform half _RimNoiseStrength;
		uniform half _RimGrad;
		uniform half _RimThickness;
		uniform sampler2D _TopTexture;
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
			half temp_output_50_0 = ( _MixGrad / 100.0 );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			half simplePerlin3D57 = snoise( ase_vertex3Pos*_MixNoiseScale );
			simplePerlin3D57 = simplePerlin3D57*0.5 + 0.5;
			half2 break132 = ( _AmountCap / float2( 100,100 ) );
			half lerpResult133 = lerp( break132.x , break132.y , _Amount);
			half Amount_Y79 = lerpResult133;
			half smoothstepResult54 = smoothstep( ( temp_output_50_0 * -1.0 ) , temp_output_50_0 , ( ( ase_vertex3Pos.y + ( simplePerlin3D57 * ( _MixNoisePower / 100.0 ) ) ) - ( Amount_Y79 - ( _Mixpos / 100.0 ) ) ));
			half4 lerpResult56 = lerp( _ColorBase , _ColorBaseMix , smoothstepResult54);
			half simplePerlin3D27 = snoise( ase_vertex3Pos*_RimNoiseScale );
			simplePerlin3D27 = simplePerlin3D27*0.5 + 0.5;
			half4 lerpResult30 = lerp( _ColorRim1 , _ColorRim2 , ( simplePerlin3D27 * _RimNoiseStrength ));
			half temp_output_95_0 = ( _RimGrad / 100.0 );
			half smoothstepResult15 = smoothstep( ( temp_output_95_0 * -1.0 ) , temp_output_95_0 , ( ase_vertex3Pos.y - ( Amount_Y79 - ( _RimThickness / 100.0 ) ) ));
			half4 lerpResult21 = lerp( lerpResult56 , lerpResult30 , smoothstepResult15);
			half3 ase_worldNormal = i.worldNormal;
			half3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			ase_vertexNormal = normalize( ase_vertexNormal );
			half4 lerpResult104 = lerp( lerpResult21 , tex2D( _TopTexture, i.uv2_texcoord2 ) , ( ase_vertexNormal.y >= 0.999 ? 1.0 : 0.0 ));
			o.Albedo = lerpResult104.rgb;
			o.Alpha = 1;
			clip( ( _Amount == 0.0 ? 0.0 : ( ase_vertexNormal.y >= 0.999 ? 1.0 : step( ase_vertex3Pos.y , lerpResult133 ) ) ) - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19701
Node;AmplifyShaderEditor.CommentaryNode;128;-1104,992;Inherit;False;1190;461.0004;Amount;11;139;138;137;136;135;134;133;132;131;130;129;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;129;-1056,1248;Inherit;False;Property;_AmountCap;AmountCap;1;0;Create;True;0;0;0;False;0;False;-6.4389,6.59976;-6.4389,6.59976;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleDivideOpNode;130;-848,1248;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;100,100;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;115;-2994,-994;Inherit;False;1412;683;Mix Noise;17;112;59;58;113;57;47;111;16;102;84;52;110;103;50;107;106;54;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;131;-880,1344;Inherit;False;Property;_Amount;Amount;0;0;Create;True;0;0;0;False;0;False;1;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;132;-736,1248;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;112;-2864,-656;Inherit;False;Property;_MixNoisePower;Mix Noise Power;7;0;Create;True;0;0;0;False;0;False;0;1.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-2944,-736;Inherit;False;Property;_MixNoiseScale;Mix Noise Scale;8;0;Create;True;0;0;0;False;0;False;0;51;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;58;-2944,-880;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;133;-624,1248;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;116;-2544,-288;Inherit;False;964;507;Rim Noise;10;9;94;81;101;96;97;15;12;95;100;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;113;-2640,-656;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;57;-2752,-832;Inherit;False;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-2560,-544;Inherit;False;Property;_Mixpos;Mix pos;5;0;Create;True;0;0;0;False;0;False;0;0.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;-416,1504;Inherit;False;Amount Y;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;-2512,-768;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;16;-2528,-944;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;102;-2480,-624;Inherit;False;79;Amount Y;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;84;-2416,-544;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-2288,-448;Inherit;False;Property;_MixGrad;Mix Grad;6;0;Create;True;0;0;0;False;0;False;0;1.13;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-2496,-32;Inherit;False;Property;_RimThickness;Rim Thickness;13;0;Create;True;0;0;0;False;0;False;0;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;110;-2304,-848;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;103;-2288,-592;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;50;-2096,-448;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;94;-2304,-32;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-2368,-112;Inherit;False;79;Amount Y;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-2272,80;Inherit;False;Property;_RimGrad;Rim Grad;14;0;Create;True;0;0;0;False;0;False;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;26;-1520,480;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;28;-1520,624;Inherit;False;Property;_RimNoiseScale;Rim Noise Scale;11;0;Create;True;0;0;0;False;0;False;0;1200;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;107;-2112,-736;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-1968,-528;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;101;-2144,-96;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;96;-2176,-240;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;95;-2112,80;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;27;-1312,480;Inherit;False;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-1312,576;Inherit;False;Property;_RimNoiseStrength;Rim Noise Strength;12;0;Create;True;0;0;0;False;0;False;0;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;54;-1792,-544;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;97;-1968,-160;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-1968,-48;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;22;-1152,-320;Inherit;False;Property;_ColorBase;Color Base;3;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;33;-1152,-112;Inherit;False;Property;_ColorBaseMix;Color Base Mix;4;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.2429999,0.08477675,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-1088,480;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;29;-1152,288;Inherit;False;Property;_ColorRim2;Color Rim 2;10;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.4769999,0.3129479,0.1965239,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;5;-1152,96;Inherit;False;Property;_ColorRim1;Color Rim 1;9;1;[Header];Create;True;1;Rim;0;0;False;0;False;0,0,0,0;0.5490196,0.4039215,0.262745,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.PosVertexDataNode;134;-640,1104;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;15;-1792,-80;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;56;-832,-176;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;30;-880,256;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;78;-720,560;Inherit;False;1;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;136;-464,1184;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;135;-464,1040;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalVertexDataNode;141;-592,752;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;21;-592,64;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;3;-512,560;Inherit;True;Property;_TopTexture;Top Texture;2;0;Create;True;0;0;0;False;0;False;-1;None;d99c21ecee8df0c4cb1c4fc0f0e687b9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.WireNode;138;-160,1232;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Compare;137;-272,1088;Inherit;False;3;4;0;FLOAT;0;False;1;FLOAT;0.999;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Compare;140;-384,752;Inherit;False;3;4;0;FLOAT;0;False;1;FLOAT;0.999;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;104;-144,560;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Compare;139;-96,1088;Inherit;False;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;338.725,648.4763;Half;False;True;-1;2;ASEMaterialInspector;0;0;Standard;VRCCoffee/Iced_Espresso;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;False;0;True;Opaque;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;2;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;15;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;130;0;129;0
WireConnection;132;0;130;0
WireConnection;133;0;132;0
WireConnection;133;1;132;1
WireConnection;133;2;131;0
WireConnection;113;0;112;0
WireConnection;57;0;58;0
WireConnection;57;1;59;0
WireConnection;79;0;133;0
WireConnection;111;0;57;0
WireConnection;111;1;113;0
WireConnection;84;0;47;0
WireConnection;110;0;16;2
WireConnection;110;1;111;0
WireConnection;103;0;102;0
WireConnection;103;1;84;0
WireConnection;50;0;52;0
WireConnection;94;0;9;0
WireConnection;107;0;110;0
WireConnection;107;1;103;0
WireConnection;106;0;50;0
WireConnection;101;0;81;0
WireConnection;101;1;94;0
WireConnection;95;0;12;0
WireConnection;27;0;26;0
WireConnection;27;1;28;0
WireConnection;54;0;107;0
WireConnection;54;1;106;0
WireConnection;54;2;50;0
WireConnection;97;0;96;2
WireConnection;97;1;101;0
WireConnection;100;0;95;0
WireConnection;31;0;27;0
WireConnection;31;1;32;0
WireConnection;15;0;97;0
WireConnection;15;1;100;0
WireConnection;15;2;95;0
WireConnection;56;0;22;0
WireConnection;56;1;33;0
WireConnection;56;2;54;0
WireConnection;30;0;5;0
WireConnection;30;1;29;0
WireConnection;30;2;31;0
WireConnection;136;0;134;2
WireConnection;136;1;133;0
WireConnection;21;0;56;0
WireConnection;21;1;30;0
WireConnection;21;2;15;0
WireConnection;3;1;78;0
WireConnection;138;0;131;0
WireConnection;137;0;135;2
WireConnection;137;3;136;0
WireConnection;140;0;141;2
WireConnection;104;0;21;0
WireConnection;104;1;3;0
WireConnection;104;2;140;0
WireConnection;139;0;138;0
WireConnection;139;3;137;0
WireConnection;0;0;104;0
WireConnection;0;10;139;0
ASEEND*/
//CHKSM=D8501D88A1819FE227E19841DC071A34A3EA2A6F