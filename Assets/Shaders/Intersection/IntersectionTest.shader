Shader "Custom/IntersectionHighlights" {
  Properties {
    _MainTexture ("Main Texture", 2D) = "white" {}
    _EdgeNoise ("Edge Noise", 2D) = "white" {}
    _RegularColor ("Main Color", Color) = (1, 1, 1, 0.5) // Color when not intersecting
    _HighlightColor ("Highlight Color", Color) = (1, 1, 1, 0.5) // Color when intersecting
    _HighlightColorMultiplier ("Highlight Color Multiplier", Float) = 1.0
    _HighlightThresholdMax ("Highlight Threshold Max", Float) = 1 // Max difference for intersections
    _HighlightAnimationSpeed ("Highlight Animation Speed", Float) = 4.0
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
 
      uniform sampler2D_float _CameraDepthTexture; //Depth Texture
      sampler2D _MainTexture;
      sampler2D _EdgeNoise;

      float4 _EdgeNoise_ST;
      float4 _MainTexture_ST;

      half4 _RegularColor;

      half4 _HighlightColor;
      half _HighlightColorMultiplier;
      half _HighlightThresholdMax;
      half _HighlightAnimationSpeed;
 
      struct Input {
        float4 vertex : SV_POSITION;
        float2 uv : TEXCOORD0;
        float2 noiseUV : TEXCOORD1;
        float4 screenPosition : TEXCOORD2;    //Screen position of pos
      };
 
      Input vert(appdata_full v) {
        Input o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.screenPosition = ComputeScreenPos(o.vertex);
        o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTexture);
        o.noiseUV = TRANSFORM_TEX(v.texcoord1.xy, _EdgeNoise);
        return o;
      }
 
      half4 frag(Input i) : COLOR {
        float4 mainTexture = tex2D(_MainTexture, i.uv) * _RegularColor;
        float edgeNoise = clamp(tex2D(_EdgeNoise, i.noiseUV + _Time.x * _HighlightAnimationSpeed).r, 0.0, 1.0);
        float4 finalColor = _RegularColor;
  
        //Get the distance to the camera from the depth buffer for this point
        float sceneZ = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r);

        //Actual distance to the camera
        float partZ = i.screenPosition.w;

        //If the two are similar, then there is an object intersecting with our object
        float diff = abs(sceneZ - partZ) / _HighlightThresholdMax;
        diff = clamp(diff, 0.0, 1.0);
        finalColor = lerp(_HighlightColorMultiplier * _HighlightColor * edgeNoise, mainTexture, half4(diff, diff, diff, diff));

        half4 c = finalColor;
        return c;
      }
      ENDCG
    }
  }
  FallBack "VertexLit"
}