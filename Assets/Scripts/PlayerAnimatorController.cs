using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [Header("References")]
    public Transform playerModel;

    [Header("Speeds")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;

    [Header("Smoothing")]
    public float smoothSpeed = 8f;

    public float moveDeadzone = 0.01f;

    private Animator animator;
    private float currentMoveX = 0f;
    private float currentMoveY = 0f;

    void Awake()
    {
        if (playerModel != null)
        {
            animator = playerModel.GetComponent<Animator>();
            if (animator == null)
            {
                animator = playerModel.GetComponentInChildren<Animator>();
                if (animator != null)
                {
                    playerModel = animator.transform;
                }
            }
        }
        else
        {
            animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                playerModel = animator.transform;
            }
            else
            {
                var found = transform.Find("Player Model");
                if (found != null)
                {
                    playerModel = found;
                    animator = playerModel.GetComponentInChildren<Animator>();
                }
            }
        }
    }

    void Update()
    {
        float rawX = Input.GetAxisRaw("Horizontal");
        float rawY = Input.GetAxisRaw("Vertical");

        Vector2 rawInput = new Vector2(rawX, rawY);

        float rawMag = rawInput.magnitude;
        float magClamped = Mathf.Clamp01(rawMag);

        Vector2 dir = rawMag > 0f ? rawInput / rawMag : Vector2.zero;

        float targetX = dir.x * magClamped;
        float targetY = dir.y * magClamped;

        currentMoveX = Mathf.Lerp(currentMoveX, targetX, Time.deltaTime * smoothSpeed);
        currentMoveY = Mathf.Lerp(currentMoveY, targetY, Time.deltaTime * smoothSpeed);

        if (Mathf.Abs(currentMoveX) < moveDeadzone) currentMoveX = 0f;
        if (Mathf.Abs(currentMoveY) < moveDeadzone) currentMoveY = 0f;

        animator.SetFloat("moveX", currentMoveX);
        animator.SetFloat("moveY", currentMoveY);

        bool wantRun = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool isMoving = magClamped > moveDeadzone;
        bool isRunning = wantRun && isMoving;

        animator.SetBool("isRunning", isRunning);

        float baseSpeed = isRunning ? runSpeed : walkSpeed;
        float speedFloat = baseSpeed * magClamped; 
        animator.SetFloat("speedFloat", speedFloat, 0.08f, Time.deltaTime);
    }
}