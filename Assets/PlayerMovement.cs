using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float runSpeed = 8f;

    private Animator anim;

    void Start()
    {
        // Находим аниматор у ребёнка (персонажа)
        anim = GetComponentInChildren<Animator>();
    }

    void LateUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Анимации (на персонаже)
        anim.SetFloat("moveX", h);     // если направления не те — поменяй h и v местами
        anim.SetFloat("moveY", v);
        anim.SetFloat("Speed", (Mathf.Abs(h) + Mathf.Abs(v)) > 0.1f ? 1f : 0f);

        // Движение всего PlayerRoot (и персонаж, и камера двигаются вместе идеально)
        Vector3 move = new Vector3(h, 0, v).normalized;
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        transform.position += move * speed * Time.deltaTime;
    }
}