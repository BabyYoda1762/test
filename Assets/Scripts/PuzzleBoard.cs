using UnityEngine;

public class PuzzleBoard : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera mainCamera;
    public Camera puzzleCamera;

    [Header("Grid Settings")]
    public int rows = 3;
    public int cols = 3;

    public Tile[,] tiles;
    public Vector2Int emptyCell;
    public Transform puzzleRoot;

    public bool IsActive { get; private set; } = false;

    void Awake()
    {
        tiles = new Tile[rows, cols];
    }

    void Start()
    {
        // Заполняем массив плиток по row/col
        Tile[] found = puzzleRoot.GetComponentsInChildren<Tile>(true);
        foreach (var t in found)
        {
            tiles[t.row, t.col] = t;
        }

        FindEmptyCell();
    }

    void Update()
    {
        if (IsActive)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DeactivatePuzzle();
            }

            if (Input.GetMouseButtonDown(0)) // ЛКМ
            {
                Ray ray = puzzleCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Tile tile = hit.collider.GetComponentInParent<Tile>();
                    if (tile != null)
                    {
                        TryMove(tile);
                    }
                }
            }
        }
    }

    private void FindEmptyCell()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (tiles[r, c] == null)
                {
                    emptyCell = new Vector2Int(r, c);
                    return;
                }
            }
        }
    }

    public void TryMove(Tile tile)
    {
        if (tile == null) return;

        int dist = Mathf.Abs(tile.row - emptyCell.x) + Mathf.Abs(tile.col - emptyCell.y);
        if (dist == 1) // соседняя клетка
        {
            // Меняем местами в массиве
            tiles[emptyCell.x, emptyCell.y] = tile;
            tiles[tile.row, tile.col] = null;

            // Сохраняем старые координаты пустой клетки
            Vector2Int oldEmpty = new Vector2Int(tile.row, tile.col);

            // Меняем row/col плитки
            tile.row = emptyCell.x;
            tile.col = emptyCell.y;
            emptyCell = oldEmpty;

            // Меняем позиции в мире
            Vector3 tilePos = tile.transform.position;
            Vector3 emptyPos = puzzleRoot.position + new Vector3(oldEmpty.y, 0, -oldEmpty.x);

            tile.transform.position = emptyPos;
        }
    }

    public void ActivatePuzzle()
    {
        if (mainCamera == null || puzzleCamera == null) return;
        if (IsActive) return;

        mainCamera.gameObject.SetActive(false);
        puzzleCamera.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        IsActive = true;
    }

    public void DeactivatePuzzle()
    {
        if (mainCamera == null || puzzleCamera == null) return;
        if (!IsActive) return;

        puzzleCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IsActive = false;
    }
}
