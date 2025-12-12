using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    public int number;
    public int row, col;
    public TextMeshPro textMesh;
    [HideInInspector] public PuzzleBoard board;
    private Vector3 originalScale;
    private bool isHovered = false;
    void Awake()
    {
        originalScale = transform.localScale;
    }
    public void SetNumber(int n)
    {
        number = n;
        if (textMesh != null)
            textMesh.text = n.ToString();
    }
    void OnMouseDown()
    {
        if (board != null && board.IsActive)
            board.TryMoveTile(this);
    }
    void OnMouseEnter()
    {
        if (board != null && board.IsActive && !isHovered)
        {
            isHovered = true;
            transform.localScale = originalScale * 1.1f;
        }
    }
    void OnMouseExit()
    {
        if (isHovered)
        {
            isHovered = false;
            transform.localScale = originalScale;
        }
    }
    void OnEnable()
    {
        if (originalScale == Vector3.zero)
            originalScale = transform.localScale;
    }
}