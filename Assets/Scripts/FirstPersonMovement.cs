using System.Collections.Generic;
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

    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Time.timeScale == 0f)
        {
            rigidbody.linearVelocity = new Vector3(0, rigidbody.linearVelocity.y, 0);
            return;
        }

        IsRunning = canRun && Input.GetKey(runningKey);

        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        Vector2 targetVelocity = new Vector2(
            Input.GetAxis("Horizontal") * targetMovingSpeed,
            Input.GetAxis("Vertical") * targetMovingSpeed
        );

        Vector3 velocity = transform.rotation * new Vector3(targetVelocity.x, 0, targetVelocity.y);
        velocity.y = rigidbody.linearVelocity.y;
        rigidbody.linearVelocity = velocity;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Сохраняем позицию и поворот игрока
            PlayerPrefs.SetFloat("PlayerPosX", transform.position.x);
            PlayerPrefs.SetFloat("PlayerPosY", transform.position.y);
            PlayerPrefs.SetFloat("PlayerPosZ", transform.position.z);
            PlayerPrefs.SetFloat("PlayerRotY", transform.eulerAngles.y);
            PlayerPrefs.Save();

            // Разблокируем курсор
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Загружаем сцену главного меню
            SceneManager.LoadScene("Main Menu"); // Замени "MainMenu" на точное имя твоей сцены меню
        }
    }
}