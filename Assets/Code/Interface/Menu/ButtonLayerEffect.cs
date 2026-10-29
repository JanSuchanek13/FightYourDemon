using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonLayerEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Serializable]
    public class Layer
    {
        public RectTransform element;
        [Tooltip("Wohin dieses Element beim Hover wandert (relativ, in UI-Einheiten)")]
        public Vector2 spreadOffset = Vector2.zero;
        [Tooltip("Wie stark dieses Element auf die Maus reagiert (0 = gar nicht)")]
        public float parallaxFactor = 0f;

        [HideInInspector] public Vector2 basePos;  // Ruheposition
    }

    [SerializeField] private Layer[] layers;

    [Header("Hover-Spread")]
    [SerializeField] private float spreadSpeed = 10f;

    [Header("Parallax")]
    [Tooltip("Maximaler Parallax-Ausschlag in UI-Einheiten")]
    [SerializeField] private float parallaxAmount = 15f;
    [SerializeField] private float parallaxSpeed = 6f;

    private bool _hovered;
    private RectTransform _rect;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        // Ruhepositionen merken
        foreach (var layer in layers)
            if (layer.element != null)
                layer.basePos = layer.element.anchoredPosition;
    }

    public void OnPointerEnter(PointerEventData e) => _hovered = true;
    public void OnPointerExit(PointerEventData e) => _hovered = false;

    void Update()
    {
        // --- Parallax: Mausposition relativ zur Button-Mitte (-1..1) ---
        Vector2 parallaxDir = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rect, Input.mousePosition, null, out Vector2 local))
        {
            // auf Button-Groesse normieren
            parallaxDir = new Vector2(
                Mathf.Clamp(local.x / (_rect.rect.width * 0.5f), -1f, 1f),
                Mathf.Clamp(local.y / (_rect.rect.height * 0.5f), -1f, 1f));
        }

        foreach (var layer in layers)
        {
            if (layer.element == null) continue;

            // Spread-Ziel: beim Hover nach aussen, sonst zurueck
            Vector2 spreadTarget = _hovered ? layer.spreadOffset : Vector2.zero;

            // Parallax-Ziel: Maus-Richtung mal Faktor
            Vector2 parallaxTarget = parallaxDir * parallaxAmount * layer.parallaxFactor;

            // Zielposition = Ruhe + Spread + Parallax
            Vector2 target = layer.basePos + spreadTarget + parallaxTarget;

            // sanft hinbewegen (Spread und Parallax teilen sich die Glaettung)
            layer.element.anchoredPosition = Vector2.Lerp(
                layer.element.anchoredPosition, target,
                Mathf.Max(spreadSpeed, parallaxSpeed) * Time.unscaledDeltaTime);
        }
    }
}