using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    public int number;
    public int row, col;
    public TextMeshPro textMesh;

    public void SetNumber(int n)
    {
        number = n;
        if (textMesh != null)
            textMesh.text = n.ToString();
    }

    void OnMouseDown()
    {
        PuzzleBoard board = FindObjectOfType<PuzzleBoard>();
        if (board != null && board.IsActive)
        {
            board.TryMove(this);
        }
    }
}
