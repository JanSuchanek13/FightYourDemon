using UnityEngine;
using TMPro;

public class ComboCounter : MonoBehaviour
{
    public static ComboCounter Instance;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private CanvasGroup canvasGroup;   // fuer das Ausfaden

    [Header("Timing")]
    [Tooltip("Wie lange (Sek.) man Zeit hat, den naechsten Kill zu landen")]
    [SerializeField] private float comboTimeout = 3f;
    [Tooltip("Ab welcher Combo die Anzeige erscheint")]
    [SerializeField] private int showFromCombo = 2;
    [Tooltip("Wie lange das Ausfaden nach dem Reset dauert")]
    [SerializeField] private float fadeDuration = 1.2f;

    [Header("Juice: Aufpoppen")]
    [Tooltip("Wie stark der Text bei einem Kill aufpoppt")]
    [SerializeField] private float popScale = 1.4f;
    [Tooltip("Wie schnell er zur Normalgroesse zurueckkehrt")]
    [SerializeField] private float popReturnSpeed = 8f;

    [Header("Juice: Farbverlauf mit steigender Combo")]
    [SerializeField] private Gradient comboGradient;
    [Tooltip("Bei welcher Combo die Farbe ihr Maximum erreicht")]
    [SerializeField] private int maxComboForColor = 10;

    private int _combo;
    private float _timer;
    private Vector3 _baseScale;
    private bool _fading;
    private float _fadeT;

    public int CurrentCombo => _combo;

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _baseScale = comboText != null ? comboText.transform.localScale : Vector3.one;
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        UpdateDisplay();
    }

    void Update()
    {
        // Aufpopp-Scale sanft zurueck zur Normalgroesse
        if (comboText != null)
        {
            comboText.transform.localScale = Vector3.Lerp(
                comboText.transform.localScale, _baseScale,
                Time.deltaTime * popReturnSpeed);
        }

        // Ausfaden nach Reset
        if (_fading && canvasGroup != null)
        {
            _fadeT += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, _fadeT / fadeDuration);
            if (_fadeT >= fadeDuration)
            {
                _fading = false;
                canvasGroup.alpha = 0f;
            }
            return; // waehrend des Fadens laeuft kein Combo-Timer
        }

        // Combo-Timer
        if (_combo <= 0) return;

        _timer -= Time.deltaTime;
        if (_timer <= 0f)
            ResetCombo();
    }

    // Bei jedem Kill aufrufen
    public void AddKill()
    {
        _combo++;
        _timer = comboTimeout;
        _fading = false; // ein neuer Kill bricht ein laufendes Fade ab

        if (_combo >= showFromCombo)
        {
            if (canvasGroup != null) canvasGroup.alpha = 1f;
            // Aufpoppen: kurz vergroessern, Update schrumpft wieder
            if (comboText != null)
                comboText.transform.localScale = _baseScale * popScale;
        }

        UpdateDisplay();
    }

    // Combo verfaellt -> Text bleibt kurz stehen und fadet dann aus
    public void ResetCombo()
    {
        // nur faden, wenn ueberhaupt etwas sichtbar war
        bool wasVisible = _combo >= showFromCombo;

        _combo = 0;
        _timer = 0f;

        if (wasVisible && canvasGroup != null)
        {
            _fading = true;
            _fadeT = 0f;
            // Text NICHT sofort leeren -> Wert bleibt beim Faden lesbar
        }
        else if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    private void UpdateDisplay()
    {
        if (comboText == null) return;

        if (_combo >= showFromCombo)
        {
            comboText.text = _combo + "x";

            // Farbe nach Combo-Hoehe aus dem Gradient
            if (comboGradient != null)
            {
                float f = Mathf.Clamp01((float)_combo / maxComboForColor);
                comboText.color = comboGradient.Evaluate(f);
            }
        }
        // kein Ausblenden hier - das macht jetzt das Fade in ResetCombo
    }
}