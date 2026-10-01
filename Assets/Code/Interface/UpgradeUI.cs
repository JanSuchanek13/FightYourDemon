using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    private HeightUpgradeManager manager;
    public MouseLook mouseLook;
    public GameObject Button1;
    public GameObject Button2;

    void Awake()
    {
        //gameObject.SetActive(false);
    }

    public void Show(HeightUpgradeManager upgradeManager, int stage)
    {
        CheckButtons(stage);
        manager = upgradeManager;
        gameObject.SetActive(true);
        CursorManager.ShowCursor();
        mouseLook.GetComponent<MouseLook>().enabled = false;
    }

    public void CheckButtons(int stage)
    {
        switch (stage)
        {
            case 1:
                if (ConfigArtefact.Instance.stage1Price > MoneyManager.Instance.GetHorns())
                {
                    Button1.GetComponent<Button>().interactable = false;
                    Button2.GetComponent<Button>().interactable = false;
                }
                break;
            default:
                if (ConfigArtefact.Instance.stage1Price > MoneyManager.Instance.GetHorns())
                {
                    Button1.GetComponent<Button>().interactable = false;
                    Button2.GetComponent<Button>().interactable = false;
                }
                break;
        }
    }

    public void SelectUpgrade()
    {
        gameObject.SetActive(false);
        CursorManager.HideCursor();
        Cursor.lockState = CursorLockMode.Locked;
        mouseLook.GetComponent<MouseLook>().enabled = true;
        manager.ApplyUpgrade();
    }

}
