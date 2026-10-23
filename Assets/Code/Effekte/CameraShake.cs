using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Formel")]
    [Tooltip("Exponent fuer die Umrechnung: hoeher = Abstufungen deutlicher (2 = quadratisch, 3 = kubisch)")]
    [SerializeField] private float exponent = 2f;
    [Tooltip("Maximaler Rotationsausschlag in Grad bei vollem Trauma")]
    [SerializeField] private float maxAngle = 12f;

    [Header("Perlin Noise")]
    [Tooltip("Wie schnell das Noise durchlaufen wird (Zittertempo)")]
    [SerializeField] private float frequency = 25f;

    [Header("Abklingen")]
    [Tooltip("Wie schnell das Trauma von selbst abklingt (pro Sekunde)")]
    [SerializeField] private float decay = 1.5f;

    private float _trauma;   // 0..1, der rohe Eingangswert
    private float _seed;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        _seed = Random.value * 1000f;
    }

    // von aussen aufrufen; value ist der ROHE Wert (0..1), nicht die Staerke
    public void AddShake(float value)
    {
        // Trauma aufaddieren, aber bei 1 deckeln
        _trauma = Mathf.Clamp01(_trauma + value);
    }

    void Update()
    {
        if (_trauma <= 0f)
        {
            // in Ruhe: Rotation sauber auf null
            transform.localRotation = Quaternion.identity;
            return;
        }

        // --- die Formel: Trauma exponentiell in Intensitaet umrechnen ---
        float intensity = Mathf.Pow(_trauma, exponent);

        // --- Perlin Noise pro Achse (jeweils eigener Seed-Versatz) ---
        float t = Time.time * frequency;
        float pitch = (Mathf.PerlinNoise(_seed, t) - 0.5f) * 2f;
        float yaw = (Mathf.PerlinNoise(_seed + 100f, t) - 0.5f) * 2f;
        float roll = (Mathf.PerlinNoise(_seed + 200f, t) - 0.5f) * 2f;

        Vector3 angles = new Vector3(pitch, yaw, roll) * (maxAngle * intensity);
        transform.localRotation = Quaternion.Euler(angles);

        // Trauma klingt ab
        _trauma = Mathf.Max(0f, _trauma - decay * Time.deltaTime);
    }
}