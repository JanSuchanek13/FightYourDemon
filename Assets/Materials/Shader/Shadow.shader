Shader "Custom/HatchOverlay"
{
    Properties
    {
        _InkColor ("Strichfarbe", Color) = (0.15, 0.12, 0.1, 1)
        _Opacity ("Deckkraft", Range(0,1)) = 0.85

        [Header(Schatten Richtung)]
        _ShadowDir ("Schattenrichtung (XYZ)", Vector) = (0, -1, 0, 0)
        _Threshold ("Schatten-Grenze", Range(-1, 1)) = 0.1
        _Softness ("Grenz-Weichheit", Range(0.001, 0.5)) = 0.1

        [Header(Schraffur)]
        _HatchScale ("Strich-Dichte", Float) = 0.15
        _HatchDensity ("Strich-Anteil", Range(0.05, 0.95)) = 0.5
        _HatchJitter ("Strich-Unregelmaessigkeit", Range(0, 30)) = 10.0
        _Boil ("Wackeln (0 = aus)", Range(0, 1)) = 0.0
        _BoilFps ("Wackel-Tempo", Float) = 8.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "HatchOverlay"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _InkColor;
                float  _Opacity;
                float4 _ShadowDir;
                float  _Threshold;
                float  _Softness;
                float  _HatchScale;
                float  _HatchDensity;
                float  _HatchJitter;
                float  _Boil;
                float  _BoilFps;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.normalWS = normalize(TransformObjectToWorldNormal(IN.normalOS));
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                float3 n = normalize(IN.normalWS);

                // --- Schatten-Maske anhand frei waehlbarer Richtung ---
                // Flaechen, deren Normale der Schattenrichtung entgegenzeigt, sind "im Schatten"
                float3 dir = normalize(_ShadowDir.xyz);
                float shade = dot(n, -dir); // 1 = Normale zeigt genau gegen die Richtung
                float shadowMask = smoothstep(_Threshold - _Softness,
                                              _Threshold + _Softness, shade);

                // --- Schraffur in Screen-Space ---
                float2 sc = IN.positionHCS.xy;

                float2 boilOffset = 0;
                if (_Boil > 0.001)
                {
                    float t = floor(_Time.y * _BoilFps);
                    boilOffset = float2(frac(sin(t * 12.9) * 43758.5),
                                        frac(sin(t * 78.2) * 43758.5)) * 20.0 * _Boil;
                }
                sc += boilOffset;

                float wobble = sin(sc.y * 0.08) * _HatchJitter
                             + sin(sc.x * 0.05) * _HatchJitter * 0.5;
                float diag = (sc.x + sc.y + wobble) * _HatchScale;
                float band = frac(diag);
                float hatch = 1.0 - smoothstep(_HatchDensity, _HatchDensity + 0.15, band);

                // Alpha = Striche, nur im Schattenbereich, mal Deckkraft
                float ink = hatch * shadowMask * _Opacity;

                return half4(_InkColor.rgb, ink);
            }
            ENDHLSL
        }
    }
}