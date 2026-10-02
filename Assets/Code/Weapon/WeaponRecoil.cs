using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [Header("Rueckkehr")]
    [Tooltip("Wie schnell der Recoil aufgebaut wird")]
    [SerializeField] private float snappiness = 10f;
    [Tooltip("Wie schnell die Kamera wieder zur Ruhe zurueckkehrt")]
    [SerializeField] private float returnSpeed = 6f;

    // Ziel-Recoil (wohin die Kamera gerade geschoben wird) und aktueller Wert
    private Vector3 _targetRot;
    private Vector3 _currentRot;

    void Update()
    {
        // Ziel sanft zurueck auf null ziehen (Recoil klingt ab)
        _targetRot = Vector3.Lerp(_targetRot, Vector3.zero, returnSpeed * Time.deltaTime);
        // aktuellen Wert dem Ziel annaehern (weiches Hochruckeln)
        _currentRot = Vector3.Slerp(_currentRot, _targetRot, snappiness * Time.deltaTime);

        // als lokale Rotation anwenden
        transform.localRotation = Quaternion.Euler(_currentRot);
    }

    // beim Schuss aufrufen; Werte kommen aus deinem ScriptableObject
    public void ApplyRecoil(float vertical, float horizontal, float z = 0f)
    {
        _targetRot += new Vector3(
            -vertical,                                   // nach oben kicken
            Random.Range(-horizontal, horizontal),       // leicht zufaellig seitlich
            z);
    }
}