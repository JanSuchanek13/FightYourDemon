using UnityEngine;

public class OrbitObjects : MonoBehaviour
{
    [Header("Objekte")]
    [Tooltip("Die Objekte, die kreisen sollen")]
    [SerializeField] private Transform[] objects;

    [Header("Einstellungen")]
    [Tooltip("Radius der Kreisbahn")]
    [SerializeField] private float radius = 3f;

    [Tooltip("Geschwindigkeit in Grad pro Sekunde")]
    [SerializeField] private float speed = 90f;

    [Tooltip("Achse, um die gekreist wird (Y = horizontal)")]
    [SerializeField] private Vector3 axis = Vector3.up;

    private float _angle;

    void Update()
    {
        if (objects == null || objects.Length == 0) return;

        _angle += speed * Time.deltaTime;

        int count = objects.Length;
        for (int i = 0; i < count; i++)
        {
            if (objects[i] == null) continue;

            // gleichmaessige Verteilung: jedes Objekt um 360/count versetzt
            float offset = (360f / count) * i;
            float rad = (_angle + offset) * Mathf.Deg2Rad;

            // Position auf dem Kreis in der lokalen XZ-Ebene
            Vector3 localPos = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius;

            // an die gewuenschte Achse anpassen und um das eigene Objekt legen
            localPos = Quaternion.FromToRotation(Vector3.up, axis.normalized) * localPos;
            objects[i].position = transform.position + localPos;
        }
    }
}