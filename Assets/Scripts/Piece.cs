using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Pattern
{
    public int x;
    public int y;
    public SpriteRenderer Sprite;
}

public class Piece : MonoBehaviour
{
    [SerializeField] int m_point;
    [SerializeField] Color m_color;
    [SerializeField] Transform m_origin;
    [SerializeField] Vector3 m_defaultScale = new Vector3(0.45f, 0.45f, 0.45f);

    [SerializeField] List<Pattern> m_piecePatterns;

    private int m_holderIndex;

    public int Point => m_point;
    public Color Color => m_color;
    public Transform Origin => m_origin;
    public int HolderIndex => m_holderIndex;
    public Vector3 DefaultScale => m_defaultScale;

    public List<Pattern> Patterns => m_piecePatterns;
    public Collider2D Collider => GetComponent<Collider2D>();

    public System.Action<Piece, Vector2Int> m_onDragPieceCallbacks;
    public System.Action<Piece, Vector2Int> m_onDropPieceCallbacks;

    private void OnMouseDown()
    {
        if (GameManager.Instance.State != GameState.Playing) return;
        m_origin.transform.localScale = new Vector3(1f, 1f, 1f);

        SoundManager.Instance.PlaySound("Drag");
    }
    private void OnMouseDrag()
    {
        if (GameManager.Instance.State != GameState.Playing) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int coordinate = new Vector2Int(-1, -1);
        float distance = float.MaxValue;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(mousePosition, new Vector2(1f, 1f), 0f);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Square"))
            {
                var coor = MapManager.Instance.DetectPiecePattern(m_piecePatterns, collider);
                if (coor != new Vector2Int(-1, -1))
                {
                    if (Vector3.Distance(collider.transform.position, mousePosition) < distance)
                    {
                        distance = Vector2.Distance(collider.transform.position, mousePosition);
                        coordinate = coor;
                    }
                }
            }
        }

        transform.position = mousePosition;
        m_onDragPieceCallbacks?.Invoke(this, coordinate);
    }
    private void OnMouseUp()
    {
        if (GameManager.Instance.State != GameState.Playing) return;
        
        SoundManager.Instance.PlaySound("Collide");

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int coordinate = new Vector2Int(-1, -1);
        float distance = float.MaxValue;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(mousePosition, new Vector2(1f, 1f), 0f);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Square"))
            {
                var coor = MapManager.Instance.DetectPiecePattern(m_piecePatterns, collider);
                if (coor != new Vector2Int(-1, -1))
                {
                    if (Vector3.Distance(collider.transform.position, mousePosition) < distance)
                    {
                        distance = Vector3.Distance(collider.transform.position, mousePosition);
                        coordinate = coor;
                    }
                }
            }    
        }
        
        m_onDropPieceCallbacks?.Invoke(this, coordinate);
    }

    public void InitializePiece(int holderIndex, System.Action<Piece, Vector2Int> onDragPieceCallbacks, System.Action<Piece, Vector2Int> onDropPieceCallbacks)
    {
        m_holderIndex = holderIndex;
        m_onDragPieceCallbacks += onDragPieceCallbacks;
        m_onDropPieceCallbacks += onDropPieceCallbacks;

        var sprites = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].color = m_color;
        }
    }
    public void DropPiece(int order)
    {
        GetComponent<Collider2D>().enabled = false;
        m_origin.GetComponent<SpriteRenderer>().sortingOrder = order;

        foreach (var pattern in m_piecePatterns)
        {
            pattern.Sprite.sortingOrder = order;
        }

        foreach (var pattern in m_piecePatterns)
        {
            pattern.Sprite.transform.parent = transform.parent;
        }
    }
}
