using UnityEngine;
using UnityEngine.SceneManagement;

public class FallDeathReload : MonoBehaviour
{
    [Header("Settings")]
    public float deathFallHeight = 30f;
    public LayerMask groundLayer;
    public RisingEnvironment risingEnvironment;
    public WeaponController weaponController;

    private float highestY;
    public bool isGrounded;
    private bool hasFallenEnough;
    private bool sceneReload;

    void Start()
    {
        highestY = transform.position.y;
    }

    void Update()
    {
        // Höchste Position merken
        if (!isGrounded && transform.position.y > highestY)
        {
            highestY = transform.position.y;
        }

        // Fallhöhe prüfen
        float fallDistance = highestY - transform.position.y;
        hasFallenEnough = fallDistance >= deathFallHeight;

        if (transform.position.y < 2 && weaponController.currentmaxAmmo == 0) //prüft damit Ammo nie auf dem Boden leer geht
        {
            weaponController.currentmaxAmmo = weaponController.maxAmmo;
            weaponController.SetUp();
        }

        if (transform.position.y < 0) //prüft falls man durch Plattform fällt
        {
            risingEnvironment.StartNew();
            transform.position = new Vector3(0,1,10);
        }
    }

    
    void OnCollisionEnter(Collision collision)
    {
        // Prüfen ob Boden
        if (IsGround(collision.gameObject))
        {
            isGrounded = true;

            if (hasFallenEnough)
            {
                if (sceneReload) //soll die scene neu geladen werden, wenn man Abstürzt?
                {
                    ReloadScene();
                }
                else
                {
                    Debug.Log("Reload");
                    risingEnvironment.StartNew();
                    //weaponController.currentmaxAmmo = weaponController.maxAmmo;
                    //weaponController.SetUp();
                }
            }

            // Reset
            highestY = transform.position.y;
            hasFallenEnough = false;
        }
    }
    

    void OnCollisionExit(Collision collision)
    {
        if (IsGround(collision.gameObject))
        {
            isGrounded = false;
        }
    }

    bool IsGround(GameObject obj)
    {
        return ((1 << obj.layer) & groundLayer) != 0;
    }

    void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}