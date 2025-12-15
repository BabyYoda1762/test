using UnityEngine;
using UnityEngine.UI;

public class MenuSettings : MonoBehaviour
{
    public Slider sensitivitySlider;
    public Slider fovSlider;
    public Button resetButton;

    // Значения по умолчанию
    public float defaultSensitivity = 2f;
    public float defaultFOV = 60f;

    void Start()
    {
        // Загружаем сохранённые значения (или дефолтные)
        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", defaultSensitivity);
        fovSlider.value = PlayerPrefs.GetFloat("FOV", defaultFOV);

        // Подписка на изменения
        sensitivitySlider.onValueChanged.AddListener(v => PlayerPrefs.SetFloat("Sensitivity", v));
        fovSlider.onValueChanged.AddListener(v => PlayerPrefs.SetFloat("FOV", v));

        // Подписка на кнопку Reset
        resetButton.onClick.AddListener(ResetDefaults);
    }

    void ResetDefaults()
    {
        // Возвращаем дефолтные значения
        sensitivitySlider.value = defaultSensitivity;
        fovSlider.value = defaultFOV;

        // Сохраняем их в PlayerPrefs
        PlayerPrefs.SetFloat("Sensitivity", defaultSensitivity);
        PlayerPrefs.SetFloat("FOV", defaultFOV);
    }
}
