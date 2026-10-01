using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class ScreenSpaceOutlineFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Color outlineColor = Color.black;
        [Min(0f)] public float thickness = 1.0f;
        [Min(0f)] public float depthThreshold = 0.5f;
        [Min(0f)] public float normalThreshold = 0.4f;
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public Settings settings = new Settings();
    public Shader outlineShader;

    private Material _material;
    private OutlinePass _pass;

    public override void Create()
    {
        if (outlineShader == null) return;
        _material = CoreUtils.CreateEngineMaterial(outlineShader);
        _pass = new OutlinePass(_material, settings)
        {
            renderPassEvent = settings.renderPassEvent
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_pass == null || _material == null) return;
        // Wir brauchen Tiefe UND Normalen
        _pass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal);
        renderer.EnqueuePass(_pass);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(_material);
    }

    // ---- Der Pass ----
    class OutlinePass : ScriptableRenderPass
    {
        private readonly Material _mat;
        private readonly Settings _settings;

        public OutlinePass(Material mat, Settings settings)
        {
            _mat = mat;
            _settings = settings;
        }

        private void UpdateMaterial()
        {
            _mat.SetColor("_OutlineColor", _settings.outlineColor);
            _mat.SetFloat("_OutlineThickness", _settings.thickness);
            _mat.SetFloat("_DepthThreshold", _settings.depthThreshold);
            _mat.SetFloat("_NormalThreshold", _settings.normalThreshold);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UpdateMaterial();

            var resourceData = frameData.Get<UniversalResourceData>();
            if (resourceData.isActiveTargetBackBuffer) return;

            var source = resourceData.activeColorTexture;

            // Ziel-Textur (gleiche Beschreibung wie die Quelle)
            var descriptor = renderGraph.GetTextureDesc(source);
            descriptor.name = "OutlineTarget";
            descriptor.clearBuffer = false;
            var destination = renderGraph.CreateTexture(descriptor);

            // Blit source -> destination durch unser Material (Pass 0)
            var blitParams = new RenderGraphUtils.BlitMaterialParameters(
                source, destination, _mat, 0);
            renderGraph.AddBlitPass(blitParams, "Screen Space Outline");

            // Ergebnis zurueck als aktives Farbziel setzen
            resourceData.cameraColor = destination;
        }
    }
}