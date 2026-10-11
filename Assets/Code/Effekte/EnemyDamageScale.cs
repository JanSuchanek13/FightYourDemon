using UnityEngine;

[DisallowMultipleComponent]
public class EnemyDamageScale : MonoBehaviour
{
    [SerializeField] private float normalScale = 1f;
    [Tooltip("Zusätzlicher Scale bei fast 0 Leben.")]
    [SerializeField] private float maxExtraScale = 0.6f;
    [Tooltip("1 = linear, höher = Effekt wird erst kurz vor dem Tod stark.")]
    [SerializeField] private float curvePower = 2.5f;
    [Tooltip("Kleiner = schnelleres Aufpusten.")]
    [SerializeField] private float smoothTime = 0.12f;

    private const float SettleEpsilon = 0.0005f;

    private Transform cachedTransform;
    private float currentScale;
    private float targetScale;
    private float velocity;

    private void Awake()
    {
        cachedTransform = transform;
        normalScale = cachedTransform.localScale.x;
        currentScale = targetScale = normalScale;
        enabled = false; // Update läuft nur während der Animation
    }

    // Aufrufen, wenn sich die Leben ändern (z.B. in deiner TakeDamage-Methode)
    public void OnHealthChanged(float health, float maxHealth)
    {
        float damage01 = 1f - Mathf.Clamp01(health / maxHealth);
        targetScale = normalScale + maxExtraScale * Mathf.Pow(damage01, curvePower);
        enabled = true;
    }
    public void ResetScale()
    {
        currentScale = targetScale = normalScale;
        velocity = 0f;
        cachedTransform.localScale = new Vector3(normalScale, normalScale, normalScale);
        enabled = false;
    }

    private void Update()
    {
        currentScale = Mathf.SmoothDamp(currentScale, targetScale, ref velocity, smoothTime);

        if (Mathf.Abs(currentScale - targetScale) < SettleEpsilon && Mathf.Abs(velocity) < SettleEpsilon)
        {
            currentScale = targetScale;
            velocity = 0f;
            enabled = false;
        }

        cachedTransform.localScale = new Vector3(currentScale, currentScale, currentScale);
    }
}