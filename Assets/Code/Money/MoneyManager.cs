using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    [Header("UI")]
    public GameObject ui_money;
    public GameObject ui_horns;
    [SerializeField] private MoneyPopup popupPrefab;
    [SerializeField] private Transform popupContainer;  // vertikales Layout

    [Header("Effect")]
    [SerializeField] private float popScale = 1.3f;
    [SerializeField] private float popReturnSpeed = 8f;
    [SerializeField] private int maxPopups = 6;
    private Vector3 _totalBaseScale;
    private readonly Queue<MoneyPopup> _pool = new Queue<MoneyPopup>();
    private readonly List<MoneyPopup> _active = new List<MoneyPopup>();

    public int Money => money;

    [Header("Money")]
    private int money = 0;
    private int horns = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _totalBaseScale = ui_money.transform.localScale;
        UpdateTotal();
    }

    private void Update()
    {
        ui_money.GetComponent<TMPro.TextMeshProUGUI>().text = money.ToString();
        ui_horns.GetComponent<TMPro.TextMeshProUGUI>().text = horns.ToString();

        ui_money.transform.localScale = Vector3.Lerp(
            ui_money.transform.localScale, _totalBaseScale,
            Time.deltaTime * popReturnSpeed);
    }

    public int GetMoney()
    {
        return money;
    }

    public void AddMoney(int amount)
    {
        money += amount;

        UpdateTotal();

        // Gesamtanzeige aufpoppen
        ui_money.transform.localScale = _totalBaseScale * popScale;

        SpawnPopup(amount);
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            return true;
        }

        return false;
    }

    public int GetHorns()
    {
        return horns;
    }

    public void AddHorns(int amount)
    {
        horns += amount;
    }

    public bool SpendHorns(int amount)
    {
        if (horns >= amount)
        {
            horns -= amount;
            return true;
        }

        return false;
    }

    private void UpdateTotal()
    {
        ui_money.GetComponent<TMPro.TextMeshProUGUI>().text = money.ToString();
    }

    private void SpawnPopup(int amount)
    {
        // aeltestes entfernen, wenn zu viele
        if (_active.Count >= maxPopups)
        {
            var oldest = _active[0];
            _active.RemoveAt(0);
            ReturnPopup(oldest);
        }

        MoneyPopup popup = _pool.Count > 0 ? _pool.Dequeue()
                                           : Instantiate(popupPrefab, popupContainer);

        _active.Add(popup);
        popup.Show(amount, ReturnPopup);
    }

    private void ReturnPopup(MoneyPopup popup)
    {
        if (_active.Contains(popup)) _active.Remove(popup);
        popup.gameObject.SetActive(false);
        _pool.Enqueue(popup);   // recyceln statt zerstoeren
    }
}