Shader "Hidden/SimpleSlice" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_SliceThickness ("Slice Thickness", Float) = 0.5
		_SliceDirection ("Slice Direction", Vector) = (1, 0, 1, 0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		half _SliceThickness;
		half4 _SliceDirection;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half3 direction = IN.worldPos.xyz * _SliceDirection.xyz;
			clip (frac((direction.x + direction.y + direction.z) * 5) - _SliceThickness);

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
