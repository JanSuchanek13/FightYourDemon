using UnityEngine;
using UnityEngine.SceneManagement;
using CodeMonkey.Utils;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Transform buttons;

    private void Awake()
    {
        buttons.Find("Start").GetComponent<Button_UI>().ClickFunc = () =>
        {
            LoadScene(1);
        };

        buttons.Find("Settings").GetComponent<Button_UI>().ClickFunc = () =>
        {
            LoadScene(1);
        };

        buttons.Find("Quit").GetComponent<Button_UI>().ClickFunc = () =>
        {
            QuitGame();
        };
    }

    // Szene per Name laden (Button: diese Methode + Szenenname eintragen)
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;   // sicherstellen, dass die Zeit laeuft (falls aus Pause)
        SceneManager.LoadScene(sceneName);
    }

    // Szene per Build-Index laden
    public void LoadScene(int buildIndex)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(buildIndex);
    }

    // aktuelle Szene neu laden (z.B. fuer "Nochmal"/Retry)
    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Spiel beenden
    public void QuitGame()
    {
        Debug.Log("Quit"); // im Editor sichtbar; echtes Beenden nur im Build
        Application.Quit();
    }
}