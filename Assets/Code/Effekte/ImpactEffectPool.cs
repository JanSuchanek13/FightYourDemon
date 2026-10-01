using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ImpactEffectPool : MonoBehaviour
{
    public static ImpactEffectPool Instance;

    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 100;

    // ein eigener Pool pro Prefab
    private readonly Dictionary<ParticleSystem, ObjectPool<ParticleSystem>> _pools
        = new Dictionary<ParticleSystem, ObjectPool<ParticleSystem>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Effekt eines bestimmten Prefabs an Position/Rotation abspielen
    public void Play(ParticleSystem prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return;

        var pool = GetOrCreatePool(prefab);
        var ps = pool.Get();
        ps.transform.SetPositionAndRotation(position, rotation);
        ps.Play();
    }

    private ObjectPool<ParticleSystem> GetOrCreatePool(ParticleSystem prefab)
    {
        // schon ein Pool fuer dieses Prefab? -> zurueckgeben
        if (_pools.TryGetValue(prefab, out var existing))
            return existing;

        // sonst neuen Pool fuer dieses Prefab anlegen
        ObjectPool<ParticleSystem> pool = null;
        pool = new ObjectPool<ParticleSystem>(
            createFunc: () =>
            {
                var ps = Instantiate(prefab);
                var ret = ps.gameObject.AddComponent<ReturnToPool>();
                ret.pool = pool; // merkt sich seinen eigenen Pool
                return ps;
            },
            actionOnGet: ps => ps.gameObject.SetActive(true),
            actionOnRelease: ps => ps.gameObject.SetActive(false),
            actionOnDestroy: ps => Destroy(ps.gameObject),
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );

        _pools.Add(prefab, pool);
        return pool;
    }
}