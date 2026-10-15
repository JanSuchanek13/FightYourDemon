using UnityEngine;

public class ShootZoom : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [Header("Zoom")]
    [Tooltip("Wie viel Zoom pro Schuss dazukommt (positiv = reinzoomen)")]
    [SerializeField] private float zoomPerShot = 1.5f;
    [Tooltip("Maximaler Zoom (FOV-Aenderung)")]
    [SerializeField] private float maxZoom = 8f;
    [Tooltip("Wie schnell der Zoom zurueckfadet")]
    [SerializeField] private float returnSpeed = 4f;

    private float _baseFov;
    private float _zoom;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        _baseFov = cam.fieldOfView;
    }

    // beim Schuss aufrufen
    public void AddZoom()
    {
        _zoom = Mathf.Min(_zoom + zoomPerShot, maxZoom);
    }

    void Update()
    {
        // Zoom sanft zurueck auf 0 faden
        _zoom = Mathf.Lerp(_zoom, 0f, returnSpeed * Time.deltaTime);
        cam.fieldOfView = _baseFov - _zoom;
    }
}