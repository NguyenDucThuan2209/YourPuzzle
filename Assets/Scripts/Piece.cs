using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] Color m_color;
    [SerializeField] Transform m_origin;
    [SerializeField] List<Vector2> m_pattern;

    private Transform m_lastSquareCollide;

    public Color Color => m_color;
    public Transform Origin => m_origin;
    public List<Vector2> Pattern => m_pattern;

    public System.Action<Transform> m_onDropPieceCallbacks;

    private void OnMouseDown()
    {
        m_origin.transform.localScale = new Vector3(1f, 1f, 1f);
    }
    private void OnMouseDrag()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        transform.position = mousePosition;
    }
    private void OnMouseUp()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D lastCollider = null;
        float distance = 0;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(mousePosition, new Vector2(1f, 1f), 0f);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Square"))
            {
                if (lastCollider != null)
                {
                    if (Vector3.Distance(collider.transform.position, mousePosition) < distance)
                    {
                        distance = Vector3.Distance(collider.transform.position, mousePosition);
                        lastCollider = collider;
                    }
                }
                else
                {
                    lastCollider = collider;
                }
            }    
        }

        transform.GetComponent<Collider2D>().enabled = false;
        m_onDropPieceCallbacks?.Invoke(lastCollider.transform);

        StartCoroutine(GameManager.IE_Translate(m_origin, m_origin.position, lastCollider.transform.position, 0.25f));
    }

    public void InitializePiece(Color color, System.Action<Transform> onDropPieceCallbacks)
    {
        m_color = color;
        m_onDropPieceCallbacks += onDropPieceCallbacks;

        var sprites = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].color = m_color;
        }
    }
}
