Shader "Custom/Dissolve" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallicmap", 2D) = "white" {}
		_BumpScale("Normal", Float) = 1.0
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_DissolvePercentage ("Dissolve Percentage", Range(0,1)) = 0.0
		_DissolveScale ("Dissolve Noise Scale", Float) = 1.0
		_DissolveGlow ("Dissolve Glow Scale", Float) = 0.1
		_DissolveEmission ("Dissolve Emission Scale", Float) = 3.0
		_DissolveEmissionColor ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows addshadow
		#include "../Includes/Noise.cginc"

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _MetallicGlossMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		half _BumpScale;
		fixed4 _Color;
		fixed4 _DissolveEmissionColor;
		half _DissolvePercentage;
		half _DissolveScale;
		half _DissolveGlow;
		half _DissolveEmission;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;

			// Metallic and smoothness come from slider variables
			o.Metallic = tex2D (_MetallicGlossMap, IN.uv_MainTex) * _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			float3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			normal.xz *= _BumpScale;
			o.Normal = normal;

			// Dissolve object
			half dissolveValue = (snoise(IN.worldPos * _DissolveScale) + 1.0) / 2.0 - _DissolvePercentage;
			clip(dissolveValue);

			// Add glowing dissolve seams
			if(dissolveValue < _DissolveGlow){
				o.Emission = _DissolveEmissionColor * _DissolveEmission;
			}
			
		}
		ENDCG
	}
	FallBack "Diffuse"
}
