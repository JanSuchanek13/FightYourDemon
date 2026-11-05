using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RisingEnvironment : MonoBehaviour
{
    public static RisingEnvironment Instance;

    [Header("Player")]
    public GameObject player;

    public GameObject TopLevel;
    public GameObject BottomLevel;
    public List<GameObject> crates = new List<GameObject>();

    [Header("Height")]
    public HeightStageController heightStage;
    public int currentStage = 1;

    public float distanceBoost = 0;
    public float flyingBoost = 1;


    [Header("Demon")]
    public int DemonCount;
    public int BulletDemonCount;
    public int MoneyDemonCount;
    public int ArcaneDemonCount;
    public int OrbitDemonCount;

    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem SpawnPrefab;
    [SerializeField] private float resetDelay = 0.5f;
    private WaitForSeconds resetWait;
    private readonly HashSet<GameObject> pendingResets = new HashSet<GameObject>();

    [Header("Sounds")]
    public AudioClip hitGroundSound;

    [Header("Boss")]
    public int bossCount;

    [Header("Boden-Status")]
    [SerializeField] private FallDeathReload playerGround;   // vom Player
    private bool _wasGrounded;

    [Header("StartNew")]
    [Tooltip("Wie viele Sekunden lang nach dem Aufprall gespawnt wird")]
    public float startNewSeconds = 10f;

    private float _spawnBudget;   // verbleibendes Kontingent; <0 = unbegrenzt

    [Header("Spawning")]
    public List<DemonType> demonTypes = new List<DemonType>();
    [Tooltip("Daemonen pro Sekunde")]
    public float spawnRate = 2f;
    [Tooltip("Max. Spawns pro Frame (gegen Nachhol-Bursts bei Framedrops)")]
    public int maxSpawnsPerFrame = 3;

    private float _spawnTimer;
    private DemonType _normalType;
    private readonly Dictionary<DestructibleCrate.demonClass, DemonType> _byClass = new();
    private readonly List<GameObject> _active = new();
    private readonly Dictionary<GameObject, DemonType> _activeType = new();

    [System.Serializable]
    public class DemonType
    {
        public string label;                              // nur zur Uebersicht
        public DestructibleCrate.demonClass demonClass;
        public Transform sourceFolder;                    // Ordner mit allen Objekten dieses Typs
        [Tooltip("true = spawnt per Wahrscheinlichkeit; false = der Normal-Typ (ueber die Rate)")]
        public bool special;
        [Range(0f, 1f)] public float spawnChance;         // nur fuer special
        [HideInInspector] public Queue<GameObject> pool;
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        player = GameObject.FindGameObjectWithTag("Player");

        DemonCount = transform.GetChild(0).childCount;
        UpdateDemonList();

        resetWait = new WaitForSeconds(resetDelay);

        foreach (var type in demonTypes) //Spawn Queues
        {
            type.pool = new Queue<GameObject>();
            if (type.sourceFolder != null)
            {
                foreach (Transform child in type.sourceFolder)
                {
                    child.gameObject.SetActive(false);
                    type.pool.Enqueue(child.gameObject);
                }
            }
            _byClass[type.demonClass] = type;
            if (!type.special) _normalType = type;   // der eine Normal-Typ
        }

        if (playerGround == null && player != null)
            playerGround = player.GetComponent<FallDeathReload>();
        _wasGrounded = false;

        _spawnBudget = -1f;   // Standard: laeuft dauerhaft
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Normal-Pool: " + _normalType.pool.Count + " | aktiv: " + _active.Count);
        // --- Boden-Zustand auswerten ---
        bool grounded = playerGround != null && playerGround.isGrounded;

        if (grounded && !_wasGrounded)
        {
            // gerade aufgekommen -> StartNew-Verhalten (Sound + Kontingent)
            OnLanded();
        }
        else if (!grounded && _wasGrounded)
        {
            // gerade abgehoben -> Deckel weg, normale Rate
            _spawnBudget = -1f;   // unbegrenzt
        }
        _wasGrounded = grounded;

        // 1) zu tiefe Daemonen zurueck in ihre Queue (kein sofortiger Respawn mehr)
        float limitY = BottomLevel.transform.position.y;
        for (int i = _active.Count - 1; i >= 0; i--)
        {
            GameObject d = _active[i];
            if (d == null) { _active.RemoveAt(i); continue; }
            if (d.transform.position.y < limitY)
                ReturnDemon(d, i);
        }

        // 2) Spawn-Takt nach fester Rate
        if (spawnRate <= 0f) return;
        float interval = 1f / spawnRate;
        _spawnTimer += Time.deltaTime;

        int spawnedThisFrame = 0;
        while (_spawnTimer >= interval && spawnedThisFrame < maxSpawnsPerFrame)
        {
            _spawnTimer -= interval;
            if (!SpawnOne()) break;   // nichts gespawnt (Kontingent leer o. Vorrat leer)
            spawnedThisFrame++;
        }
    }

    public void NotifyDemonKilled(GameObject demon)
    {
        int idx = _active.IndexOf(demon);
        if (idx >= 0)
            ReturnDemon(demon, idx);   // sofort zurueck in die Queue
    }

    /*void SpawnOne()
    {
        if (_spawnBudget == 0f) return;   // Kontingent aufgebraucht -> nichts mehr

        // Spezialtypen der Reihe nach durchgehen; erster Treffer gewinnt
        foreach (var type in demonTypes)
        {
            if (!type.special || type.pool.Count == 0) continue;
            if (Random.value < type.spawnChance)
            {
                Activate(type);
                return;   // statt eines normalen wird dieser Spezialtyp gespawnt
            }
        }
        // kein Spezial getroffen -> Normal
        if (_normalType != null && _normalType.pool.Count > 0)
            Activate(_normalType);

        if (_spawnBudget > 0f) _spawnBudget -= 1f;
    }*/

    bool SpawnOne()
    {
        if (_spawnBudget == 0f) return false;

        foreach (var type in demonTypes)
        {
            if (!type.special || type.pool.Count == 0) continue;
            if (Random.value < type.spawnChance)
            {
                Activate(type);
                if (_spawnBudget > 0f) _spawnBudget -= 1f;
                return true;
            }
        }
        if (_normalType != null && _normalType.pool.Count > 0)
        {
            Activate(_normalType);
            if (_spawnBudget > 0f) _spawnBudget -= 1f;
            return true;
        }
        return false;
    }

    void Activate(DemonType type)
    {
        GameObject demon = type.pool.Dequeue();

        // Zielposition oben (wie in deinem bisherigen "nicht startnew"-Zweig)
        float x = Random.Range(BottomLevel.transform.position.x, TopLevel.transform.position.x);
        float z = Random.Range(TopLevel.transform.position.z, BottomLevel.transform.position.z);
        float y = Random.Range(
            TopLevel.transform.position.y - ((TopLevel.transform.position.y - BottomLevel.transform.position.y) / 6f),
            TopLevel.transform.position.y);
        Vector3 pos = new Vector3(x, y, z);
        demon.transform.position = pos;

        ImpactEffectPool.Instance.Play(SpawnPrefab, pos, Quaternion.Euler(90f, 0f, 0f));

        // Stage bestimmen (wie bisher)
        float stageF = float.Parse(heightStage.CurrentStage.stageName, System.Globalization.NumberStyles.Float);
        currentStage = (int)stageF;

        // Initialisieren + aktivieren
        if (type.demonClass == DestructibleCrate.demonClass.OrbitDemon) //alle Objecte in Orbit Demon
        {
            foreach (Transform sub in demon.transform)
                sub.gameObject.SetActive(true);
            demon.SetActive(true);
            foreach (Transform sub in demon.transform)
                sub.GetComponent<DestructibleCrate>().ResetCrate(currentStage);
        }
        else
        {
            demon.SetActive(true);
            demon.GetComponent<DestructibleCrate>().ResetCrate(currentStage);
        }

        _active.Add(demon);
        _activeType[demon] = type;
    }

    void ReturnDemon(GameObject demon, int activeIndex)
    {
        demon.SetActive(false);
        if (_activeType.TryGetValue(demon, out var type))
        {
            type.pool.Enqueue(demon);
            _activeType.Remove(demon);
        }
        else
        {
            Debug.LogWarning("Daemon ohne _activeType-Eintrag zurueckgenommen: " + demon.name);
        }
        _active.RemoveAt(activeIndex);
    }

    void ReturnAllDemons()
    {
        // von hinten nach vorne, weil ReturnDemon aus _active entfernt
        for (int i = _active.Count - 1; i >= 0; i--)
        {
            GameObject d = _active[i];
            if (d == null) { _active.RemoveAt(i); continue; }
            ReturnDemon(d, i);
        }
    }

    public void UpdateDemonList() //automatically get all Demons
    {
        crates.Clear();
        foreach (Transform demon in transform.GetChild(0))
        {
            crates.Add(demon.transform.gameObject);
        }
    }

    public void StartNew()
    {
        AudioManager.Instance.Play2D(hitGroundSound, 1f);
        _spawnBudget = spawnRate * startNewSeconds;   // so viele wie in 10 s
        _spawnTimer = 0f;
    }

    private void OnLanded()
    {
        AudioManager.Instance.Play2D(hitGroundSound, 1f);
        ReturnAllDemons();                            // alle aktiven zurueck in die Queues
        _spawnBudget = spawnRate * startNewSeconds;   // Deckel: so viele wie in 10 s
        _spawnTimer = 0f;
    }

    public void ResetCrate(GameObject crate, bool startnew)
    {
        //Debug.Log("Reset" + crate.GetComponent<DestructibleCrate>()._demonClass);
        if (crate.GetComponent<DestructibleCrate>() != null && crate.GetComponent<DestructibleCrate>()._demonClass == DestructibleCrate.demonClass.MiniDemon)
        {
            if (!startnew)
            {
                ImpactEffectPool.Instance.Play(SpawnPrefab, crate.transform.position, Quaternion.Euler(90f, 0f, 0f));
            }
            DespawnMiniDemon(crate.transform);
            crate.GetComponent<DestructibleCrate>().ResetCrate(currentStage);
            return;
        }

        float currentStageFloat = float.Parse(heightStage.CurrentStage.stageName, System.Globalization.NumberStyles.Float); //stage Namen als float umwandeln
        currentStage = (int)currentStageFloat;                                                                              //float als int umwandeln
        float x = Random.Range(BottomLevel.transform.position.x, TopLevel.transform.position.x);
        float z = Random.Range(TopLevel.transform.position.z, BottomLevel.transform.position.z);

        if (startnew) //y position komplett zurück setzen
        {
            float y = Random.Range(TopLevel.transform.position.y, player.transform.position.y -10);
            crate.transform.position = new Vector3(x, y, z);
            StartCoroutine(ResetCrate2Routine(crate, new Vector3(x, y, z)));
        }
        else
        {
            float y = Random.Range(TopLevel.transform.position.y - ((TopLevel.transform.position.y - BottomLevel.transform.position.y) / 6), TopLevel.transform.position.y);
            if (crate.activeInHierarchy)
            {
                ImpactEffectPool.Instance.Play(SpawnPrefab, crate.transform.position, Quaternion.Euler(90f, 0f, 0f));
            }
            StartCoroutine(ResetCrate2Routine(crate, new Vector3(x, y, z)));
        }
        
        
        
    }


    private IEnumerator ResetCrate2Routine(GameObject crate, Vector3 newPos)
    {
        ImpactEffectPool.Instance.Play(SpawnPrefab, newPos, Quaternion.Euler(90f, 0f, 0f));
        yield return resetWait;

        pendingResets.Remove(crate); // in jedem Fall freigeben

        if (crate == null) yield break; // Crate wurde in der Zwischenzeit zerstört

        crate.transform.position = newPos;
        if (crate.name.Contains("OrbitDemon"))
        {
            foreach (Transform Demon in crate.transform)
            {
                Demon.GetComponent<DestructibleCrate>().ResetCrate(currentStage);
                Demon.gameObject.active = true;
            }
        }
        else
        {
            crate.GetComponent<DestructibleCrate>().ResetCrate(currentStage);
            crate.gameObject.active = true;
        }
    }

    public void BoostPlayer(float playerBoostForce, Vector3 demonPos)
    {
        if (player.TryGetComponent(out Rigidbody rb))
        {
            // nur ein Runterfallen abfangen, vorhandenen Aufwaertsschwung behalten
            if (rb.linearVelocity.y < 0f)
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            playerBoostForce += (Vector3.Distance(player.transform.position, demonPos)*2) * distanceBoost; //Distance Boost addieren
            playerBoostForce = playerBoostForce * flyingBoost; //Flying Boost multi

            rb.AddForce(Vector3.up * playerBoostForce, ForceMode.Impulse);
        }
    }

    public void StageBoost(float playerBoostForce)
    {
        BoostPlayer(playerBoostForce, player.transform.position);
    }

    public void UpdateX()
    {
        float topX = TopLevel.transform.position.x / 1.5f;
        float bottomX = BottomLevel.transform.position.x / 1.5f;

        TopLevel.transform.position = new Vector3(topX, TopLevel.transform.position.y, TopLevel.transform.position.z);
        BottomLevel.transform.position = new Vector3(bottomX, BottomLevel.transform.position.y, BottomLevel.transform.position.z);

        MoneyManager.Instance.SpendHorns(ConfigArtefact.Instance.stage1Price);
    }

    public void UpdateZ()
    {
        float topZ = TopLevel.transform.position.z + 20;
        float bottomZ = BottomLevel.transform.position.z +20;

        TopLevel.transform.position = new Vector3(TopLevel.transform.position.x, TopLevel.transform.position.y, topZ);
        BottomLevel.transform.position = new Vector3(BottomLevel.transform.position.x, BottomLevel.transform.position.y, bottomZ);

        MoneyManager.Instance.SpendHorns(ConfigArtefact.Instance.stage1Price);
    }

    public void UpdateDemon(int add)
    {
        for (int i = 0; i < add; i++)
        {
            transform.GetChild(1).transform.GetChild(0).transform.gameObject.active = true;
            transform.GetChild(1).transform.GetChild(0).transform.parent = transform.GetChild(0).transform;
        }
        DemonCount += add;
        UpdateDemonList();
    }

    public void UpdateBulletDemon(int add)
    {
        for (int i = 0; i < add; i++)
        {
            transform.GetChild(2).transform.GetChild(0).transform.gameObject.active = true;
            transform.GetChild(2).transform.GetChild(0).transform.parent = transform.GetChild(0).transform;
        }
        BulletDemonCount += add;
        UpdateDemonList();
    }

    public void UpdateMoneyDemon(int add)
    {
        for (int i = 0; i < add; i++)
        {
            transform.GetChild(3).transform.GetChild(0).transform.gameObject.active = true;
            transform.GetChild(3).transform.GetChild(0).transform.parent = transform.GetChild(0).transform;
        }
        MoneyDemonCount += add;
        UpdateDemonList();
    }

    public void UpdateArcaneDemon(int add)
    {
        for (int i = 0; i < add; i++)
        {
            transform.GetChild(4).transform.GetChild(0).transform.gameObject.active = true;
            transform.GetChild(4).transform.GetChild(0).transform.parent = transform.GetChild(0).transform;
        }
        ArcaneDemonCount += add;
        UpdateDemonList();
    }

    public void UpdateOrbitDemon(int add)
    {
        for (int i = 0; i < add; i++)
        {
            transform.GetChild(6).transform.GetChild(0).transform.gameObject.active = true;
            transform.GetChild(6).transform.GetChild(0).transform.parent = transform.GetChild(0).transform;
        }
        OrbitDemonCount += add;
        UpdateDemonList();
    }

    public void UpdateBombDemon(int add) //noch falsch weil Orbit Demon Ordner verknüpft
    {
        for (int i = 0; i < add; i++)
        {
            transform.GetChild(6).transform.GetChild(0).transform.gameObject.active = true;
            transform.GetChild(6).transform.GetChild(0).transform.parent = transform.GetChild(0).transform;
        }
        OrbitDemonCount += add;
        UpdateDemonList();
    }

    public void SpawnMiniDemon(Vector3 spawnPos) 
    {
        transform.GetChild(7).transform.GetChild(0).transform.gameObject.active = true;
        transform.GetChild(7).transform.GetChild(0).transform.position = spawnPos;
        transform.GetChild(7).transform.GetChild(0).transform.parent = transform.GetChild(0).transform;
        UpdateDemonList();
    }

    public void DespawnMiniDemon(Transform demon) 
    {
        demon.gameObject.active = false;
        demon.parent = transform.GetChild(7).transform;
    }

    public void ShowBossDemon(int numb)
    {
        if (numb >= bossCount)
        {
            transform.GetChild(5).transform.GetChild(numb).transform.gameObject.active = true;
            transform.GetChild(5).transform.GetChild(numb).transform.position = player.transform.position + new Vector3(0, 20, 100);
        }
    }

    public void HideBossDemon(int numb)
    {
        if (numb >= bossCount)
        {
            transform.GetChild(5).transform.GetChild(numb).transform.gameObject.active = false;
            transform.GetChild(5).transform.GetChild(numb).transform.position = new Vector3(0, -20, 0);
        }
    }

    public void UpdateDistanceBoost(float add)
    {
        distanceBoost += add;
    }

    public void UpdateFlyingBoost(float add)
    {
        flyingBoost += add;
    }
}
