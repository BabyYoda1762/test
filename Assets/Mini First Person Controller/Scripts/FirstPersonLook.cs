using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField] Transform playerBody; // объект "Игрок"
    public float sensitivity = 2f;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Горизонтальный поворот тела
        playerBody.Rotate(Vector3.up * mouseX);

        // Вертикальный поворот камеры
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
