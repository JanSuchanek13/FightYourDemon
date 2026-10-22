using UnityEngine;

public class ExplosionStagger : MonoBehaviour
{
    public static ExplosionStagger Instance { get; private set; }

    [Header("Abstand")]
    [SerializeField] private float stagger = 0.08f;
    [SerializeField] private float randomJitter = 0.03f;
    [Tooltip("Ab welchem Kettenglied der Abstand zu schrumpfen beginnt")]
    [SerializeField] private int speedUpFrom = 3;
    [Tooltip("Wie stark der Abstand pro Glied schrumpft (0..1 pro Stufe)")]
    [SerializeField] private float speedUpFactor = 0.12f;
    [Tooltip("Kleinstmoeglicher Abstand, damit die Sounds nicht verschmelzen")]
    [SerializeField] private float minStagger = 0.025f;

    private float _nextFreeTime;

    [Header("Pitch")]
    [Tooltip("Wie viel Pitch pro Kettenglied dazukommt")]
    [SerializeField] private float pitchStep = 0.08f;
    [Tooltip("Maximaler Pitch, damit es nicht ins Piepsige kippt")]
    [SerializeField] private float maxPitch = 2f;

    private int _chainIndex;

    // gibt Verzoegerung UND Pitch fuer diesen Gegner zurueck
    public void GetStagger(out float delay, out float pitch)
    {
        float now = Time.time;

        if (_nextFreeTime < now)
        {
            _nextFreeTime = now;
            _chainIndex = 0;
        }

        delay = _nextFreeTime - now;
        pitch = Mathf.Min(1f + _chainIndex * pitchStep, maxPitch);

        // Abstand ab speedUpFrom schrittweise verkuerzen
        float currentStagger = stagger;
        if (_chainIndex >= speedUpFrom)
        {
            int stepsOver = _chainIndex - speedUpFrom + 1;
            currentStagger = stagger * Mathf.Pow(1f - speedUpFactor, stepsOver);
            currentStagger = Mathf.Max(currentStagger, minStagger);
        }

        _nextFreeTime += currentStagger + Random.Range(0f, randomJitter);
        _chainIndex++;
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

}