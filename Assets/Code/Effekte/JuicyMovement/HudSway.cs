using UnityEngine;

public class HudSway : MonoBehaviour
{
    [SerializeField] private RectTransform hudRoot;     // Container deines HUDs
    [SerializeField] private Rigidbody playerBody;      // statt Transform

    [Header("Maus-Sway")]
    [SerializeField] private float mouseSwayAmount = 8f;

    [Header("Vertikaler Sway (Beschleunigung)")]
    [SerializeField] private float verticalSwayAmount = 0.5f;  // klein halten
    [SerializeField] private float verticalSmooth = 8f;

    [Header("Allgemein")]
    [SerializeField] private float maxSway = 30f;
    [SerializeField] private float returnSpeed = 6f;

    private Vector2 _basePos;
    private float _lastVertVel;
    private float _measuredAccel;
    private float _smoothAccel;

    void Start()
    {
        _basePos = hudRoot.anchoredPosition;
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
        // Maus-Sway
        float mouseX = Input.GetAxis("Mouse X") * mouseSwayAmount;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSwayAmount;

        // vertikaler Sway aus geglaetteter Beschleunigung
        _smoothAccel = Mathf.Lerp(_smoothAccel, _measuredAccel, verticalSmooth * Time.deltaTime);
        float vertOffset = -_smoothAccel * verticalSwayAmount;

        // HUD entgegen der Blickbewegung + vertikaler Traegheit
        Vector2 offset = new Vector2(-mouseX, -mouseY + vertOffset);
        offset = Vector2.ClampMagnitude(offset, maxSway);

        Vector2 target = _basePos + offset;
        hudRoot.anchoredPosition = Vector2.Lerp(hudRoot.anchoredPosition, target,
                                                returnSpeed * Time.deltaTime);
    }
}