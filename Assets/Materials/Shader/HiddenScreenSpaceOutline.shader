Shader "Hidden/ScreenSpaceOutline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Thickness (px)", Float) = 1.0
        _DepthThreshold ("Depth Threshold", Float) = 0.5
        _NormalThreshold ("Normal Threshold", Float) = 0.4
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        ZWrite Off ZTest Always Cull Off

        Pass
        {
            Name "ScreenSpaceOutline"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            // Reihenfolge wichtig: Core.hlsl zuerst (definiert TEXTURE2D_X u.a.),
            // dann Blit.hlsl (liefert Vert + Fullscreen-Dreieck und _BlitTexture)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            TEXTURE2D(_CameraNormalsTexture);
            SAMPLER(sampler_CameraNormalsTexture);

            float4 _OutlineColor;
            float  _OutlineThickness;
            float  _DepthThreshold;
            float  _NormalThreshold;

            float SampleDepth(float2 uv)
            {
                return SampleSceneDepth(uv);
            }

            float3 SampleNormal(float2 uv)
            {
                return SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, uv).rgb;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                float2 texel = _BlitTexture_TexelSize.xy * _OutlineThickness;

                // 4 Nachbar-Samples (Kreuz)
                float2 uvR = uv + float2(texel.x, 0);
                float2 uvL = uv - float2(texel.x, 0);
                float2 uvU = uv + float2(0, texel.y);
                float2 uvD = uv - float2(0, texel.y);

                // --- Tiefe ---
                float d0 = SampleDepth(uv);
                float dEdge = abs(SampleDepth(uvR) - d0)
                            + abs(SampleDepth(uvL) - d0)
                            + abs(SampleDepth(uvU) - d0)
                            + abs(SampleDepth(uvD) - d0);
                float depthEdge = step(_DepthThreshold * 0.01, dEdge);

                // --- Normalen ---
                float3 n0 = SampleNormal(uv);
                float nEdge = distance(SampleNormal(uvR), n0)
                            + distance(SampleNormal(uvL), n0)
                            + distance(SampleNormal(uvU), n0)
                            + distance(SampleNormal(uvD), n0);
                float normalEdge = step(_NormalThreshold, nEdge);

                float edge = max(depthEdge, normalEdge);

                half4 sceneColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv);
                return lerp(sceneColor, _OutlineColor, edge * _OutlineColor.a);
            }
            ENDHLSL
        }
    }
}