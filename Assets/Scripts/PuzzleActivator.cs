using UnityEngine;

public class PuzzleActivator : MonoBehaviour
{
    public PuzzleBoard board;

    private bool playerInTrigger = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            if (board.IsActive)
            {
                board.DeactivatePuzzle();
            }
        }
    }

    void Update()
    {
        if (!playerInTrigger) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!board.IsActive)
            {
                board.ActivatePuzzle();
                board.Shuffle(); // запуск головоломки
            }
            else
            {
                board.DeactivatePuzzle(); // выход из головоломки
            }
        }
    }
}