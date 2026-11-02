using UnityEngine;

public class SplitDemonSpawn : MonoBehaviour
{
    [Header("Spalter")]
    [SerializeField] private int minSplits = 3;
    [SerializeField] private int maxSplits = 5;
    [Tooltip("Wie weit die Kleinen um den Todespunkt verteilt werden")]
    [SerializeField] private float splitRadius = 0.8f;

    public void SpawnSplitDemons(Vector3 center)
    {

        int count = Random.Range(minSplits, maxSplits + 1); // maxSplits inklusive

        for (int i = 0; i < count; i++)
        {
            // gleichmaessig im Kreis + etwas Zufall gegen perfekte Symmetrie
            float angle = (360f / count) * i + Random.Range(-15f, 15f);
            Vector3 offset = Quaternion.Euler(angle, angle, angle) * Vector3.forward
                             * splitRadius * Random.Range(0.6f, 1f);
            Vector3 spawnPos = center + offset;

            // >>> AN DEINEN SPAWNER ANPASSEN: recycelte Instanz des kleinen Prefabs holen <<<
            RisingEnvironment.Instance.SpawnMiniDemon(spawnPos);
        }
    }
}
