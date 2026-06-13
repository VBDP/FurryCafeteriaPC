// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VRCCoffee/Cafe LiquidMask"
{
	Properties
	{
		_Height("Height", Range( 0 , 1)) = 0
		_MinMax("MinMax", Vector) = (0,0,0,0)
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "AlphaTest-2" "IgnoreProjector" = "True" }
		Cull Off
		ZWrite On
		Stencil
		{
			Ref 174
			CompFront Never
			FailFront Replace
			CompBack Never
			FailBack Replace
		}
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha noshadow 
		struct Input
		{
			float3 worldPos;
		};

		uniform float2 _MinMax;
		uniform float _Height;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Alpha = 0.0;
			float3 ase_worldPos = i.worldPos;
			float3 objToWorld44 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
			float2 break43 = ( _MinMax / ( 100.0 / ase_objectScale.y ) );
			float lerpResult47 = lerp( break43.x , break43.y , _Height);
			clip( step( ( ase_worldPos.y - objToWorld44.y ) , lerpResult47 ) - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
0;73;1400;808;1130.938;161.1021;1;True;False
Node;AmplifyShaderEditor.ObjectScaleNode;51;-899.2714,471.2499;Inherit;False;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector2Node;40;-893.3864,347.9112;Inherit;False;Property;_MinMax;MinMax;1;0;Create;True;0;0;0;False;0;False;0,0;-4.91,4.91;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleDivideOpNode;50;-720.2714,438.2499;Inherit;False;2;0;FLOAT;100;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;41;-574.4274,353.2392;Inherit;False;2;0;FLOAT2;500,0;False;1;FLOAT;100;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;43;-464.5872,353.5813;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.WorldPosInputsNode;42;-553.1614,42.59731;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;45;-679.1263,553.3176;Inherit;False;Property;_Height;Height;0;0;Create;True;0;0;0;False;0;False;0;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TransformPositionNode;44;-553.1614,183.5975;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;46;-323.8029,144.4366;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;47;-348.781,354.2505;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-160.6544,135.6624;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;49;-149.0032,253.8879;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-1.949157,-65.86899;Inherit;False;Property;_Layer;Layer;2;0;Create;True;0;0;0;False;0;False;0;0;0;255;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-6,12;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;VRCCoffee/Cafe LiquidMask;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;1;False;-1;0;False;-1;False;10;False;-1;0;False;-1;False;0;Custom;0.5;True;False;-2;True;Transparent;;AlphaTest;All;18;all;True;True;True;True;0;False;-1;True;174;False;53;255;False;-1;255;False;-1;8;False;-1;0;False;-1;3;False;-1;0;False;-1;8;False;-1;0;False;-1;3;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;3;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;50;1;51;2
WireConnection;41;0;40;0
WireConnection;41;1;50;0
WireConnection;43;0;41;0
WireConnection;46;0;42;2
WireConnection;46;1;44;2
WireConnection;47;0;43;0
WireConnection;47;1;43;1
WireConnection;47;2;45;0
WireConnection;49;0;46;0
WireConnection;49;1;47;0
WireConnection;0;9;70;0
WireConnection;0;10;49;0
ASEEND*/
//CHKSM=5080024FBBA26FDAFBD79C91F64C5262F62BAE08