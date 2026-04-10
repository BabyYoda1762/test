using UnityEngine;

public class PlayerRunAnimationController : MonoBehaviour
{
    public GameObject playerModel;
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    private Animator animator;
    private float currentSpeed;

    void Start()
    {
        if (playerModel != null)
        {
            animator = playerModel.GetComponent<Animator>();
        }

        currentSpeed = walkSpeed;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed = runSpeed;
            }
            else
            {
                currentSpeed = walkSpeed;
            }

            animator.SetFloat("moveX", horizontal);
            animator.SetFloat("moveY", vertical);
            animator.SetFloat("Speed", currentSpeed);
        }
        else
        {
            animator.SetFloat("moveX", 0);
            animator.SetFloat("moveY", 0);
            animator.SetFloat("Speed", 0);
        }
    }
}