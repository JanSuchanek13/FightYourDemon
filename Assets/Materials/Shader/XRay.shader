Shader "Custom/XRayJitter"
{
    Properties
    {
        _XRayColor ("Durchscheinfarbe", Color) = (0.2, 0.8, 1, 1)
        _XRayIntensity ("Durchschein-Staerke", Range(0,3)) = 1.0

        [Header(Jitter)]
        _JitterAmount ("Jitter-Staerke", Range(0, 0.2)) = 0.03
        _JitterFps ("Jitter-Tempo (Spruenge pro Sek)", Float) = 12.0
        _JitterChance ("Jitter-Haeufigkeit", Range(0, 1)) = 0.7
        _SliceAmount ("Zeilen-Versatz (Glitch)", Range(0, 0.2)) = 0.02
        _SliceScale ("Zeilen-Dichte", Float) = 20.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+100" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "XRayJitter"
            ZTest Greater
            ZWrite Off
            Blend One One
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _XRayColor;
                float  _XRayIntensity;
                float  _JitterAmount;
                float  _JitterFps;
                float  _JitterChance;
                float  _SliceAmount;
                float  _SliceScale;
            CBUFFER_END

            float hash11(float x)
            {
                return frac(sin(x * 12.9898) * 43758.5453);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);

                // Zeit in feste Spruenge quantisieren -> ruckartig, nicht fluessig
                float step = floor(_Time.y * _JitterFps);

                // Objekt-Seed, damit die drei Gegner unterschiedlich zappeln
                float3 objPos = unity_ObjectToWorld._m03_m13_m23;
                float objSeed = frac((objPos.x + objPos.y * 1.7 + objPos.z * 3.1) * 0.37);

                // ganzes Objekt pro Zeitschritt zufaellig versetzen
                float rx = hash11(step + objSeed * 91.0) * 2.0 - 1.0;
                float ry = hash11(step + objSeed * 57.0 + 5.0) * 2.0 - 1.0;

                // nur manchmal springen (sonst kurz "ruhig") -> nervoeser Glitch
                float active = step < 0.0 ? 0.0 : (hash11(step + objSeed * 13.0) < _JitterChance ? 1.0 : 0.0);

                float4 clip = TransformObjectToHClip(IN.positionOS.xyz);

                // globaler Versatz des ganzen Objekts (Bildschirmebene, Tiefe bleibt)
                clip.xy += float2(rx, ry) * _JitterAmount * active * clip.w;

                // zusaetzlicher zeilenweiser Versatz -> horizontales "Tearing"
                float sliceRow = floor(IN.positionOS.y * _SliceScale);
                float slice = hash11(sliceRow + step) * 2.0 - 1.0;
                clip.x += slice * _SliceAmount * active * clip.w;

                OUT.positionHCS = clip;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                return half4(_XRayColor.rgb * _XRayIntensity, 1);
            }
            ENDHLSL
        }
    }
}