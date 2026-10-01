using System.Collections;
using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    [Header("Property-Namen (Panoramic: _Tint / _Exposure)")]
    [SerializeField] private string tintProperty = "_Tint";
    [SerializeField] private string exposureProperty = "_Exposure";

    [Header("Umgebungslicht mit-updaten (kostet etwas Leistung)")]
    [SerializeField] private bool updateEnvironment = true;

    [Header("Standardwerte fuer den Event-Aufruf")]
    [SerializeField] private float eventTintDuration = 1.5f;
    [SerializeField] private float eventFlashPeak = 3f;
    [SerializeField] private float eventFlashDuration = 0.4f;

    private Material _runtimeSky;
    private int _tintID;
    private int _exposureID;
    private float _baseExposure = 1f;

    private Coroutine _tintCo;
    private Coroutine _flashCo;

    void Awake()
    {
        _tintID = Shader.PropertyToID(tintProperty);
        _exposureID = Shader.PropertyToID(exposureProperty);

        // Instanz erzeugen, damit das Skybox-Asset nicht dauerhaft veraendert wird
        if (RenderSettings.skybox != null)
        {
            _runtimeSky = new Material(RenderSettings.skybox);
            RenderSettings.skybox = _runtimeSky;

            if (_runtimeSky.HasProperty(_exposureID))
                _baseExposure = _runtimeSky.GetFloat(_exposureID);
        }
    }

    // --- Einfaerben (sanfter Uebergang) ---
    public void TintSkybox(Color target, float duration = 1f)
    {
        if (_runtimeSky == null || !_runtimeSky.HasProperty(_tintID)) return;
        if (_tintCo != null) StopCoroutine(_tintCo);   // nur den Tint neu starten
        _tintCo = StartCoroutine(TintRoutine(target, duration));
    }

    private IEnumerator TintRoutine(Color target, float duration)
    {
        Color start = _runtimeSky.GetColor(_tintID);
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            _runtimeSky.SetColor(_tintID, Color.Lerp(start, target, t / duration));
            if (updateEnvironment) DynamicGI.UpdateEnvironment();
            yield return null;
        }
        _runtimeSky.SetColor(_tintID, target);
        if (updateEnvironment) DynamicGI.UpdateEnvironment();
        _tintCo = null;
    }

    // --- Aufblitzen ueber die Exposure ---
    public void FlashOnce(float peak = 3f, float duration = 0.4f)
    {
        if (_runtimeSky == null || !_runtimeSky.HasProperty(_exposureID)) return;
        if (_flashCo != null) StopCoroutine(_flashCo);   // nur den Flash neu starten
        _flashCo = StartCoroutine(FlashRoutine(peak, duration));
    }

    private IEnumerator FlashRoutine(float peak, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;
            _runtimeSky.SetFloat(_exposureID, Mathf.Lerp(peak, _baseExposure, p));
            yield return null;
        }
        _runtimeSky.SetFloat(_exposureID, _baseExposure);
        _flashCo = null;
    }

    // --- beides gleichzeitig ausloesen ---
    public void TintWithFlash(Color target, float duration = 1f, float flashPeak = 3f, float flashDuration = 0.4f)
    {
        TintSkybox(target, 1);
        FlashOnce(8, 1);
    }

    // Fuer UnityEvents: nimmt einen Hexcode als String (z.B. "#FF4400" oder "FF4400")
    public void TintWithFlashHex(string hex)
    {
        if (ColorUtility.TryParseHtmlString(NormalizeHex(hex), out Color col))
        {
            TintWithFlash(col, eventTintDuration, eventFlashPeak, eventFlashDuration);
        }
        else
        {
            Debug.LogWarning($"SkyboxController: '{hex}' ist kein gueltiger Hexcode.");
        }
    }

    // ergaenzt ein fehlendes '#', damit beide Schreibweisen funktionieren
    private string NormalizeHex(string hex)
    {
        if (string.IsNullOrEmpty(hex)) return hex;
        return hex.StartsWith("#") ? hex : "#" + hex;
    }
}