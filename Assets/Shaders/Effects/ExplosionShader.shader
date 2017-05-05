Shader "Shrrrp/VertexNoiseOffset" {
	Properties {
		_RampTex ("Ramp Color", 2D) = "white" {}
		_RampOffset ("Ramp Offset", Range(-0.5, 0.5)) = 0

		_NoiseTex ("Noise Texture", 2D) = "gray" {}

		[Toggle] _Animate ("Autom. Animation", Float) = 0
		_Period ("Period", Range(0, 10)) = 0.5

		_Amount ("Amount", Range(0, 5)) = 0.1
		_ClipRange ("Clip Range", Range(0, 1)) = 1
		_MovementSpeed ("Movement Speed", Range(0, 4)) = 1

		_EmissionStrength ("Emission Strength", Float) = 0
	}
	SubShader {
		Tags
		{
			 "RenderType"="Opaque"
			 //"ForceNoShadowCasting"="True"
		}
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard vertex:vert addshadow
		#pragma target 3.0

		sampler2D _RampTex;
		sampler2D _NoiseTex;

		half _RampOffset;
		float _Period;
		half _Amount;
		half _ClipRange;
		float _MovementSpeed;
		float _Animate;

		half _EmissionStrength;

		struct Input {
			float2 uv_NoiseTex;
			float3 localPos;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float3 noise = tex2D(_NoiseTex, IN.uv_NoiseTex + _Time[0] * _MovementSpeed);
			float n = saturate(noise.r + _RampOffset);

			clip(_ClipRange - n);
			half4 c = tex2D(_RampTex, float2(n, 0.5));

			o.Albedo = c.rgb;
			o.Emission = c.rgb * _EmissionStrength;
		}

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);

			float3 displacement = tex2Dlod(_NoiseTex, float4(v.texcoord.xy + _Time[0] * _MovementSpeed, 0, 0));
			float time = 1;
			// Sin -> To animate thing going up and down
			//float time = sin(_Time[3] * _Period + displacement.r * 10);
			if(_Animate)
				time = sin(_Time[3] * _Period + (displacement.r * displacement.g * displacement.b) * 1);
			else
				time = sin(_Period + (displacement.r * displacement.g * displacement.b) * 1);

			v.vertex.xyz += v.normal * displacement.r * _Amount * time;
			o.localPos = v.vertex.xyz;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
