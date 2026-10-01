using UnityEngine;
using TMPro;
using System;

public class MoneyPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private float lifetime = 1.5f;   // wie lange sichtbar
    [SerializeField] private float fadeTime = 0.5f;   // Ausfaden am Ende
    [SerializeField] private float popScale = 1.4f;
    [SerializeField] private float popReturnSpeed = 8f;

    private float _timer;
    private Vector3 _baseScale;
    private Action<MoneyPopup> _onDone;

    public void Show(int amount, Action<MoneyPopup> onDone)
    {
        _onDone = onDone;
        _baseScale = transform.localScale;

        text.text = "+" + amount;
        canvasGroup.alpha = 1f;
        transform.localScale = _baseScale * popScale; // aufpoppen
        _timer = lifetime;

        gameObject.SetActive(true);
    }

    void Update()
    {
        // Scale sanft zurueck
        transform.localScale = Vector3.Lerp(transform.localScale, _baseScale,
                                            Time.deltaTime * popReturnSpeed);

        _timer -= Time.deltaTime;

        // die letzten Sekunden ausfaden
        if (_timer <= fadeTime)
            canvasGroup.alpha = Mathf.Clamp01(_timer / fadeTime);

        if (_timer <= 0f)
            _onDone?.Invoke(this); // Manager benachrichtigen
    }
}