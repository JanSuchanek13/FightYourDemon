Shader "Custom/ProceduralCracksTransparent"
{
    Properties
    {
        _CrackColor ("Rissfarbe", Color) = (0, 0, 0, 1)
        _Scale ("Riss-Dichte", Float) = 8.0
        _CrackWidth ("Riss-Breite", Range(0.0, 0.5)) = 0.06
        _Jitter ("Unregelmaessigkeit", Range(0.0, 1.0)) = 1.0
        _CrackAmount ("Riss-Grad", Range(0.0, 1.0)) = 0.3
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
            Name "Cracks"
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _CrackColor;
                float  _Scale;
                float  _CrackWidth;
                float  _Jitter;
                float  _CrackAmount;
            CBUFFER_END

            float2 Hash2(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)),
                           dot(p, float2(269.5, 183.3)));
                return frac(sin(p) * 43758.5453);
            }

            // Weiches Value-Noise fuer das Gate (0..1)
            float ValueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                float a = Hash2(i).x;
                float b = Hash2(i + float2(1, 0)).x;
                float c = Hash2(i + float2(0, 1)).x;
                float d = Hash2(i + float2(1, 1)).x;
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            // Voronoi: Abstand zur naechsten Zellkante
            float VoronoiEdge(float2 uv)
            {
                float2 cell = floor(uv);
                float2 f    = frac(uv);

                float f1 = 8.0;
                float2 nearestOffset = 0;
                float2 nearestPoint  = 0;

                for (int y = -1; y <= 1; y++)
                for (int x = -1; x <= 1; x++)
                {
                    float2 o = float2(x, y);
                    float2 p = o + Hash2(cell + o) * _Jitter;
                    float d = dot(p - f, p - f);
                    if (d < f1)
                    {
                        f1 = d;
                        nearestOffset = o;
                        nearestPoint  = p;
                    }
                }

                float edge = 8.0;
                for (int y2 = -2; y2 <= 2; y2++)
                for (int x2 = -2; x2 <= 2; x2++)
                {
                    float2 o = nearestOffset + float2(x2, y2);
                    float2 p = o + Hash2(cell + o) * _Jitter;
                    float2 diff = p - nearestPoint;
                    if (dot(diff, diff) > 0.00001)
                    {
                        float d = dot(f - 0.5 * (nearestPoint + p), normalize(diff));
                        edge = min(edge, abs(d));
                    }
                }
                return edge;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                float2 suv = IN.uv * _Scale;

                // Risslinie aus dem Voronoi-Kantenabstand
                float edge = VoronoiEdge(suv);
                // bei niedrigem Riss-Grad die Linie zusaetzlich verduennen (Haarrisse)
                float width = _CrackWidth * lerp(0.35, 1.0, _CrackAmount);
                float crackLine = 1.0 - smoothstep(0.0, width, edge);

                // Gate: nur dort Risse, wo das Rauschfeld unter dem Riss-Grad liegt.
                // Mit steigendem _CrackAmount waechst die gerissene Flaeche.
                float gateNoise = ValueNoise(suv * 0.5);
                float gate = smoothstep(gateNoise - 0.15, gateNoise + 0.15, _CrackAmount);

                float crack = crackLine * gate;

                // Alpha = Rissmaske -> Rest bleibt durchsichtig
                return half4(_CrackColor.rgb, crack * _CrackColor.a);
            }
            ENDHLSL
        }
    }
}