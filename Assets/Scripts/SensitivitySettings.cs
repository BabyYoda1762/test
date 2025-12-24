using UnityEngine;
using UnityEngine.UI;

public class MenuSettings : MonoBehaviour
{
    public Slider sensitivitySlider;
    public Slider fovSlider;

    public float defaultSensitivity = 2f;
    public float defaultFOV = 60f;

    private const string SENS_KEY = "Sensitivity";
    private const string FOV_KEY = "FOV";

    void Start()
    {
        if (sensitivitySlider == null || fovSlider == null)
        {
            Debug.LogError("Слайдеры не привязаны в MenuSettings!");
            return;
        }

        // Загружаем сохранённые значения
        float savedSens = PlayerPrefs.GetFloat(SENS_KEY, defaultSensitivity);
        float savedFov = PlayerPrefs.GetFloat(FOV_KEY, defaultFOV);

        sensitivitySlider.value = savedSens;
        fovSlider.value = savedFov;

        // Сразу сохраняем при изменении
        sensitivitySlider.onValueChanged.AddListener(value =>
        {
            PlayerPrefs.SetFloat(SENS_KEY, value);
            PlayerPrefs.Save();
        });

        fovSlider.onValueChanged.AddListener(value =>
        {
            PlayerPrefs.SetFloat(FOV_KEY, value);
            PlayerPrefs.Save();
        });
    }
}