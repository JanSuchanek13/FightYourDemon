using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup uiGroup;

    [Header("3D-Pool")]
    [Tooltip("Wie viele 3D-Quellen gleichzeitig spielen koennen")]
    [SerializeField] private int poolSize = 16;
    [Tooltip("Max. Hoerweite der 3D-Sounds")]
    [SerializeField] private float max3DDistance = 50f;

    // 2D-Quelle fuer Schuesse/UI
    private AudioSource _source2D;

    // 3D-Pool
    private readonly List<AudioSource> _pool3D = new List<AudioSource>();

    void Awake()
    {
        // einfaches Singleton (nicht szenenuebergreifend, wie gewuenscht)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SetupSources();
    }

    private void SetupSources()
    {
        // --- 2D-Quelle ---
        _source2D = gameObject.AddComponent<AudioSource>();
        _source2D.playOnAwake = false;
        _source2D.spatialBlend = 0f;          // 2D
        _source2D.outputAudioMixerGroup = sfxGroup;

        // --- 3D-Pool ---
        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject("PooledAudio3D_" + i);
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.spatialBlend = 1f;            // 3D
            src.rolloffMode = AudioRolloffMode.Linear;
            src.maxDistance = max3DDistance;
            src.outputAudioMixerGroup = sfxGroup;
            _pool3D.Add(src);
        }
    }

    // --- 2D: Schuesse, allgemeine SFX ---
    public void Play2D(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        _source2D.PlayOneShot(clip, volume);
    }

    // --- 2D: UI-Sounds (eigene Gruppe) ---
    public void PlayUI(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        // UI laeuft kurz ueber eine temporaere Zuweisung der Gruppe
        _source2D.outputAudioMixerGroup = uiGroup;
        _source2D.PlayOneShot(clip, volume);
        _source2D.outputAudioMixerGroup = sfxGroup; // zurueck auf SFX
    }

    // --- 3D: Treffer, Explosionen an einer Weltposition ---
    public void Play3D(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;

        AudioSource src = GetFreeSource();
        src.transform.position = position;
        src.clip = clip;
        src.volume = volume;
        src.Play();
    }

    // freie Quelle finden, sonst die am laengsten laufende recyceln
    private AudioSource GetFreeSource()
    {
        foreach (var src in _pool3D)
            if (!src.isPlaying) return src;

        // alle belegt -> die erste (aelteste) nehmen
        return _pool3D[0];
    }
}