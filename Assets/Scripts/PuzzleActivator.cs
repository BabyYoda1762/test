using UnityEngine;

public class PuzzleActivator : MonoBehaviour
{
    public PuzzleBoard board;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E)) // например, кнопка E
        {
            board.ActivatePuzzle();
        }
    }
}
