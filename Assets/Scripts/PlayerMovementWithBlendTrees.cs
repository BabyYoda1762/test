using UnityEngine;

public class PlayerRunAnimationController : MonoBehaviour
{
    public GameObject playerModel; // Ссылка на объект модели игрока
    public float walkSpeed = 2f; // Скорость ходьбы
    public float runSpeed = 5f; // Скорость бега

    private Animator animator; // Ссылка на компонент Animator
    private float currentSpeed; // Текущая скорость

    void Start()
    {
        // Получаем компонент Animator из объекта playerModel
        if (playerModel != null)
        {
            animator = playerModel.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("Player model is not assigned!");
        }

        // Устанавливаем начальную скорость ходьбы
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        // Получаем ввод от игрока
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Проверяем, нажата ли клавиша Shift
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed = runSpeed;
            }
            else
            {
                currentSpeed = walkSpeed;
            }

            // Передаем параметры в Animator
            animator.SetFloat("moveX", horizontal);
            animator.SetFloat("moveY", vertical);
            animator.SetFloat("Speed", currentSpeed);
        }
        else
        {
            // Если игрок не двигается, сбрасываем параметры
            animator.SetFloat("moveX", 0);
            animator.SetFloat("moveY", 0);
            animator.SetFloat("Speed", 0);
        }
    }
}