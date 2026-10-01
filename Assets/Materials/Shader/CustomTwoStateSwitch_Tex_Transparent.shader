Shader "Custom/TwoStateSwitch_Tex_Transparent"
{
    Properties
    {
        _TexA ("Textur A", 2D) = "white" {}
        _TexB ("Textur B", 2D) = "white" {}
        _Interval ("Sekunden pro Zustand", Float) = 0.5
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
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float  offset      : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            TEXTURE2D(_TexA);  SAMPLER(sampler_TexA);
            TEXTURE2D(_TexB);  SAMPLER(sampler_TexB);

            CBUFFER_START(UnityPerMaterial)
                float4 _TexA_ST;
                float4 _TexB_ST;
                float  _Interval;
            CBUFFER_END

            float Hash(float3 p)
            {
                p = frac(p * float3(0.1031, 0.1030, 0.0973));
                p += dot(p, p.yxz + 33.33);
                return frac((p.x + p.y) * p.z);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;

                float3 objWorldPos = unity_ObjectToWorld._m03_m13_m23;
                OUT.offset = Hash(objWorldPos);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                float phase = _Time.y / max(_Interval, 0.0001) + IN.offset * 2.0;
                float state = fmod(floor(phase), 2.0); // 0 oder 1, harter Schnitt

                float2 uvA = IN.uv * _TexA_ST.xy + _TexA_ST.zw;
                float2 uvB = IN.uv * _TexB_ST.xy + _TexB_ST.zw;

                half4 a = SAMPLE_TEXTURE2D(_TexA, sampler_TexA, uvA);
                half4 b = SAMPLE_TEXTURE2D(_TexB, sampler_TexB, uvB);

                half4 col = lerp(a, b, state); // inkl. Alpha-Kanal
                return col;
            }
            ENDHLSL
        }
    }
}