using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class HitFlash : MonoBehaviour
{
    [Tooltip("Wie lange der Flash dauert (Sekunden)")]
    public float flashDuration = 0.15f;

    [Tooltip("Maximale Flash-Staerke (0..1)")]
    [Range(0f, 1f)] public float maxFlash = 1f;

    private Renderer _rend;
    private MaterialPropertyBlock _mpb;
    private float _timer;
    private static readonly int FlashID = Shader.PropertyToID("_Flash");

    void Awake()
    {
        _rend = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
    }

    // Diese Methode beim Treffer aufrufen
    public void Flash()
    {
        _timer = flashDuration;
    }

    void Update()
    {
        if (_timer <= 0f) return;

        _timer -= Time.deltaTime;
        float t = Mathf.Clamp01(_timer / flashDuration); // 1 -> 0

        _rend.GetPropertyBlock(_mpb);
        _mpb.SetFloat(FlashID, t * maxFlash);
        _rend.SetPropertyBlock(_mpb);
    }
}