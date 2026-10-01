using UnityEngine;

public class BulletHolePool : MonoBehaviour
{
    [SerializeField] private GameObject holePrefab;      // Quad mit dem BulletHole-Material
    [SerializeField] private int maxHoles = 50;          // so viele Loecher gleichzeitig
    [SerializeField] private float surfaceOffset = 0.01f; // leicht von der Wand abheben

    private GameObject[] _holes;
    private int _next;

    void Awake()
    {
        // alle Loecher einmal vorab erzeugen und ausschalten
        _holes = new GameObject[maxHoles];
        for (int i = 0; i < maxHoles; i++)
        {
            _holes[i] = Instantiate(holePrefab, transform);
            _holes[i].SetActive(false);
        }
    }

    // vom Raycast aufgerufen: naechstes freies (oder aeltestes) Loch platzieren
    public void Place(Vector3 point, Vector3 normal)
    {
        var hole = _holes[_next];
        _next = (_next + 1) % maxHoles; // Ringpuffer: nach dem letzten wieder von vorne

        hole.transform.SetPositionAndRotation(
            point + normal * surfaceOffset,
            Quaternion.LookRotation(-normal));
        hole.SetActive(true);
    }
}