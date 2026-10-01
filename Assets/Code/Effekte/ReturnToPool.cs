using UnityEngine;
using UnityEngine.Pool;

public class ReturnToPool : MonoBehaviour
{
    public IObjectPool<ParticleSystem> pool;
    private ParticleSystem _ps;

    void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
        var main = _ps.main;
        main.stopAction = ParticleSystemStopAction.Callback; // ruft OnParticleSystemStopped
    }

    void OnParticleSystemStopped()
    {
        pool.Release(_ps);
    }
}