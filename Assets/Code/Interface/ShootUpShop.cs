using UnityEngine;

public class ShootUpShop : MonoBehaviour, IDamageable
{
    public float health = 30f;
    public GameObject shopUI;
    public MouseLook mouseLook;
    public float slowDownDuration = 1.2f;


    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
            OpenShop();
    }

    void OpenShop()
    {
        //StartCoroutine(SlowTimeAndOpenUpgrade());
        //WeaponController.Instance.currentAmmo += 1; //keine Lösung für Shotgun
        WeaponController.Instance.enabled = false;
        shopUI.SetActive(true);
        CursorManager.ShowCursor();
        Cursor.lockState = CursorLockMode.None;   // Lock aktiv aufheben
        mouseLook.GetComponent<MouseLook>().enabled = false;
    }

    public void ResumeGame()
    {
        shopUI.SetActive(false);
        CursorManager.HideCursor();
        Cursor.lockState = CursorLockMode.Locked;
        mouseLook.GetComponent<MouseLook>().enabled = true;

        StartCoroutine(ResumeTime());
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
