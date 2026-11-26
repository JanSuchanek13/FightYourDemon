using UnityEngine;
using UnityEngine.SceneManagement;
using CodeMonkey.Utils;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private Transform buttons;
    [SerializeField] private AudioSettings settings;
    [SerializeField] private GameObject settingsUI;
    private bool _isOpenSettings;

    [Header("Menue")]
    [SerializeField] private GameObject pausePanel;
    private bool _isOpen;

    [Header("Stop Things")]
    public bool openMenu;
    public MouseLook mouseLook;

    private void Awake()
    {
        buttons.Find("Resume").GetComponent<Button_UI>().ClickFunc = () =>
        {
            ToggleMenu();
        };

        buttons.Find("Settings").GetComponent<Button_UI>().ClickFunc = () =>
        {
            OpenSettings();
        };

        buttons.Find("Main Menu").GetComponent<Button_UI>().ClickFunc = () =>
        {
            LoadScene(0);
        };


        // Menue startet geschlossen
        if (pausePanel != null) pausePanel.SetActive(false);
        _isOpen = false;
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

    private void OpenSettings()
    {
        settingsUI.SetActive(!_isOpenSettings);
        _isOpenSettings = !_isOpenSettings;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleMenu();
    }

    public void ToggleMenu()
    {
        _isOpen = !_isOpen;
        if (pausePanel != null) pausePanel.SetActive(_isOpen);


        if (openMenu) //ist ein anderes Menu offen für das man den Cursor braucht
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Zeit anhalten und Cursor freigeben, wenn offen
            Time.timeScale = _isOpen ? 0f : 1f;
            Cursor.lockState = _isOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = _isOpen;
            WeaponController.Instance.enabled = !_isOpen;
            mouseLook.GetComponent<MouseLook>().enabled = !_isOpen;
        }
    }
}