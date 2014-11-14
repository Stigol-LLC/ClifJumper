		Shader "Custom/SCrack"
		 {
		     Properties
		     {
		         [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		         _Color ("Tint", Color) = (1,1,1,1)
		         [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		     }
		 
		     SubShader
		     {
		         Tags
		         {
		           //  "Queue"="Transparent" //+
					 "Queue"="AlphaTest"

		             "IgnoreProjector"="True"
		             "RenderType"="Transparent"
		             "PreviewType"="Plane"
		             "CanUseSpriteAtlas"="True"
		         }
		 
				ColorMask 0
		         Cull Off
		         Lighting Off
		         ZWrite Off
		         Fog { Mode Off }
		         Blend One OneMinusSrcAlpha
		 
		         Pass
		         {
		             Stencil
		             {
		              
						
						Ref 50
						Comp Always
						Pass Replace
		             }
		   
		         CGPROGRAM
		             #pragma vertex vert
		             #pragma fragment frag
		             #pragma multi_compile DUMMY PIXELSNAP_ON
		             #include "UnityCG.cginc"
		       
		             struct appdata_t
		             {
		                 float4 vertex   : POSITION;
		                 float4 color    : COLOR;
		                 float2 texcoord : TEXCOORD0;
		             };
		 
		             struct v2f
		             {
		                 float4 vertex   : SV_POSITION;
		                 fixed4 color    : COLOR;
		                 half2 texcoord  : TEXCOORD0;
		             };
		       
		             fixed4 _Color;
		 
		             v2f vert(appdata_t IN)
		             {
		                 v2f OUT;
		                 OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
						 OUT.color = float4(0,0,0,0);
					
		                 #ifdef PIXELSNAP_ON
		                 OUT.vertex = UnityPixelSnap (OUT.vertex);
		                 #endif
		 
		                 return OUT;
		             }
		 
		             sampler2D _MainTex;
		 
		             fixed4 frag(v2f IN) : SV_Target
		             {
		                // fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
		                // c.rgb *= c.a;
						// c.a = 0;
		                 return IN.color;
		             }
		         ENDCG
		         }
		     }
		 }
