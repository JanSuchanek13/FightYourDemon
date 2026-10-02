using UnityEngine;
using TMPro;
using System;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Bewegung & Leben")]
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float floatSpeed = 1.5f;   // wie schnell es hochschwebt (Welt-Einheiten/s)
    [SerializeField] private float popScale = 1.5f;
    [SerializeField] private float popReturnSpeed = 10f;

    [Header("Random Tilt")]
    [SerializeField] private float maxTilt = 15f;   // max. Neigung in Grad (+/-)

    private float _timer;
    private Vector3 _worldPos;       // Position im Raum, an der die Zahl haengt
    private Vector3 _baseScale;
    private Camera _cam;
    private RectTransform _rect;
    private Action<DamageNumber> _onDone;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Show(float damage, Vector3 worldPos, Camera cam, Color color, Action<DamageNumber> onDone)
    {
        _worldPos = worldPos;
        _cam = cam;
        _onDone = onDone;
        _baseScale = Vector3.one;

        text.text = Mathf.RoundToInt(damage).ToString();
        text.color = color;                          // Farbe uebernehmen
        canvasGroup.alpha = 1f;
        transform.localScale = _baseScale * popScale;

        // zufaellige Neigung
        float tilt = UnityEngine.Random.Range(-maxTilt, maxTilt);
        transform.localRotation = Quaternion.Euler(0f, 0f, tilt);

        _timer = lifetime;

        gameObject.SetActive(true);
        UpdateScreenPosition();
    }

    void Update()
    {
        // im Raum nach oben schweben
        _worldPos += Vector3.up * floatSpeed * Time.deltaTime;
        UpdateScreenPosition();

        // Aufpopp-Scale zurueck
        transform.localScale = Vector3.Lerp(transform.localScale, _baseScale,
                                            Time.deltaTime * popReturnSpeed);

        _timer -= Time.deltaTime;
        canvasGroup.alpha = Mathf.Clamp01(_timer / lifetime); // ueber die Lebenszeit ausfaden

        if (_timer <= 0f)
            _onDone?.Invoke(this);
    }

    private void UpdateScreenPosition()
    {
        if (_cam == null) return;

        Vector3 screenPos = _cam.WorldToScreenPoint(_worldPos);

        // hinter der Kamera? -> ausblenden
        if (screenPos.z < 0f)
        {
            canvasGroup.alpha = 0f;
            return;
        }
        _rect.position = screenPos;
    }
}