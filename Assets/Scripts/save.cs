using UnityEngine;

public class SettingsApplier : MonoBehaviour
{
    public float defaultSensitivity = 2f;
    public float defaultFOV = 60f;

    private const string SENS_KEY = "Sensitivity";
    private const string FOV_KEY = "FOV";

    private FirstPersonLook lookScript;
    private Camera playerCamera;

    void Start()
    {
        // Применяем сразу при старте сцены
        ApplySettings();
    }

    void Update()
    {
        // Если по какой-то причине компоненты ещё не найдены (редко, но на всякий)
        if (lookScript == null || playerCamera == null)
        {
            FindComponents();
            ApplySettings();
        }
    }

    void FindComponents()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
                playerCamera = FindObjectOfType<Camera>();
        }

        if (lookScript == null)
        {
            lookScript = FindObjectOfType<FirstPersonLook>();
        }
    }

    void ApplySettings()
    {
        FindComponents();

        float sens = PlayerPrefs.GetFloat(SENS_KEY, defaultSensitivity);
        float fov = PlayerPrefs.GetFloat(FOV_KEY, defaultFOV);

        if (lookScript != null)
        {
            lookScript.sensitivity = sens;
            // Debug.Log($"Применена сенситивити: {sens}");
        }

        if (playerCamera != null)
        {
            playerCamera.fieldOfView = fov;
            // Debug.Log($"Применён FOV: {fov}");
        }
    }
}