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

    [Header("Flugbahn")]
    [Tooltip("Anfangs-Geschwindigkeit nach oben")]
    [SerializeField] private float launchUp = 3.5f;
    [Tooltip("Seitliche Geschwindigkeit (links/rechts)")]
    [SerializeField] private float launchSide = 2f;
    [Tooltip("Schwerkraft, die die Zahl wieder runterzieht")]
    [SerializeField] private float gravity = 9f;
    [Tooltip("Zufaellige Streuung, damit nicht alle gleich fliegen")]
    [SerializeField] private float randomness = 0.6f;

    private float _timer;
    private Vector3 _worldPos;       // Position im Raum, an der die Zahl haengt
    private Vector3 _velocity;
    private Vector3 _baseScale;
    private Camera _cam;
    private RectTransform _rect;
    private Action<DamageNumber> _onDone;
    private static int _sideToggle = 1; // wechselt bei jedem Spawn zwischen links und rechts

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


        // Pop: gross starten, Update schrumpft zurueck
        transform.localScale = _baseScale * popScale;

        // zufaellige leichte Neigung
        transform.localRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-15f, 15f));

        // --- Startgeschwindigkeit: schraeg raus, abwechselnd links/rechts ---
        _sideToggle *= -1; // umschalten
        float side = launchSide * _sideToggle
                   + UnityEngine.Random.Range(-randomness, randomness);
        float up = launchUp + UnityEngine.Random.Range(-randomness, randomness);
        _velocity = new Vector3(side, up, 0f);

        _timer = lifetime;
        gameObject.SetActive(true);
        UpdateScreenPosition();
    }

    void Update()
    {
        // Flugbahn: Schwerkraft auf die Geschwindigkeit, dann Position bewegen
        _velocity.y -= gravity * Time.deltaTime;
        _worldPos += _velocity * Time.deltaTime;
        UpdateScreenPosition();

        // Pop-Scale schnell zurueck zur Normalgroesse
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