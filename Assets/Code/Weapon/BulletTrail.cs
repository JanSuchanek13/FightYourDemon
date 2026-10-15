using UnityEngine;
using System;

public class BulletTrail : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private float lifetime = 0.08f;   // sehr kurz -> wie ein Blitz
    [SerializeField] private AnimationCurve widthOverLife = AnimationCurve.Linear(0, 1, 1, 0);

    private float _timer;
    private float _baseWidth;
    private Action<BulletTrail> _onDone;

    void Awake()
    {
        _baseWidth = line.widthMultiplier;
    }

    public void Show(Vector3 start, Vector3 end, Action<BulletTrail> onDone)
    {
        _onDone = onDone;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        line.widthMultiplier = _baseWidth;
        _timer = lifetime;
        gameObject.SetActive(true);
    }

    void Update()
    {
        _timer -= Time.deltaTime;
        float t = Mathf.Clamp01(_timer / lifetime); // 1 -> 0

        // ausduennen ueber die Lebenszeit
        line.widthMultiplier = _baseWidth * widthOverLife.Evaluate(1f - t);

        if (_timer <= 0f)
            _onDone?.Invoke(this);
    }
}