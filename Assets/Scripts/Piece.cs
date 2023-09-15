using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] Color m_color;
    [SerializeField] List<Vector2> m_pattern;

    public Color Color => m_color;
    public List<Vector2> Pattern => m_pattern;

    public void InitializePiece(Color color)
    {
        m_color = color;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().color = m_color;
        }
    }
}
