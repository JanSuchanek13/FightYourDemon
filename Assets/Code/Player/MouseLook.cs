using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Settings")]
    public float sensitivityX = 1f;   // horizontal (Umschauen links/rechts)
    public float sensitivityY = 1f;   // vertikal (hoch/runter)
    public Transform playerBody;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        sensitivityX = PlayerPrefs.GetFloat("SensitivityX", sensitivityX);
        sensitivityY = PlayerPrefs.GetFloat("SensitivityY", sensitivityY);
    }

    void Update()   // pro Frame, nicht im Physik-Takt
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void SetSensitivityX(float value) => sensitivityX = value;
    public void SetSensitivityY(float value) => sensitivityY = value;
}