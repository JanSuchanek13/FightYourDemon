using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
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