using UnityEngine;

public class ConfigArtefact : MonoBehaviour
{
    public static ConfigArtefact Instance;

    public int stage1Price = 10;


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

}
