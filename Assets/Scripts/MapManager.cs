using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static MapManager m_instance;
    public static MapManager Instance => m_instance;

    [SerializeField] Vector2Int m_mapSize;
    [SerializeField] Piece[] m_piecePrefabs;
    [SerializeField] Transform[] m_piecesHolder;
    [SerializeField] Transform m_squareContainer;

    private Collider2D[,] m_mapMatrix;
    private SpriteRenderer[,] m_mapContaint;
    private List<Piece> m_currentPieces = new List<Piece>();

    private void Awake()
    {
        if (m_instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        m_instance = this;
    }
    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) return;

        if (m_currentPieces.Count == 0)
        {
            SpawnPieces();
        }
    }

    private void PaintColor(Color color, List<Vector2Int> paintCoors)
    {
        color.a = 0.75f;

        for (int i = 0; i < m_mapSize.x; i++)
        {
            for (int j = 0; j < m_mapSize.y; j++)
            {
                if (paintCoors.Contains(new Vector2Int(i, j)))
                {
                    m_mapMatrix[i, j].GetComponent<SpriteRenderer>().color = color;
                }
                else
                {
                    m_mapMatrix[i, j].GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }
    }
    private void OnDragPiece(Piece piece, Vector2Int coordinate)
    {
        if (coordinate == new Vector2Int(-1, -1))
        {
            PaintColor(piece.Color, new List<Vector2Int>());
            return;
        }

        var startX = coordinate.x;
        var startY = coordinate.y;
        var patternsCoordinates = new List<Vector2Int>() { new Vector2Int(startX, startY)};

        foreach (var pattern in piece.Patterns)
        {
            var x = pattern.x + startX;
            var y = pattern.y + startY;
            patternsCoordinates.Add(new Vector2Int(x, y));
        }

        PaintColor(piece.Color, patternsCoordinates);
    }
    private void OnDropPiece(Piece piece, Vector2Int coordinate)
    {
        if (coordinate == new Vector2Int(-1, -1))
        {
            StartCoroutine(GameManager.IE_Scale(piece.Origin,
                                                Vector3.one, 
                                                piece.DefaultScale, 
                                                0.25f));
            StartCoroutine(GameManager.IE_Translate(piece.transform,
                                                    piece.transform.position, 
                                                    m_piecesHolder[piece.HolderIndex].position, 
                                                    0.25f
                                                    ));
            return;
        }

        GameManager.Instance.ScorePoint(piece.Point);

        m_currentPieces.Remove(piece);
        piece.Collider.enabled = false;
        m_mapMatrix[coordinate.x, coordinate.y].GetComponent<SpriteRenderer>().enabled = false;
        m_mapContaint[coordinate.x, coordinate.y] = piece.Origin.GetComponent<SpriteRenderer>();
        foreach (var pattern in piece.Patterns)
        {
            var x = pattern.x + coordinate.x;
            var y = pattern.y + coordinate.y;

            m_mapContaint[x, y] = pattern.Sprite;
            m_mapMatrix[x, y].GetComponent<SpriteRenderer>().enabled = false;
        }

        StartCoroutine(GameManager.IE_Translate(piece.Origin,
                                                piece.transform.position, 
                                                m_mapMatrix[coordinate.x, coordinate.y].transform.position, 
                                                0.25f, 
                                                () =>
                                                {
                                                    piece.DropPiece(m_mapMatrix[coordinate.x, coordinate.y].GetComponent<SpriteRenderer>().sortingOrder);

                                                    if (!CheckStreak())
                                                    {
                                                        if (CheckGameOver())
                                                        {
                                                            GameManager.Instance.EndGame();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        SoundManager.Instance.PlaySound("Merge");
                                                    }
                                                }
                                                ));
    }
    private void SpawnPieces()
    {
        m_currentPieces = new List<Piece>();
        for (int i = 0; i < m_piecesHolder.Length; i++)
        {
            var initialPosition = new Vector2(m_piecesHolder[i].position.x, -15f);
            var randomPiece = Instantiate(m_piecePrefabs[Random.Range(0, m_piecePrefabs.Length)], initialPosition, Quaternion.identity, transform);

            m_currentPieces.Add(randomPiece);
            randomPiece.InitializePiece(i, OnDragPiece, OnDropPiece);
            StartCoroutine(GameManager.IE_Translate(randomPiece.transform,
                                                    initialPosition, 
                                                    m_piecesHolder[i].position, 
                                                    0.5f
                                                    ));
        }
    }
    private bool CheckStreak()
    {
        var streakCount = 0;
        var rowStreaks = new List<int>();
        var columnStreaks = new List<int>();

        // Detect streak on column
        for (int i = 0; i < m_mapSize.x; i++)
        {
            int index = 0;
            while (index < m_mapSize.y)
            {
                if (m_mapContaint[i, index] == null)
                {
                    break;
                }
                index++;
            }
            if (index == m_mapSize.y)
            {
                streakCount++;
                columnStreaks.Add(i);
            }
        }
        // Detect streak on row
        for (int j = 0; j < m_mapSize.y; j++)
        {
            int index = 0;
            while (index < m_mapSize.x)
            {
                if (m_mapContaint[index, j] == null)
                {
                    break;
                }
                index++;
            }
            if (index == m_mapSize.x)
            {
                streakCount++;
                rowStreaks.Add(j);
            }
        }

        foreach (var column in columnStreaks)
        {
            for (int j = 0; j < m_mapSize.y; j++)
            {
                var square = m_mapContaint[column, j].GetComponent<SpriteRenderer>();

                if (square != null)
                {
                    Debug.LogWarning($"Clear square start: {column}, {j}");
                    var curCol = column;
                    var curRow = j;
                    m_mapContaint[column, j] = null;

                    StartCoroutine(GameManager.IE_Scale(square.transform,
                                                        Vector3.one,
                                                        Vector3.zero,
                                                        0.15f,
                                                        () =>
                                                        {
                                                            Debug.LogWarning($"Clear square done: {curCol}, {curRow}");
                                                            square.enabled = false;
                                                            m_mapMatrix[curCol, curRow].GetComponent<SpriteRenderer>().enabled = true;
                                                            m_mapMatrix[curCol, curRow].GetComponent<SpriteRenderer>().color = Color.white;
                                                        }
                                                        ));
                }
            }
        }

        foreach (var row in rowStreaks)
        {
            for (int i = 0; i < m_mapSize.x; i++)
            {
                var square = m_mapContaint[i, row]?.GetComponent<SpriteRenderer>();

                if (square != null)
                {
                    Debug.LogWarning($"Clear square start: {i}, {row}");
                    var curCol = i;
                    var curRow = row;
                    m_mapContaint[i, row] = null;
                    StartCoroutine(GameManager.IE_Scale(square.transform,
                                                        Vector3.one,
                                                        Vector3.zero,
                                                        0.25f,
                                                        () =>
                                                        {
                                                            Debug.LogWarning($"Clear square done: {curCol}, {curRow}");
                                                            square.enabled = false;
                                                            m_mapMatrix[curCol, curRow].GetComponent<SpriteRenderer>().enabled = true;
                                                            m_mapMatrix[curCol, curRow].GetComponent<SpriteRenderer>().color = Color.white;
                                                        }
                                                        ));
                }
            }
        }

        GameManager.Instance.ScorePoint(streakCount * 100);
        return streakCount > 0;
    }
    private bool CheckGameOver()
    {
        for (int i = 0; i < m_mapSize.x; i++)
        {
            for (int j = 0; j < m_mapSize.y; j++)
            {
                if (m_mapMatrix[i, j].GetComponent<SpriteRenderer>().enabled)
                {
                    for (int k = 0; k < m_currentPieces.Count; k++)
                    {
                        if (DetectPiecePattern(m_currentPieces[k].Patterns, m_mapMatrix[i, j]) != new Vector2(-1, -1))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    public void InitializeMap()
    {
        int childIndex = 0;
        m_currentPieces = new List<Piece>();
        m_mapMatrix = new Collider2D[m_mapSize.x, m_mapSize.y];
        m_mapContaint = new SpriteRenderer[m_mapSize.x, m_mapSize.y];

        m_squareContainer.gameObject.SetActive(true);
        for (int i = 0; i < m_piecesHolder.Length; i++)
        {
            m_piecesHolder[i].gameObject.SetActive(true);
        }

        for (int j = 0; j < m_mapSize.y; j++)
        {
            for (int i = 0; i < m_mapSize.x; i++)
            {
                m_mapMatrix[i, j] = m_squareContainer.GetChild(childIndex).GetComponent<Collider2D>();
                m_mapMatrix[i, j].transform.name = $"Square_{i}_{j}_{childIndex}";
                childIndex++;
            }
        }
    }
    public void DisposeMap()
    {
        if (m_mapMatrix == null) return;

        m_squareContainer.gameObject.SetActive(false);
        for (int i = 0; i < m_piecesHolder.Length; i++)
        {
            m_piecesHolder[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).CompareTag("Piece"))
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < m_mapSize.x; i++)
        {
            for (int j = 0; j < m_mapSize.y; j++)
            {
                if (m_mapMatrix[i, j] != null)
                {
                    m_mapMatrix[i, j].GetComponent<SpriteRenderer>().enabled = true;
                    m_mapMatrix[i, j].GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }
    }
    public Vector2Int DetectPiecePattern(List<Pattern> pattern, Collider2D dropSquare)
    {
        for (int i = 0; i < m_mapSize.x; i++)
        {
            for (int j = 0; j < m_mapSize.y; j++)
            {
                if (m_mapMatrix[i, j] == dropSquare)
                {
                    if (!m_mapMatrix[i, j].GetComponent<SpriteRenderer>().enabled)
                    {
                        return new Vector2Int(-1, -1);
                    }

                    for (int k = 0; k < pattern.Count; k++)
                    {
                        int x = pattern[k].x + i;
                        int y = pattern[k].y + j;

                        if (x < 0 || y < 0 || x >= m_mapSize.x || y >= m_mapSize.y)
                        {
                            return new Vector2Int(-1, -1);
                        }

                        if (!m_mapMatrix[x, y].GetComponent<SpriteRenderer>().enabled)
                        {
                            return new Vector2Int(-1, -1);
                        }
                    }

                    return new Vector2Int(i, j);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }
}
