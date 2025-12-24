using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResolutionDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    // Ключ для сохранения выбранного индекса
    private const string RESOLUTION_INDEX_KEY = "ResolutionIndex";

    // Массив разрешений (в том же порядке, что и в дропдауне)
    private readonly Resolution[] resolutions = new Resolution[]
    {
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 1600, height = 900 },
        new Resolution { width = 1280, height = 720 },
        new Resolution { width = 1024, height = 768 },
        new Resolution { width = 800,  height = 600 }
    };

    void Start()
    {
        if (dropdown == null)
        {
            Debug.LogError("TMP_Dropdown не привязан в ResolutionDropdown!");
            return;
        }

        // Загружаем сохранённый индекс (по умолчанию — 0, т.е. 1920x1080)
        int savedIndex = PlayerPrefs.GetInt(RESOLUTION_INDEX_KEY, 0);

        // Защита от ошибок (если сохранённый индекс вне диапазона)
        savedIndex = Mathf.Clamp(savedIndex, 0, resolutions.Length - 1);

        // Устанавливаем значение в дропдаун
        dropdown.value = savedIndex;

        // Сразу применяем сохранённое разрешение (важно для меню и при запуске)
        ApplyResolution(savedIndex);

        // Подписываемся на изменения
        dropdown.onValueChanged.AddListener(index =>
        {
            ApplyResolution(index);
            PlayerPrefs.SetInt(RESOLUTION_INDEX_KEY, index);
            PlayerPrefs.Save(); // Сохраняем сразу
        });
    }

    void ApplyResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);

        // Если хочешь сохранить режим экрана (полноэкранный/оконный), можно добавить отдельный toggle
        // Сейчас всегда fullscreen = true, как у тебя было
    }
}