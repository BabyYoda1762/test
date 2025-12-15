using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5;
    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;

    Rigidbody rigidbody;
    public System.Collections.Generic.List<System.Func<float>> speedOverrides = new System.Collections.Generic.List<System.Func<float>>();

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        // восстановление позиции при старте
        if (PlayerPrefs.HasKey("PlayerPosX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerPosX");
            float y = PlayerPrefs.GetFloat("PlayerPosY");
            float z = PlayerPrefs.GetFloat("PlayerPosZ");
            float rotY = PlayerPrefs.GetFloat("PlayerRotY");

            transform.position = new Vector3(x, y, z);
            transform.eulerAngles = new Vector3(0, rotY, 0);
        }
    }

    void FixedUpdate()
    {
        IsRunning = canRun && Input.GetKey(runningKey);

        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed,
                                             Input.GetAxis("Vertical") * targetMovingSpeed);

        rigidbody.linearVelocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.linearVelocity.y, targetVelocity.y);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // сохраняем позицию перед выходом в меню
            PlayerPrefs.SetFloat("PlayerPosX", transform.position.x);
            PlayerPrefs.SetFloat("PlayerPosY", transform.position.y);
            PlayerPrefs.SetFloat("PlayerPosZ", transform.position.z);
            PlayerPrefs.SetFloat("PlayerRotY", transform.eulerAngles.y);
            PlayerPrefs.Save();

            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SceneManager.LoadScene("MainMenu");
        }
    }
}
