using UnityEngine;
using System.Collections.Generic;

public class BulletTrailPool : MonoBehaviour
{
    [SerializeField] private BulletTrail prefab;

    private readonly Queue<BulletTrail> _pool = new Queue<BulletTrail>();

    public void Spawn(Vector3 start, Vector3 end)
    {
        BulletTrail trail = _pool.Count > 0 ? _pool.Dequeue()
                                            : Instantiate(prefab, transform);
        trail.Show(start, end, Return);
    }

    private void Return(BulletTrail trail)
    {
        trail.gameObject.SetActive(false);
        _pool.Enqueue(trail);
    }
}