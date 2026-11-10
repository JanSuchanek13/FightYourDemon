using UnityEngine;
using UnityEngine.UI;

public class CrosshairHitmarker : MonoBehaviour
{
    [SerializeField] private Image crosshair;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hitSprite;

    [Header("Timing")]
    [Tooltip("Wie lange der Hitmarker sichtbar bleibt (Sekunden)")]
    [SerializeField] private float showTime = 0.1f;

    [Header("Aufpoppen")]
    [SerializeField] private float popScale = 1.6f;
    [SerializeField] private float popReturnSpeed = 14f;

    private float _timer;
    private RectTransform _rect;
    private Vector3 _baseScale;

    void Awake()
    {
        _rect = crosshair.GetComponent<RectTransform>();
        _baseScale = _rect.localScale;

        if (crosshair != null && normalSprite == null)
            normalSprite = crosshair.sprite;   // Ausgangs-Sprite merken
    }

    // bei einem Treffer aufrufen
    public void ShowHit()
    {
        if (crosshair == null) return;
        crosshair.sprite = hitSprite;
        _rect.localScale = _baseScale * popScale;   // aufpoppen
        _timer = showTime;
    }

    void Update()
    {
        // Scale immer sanft zurueck zur Normalgroesse
        _rect.localScale = Vector3.Lerp(_rect.localScale, _baseScale,
                                        popReturnSpeed * Time.deltaTime);

        if (_timer <= 0f) return;

        _timer -= Time.deltaTime;
        if (_timer <= 0f)
            crosshair.sprite = normalSprite;   // zurueck zum normalen Sprite
    }
}