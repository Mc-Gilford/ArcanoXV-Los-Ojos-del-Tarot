Shader "Custom/RainShader"
{
    Properties
    {
        _MainTex ("Color (RGB) Alpha (A)", 2D) = "gray" {}
        _TintColor ("Tint Color (RGB)", Color) = (1, 1, 1, 1)
        _PointSpotLightMultiplier ("Point/Spot Light Multiplier", Range(0, 10)) = 2
        _DirectionalLightMultiplier ("Directional Light Multiplier", Range(0, 10)) = 1
        _InvFade ("Soft Particles Factor", Range(0.01, 100.0)) = 1.0
        _AmbientLightMultiplier ("Ambient light multiplier", Range(0, 1)) = 0.25
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "RainPass"
            Tags
            {
                "LightMode"="UniversalForward"
            }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_particles

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)

                float4 _MainTex_ST;
                float4 _TintColor;

                float _PointSpotLightMultiplier;
                float _DirectionalLightMultiplier;
                float _AmbientLightMultiplier;
                float _InvFade;

            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                output.positionHCS =
                    TransformObjectToHClip(input.positionOS.xyz);

                output.uv =
                    TRANSFORM_TEX(input.uv, _MainTex);

                output.color =
                    input.color * _TintColor;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 tex =
                    SAMPLE_TEXTURE2D(
                        _MainTex,
                        sampler_MainTex,
                        input.uv
                    );

                return tex * input.color;
            }

            ENDHLSL
        }
    }
}