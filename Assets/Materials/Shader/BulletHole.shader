Shader "Custom/BulletHole"
{
    Properties
    {
        _HoleColor ("Lochfarbe", Color) = (0.02, 0.02, 0.02, 1)
        _RingColor ("Randfarbe (Verbrennung)", Color) = (0.1, 0.08, 0.06, 1)
        _HoleSize ("Lochgroesse", Range(0.0, 1.0)) = 0.35
        _CrackAmount ("Riss-Laenge", Range(0.0, 1.0)) = 0.5
        _CrackCount ("Riss-Anzahl", Float) = 9
        _Edge ("Kantenweichheit", Range(0.001, 0.3)) = 0.06
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
            Name "BulletHole"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Back
            Offset -1, -1   // leicht nach vorne, gegen Z-Fighting mit der Oberflaeche

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
                float  seed        : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _HoleColor;
                float4 _RingColor;
                float  _HoleSize;
                float  _CrackAmount;
                float  _CrackCount;
                float  _Edge;
            CBUFFER_END

            float Hash(float x)
            {
                return frac(sin(x * 12.9898) * 43758.5453);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;

                // pro Quad ein anderer Seed aus der Weltposition -> jedes Loch anders
                float3 wp = unity_ObjectToWorld._m03_m13_m23;
                OUT.seed = frac((wp.x + wp.y * 1.7 + wp.z * 3.1) * 0.371);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                // zentrierte Koordinaten: Mitte = 0, Rand = ~1
                float2 c = (IN.uv - 0.5) * 2.0;
                float  r = length(c);
                float  ang = atan2(c.y, c.x); // -PI..PI

                // --- zentrales Loch ---
                float hole = 1.0 - smoothstep(_HoleSize, _HoleSize + _Edge, r);

                // --- radiale Risse ---
                // Winkel in Speichen aufteilen, jede Speiche zufaellig lang
                float spokes = max(_CrackCount, 1.0);
                float a = (ang + 3.14159265) / 6.2831853; // 0..1
                float idx = floor(a * spokes);
                float rnd = Hash(idx + IN.seed * 37.0);

                // wie weit reicht dieser Riss nach aussen
                float crackLen = _HoleSize + rnd * _CrackAmount;
                // Naehe zur Speichenmitte -> duenne Linie
                float centerOfSpoke = (idx + 0.5) / spokes;
                float distToSpoke = abs(a - centerOfSpoke);
                float spokeLine = 1.0 - smoothstep(0.0, 0.02 + 0.03 * rnd, distToSpoke);
                // Riss nur innerhalb seiner Laenge, nach aussen ausduennend
                float crack = spokeLine * (1.0 - smoothstep(_HoleSize, crackLen, r));

                // --- Verbrennungsring um das Loch ---
                float ring = smoothstep(_HoleSize, _HoleSize + _Edge, r)
                           * (1.0 - smoothstep(_HoleSize + _Edge, _HoleSize + _Edge * 3.0, r));

                // Zusammensetzen
                float mask = saturate(max(hole, crack));
                half3 col = lerp(_RingColor.rgb, _HoleColor.rgb, hole);
                col = lerp(col, _RingColor.rgb, ring * 0.6);

                float alpha = saturate(max(mask, ring * 0.5));
                return half4(col, alpha);
            }
            ENDHLSL
        }
    }
}