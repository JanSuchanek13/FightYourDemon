using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    [Header("Sprite-Wechsel (letzter Layer)")]
    [Tooltip("Sprite, das der letzte Layer beim Hover zeigt.")]
    [SerializeField] private Sprite hoverSprite;

    private bool _hovered;
    private RectTransform _rect;
    private float _lerpSpeed;

    private Image _swapImage;     // Image des letzten Layers
    private Sprite _normalSprite; // Ruhe-Sprite

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _lerpSpeed = Mathf.Max(spreadSpeed, parallaxSpeed);

        // Ruhepositionen merken
        foreach (var layer in layers)
            if (layer.element != null)
                layer.basePos = layer.element.anchoredPosition;

        // Image des letzten Layers und Ruhe-Sprite einmalig cachen
        if (layers.Length > 0 && layers[layers.Length - 1].element != null)
        {
            _swapImage = layers[layers.Length - 1].element.GetComponent<Image>();
            if (_swapImage != null)
                _normalSprite = _swapImage.sprite;
        }
    }

    public void OnPointerEnter(PointerEventData e)
    {
        _hovered = true;
        SetSprite(hoverSprite);
    }

    public void OnPointerExit(PointerEventData e)
    {
        _hovered = false;
        SetSprite(_normalSprite);
    }

    // Wird nur bei Hover-Beginn/-Ende aufgerufen, nicht pro Frame
    private void SetSprite(Sprite sprite)
    {
        if (_swapImage != null && sprite != null)
            _swapImage.sprite = sprite;
    }

    void Update()
    {
        // --- Parallax: Mausposition relativ zur Button-Mitte (-1..1) ---
        Vector2 parallaxDir = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rect, Input.mousePosition, null, out Vector2 local))
        {
            parallaxDir = new Vector2(
                Mathf.Clamp(local.x / (_rect.rect.width * 0.5f), -1f, 1f),
                Mathf.Clamp(local.y / (_rect.rect.height * 0.5f), -1f, 1f));
        }

        float lerp = _lerpSpeed * Time.unscaledDeltaTime;

        foreach (var layer in layers)
        {
            if (layer.element == null) continue;

            Vector2 spreadTarget = _hovered ? layer.spreadOffset : Vector2.zero;
            Vector2 parallaxTarget = parallaxDir * parallaxAmount * layer.parallaxFactor;
            Vector2 target = layer.basePos + spreadTarget + parallaxTarget;

            layer.element.anchoredPosition = Vector2.Lerp(layer.element.anchoredPosition, target, lerp);
        }
    }
}