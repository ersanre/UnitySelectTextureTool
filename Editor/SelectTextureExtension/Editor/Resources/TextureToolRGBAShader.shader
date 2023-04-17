// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TextureToolRGBAShader"
{
	Properties
	{
		_MainTexture("MainTexture", 2D) = "white" {}
		_U("U", Float) = 0
		_V("V", Float) = 0
		_CustomValue("CustomValue", Int) = 3

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 2.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform sampler2D _MainTexture;
			uniform float _U;
			uniform float _V;
			uniform int _CustomValue;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float2 texCoord39 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult41 = (float2(_U , _V));
				float4 tex2DNode3 = tex2D( _MainTexture, ( texCoord39 + appendResult41 ) );
				float3 temp_output_16_0 = (tex2DNode3).rgb;
				float temp_output_15_0 = (tex2DNode3).a;
				float3 temp_output_27_0 = ( temp_output_16_0 * temp_output_15_0 );
				float temp_output_60_0 = (temp_output_27_0).x;
				float temp_output_61_0 = (temp_output_27_0).y;
				float lerpResult72 = lerp( temp_output_60_0 , temp_output_61_0 , (float)saturate( _CustomValue ));
				float temp_output_62_0 = (temp_output_27_0).z;
				int temp_output_79_0 = ( _CustomValue - 1 );
				float lerpResult75 = lerp( lerpResult72 , temp_output_62_0 , (float)saturate( temp_output_79_0 ));
				int temp_output_86_0 = ( temp_output_79_0 - 1 );
				float lerpResult76 = lerp( lerpResult75 , temp_output_15_0 , (float)saturate( temp_output_86_0 ));
				int temp_output_88_0 = ( temp_output_86_0 - 1 );
				float lerpResult82 = lerp( lerpResult76 , ( ( temp_output_60_0 + temp_output_61_0 + temp_output_62_0 ) * 0.333 ) , (float)saturate( temp_output_88_0 ));
				int temp_output_90_0 = ( temp_output_88_0 - 1 );
				float lerpResult83 = lerp( lerpResult82 , 0.0 , (float)saturate( temp_output_90_0 ));
				int temp_output_92_0 = ( temp_output_90_0 - 1 );
				float lerpResult84 = lerp( lerpResult83 , 1.0 , (float)saturate( temp_output_92_0 ));
				float temp_output_96_0 = lerpResult84;
				float4 appendResult97 = (float4(temp_output_96_0 , temp_output_96_0 , temp_output_96_0 , 1.0));
				float4 appendResult71 = (float4(temp_output_16_0 , temp_output_15_0));
				float4 lerpResult85 = lerp( appendResult97 , appendResult71 , (float)saturate( ( temp_output_92_0 - 1 ) ));
				
				
				finalColor = lerpResult85;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18800
1920;0;1920;1059;-1246.242;192.3209;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;37;-1442.18,119.0292;Inherit;False;Property;_U;U;1;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-1441.18,219.0292;Inherit;False;Property;_V;V;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;41;-1275.18,114.0292;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;39;-1395.18,-76.97083;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;40;-1142.18,9.029175;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;3;-934,-61.5;Inherit;True;Property;_MainTexture;MainTexture;0;0;Create;True;0;0;0;False;0;False;-1;d587f593f348e5b4f95880452f150dbf;aa762ee7680f35f4fb9a9210fa94ce61;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;16;-768.3434,-740.4649;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;15;-597.7527,-80.96892;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;74;499.9204,-328.8571;Inherit;False;Property;_CustomValue;CustomValue;3;0;Create;True;0;0;0;False;0;False;3;7;False;0;1;INT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-517.3997,-475.9489;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;60;-296.9294,-909.1827;Inherit;False;True;False;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;80;651.489,-375.2181;Inherit;False;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.ComponentMaskNode;61;-253.6507,-707.7579;Inherit;False;False;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;79;667.489,-262.2181;Inherit;False;2;0;INT;0;False;1;INT;1;False;1;INT;0
Node;AmplifyShaderEditor.LerpOp;72;797.9204,-481.8571;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;62;-196.5801,-447.0041;Inherit;False;False;False;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;86;807.2576,-94.7057;Inherit;False;2;0;INT;0;False;1;INT;1;False;1;INT;0
Node;AmplifyShaderEditor.SaturateNode;81;832.489,-261.2181;Inherit;False;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;67;81.48071,-1068.584;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;75;981.9204,-372.8571;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;88;1066.143,28.93091;Inherit;False;2;0;INT;0;False;1;INT;1;False;1;INT;0
Node;AmplifyShaderEditor.SaturateNode;87;972.2576,-113.7058;Inherit;False;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;360.0056,-1069.833;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.333;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;76;1173.92,-188.457;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;89;1231.143,9.930809;Inherit;False;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;90;1271.143,130.9309;Inherit;False;2;0;INT;0;False;1;INT;1;False;1;INT;0
Node;AmplifyShaderEditor.LerpOp;82;1367.52,-95.71371;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;91;1436.143,110.9308;Inherit;False;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;92;1385.143,259.9309;Inherit;False;2;0;INT;0;False;1;INT;1;False;1;INT;0
Node;AmplifyShaderEditor.LerpOp;83;1568.619,32.11499;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;93;1550.143,240.9308;Inherit;False;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.LerpOp;84;1715.105,214.3704;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;96;2043.22,250.517;Inherit;False;True;True;True;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;94;1522.143,533.9309;Inherit;False;2;0;INT;0;False;1;INT;1;False;1;INT;0
Node;AmplifyShaderEditor.SaturateNode;95;1860.143,592.9308;Inherit;False;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.DynamicAppendNode;71;487.112,335.5548;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;97;2296.22,239.517;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-39.97091,-1986.216;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;55;-1001.997,-1980.476;Inherit;False;Constant;_Color2;Color 0;7;0;Create;True;0;0;0;False;0;False;0,0,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;98;-281.1526,347.1389;Inherit;False;Constant;_Color0;Color 0;4;0;Create;True;0;0;0;False;0;False;1,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-52.25203,-1759.889;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;285.8667,-1453.329;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;21.34052,275.1165;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;56;-1011.955,-1746.729;Inherit;False;Constant;_Color3;Color 0;7;0;Create;True;0;0;0;False;0;False;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;85;2480.548,365.5879;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;2758.866,-127.3389;Float;False;True;-1;2;ASEMaterialInspector;100;1;TextureToolRGBAShader;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;2;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;True;0;False;-1;0;False;-1;False;False;False;False;False;False;True;0;False;-1;True;0;False;-1;True;True;True;True;True;0;False;-1;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;False;0
WireConnection;41;0;37;0
WireConnection;41;1;38;0
WireConnection;40;0;39;0
WireConnection;40;1;41;0
WireConnection;3;1;40;0
WireConnection;16;0;3;0
WireConnection;15;0;3;0
WireConnection;27;0;16;0
WireConnection;27;1;15;0
WireConnection;60;0;27;0
WireConnection;80;0;74;0
WireConnection;61;0;27;0
WireConnection;79;0;74;0
WireConnection;72;0;60;0
WireConnection;72;1;61;0
WireConnection;72;2;80;0
WireConnection;62;0;27;0
WireConnection;86;0;79;0
WireConnection;81;0;79;0
WireConnection;67;0;60;0
WireConnection;67;1;61;0
WireConnection;67;2;62;0
WireConnection;75;0;72;0
WireConnection;75;1;62;0
WireConnection;75;2;81;0
WireConnection;88;0;86;0
WireConnection;87;0;86;0
WireConnection;68;0;67;0
WireConnection;76;0;75;0
WireConnection;76;1;15;0
WireConnection;76;2;87;0
WireConnection;89;0;88;0
WireConnection;90;0;88;0
WireConnection;82;0;76;0
WireConnection;82;1;68;0
WireConnection;82;2;89;0
WireConnection;91;0;90;0
WireConnection;92;0;90;0
WireConnection;83;0;82;0
WireConnection;83;2;91;0
WireConnection;93;0;92;0
WireConnection;84;0;83;0
WireConnection;84;2;93;0
WireConnection;96;0;84;0
WireConnection;94;0;92;0
WireConnection;95;0;94;0
WireConnection;71;0;16;0
WireConnection;71;3;15;0
WireConnection;97;0;96;0
WireConnection;97;1;96;0
WireConnection;97;2;96;0
WireConnection;58;0;55;0
WireConnection;59;0;56;0
WireConnection;99;0;16;0
WireConnection;99;1;98;0
WireConnection;85;0;97;0
WireConnection;85;1;71;0
WireConnection;85;2;95;0
WireConnection;0;0;85;0
ASEEND*/
//CHKSM=DBFAA61F55B1AE7B462ECB185E6A137A9C4CB9F9