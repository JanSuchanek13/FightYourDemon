using UnityEngine;
using TMPro;

public class HeightUpgradeManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Upgrade Height Settings")]
    public float startHeight = 30f;
    public float heightMultiplier = 1.1f;

    [Header("Time Slowdown")]
    public float slowDownDuration = 1.2f;

    [Header("UI")]
    public UpgradeUI upgradeUI;

    private float nextUpgradeHeight;
    private int stage = 0;
    private bool upgradeTriggered;

    void Start()
    {
        //CursorManager.HideCursor();
        //Cursor.lockState = CursorLockMode.Locked;
        nextUpgradeHeight = startHeight;
    }

    void Update()
    {
        if (upgradeTriggered) return;

        float currentHeight = player.position.y;

        if (currentHeight >= nextUpgradeHeight)
        {
            stage += 1;
            upgradeTriggered = true;
            StartCoroutine(SlowTimeAndOpenUpgrade());
        }
    }

    System.Collections.IEnumerator SlowTimeAndOpenUpgrade()
    {
        float t = 0f;
        float startTimeScale = Time.timeScale;

        while (t < slowDownDuration)
        {
            t += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(startTimeScale, 0f, t / slowDownDuration);
            yield return null;
        }

        Time.timeScale = 0f;
        upgradeUI.Show(this, stage);
    }

    public void ApplyUpgrade()
    {
        // Nächstes Upgrade-Ziel berechnen
        nextUpgradeHeight *= heightMultiplier;
        upgradeTriggered = false;

        StartCoroutine(ResumeTime());
    }

    System.Collections.IEnumerator ResumeTime()
    {
        float t = 0f;

        while (t < 0.5f)
        {
            t += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(0f, 1f, t / 0.5f);
            yield return null;
        }

        Time.timeScale = 1f;
    }
}
