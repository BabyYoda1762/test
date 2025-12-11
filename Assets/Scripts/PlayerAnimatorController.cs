using UnityEngine;

/// <summary>
/// Объединённый контроллер анимации движения и бега.
/// корректно находит Animator на дочернем объекте (Player Model).
/// </summary>
public class PlayerAnimatorController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Transform модели игрока (где висит Animator). Если пусто, скрипт попробует найти дочерний Animator или объект с именем 'Player Model'.")]
    public Transform playerModel;

    [Header("Speeds")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;

    [Header("Smoothing")]
    [Tooltip("Скорость сглаживания параметров moveX/moveY")]
    public float smoothSpeed = 8f;

    [Tooltip("Порог для счёта как 'есть движение'")]
    public float moveDeadzone = 0.01f;

    // internal
    private Animator animator;
    private float currentMoveX = 0f;
    private float currentMoveY = 0f;

    void Awake()
    {
        // Попробуем использовать назначенную модель
        if (playerModel != null)
        {
            animator = playerModel.GetComponent<Animator>();
            if (animator == null)
            {
                // если на самом playerModel нет Animator, попробуем найти в его дочерних
                animator = playerModel.GetComponentInChildren<Animator>();
                if (animator != null)
                {
                    // привяжем модель на уровень объекта, где висит Animator
                    playerModel = animator.transform;
                }
            }
        }
        else
        {
            // Если playerModel не назначен — попытаемся найти дочерний Animator у текущего объекта
            animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                playerModel = animator.transform;
            }
            else
            {
                // Попытка найти объект явно с именем "Player Model"
                var found = transform.Find("Player Model");
                if (found != null)
                {
                    playerModel = found;
                    animator = playerModel.GetComponentInChildren<Animator>();
                }
            }
        }

        if (animator == null)
        {
            Debug.LogError("[PlayerAnimatorController] Animator not found. Assign 'playerModel' in inspector or ensure there's an Animator on a child object (like 'Player Model'). Disabling script.");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        // Получаем "сырые" оси (значения -1,0,1), чтобы быть отзывчивыми
        float rawX = Input.GetAxisRaw("Horizontal");
        float rawY = Input.GetAxisRaw("Vertical");

        Vector2 rawInput = new Vector2(rawX, rawY);

        // magnitude может быть > 1 при диагонали => ограничиваем
        float rawMag = rawInput.magnitude;
        float magClamped = Mathf.Clamp01(rawMag); // диагональ -> 1, не 1.414

        // Direction (нормализованный) — если нет движения, ставим 0
        Vector2 dir = rawMag > 0f ? rawInput / rawMag : Vector2.zero;

        // Целевые параметры (в диапазоне -1..1)
        float targetX = dir.x * magClamped;
        float targetY = dir.y * magClamped;

        // Сглаживаем
        currentMoveX = Mathf.Lerp(currentMoveX, targetX, Time.deltaTime * smoothSpeed);
        currentMoveY = Mathf.Lerp(currentMoveY, targetY, Time.deltaTime * smoothSpeed);

        // Небольшой deadzone
        if (Mathf.Abs(currentMoveX) < moveDeadzone) currentMoveX = 0f;
        if (Mathf.Abs(currentMoveY) < moveDeadzone) currentMoveY = 0f;

        // Передаём moveX/moveY в Animator (используются для 2D Blend Tree)
        animator.SetFloat("moveX", currentMoveX);
        animator.SetFloat("moveY", currentMoveY);

        // Определяем бег — только если игрок держит Shift и есть движение
        bool wantRun = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool isMoving = magClamped > moveDeadzone;
        bool isRunning = wantRun && isMoving;

        animator.SetBool("isRunning", isRunning);

        // Передаём числовую скорость (опционально, можно использовать для переходов/скорости анимации)
        float baseSpeed = isRunning ? runSpeed : walkSpeed;
        float speedFloat = baseSpeed * magClamped; // в пределах от 0 до runSpeed
        animator.SetFloat("speedFloat", speedFloat, 0.08f, Time.deltaTime); // с damping
    }
}