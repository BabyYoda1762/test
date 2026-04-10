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
    private Coroutine moveCoroutine;

    public bool IsUp => isUp;

    void Start()
    {
        
        downPos = transform.position;
        upPos = downPos + Vector3.up * popHeight;

        
        if (manager == null)
            manager = FindObjectOfType<ShootingRangeManager>();

        ResetTarget(); 
    }

    public void PopUp()
    {
        if (isUp || moveCoroutine != null) return;

        moveCoroutine = StartCoroutine(MoveToPosition(upPos, true));
    }

    public void Hit(bool countScore = true)
    {
        if (!isUp) return;

        
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        
        moveCoroutine = StartCoroutine(MoveToPosition(downPos, false));

       
        if (countScore && manager != null)
            manager.OnTargetHit();
    }

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

    private IEnumerator MoveToPosition(Vector3 target, bool goingUp)
    {
        Vector3 start = transform.position;
        float t = 0;

        
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        
        transform.position = target;
        isUp = goingUp; 
        moveCoroutine = null; 
    }
}