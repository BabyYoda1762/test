using UnityEngine;

public class AnimationDriver : MonoBehaviour
{
    public Transform playerModel;
    public Transform cameraTransform;

    [Header("Smoothness")]
    public float smoothSpeed = 8f;  // 5-12f: скорость перехода анимаций

    private Animator animator;
    private Transform modelTransform;

    private float currentMoveX = 0f;
    private float currentMoveY = 0f;

    void Start()
    {
        if (playerModel == null)
            playerModel = transform.Find("Player Person Camera/Player Model");

        modelTransform = playerModel;
        animator = modelTransform.GetComponent<Animator>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector2 targetInput = new Vector2(inputX, inputY).normalized;

        // ПЛАВНОЕ СГЛАЖИВАНИЕ
        currentMoveX = Mathf.Lerp(currentMoveX, targetInput.x, Time.deltaTime * smoothSpeed);
        currentMoveY = Mathf.Lerp(currentMoveY, targetInput.y, Time.deltaTime * smoothSpeed);

        // Порог для idle
        float threshold = 0.01f;
        if (Mathf.Abs(currentMoveX) < threshold) currentMoveX = 0f;
        if (Mathf.Abs(currentMoveY) < threshold) currentMoveY = 0f;

        animator.SetFloat("moveX", currentMoveX);
        animator.SetFloat("moveY", currentMoveY);
    }
}