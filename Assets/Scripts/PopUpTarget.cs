using UnityEngine;
using System.Collections;

public class PopUpTarget : MonoBehaviour
{
    public ShootingRangeManager manager; 
    public float popHeight = 2.5f; 
    public float moveSpeed = 2f; 

    private Vector3 upPos; 
    private Vector3 downPos; 
    private bool isUp = false; 
    private Coroutine moveCoroutine; // Чтобы можно было остановить движение

    // Свойство чтобы снаружи узнать поднята ли мишень
    public bool IsUp => isUp;

    void Start()
    {
        
        downPos = transform.position;
        upPos = downPos + Vector3.up * popHeight;

        
        if (manager == null)
            manager = FindObjectOfType<ShootingRangeManager>();

        ResetTarget(); 
    }

    // Вызывается чтобы поднять мишень
    public void PopUp()
    {
        if (isUp || moveCoroutine != null) return; // Уже поднята или движется

        moveCoroutine = StartCoroutine(MoveToPosition(upPos, true));
    }

    // Вызывается когда в мишень попадают
    public void Hit(bool countScore = true)
    {
        if (!isUp) return; // Если не поднята - нахуй не надо

        
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        
        moveCoroutine = StartCoroutine(MoveToPosition(downPos, false));

       
        if (countScore && manager != null)
            manager.OnTargetHit();
    }

    // Сбрасываем мишень в начальное состояние
    public void ResetTarget()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        transform.position = downPos;
        isUp = false;
    }

    // КЕбань для плавного движения мишени
    private IEnumerator MoveToPosition(Vector3 target, bool goingUp)
    {
        Vector3 start = transform.position;
        float t = 0;

        
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null; // Ждем следующий кадр
        }

        
        transform.position = target;
        isUp = goingUp; 
        moveCoroutine = null; 
    }
}