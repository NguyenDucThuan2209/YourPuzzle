using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static MapManager m_instance;
    public static MapManager Instance => m_instance;

    [SerializeField] Piece[] m_piecePrefabs;
    [SerializeField] Vector2 m_mapSize;

    private Piece m_currentPiece;
    private Collider2D[,] m_mapMatrix;

    private void Awake()
    {
        if (m_instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        m_instance = this;
    }
    private void Start()
    {
        InitializeMap();
    }


    public void OnDropPiece(Transform transform)
    {
        Debug.LogWarning("Drop Piece to " + transform.name);
    }

    public void InitializeMap()
    {
        int childIndex = 0;
        m_mapMatrix = new Collider2D[(int)m_mapSize.x, (int)m_mapSize.y];
        for (int i = 0; i < m_mapSize.x; i++)
        {
            for (int j = 0; j < m_mapSize.y; j++)
            {
                m_mapMatrix[i, j] = transform.GetChild(childIndex).GetComponent<Collider2D>();
                childIndex++;
            }
        }
    }
}
