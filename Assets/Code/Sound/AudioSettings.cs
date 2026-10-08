using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AudioSettings : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("Slider")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Menue")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Beispiel-Sound")]
    [SerializeField] private AudioClip sfxPreviewClip;
    [SerializeField] private AudioClip musicPreviewClip;

    private const string MASTER = "MasterVol";
    private const string MUSIC = "MusicVol";
    private const string SFX = "SFXVol";

    private bool _isOpen;

    void Start()
    {
        float master = PlayerPrefs.GetFloat(MASTER, 1f);
        float music = PlayerPrefs.GetFloat(MUSIC, 1f);
        float sfx = PlayerPrefs.GetFloat(SFX, 1f);

        masterSlider.SetValueWithoutNotify(master);
        musicSlider.SetValueWithoutNotify(music);
        sfxSlider.SetValueWithoutNotify(sfx);

        ApplyVolume(MASTER, master);
        ApplyVolume(MUSIC, music);
        ApplyVolume(SFX, sfx);

        masterSlider.onValueChanged.AddListener(v => SetMaster(v));
        musicSlider.onValueChanged.AddListener(v => SetMusic(v));
        sfxSlider.onValueChanged.AddListener(v => SetSfx(v));

        // Beispiel-Sounds beim Loslassen (jeder Slider sein eigener)
        AddReleaseListener(masterSlider, PlaySfxPreview);   // Master ueber SFX hoerbar machen
        AddReleaseListener(musicSlider, PlayMusicPreview);
        AddReleaseListener(sfxSlider, PlaySfxPreview);

        // Menue startet geschlossen
        if (settingsPanel != null) settingsPanel.SetActive(false);
        _isOpen = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleMenu();
    }

    private void ToggleMenu()
    {
        _isOpen = !_isOpen;
        if (settingsPanel != null) settingsPanel.SetActive(_isOpen);

        // Zeit anhalten und Cursor freigeben, wenn offen
        Time.timeScale = _isOpen ? 0f : 1f;
        Cursor.lockState = _isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _isOpen;
    }

    public void SetMaster(float value) => ChangeAndSave(MASTER, value);
    public void SetMusic(float value) => ChangeAndSave(MUSIC, value);
    public void SetSfx(float value) => ChangeAndSave(SFX, value);

    private void ChangeAndSave(string param, float value)
    {
        ApplyVolume(param, value);
        PlayerPrefs.SetFloat(param, value);
        PlayerPrefs.Save();
    }

    private void ApplyVolume(string param, float value)
    {
        float dB = (value <= 0.0001f) ? -80f : Mathf.Log10(value) * 20f;
        mixer.SetFloat(param, dB);
    }

    // haengt an einen Slider ein "losgelassen"-Event mit einer bestimmten Aktion
    private void AddReleaseListener(Slider slider, UnityEngine.Events.UnityAction action)
    {
        var trigger = slider.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = slider.gameObject.AddComponent<EventTrigger>();

        var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        entry.callback.AddListener(_ => action());
        trigger.triggers.Add(entry);
    }

    private void PlaySfxPreview()
    {
        if (sfxPreviewClip != null && AudioManager.Instance != null)
            AudioManager.Instance.Play2D(sfxPreviewClip);
    }

    private void PlayMusicPreview()
    {
        if (musicPreviewClip != null && AudioManager.Instance != null)
            AudioManager.Instance.PlayMusicPreview(musicPreviewClip);
    }
}