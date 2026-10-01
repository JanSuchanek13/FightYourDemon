using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.

public class AltitudeHUD : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    private float record = 0;
    private float altitudeCalc = 0.0033f;
    private float goal = 300f;

    [Header("UI")]
    public GameObject ui_altitude;
    public Slider currentAlti;
    public Slider recordAlti;
    public Slider newRecordAlti;
    public Image cloud;

    private TMPro.TextMeshProUGUI _altitudeText;
    private FallDeathReload _fallDeath;

    void Start()
    {
        _altitudeText = ui_altitude.GetComponent<TMPro.TextMeshProUGUI>();
        _fallDeath = player.GetComponent<FallDeathReload>();
    }

    // Update is called once per frame
    void Update()
    {
        _altitudeText.text = player.position.y.ToString("F0");
        currentAlti.value = player.position.y * altitudeCalc;

        record = Mathf.Clamp(player.position.y, record, goal);
        newRecordAlti.value = record * altitudeCalc;
        if (_fallDeath.isGrounded) //set new Record as the Record
        {
            recordAlti.value = record * altitudeCalc;
        }
    }

    public void UpdateAltitudeCalc(int newgoal)
    {
        if(goal < newgoal)
        {
            altitudeCalc = 1f / newgoal;
            goal = newgoal;
            recordAlti.value = record * altitudeCalc;
        }
    }

    public void ChangeCloudTint(string hex)
    {
        if (ColorUtility.TryParseHtmlString(NormalizeHex(hex), out Color col))
        {
            cloud.color = col;
        }
    }

    // ergaenzt ein fehlendes '#', damit beide Schreibweisen funktionieren
    private string NormalizeHex(string hex)
    {
        if (string.IsNullOrEmpty(hex)) return hex;
        return hex.StartsWith("#") ? hex : "#" + hex;
    }
}
