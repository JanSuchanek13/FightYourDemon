using UnityEngine;
using System.Collections.Generic;

public class DamageNumberManager : MonoBehaviour
{
    [SerializeField] private DamageNumber prefab;
    [SerializeField] private Transform container;   // ein Screen-Space-Canvas (oder Child davon)
    [SerializeField] private Camera cam;            // meist die Hauptkamera

    private readonly Queue<DamageNumber> _pool = new Queue<DamageNumber>();

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    // an Weltposition eine Schadenszahl zeigen
    public void Show(float damage, Vector3 worldPos, Color color)
    {
        DamageNumber dn = _pool.Count > 0 ? _pool.Dequeue()
                                          : Instantiate(prefab, container);
        dn.Show(damage, worldPos, cam, color, Return);
    }


    private void Return(DamageNumber dn)
    {
        dn.gameObject.SetActive(false);
        _pool.Enqueue(dn);
    }
}