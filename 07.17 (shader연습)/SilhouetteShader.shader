// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/Silhouette" {
Properties {
    _SilhouetteColor ("SilhouetteColor", Color) = (1, 0, 0, 0.5)
    _Color ("Color", Color) = (1, 1, 1, 1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
    Tags {"RenderType"="Opaque"}

    ZWrite On
    Stencil
    {
        Ref 2
        Pass Replace
    }

    Pass{
        CGPROGRAM
            #pragma surface surf Lambert
            #pragma target 3.0
            fixed4 _Color;
            sampler2D _MainTex;

            struct Input {
                float2 uv_MainTex;
                };

            void surf (Input IN, inout SurfaceOutput o){
                fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Alpha = c.a;
                }
        ENDCG    
    }
    ZWrite Off
    ZTest Greater
    Stencil{
        Ref 2
        Comp NotEqual
        }

    Pass{
        CGPROGRAM
            #pragma surface surf nolight alpha:fade noforwardadd nolightmap noambient novertexlights noshadow
            struct Input{
                float4 color:COLOR;
                };
                float4 _SilhouetteColor;
                void surf (Input IN, inout SurfaceOutput o)
                {
                    o.Emission = _SilhouetteColor.rgb;
                    o.Alpha = _SilhouetteColor.a;
                    }

                float4 Lightingnolight(SurfaceOutput s, float3 lightDir, float atten)
                {
                    return float4(s.Emission, s.Alpha);
                    }
        ENDCG
    }
    FallBack "Diffuse"
}

}
