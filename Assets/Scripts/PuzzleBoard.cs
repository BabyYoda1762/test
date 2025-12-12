using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PuzzleBoard : MonoBehaviour
{
    [Header("Grid")]
    public int rows = 3;
    public int cols = 3;
    public Transform puzzleRoot;

    [Header("Cameras")]
    public Camera mainCamera;
    public Camera puzzleCamera;

    [Header("Animation")]
    public float moveDuration = 0.15f;

    [Header("Puzzle Settings")]
    public int shuffleCount = 50;

    private Tile[,] tiles;
    private Vector3[,] cellCenters;
    private Vector2Int emptyCell = new Vector2Int(2, 2);

    public bool IsActive { get; private set; }

    void Awake()
    {
        tiles = new Tile[rows, cols];
        cellCenters = new Vector3[rows, cols];
    }

    void Start()
    {
        if (puzzleRoot == null)
        {
            Debug.LogError("Puzzle Root не назначен!");
            return;
        }

        CacheTilesFromHierarchy();

        // Пустая ячейка — правый нижний угол
        emptyCell = new Vector2Int(rows - 1, cols - 1);
        tiles[emptyCell.x, emptyCell.y] = null;
        cellCenters[emptyCell.x, emptyCell.y] = CalculateEmptyCellCenter();

       //Нумерация этой пебени
        int num = 1;
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                if (tiles[r, c] != null)
                    tiles[r, c].SetNumber(num++);
    }

    void Update()
    {
        if (!IsActive) return; // Если пазл не активен — нахуй обновления
        if (Input.GetKeyDown(KeyCode.Escape))
            DeactivatePuzzle();
    }

    private void CacheTilesFromHierarchy()
    {
        var children = puzzleRoot.GetComponentsInChildren<Tile>(true);
        List<Tile> activeTiles = new List<Tile>();
        // Фильтруем только активные — выключенные нахуй не нужны
        foreach (var tile in children)
            if (tile.gameObject.activeInHierarchy)
            {
                activeTiles.Add(tile);
            }
        // Проверяем: должно быть РОВНО 8 тайлов, иначе пиздец
        if (activeTiles.Count != 8)
        {
            Debug.LogError($"Должно быть 8 тайлов, найдено: {activeTiles.Count}");
            return;
        }

        int index = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (r == rows - 1 && c == cols - 1) continue;

                Tile tile = activeTiles[index];
                tiles[r, c] = tile;
                cellCenters[r, c] = tile.transform.position;

                tile.row = r;
                tile.col = c;
                tile.board = this;

                index++;
            }
        }
    }

    private Vector3 CalculateEmptyCellCenter()
    {
        // Тайл (2,1)
        if (tiles[2, 1] != null)
        {
            Vector3 left = tiles[2, 1].transform.position;
            Vector3 prev = tiles[2, 0] != null ? tiles[2, 0].transform.position : left;
            Vector3 step = left - prev;
            return left + step;
        }

        // жёсткая позиция (как говориться надёжно зафиксированый пациент в анестезии не нуждается)
        return new Vector3(1.7f, -0.34f, 0.329f);
    }

    public void TryMoveTile(Tile tile)
    {
        if (!IsActive || tile == null) return;

        int dr = Mathf.Abs(tile.row - emptyCell.x);
        int dc = Mathf.Abs(tile.col - emptyCell.y);

        // Двигаем только если рядом с пустой ячейкой (по горизонтали или вертикали)
        if ((dr == 1 && dc == 0) || (dr == 0 && dc == 1))
            MoveTile(tile);
    }

    private void MoveTile(Tile tile)
    {
        Vector3 target = cellCenters[emptyCell.x, emptyCell.y]; // Куда едем
        Vector2Int oldPos = new Vector2Int(tile.row, tile.col); // Откуда

        //Движение в массиве
        tiles[emptyCell.x, emptyCell.y] = tile;
        tiles[oldPos.x, oldPos.y] = null;

        // Обновляем коорды
        tile.row = emptyCell.x;
        tile.col = emptyCell.y;
        emptyCell = oldPos;

        StartCoroutine(SmoothMove(tile, target));
    }

    private IEnumerator SmoothMove(Tile tile, Vector3 target)
    {
        Vector3 start = tile.transform.position;
        float t = 0;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            tile.transform.position = Vector3.Lerp(start, target, t / moveDuration);
            yield return null;
        }
        tile.transform.position = target; // На всякий случай — фикс
    }

    public void ActivatePuzzle()
    {
        if (IsActive) return;
        if (mainCamera) mainCamera.gameObject.SetActive(false);
        if (puzzleCamera) puzzleCamera.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        IsActive = true;
    }

    public void DeactivatePuzzle()
    {
        if (!IsActive) return;
        if (puzzleCamera) puzzleCamera.gameObject.SetActive(false);
        if (mainCamera) mainCamera.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IsActive = false;
    }

    // Эта поебень чтобы пятнашки каждый раз при вхоже перемешивались
    public void Shuffle()
    {
        System.Random rng = new System.Random();
        Vector2Int[] directions = {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0)
        };

        for (int i = 0; i < shuffleCount; i++)
        {
            List<Vector2Int> possible = new List<Vector2Int>();
            foreach (var dir in directions)
            {
                Vector2Int pos = emptyCell + dir;
                if (pos.x >= 0 && pos.x < rows && pos.y >= 0 && pos.y < cols)
                    possible.Add(pos);
            }

            if (possible.Count == 0) continue;

            Vector2Int chosen = possible[rng.Next(possible.Count)];
            Tile tile = tiles[chosen.x, chosen.y];
            if (tile != null)
                MoveTileInstant(tile); // Двигаем мгновенно — без анимации
        }
    }

    private void MoveTileInstant(Tile tile)
    {
        Vector3 target = cellCenters[emptyCell.x, emptyCell.y];
        Vector2Int oldPos = new Vector2Int(tile.row, tile.col);

        tiles[emptyCell.x, emptyCell.y] = tile;
        tiles[oldPos.x, oldPos.y] = null;

        tile.row = emptyCell.x;
        tile.col = emptyCell.y;
        emptyCell = oldPos;

        tile.transform.position = target;
    }
}