Shader "Custom/Intersection" {
  Properties {
    _MainTexture ("Main Texture", 2D) = "white" {}
    [Toggle]
    _ScreenSpaceTexture("Screen space texture?", Float) = 0
    _EdgeNoise ("Edge Noise", 2D) = "white" {}
    _RegularColor ("Main Color", Color) = (1, 1, 1, 0.5) // Color when not intersecting
    _HighlightColor ("Highlight Color", Color) = (1, 1, 1, 0.5) // Color when intersecting
    _HighlightColorMultiplier ("Highlight Color Multiplier", Float) = 1.0
    _HighlightThresholdMax ("Highlight Threshold Max", Float) = 1 // Max difference for intersections
    _HighlightAnimationSpeed ("Highlight Animation Speed", Float) = 4.0
    _Falloff("Falloff", Range(0.01,10)) = 1.0
    _MagicTiling("Magic Emission Tiling", Range(0,1)) = 0.1
    _MagicEmission("Magic Emission", Float) = 3.0
  }
  SubShader {
    Tags { "Queue" = "Transparent" "RenderType" = "Transparent"  }
 
    Pass {
      Blend SrcAlpha OneMinusSrcAlpha
      ZWrite Off
      Cull Off
      Lighting Off
 
      CGPROGRAM
      #pragma target 3.0
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"
      #include "../Includes/Noise.cginc"
 
      uniform sampler2D_float _CameraDepthTexture; //Depth Texture
      sampler2D _MainTexture;
      fixed _ScreenSpaceTexture;
      sampler2D _EdgeNoise;

      float4 _EdgeNoise_ST;
      float4 _MainTexture_ST;

      half4 _RegularColor;

      half4 _HighlightColor;
      half _HighlightColorMultiplier;
      half _HighlightThresholdMax;
      half _HighlightAnimationSpeed;

      fixed _Falloff;
      fixed _MagicEmission;
      fixed _MagicTiling;
 
      struct Input {
        float4 vertex : SV_POSITION;
        float2 uv : TEXCOORD0;
        float2 noiseUV : TEXCOORD1;
        float4 screenPosition : TEXCOORD2;    //Screen position of pos
        float2 texcoordOld : TEXCOORD3;
      };
 
      Input vert(appdata_full v) {
        Input o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.screenPosition = ComputeScreenPos(o.vertex);
        o.uv = TRANSFORM_TEX(v.texcoord, _MainTexture);
        o.noiseUV = TRANSFORM_TEX(v.texcoord1.xy, _EdgeNoise);
        o.texcoordOld = v.texcoord; //Save texture coordinates without tiling
        return o;
      }
 
      /* Calculates normal uv's or screen space uv's depending on _ScreenSpaceTexture */
      float2 calculateMainUv(Input i) {
        if(_ScreenSpaceTexture < 1.0) {
          return i.uv;
        }
        return i.screenPosition.xy / i.screenPosition.w * float2(_MainTexture_ST.x, _MainTexture_ST.y);
      }

      half4 frag(Input i) : COLOR {
        float4 mainTexture = tex2D(_MainTexture, calculateMainUv(i)) * _RegularColor;
        float edgeNoise = clamp(tex2D(_EdgeNoise, i.noiseUV + _Time.x * _HighlightAnimationSpeed).r, 0.0, 1.0);
        float4 finalColor = _RegularColor;
  
        //Get the distance to the camera from the depth buffer for this point
        float sceneZ = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r);

        //Actual distance to the camera
        float partZ = i.screenPosition.w;

        //If the two are similar, then there is an object intersecting with our object
        float diff = abs(sceneZ - partZ) / _HighlightThresholdMax;
        diff = clamp(diff, 0.0, 1.0);
        finalColor = lerp(_HighlightColorMultiplier * _HighlightColor * edgeNoise, mainTexture, diff);

        //Calculate circular falloff
        fixed d = length(i.texcoordOld.xy - fixed2(0.5,0.5));
				float t = pow(1.0 - min(0.5, d) * 2.0, _Falloff);
				finalColor.a = finalColor.a*t;

        //Work the emission magic in world z direction
        float magic = tex2D(_EdgeNoise, i.noiseUV * partZ * -partZ * _MagicTiling).r;
        finalColor.rgb = lerp(finalColor.rgb, finalColor.rgb * magic * _MagicEmission, diff);

        half4 c = finalColor;
        return c;
      }
      ENDCG
    }
  }
  FallBack "VertexLit"
}