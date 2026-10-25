using System.Collections;
using System.Collections.Generic;
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
    }

    // Update is called once per frame
    void Update()
    {
        float limitY = BottomLevel.transform.position.y;

        for (int i = 0; i < crates.Count; i++)
        {
            GameObject item = crates[i];

            if (item.transform.position.y >= limitY) continue;
            if (!pendingResets.Add(item)) continue; // läuft schon -> überspringen

            ResetCrate(item, false);
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
        foreach (var item in crates)
        {
            ResetCrate(item, true);
        }
    }

    public void ResetCrate(GameObject crate, bool startnew)
    {
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
