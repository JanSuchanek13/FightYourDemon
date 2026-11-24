using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Zoom : MonoBehaviour, IScrollHandler
{
    private Vector3 baseScale;   // die 100%-Referenzgroesse

    public float zoomSpeed = 0.1f;
    [Tooltip("Kleinster Zoom (0.5 = 50%)")]
    public float minZoom = 0.25f;
    [Tooltip("Groesster Zoom")]
    public float maxZoom = 2f;
    [Tooltip("Zoom beim Start (0.5 = 50%)")]
    public float startZoom = 0.5f;

    private void Awake()
    {
        baseScale = transform.localScale;                 // aktuelle Scale = 100%
        transform.localScale = baseScale * startZoom;     // auf 50% setzen
    }

    public void OnScroll(PointerEventData eventData)
    {
        var delta = Vector3.one * (eventData.scrollDelta.y * zoomSpeed);
        var desiredScale = transform.localScale + delta;

        desiredScale = ClampDesiredScale(desiredScale);

        transform.localScale = desiredScale;
    }

    private Vector3 ClampDesiredScale(Vector3 desiredScale)
    {
        desiredScale = Vector3.Max(baseScale * minZoom, desiredScale);
        desiredScale = Vector3.Min(baseScale * maxZoom, desiredScale);
        return desiredScale;
    }
}