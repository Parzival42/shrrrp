﻿Shader "Hidden/ColorOverlay"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ColorOverlay ("Color Overlay", Color) = (1, 1, 1, 1)
		_OverlayStrength ("Overlay Strength", Float) = 0.5
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			half4 _ColorOverlay;
			half _OverlayStrength;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				col += _ColorOverlay * _OverlayStrength;

				return col;
			}
			ENDCG
		}
	}
}
