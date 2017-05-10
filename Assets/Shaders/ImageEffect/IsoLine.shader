Shader "Hidden/IsoLine" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1, 1, 1, 1)
		_LineThickness ("Line Thickness", Float) = 0.5
		_LineDirection ("Line Direction", Vector) = (1, 0, 1, 0)
		_WorldSpaceCameraPosition ("World Space Cam Position", Vector) = (0, 0, 0, 0)
	}
	SubShader {
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			uniform sampler2D_float _CameraDepthTexture;
			half4 _Color;
			half _LineThickness;
			half4 _LineDirection;
			float4 _WorldSpaceCameraPosition;

			struct VertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 ray : TEXCOORD1;
			};

			struct FragmentInput {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float2 uvDepth : TEXCOORD1;
				float4 interpolatedRay : TEXCOORD2;
			};

			FragmentInput vert (VertexInput v) {
				FragmentInput o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv.xy;
				o.uvDepth = v.uv.xy;

				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv.y = 1 - o.uv.y;
				#endif				
				
				o.interpolatedRay = v.ray;

				return o;
			}
			
			float GetLinearDepth(FragmentInput i) {
				float rawDepth = DecodeFloatRG(tex2D(_CameraDepthTexture, i.uv));
				return Linear01Depth(rawDepth);
			}

			float3 GetWorldPosition(FragmentInput i) {
				float linearDepth = GetLinearDepth(i);
				float4 wsDirection = linearDepth * i.interpolatedRay;
				float3 wsPosition = _WorldSpaceCameraPosition + wsDirection;
				return wsPosition;
			}

			fixed4 frag (FragmentInput i) : SV_Target {
				fixed4 finalColor = tex2D(_MainTex, i.uv);
				float3 worldPosition = GetWorldPosition(i);

				float dist = distance(worldPosition, _WorldSpaceCameraPosition);
				if(worldPosition.z > 1.0 && worldPosition.z < 2.0)
					finalColor.rgb = float3(1, 0, 0);

				finalColor.rgb = GetLinearDepth(i);
				return finalColor;
			}
			ENDCG
		}
	}
}
