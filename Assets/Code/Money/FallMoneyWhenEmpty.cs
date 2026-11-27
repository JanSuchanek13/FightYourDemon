using UnityEngine;

public class FallMoneyWhenEmpty : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private WeaponController weapon;

    [Tooltip("Geld pro gefallenem Meter")]
    [SerializeField] private int moneyPerMeter = 1;

    private float lastY;
    private float fallAccumulator;   // gesammelte Fallstrecke unter 1m

    void Start()
    {
        lastY = player.position.y;
    }

    void Update()
    {
        float currentY = player.position.y;
        float deltaDown = lastY - currentY;   // positiv, wenn gefallen
        lastY = currentY;

        // nur wenn Ammo komplett leer ist
        bool empty = weapon.currentAmmo == 0 && weapon.currentmaxAmmo == 0;
        if (!empty || deltaDown <= 0f)
            return;

        // gefallene Strecke sammeln und ganze Meter auszahlen
        fallAccumulator += deltaDown;
        if (fallAccumulator >= 1f)
        {
            int meters = Mathf.FloorToInt(fallAccumulator);
            fallAccumulator -= meters;
            MoneyManager.Instance.AddMoney(meters * moneyPerMeter);
        }
    }
}