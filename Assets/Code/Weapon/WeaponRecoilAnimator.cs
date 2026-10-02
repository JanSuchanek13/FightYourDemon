using UnityEngine;

/// <summary>
/// Animiert Waffen-Rückstoß performant per Feder-Dämpfer-Simulation:
///  - Slide/Schaft springt bei einem Schuss nach hinten und federt zurück
///  - Waffe kippt (Tilt) nach hinten/oben und federt zurück

[DisallowMultipleComponent]
public class WeaponRecoilAnimator : MonoBehaviour
{
    private const float MaxDeltaTime = 1f / 30f; // Stabilität bei Framedrops
    private const float SettleEpsilon = 0.0001f;

    [Header("Referenzen")]
    [Tooltip("Transform der Waffe, die gekippt wird (Recoil-Tilt).")]
    [SerializeField] private Transform weaponTransform;
    [Tooltip("Transform des Schafts/Slides, der nach hinten springt.")]
    [SerializeField] private Transform slideTransform;

    [Header("Slide / Schaft")]
    [Tooltip("Rückwärtsrichtung im lokalen Raum des Slides (z.B. 0,0,-1).")]
    [SerializeField] private Vector3 slideBackDirection = new Vector3(0f, 0f, -1f);
    [Tooltip("Anfangsgeschwindigkeit des Slides beim Schuss (m/s).")]
    [SerializeField] private float slideKickVelocity = 2.5f;
    [Tooltip("Maximaler Rückweg in Metern.")]
    [SerializeField] private float slideMaxDistance = 0.04f;
    [Tooltip("Federstärke: höher = schnelleres Zurückschnappen.")]
    [SerializeField] private float slideStiffness = 900f;
    [Tooltip("Dämpfung: ~2*sqrt(Stiffness) = kein Nachschwingen.")]
    [SerializeField] private float slideDamping = 55f;

    [Header("Recoil Tilt")]
    [Tooltip("Kipp-Achse im lokalen Raum der Waffe. (-1,0,0) = Mündung nach oben.")]
    [SerializeField] private Vector3 tiltAxis = new Vector3(-1f, 0f, 0f);
    [Tooltip("Anfangs-Winkelgeschwindigkeit beim Schuss (Grad/s).")]
    [SerializeField] private float tiltKickVelocity = 150f;
    [Tooltip("Maximaler Kippwinkel in Grad.")]
    [SerializeField] private float tiltMaxAngle = 10f;
    [SerializeField] private float tiltStiffness = 120f;
    [Tooltip("Etwas unter 2*sqrt(Stiffness) = leichtes Nachfedern.")]
    [SerializeField] private float tiltDamping = 18f;

    // Gecachte Ruhewerte
    private Vector3 slideRestPosition;
    private Quaternion weaponRestRotation;
    private Vector3 slideDir;   // normalisiert
    private Vector3 tiltDir;    // normalisiert

    // Simulationszustand
    private float slideOffset;
    private float slideVelocity;
    private float tiltAngle;
    private float tiltVelocity;

    private void Awake()
    {
        if (weaponTransform == null) weaponTransform = transform;

        slideDir = slideBackDirection.normalized;
        tiltDir = tiltAxis.normalized;

        weaponRestRotation = weaponTransform.localRotation;
        if (slideTransform != null) slideRestPosition = slideTransform.localPosition;

        enabled = false; // Im Leerlauf kein Update
    }

    /// <summary>Löst die Recoil-Animation aus. Intensity z.B. 1 = normal.</summary>
    public void Fire(float intensity = 1f)
    {
        slideVelocity += slideKickVelocity * intensity;
        tiltVelocity += tiltKickVelocity * intensity;
        enabled = true;
    }

    /// <summary>Setzt die Animation sofort zurück (z.B. beim Waffenwechsel).</summary>
    public void ResetImmediate()
    {
        slideOffset = slideVelocity = tiltAngle = tiltVelocity = 0f;
        ApplyPose();
        enabled = false;
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        if (dt > MaxDeltaTime) dt = MaxDeltaTime;

        // --- Slide: Feder-Dämpfer (semi-implizites Euler) ---
        slideVelocity += (-slideStiffness * slideOffset - slideDamping * slideVelocity) * dt;
        slideOffset += slideVelocity * dt;

        if (slideOffset < 0f)
        {
            slideOffset = 0f;
            if (slideVelocity < 0f) slideVelocity = 0f;
        }
        else if (slideOffset > slideMaxDistance)
        {
            slideOffset = slideMaxDistance;
            if (slideVelocity > 0f) slideVelocity = 0f;
        }

        // --- Tilt: Feder-Dämpfer ---
        tiltVelocity += (-tiltStiffness * tiltAngle - tiltDamping * tiltVelocity) * dt;
        tiltAngle += tiltVelocity * dt;

        if (tiltAngle > tiltMaxAngle)
        {
            tiltAngle = tiltMaxAngle;
            if (tiltVelocity > 0f) tiltVelocity = 0f;
        }

        // --- Zur Ruhe gekommen? Auf Ruhepose setzen und Script abschalten ---
        if (Mathf.Abs(slideOffset) < SettleEpsilon && Mathf.Abs(slideVelocity) < SettleEpsilon &&
            Mathf.Abs(tiltAngle) < 0.01f && Mathf.Abs(tiltVelocity) < 0.01f)
        {
            slideOffset = slideVelocity = tiltAngle = tiltVelocity = 0f;
            ApplyPose();
            enabled = false;
            return;
        }

        ApplyPose();
    }

    private void ApplyPose()
    {
        if (slideTransform != null)
            slideTransform.localPosition = slideRestPosition + slideDir * slideOffset;

        weaponTransform.localRotation = weaponRestRotation * Quaternion.AngleAxis(tiltAngle, tiltDir);
    }
}