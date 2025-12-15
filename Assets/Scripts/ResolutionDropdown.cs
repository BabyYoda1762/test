using UnityEngine;
using TMPro; // если используешь TMP_Dropdown

public class ResolutionDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    void Start()
    {
        // Подписываемся на событие выбора
        dropdown.onValueChanged.AddListener(SetResolutionFromDropdown);
    }

    void SetResolutionFromDropdown(int index)
    {
        switch (index)
        {
            case 0: Screen.SetResolution(1920, 1080, true); break;
            case 1: Screen.SetResolution(1600, 900, true); break;
            case 2: Screen.SetResolution(1280, 720, true); break;
            case 3: Screen.SetResolution(1024, 768, true); break;
            case 4: Screen.SetResolution(800, 600, true); break;
        }
    }
}