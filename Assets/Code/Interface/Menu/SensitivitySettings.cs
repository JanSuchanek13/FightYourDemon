using UnityEngine;
using UnityEngine.UI;

public class SensitivitySettings : MonoBehaviour
{
    [SerializeField] private MouseLook mouseLook;
    [SerializeField] private Slider sliderX;
    [SerializeField] private Slider sliderY;

    [SerializeField] private float minSensitivity = 0.5f;
    [SerializeField] private float maxSensitivity = 10f;

    private const string KEY_X = "SensitivityX";
    private const string KEY_Y = "SensitivityY";

    void Start()
    {
        float savedX = PlayerPrefs.GetFloat(KEY_X, 2f);
        float savedY = PlayerPrefs.GetFloat(KEY_Y, 2f);

        SetupSlider(sliderX, savedX);
        SetupSlider(sliderY, savedY);

        if (mouseLook != null)
        {
            mouseLook.SetSensitivityX(savedX);
            mouseLook.SetSensitivityY(savedY);
        }

        sliderX.onValueChanged.AddListener(OnChangedX);
        sliderY.onValueChanged.AddListener(OnChangedY);
    }

    private void SetupSlider(Slider s, float value)
    {
        s.minValue = minSensitivity;
        s.maxValue = maxSensitivity;
        s.SetValueWithoutNotify(value);
    }

    private void OnChangedX(float value)
    {
        if (mouseLook != null) mouseLook.SetSensitivityX(value);
        PlayerPrefs.SetFloat(KEY_X, value);
        PlayerPrefs.Save();
    }

    private void OnChangedY(float value)
    {
        if (mouseLook != null) mouseLook.SetSensitivityY(value);
        PlayerPrefs.SetFloat(KEY_Y, value);
        PlayerPrefs.Save();
    }
}