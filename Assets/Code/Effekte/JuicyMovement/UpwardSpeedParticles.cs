using UnityEngine;

[DisallowMultipleComponent]
public class UpwardSpeedParticles : MonoBehaviour
{
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private ParticleSystem effect;
    [SerializeField] private WingFlap wings;

    [Header("Geschwindigkeit (m/s nach oben)")]
    [SerializeField] private float minSpeed = 3f;
    [SerializeField] private float maxSpeed = 20f;

    [Header("Flügel (eigener Geschwindigkeitsbereich)")]
    [Tooltip("Ab dieser Aufwaertsgeschwindigkeit beginnen die Fluegel zu flattern")]
    [SerializeField] private float wingMinSpeed = 0.5f;
    [Tooltip("Ab dieser Geschwindigkeit flattern sie mit vollem Tempo")]
    [SerializeField] private float wingMaxSpeed = 8f;

    [Header("Partikel pro Sekunde bei maxSpeed")]
    [SerializeField] private float maxEmissionRate = 60f;

    [Header("Früher stoppen")]
    [SerializeField, Range(0.3f, 1f)] private float stopFactor = 0.85f;
    [SerializeField] private float peakDecay = 6f;
    [SerializeField] private float curvePower = 2f;

    [Header("Sound (wird wiederholt gespielt, öfter bei höherer Geschwindigkeit)")]
    [SerializeField] private AudioClip[] clips;
    [SerializeField, Range(0f, 1f)] private float volume = 0.8f;
    [Tooltip("Abstand zwischen zwei Sounds bei minSpeed (Sekunden).")]
    [SerializeField] private float maxInterval = 0.5f;
    [Tooltip("Abstand zwischen zwei Sounds bei maxSpeed (Sekunden).")]
    [SerializeField] private float minInterval = 0.08f;

    private float lastY;
    private float invSpeedRange;
    private float emitAccumulator;
    private float peakSpeed;

    private float nextPlayTime;
    private int lastClipIndex = -1;

    private void Awake()
    {
        invSpeedRange = 1f / Mathf.Max(0.0001f, maxSpeed - minSpeed);
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        float upSpeed = playerBody.linearVelocity.y;

        // --- Fluegel: eigener, niedrigerer Bereich ---
        float wingT = Mathf.Clamp01((upSpeed - wingMinSpeed) / Mathf.Max(0.0001f, wingMaxSpeed - wingMinSpeed));
        if (wings != null) wings.SetFlapSpeed(wingT);

        // --- Effekte: wie gehabt mit peakSpeed/stopFactor ---
        if (upSpeed > minSpeed)
        {
            peakSpeed = Mathf.Max(upSpeed, peakSpeed - peakDecay * dt);

            if (upSpeed >= peakSpeed * stopFactor)
            {
                float t = Mathf.Clamp01((upSpeed - minSpeed) * invSpeedRange);
                float curved = Mathf.Pow(t, curvePower);

                emitAccumulator += curved * maxEmissionRate * dt;
                int count = (int)emitAccumulator;
                if (count > 0)
                {
                    emitAccumulator -= count;
                    effect.Emit(count);
                }

                float now = Time.time;
                if (now >= nextPlayTime)
                {
                    AudioClip clip = PickClip();
                    if (clip != null) PlayClip(clip, volume);
                    nextPlayTime = now + Mathf.Lerp(maxInterval, minInterval, t);
                }
            }
            else
            {
                emitAccumulator = 0f;
            }
        }
        else
        {
            peakSpeed = 0f;
            emitAccumulator = 0f;
        }
    }

    // Zufälliger Clip, nie derselbe wie beim letzten Abspielen
    private AudioClip PickClip()
    {
        int count = clips.Length;
        if (count == 0) return null;

        int index = Random.Range(0, count);
        if (count > 1 && index == lastClipIndex)
            index = (index + 1 + Random.Range(0, count - 1)) % count;

        lastClipIndex = index;
        return clips[index];
    }


    // ------------------------------------------------------------------
    // HIER an deinen Manager anpassen
    // ------------------------------------------------------------------

    private void PlayClip(AudioClip clip, float vol)
    {
        // Beispiel (Namen ersetzen):
        AudioManager.Instance.Play2D(clip, vol);
    }
}