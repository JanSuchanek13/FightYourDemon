using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Maus-Sway")]
    [Tooltip("Wie stark die Waffe der Mausbewegung nachzieht")]
    [SerializeField] private float mouseSwayAmount = 2f;
    [Tooltip("Maximaler Ausschlag, damit es nicht uebertreibt")]
    [SerializeField] private float maxMouseSway = 6f;

    [Header("Vertikaler Sway (Springen/Fallen)")]
    [SerializeField] private Rigidbody playerBody;      // fuer die vertikale Geschwindigkeit
    [SerializeField] private float verticalSwayAmount = 0.05f;
    [SerializeField] private float maxVerticalSway = 0.15f;

    [Header("Federung")]
    [Tooltip("Wie schnell die Waffe zurueckschwingt")]
    [SerializeField] private float returnSpeed = 8f;

    [Tooltip("Wie stark die Beschleunigung geglaettet wird (hoeher = reagiert schneller, weniger glatt)")]
    [SerializeField] private float verticalSmooth = 8f;

    private Vector3 _basePos;
    private Quaternion _baseRot;
    private float _lastVertVel;
    private float _measuredAccel;   // in FixedUpdate gemessen
    private float _smoothAccel;     // in Update geglaettet

    void Start()
    {
        _basePos = transform.localPosition;
        _baseRot = transform.localRotation;
        if (playerBody != null) _lastVertVel = playerBody.linearVelocity.y;
    }

    void FixedUpdate()
    {
        if (playerBody == null) return;

        float vertVel = playerBody.linearVelocity.y;
        _measuredAccel = (vertVel - _lastVertVel) / Time.fixedDeltaTime;
        _lastVertVel = vertVel;
    }

    void Update()
    {
        // --- Maus-Sway (wie gehabt) ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSwayAmount;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSwayAmount;
        mouseX = Mathf.Clamp(mouseX, -maxMouseSway, maxMouseSway);
        mouseY = Mathf.Clamp(mouseY, -maxMouseSway, maxMouseSway);
        Quaternion targetRot = _baseRot * Quaternion.Euler(mouseY, -mouseX, -mouseX);

        // --- Vertikaler Sway: gemessene Beschleunigung glaetten und anwenden ---
        _smoothAccel = Mathf.Lerp(_smoothAccel, _measuredAccel, verticalSmooth * Time.deltaTime);
        float vertOffset = Mathf.Clamp(-_smoothAccel * verticalSwayAmount,
                                       -maxVerticalSway, maxVerticalSway);
        Vector3 targetPos = _basePos + new Vector3(0f, vertOffset, 0f);

        // --- federn ---
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos,
                                               returnSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot,
                                                   returnSpeed * Time.deltaTime);
    }
}