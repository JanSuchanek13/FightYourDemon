using System;
using UnityEngine;
using UnityEngine.Events;

public class HeightStageController : MonoBehaviour
{
    [Serializable]
    public class HeightStage
    {
        [Tooltip("Name nur zur Uebersicht im Inspector")]
        public string stageName = "Neue Stufe";

        [Tooltip("Untere Grenze (inklusive)")]
        public float minHeight = 0f;

        [Tooltip("Obere Grenze (exklusive)")]
        public float maxHeight = 100f;

        [Tooltip("Wird gefeuert, wenn der Spieler diesen Bereich betritt!!!!!!!!!!!! Super wichtig das es auch wieder raus genommen wird!!!!!!!!!!!!")]
        public UnityEvent onEnter;

        [Tooltip("Wird gefeuert, wenn der Spieler diesen Bereich verlaesst")]
        public UnityEvent onExit;

        [NonSerialized] public bool isActive;

        public bool Contains(float height)
        {
            return height >= minHeight && height < maxHeight;
        }
    }

    [Header("Referenzen")]
    [SerializeField] private Transform player;

    [Header("Stufen")]
    // Die zuletzt betretene Stufe (null, bevor eine betreten wurde)
    public HeightStage CurrentStage;
    [SerializeField] private HeightStage[] stages;

    [Header("Einstellungen")]
    [Tooltip("Sekunden zwischen den Pruefungen (0 = jeden Frame)")]
    [SerializeField] private float checkInterval = 0.1f;

    

    private float _timer;

    void Update()
    {
        if (player == null) return;

        _timer -= Time.deltaTime;
        if (_timer > 0f) return;
        _timer = checkInterval;

        float height = player.position.y;

        // 1. enter/exit wie gehabt feuern
        foreach (var stage in stages)
        {
            bool nowInside = stage.Contains(height);

            if (nowInside && !stage.isActive)
            {
                stage.isActive = true;
                stage.onEnter?.Invoke();
            }
            else if (!nowInside && stage.isActive)
            {
                stage.isActive = false;
                stage.onExit?.Invoke();
            }
        }

        // 2. CurrentStage neu bestimmen: die spezifischste der aktiven Stufen
        CurrentStage = GetMostSpecificActiveStage();
    }

    // Von allen gerade aktiven Stufen die mit dem kleinsten Hoehenbereich zurueckgeben.
    // So gewinnt eine Unterstufe (500-1000) ueber ihre Rahmenstufe (0-1000).
    private HeightStage GetMostSpecificActiveStage()
    {
        HeightStage best = null;
        float bestRange = float.MaxValue;

        foreach (var stage in stages)
        {
            if (!stage.isActive) continue;

            float range = stage.maxHeight - stage.minHeight;
            if (range < bestRange)
            {
                bestRange = range;
                best = stage;
            }
        }

        return best; // null, wenn der Spieler in keiner Stufe ist
    }
}