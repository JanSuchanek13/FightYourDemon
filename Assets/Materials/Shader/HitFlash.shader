Shader "Custom/HitFlashOverlay"
{
    Properties
    {
        _FlashColor ("Flash-Farbe", Color) = (1,1,1,1)
        _Flash ("Flash-Staerke", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "HitFlashOverlay"
            Blend One One          // additiv -> ueberstrahlt (gut mit Bloom)
            ZWrite Off
            ZTest LEqual           // nur auf sichtbaren Flaechen des Objekts
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

            // per-Instance, damit jedes Objekt einzeln blitzt (Batching bleibt)
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _Flash)
            UNITY_INSTANCING_BUFFER_END(Props)

            CBUFFER_START(UnityPerMaterial)
                float4 _FlashColor;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                float flash = UNITY_ACCESS_INSTANCED_PROP(Props, _Flash);
                // additiv: bei flash=0 wird schwarz addiert = unsichtbar
                return half4(_FlashColor.rgb * flash, 0);
            }
            ENDHLSL
        }
    }
}