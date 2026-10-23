using System;
using UnityEngine;

public class EnvironmentParticleManager : MonoBehaviour
{
    [Serializable]
    public class EnvEffect
    {
        public string name;                 // zur Uebersicht im Inspector
        public ParticleSystem system;       // das Partikelsystem
        [Tooltip("Hoehenversatz zum Spieler. Positiv = ueber dem Spieler (fallend), negativ = darunter (steigend)")]
        public float heightOffset = 10f;
    }

    [SerializeField] private Transform player;
    [SerializeField] private EnvEffect[] effects;
    [Tooltip("Welcher Effekt beim Start aktiv ist (Index, -1 = keiner)")]
    [SerializeField] private int startIndex = 0;

    private EnvEffect _active;

    void Start()
    {
        // alle erst mal ausschalten
        foreach (var e in effects)
            if (e.system != null) e.system.Stop();

        if (startIndex >= 0 && startIndex < effects.Length)
            SwitchTo(startIndex);
    }

    void LateUpdate()
    {
        if (player == null || _active == null || _active.system == null) return;

        // Emitter des aktiven Effekts in der Hoehe nachfuehren (X/Z bleiben)
        Vector3 pos = _active.system.transform.position;
        pos.y = player.position.y + _active.heightOffset;
        _active.system.transform.position = pos;
    }

    // --- Effekt per Index wechseln ---
    public void SwitchTo(int index)
    {
        if (index < 0 || index >= effects.Length) return;

        // alten Effekt sanft ausklingen lassen (emittiert nichts Neues mehr,
        // vorhandene Partikel leben aus)
        if (_active != null && _active.system != null)
            _active.system.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        _active = effects[index];
        if (_active.system != null)
            _active.system.Play();
    }

    // --- Effekt per Name wechseln (bequem fuer Events) ---
    public void SwitchTo(string effectName)
    {
        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i].name == effectName)
            {
                SwitchTo(i);
                return;
            }
        }
        Debug.LogWarning($"EnvironmentParticleManager: Effekt '{effectName}' nicht gefunden.");
    }

    // --- alle Effekte aus ---
    public void ClearEffect()
    {
        if (_active != null && _active.system != null)
            _active.system.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        _active = null;
    }
}