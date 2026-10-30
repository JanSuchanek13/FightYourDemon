using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageBanner : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bannerText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Timing")]
    [SerializeField] private float popScale = 1.6f;
    [SerializeField] private float popReturnSpeed = 10f;
    [SerializeField] private float holdTime = 1.2f;   // wie lange voll sichtbar
    [SerializeField] private float fadeTime = 0.6f;   // ausblenden danach

    // welche Stages in diesem Run schon gezeigt wurden
    private readonly HashSet<int> _shownStages = new HashSet<int>();
    private int _highestStageReached = 0;
    private Coroutine _co;

    void Start()
    {
        if (canvasGroup != null) canvasGroup.alpha = 0f;
    }

    // vom Stage-onEnter aufrufen, mit der Stage-Nummer
    public void ShowStage(int stage)
    {
        // nur bei AUFSTIEG: Stage muss hoeher sein als die hoechste bisher erreichte
        if (stage <= _highestStageReached) return;

        // nur EINMAL pro Stage pro Run
        if (_shownStages.Contains(stage)) return;

        _highestStageReached = stage;
        _shownStages.Add(stage);

        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(BannerRoutine(stage));
    }

    private IEnumerator BannerRoutine(int stage)
    {
        bannerText.text = "STAGE " + stage;
        canvasGroup.alpha = 1f;
        bannerText.transform.localScale = Vector3.one * popScale;

        // Aufpoppen zurueckfedern + halten
        float t = 0f;
        while (t < holdTime)
        {
            t += Time.deltaTime;
            bannerText.transform.localScale = Vector3.Lerp(
                bannerText.transform.localScale, Vector3.one,
                popReturnSpeed * Time.deltaTime);
            yield return null;
        }

        // ausblenden
        t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        _co = null;
    }

    // bei Run-Neustart aufrufen, damit die Banner wieder erscheinen koennen
    public void ResetForNewRun()
    {
        _shownStages.Clear();
        _highestStageReached = 0;
    }
}