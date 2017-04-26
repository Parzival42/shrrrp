Shader "Custom/IntersectionHighlights" {
  Properties {
    _RegularColor("Main Color", Color) = (1, 1, 1, 0.5) //Color when not intersecting
    _HighlightColor("Highlight Color", Color) = (1, 1, 1, 0.5) //Color when intersecting
    _HighlightThresholdMax("Highlight Threshold Max", Float) = 1 //Max difference for intersections
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
      uniform float4 _RegularColor;
      uniform float4 _HighlightColor;
      uniform float _HighlightThresholdMax;
 
      struct Input {
        float4 vertex : SV_POSITION;
        float4 screenPosition : TEXCOORD0;    //Screen position of pos
      };
 
      Input vert(appdata_base v) {
        Input o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.screenPosition = ComputeScreenPos(o.vertex);
 
        return o;
      }
 
      half4 frag(Input i) : COLOR {
        float4 finalColor = _RegularColor;
  
        //Get the distance to the camera from the depth buffer for this point
        float sceneZ = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r);

        //Actual distance to the camera
        float partZ = i.screenPosition.w;

        //If the two are similar, then there is an object intersecting with our object
        float diff = abs(sceneZ - partZ) / _HighlightThresholdMax;
        diff = clamp(diff, 0.0, 1.0);
        finalColor = lerp(_HighlightColor,  _RegularColor, float4(diff, diff, diff, diff));

        // if(diff < _HighlightThresholdMax) {
        //   finalColor = _HighlightColor;
        // }
 
        half4 c = finalColor;
        return c;
      }
      ENDCG
    }
  }
  FallBack "VertexLit"
}