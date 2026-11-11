using UnityEngine;
using UnityEngine.UI;

public class WingFlap : MonoBehaviour
{
    [SerializeField] private Image wingImage;
    [Tooltip("Die Flatter-Frames in Reihenfolge")]
    [SerializeField] private Sprite[] frames;
    [SerializeField] private Sprite restSprite;   // Fluegel angelegt / Ruhestellung

    [Header("Flatter-Tempo (Frames pro Sekunde)")]
    [SerializeField] private float minFps = 2f;   // bei langsamem Steigen
    [SerializeField] private float maxFps = 18f;  // bei vollem Tempo

    private int _frameIndex;
    private float _frameTimer;
    private float _currentFps;

    // von aussen aufrufen: intensity 0..1 (0 = kaum, 1 = volles Tempo)
    public void SetFlapSpeed(float intensity)
    {
        intensity = Mathf.Clamp01(intensity);
        _currentFps = Mathf.Lerp(minFps, maxFps, intensity);

        // bei Stillstand in Ruhestellung
        if (_currentFps <= 0.01f && restSprite != null)
        {
            wingImage.sprite = restSprite;
            _frameIndex = 0;
            _frameTimer = 0f;
        }
    }

    void Update()
    {
        if (frames == null || frames.Length == 0 || _currentFps <= 0f) return;

        _frameTimer += Time.deltaTime * _currentFps;
        if (_frameTimer >= 1f)
        {
            _frameTimer -= 1f;
            _frameIndex = (_frameIndex + 1) % frames.Length;
            wingImage.sprite = frames[_frameIndex];
        }
    }
}