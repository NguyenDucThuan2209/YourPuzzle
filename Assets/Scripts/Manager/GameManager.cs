using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    None,
    Initializing,
    Playing,
    Pausing,
    End
}

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public static GameManager Instance => m_instance;

    [Header("Game Properties")]
    [SerializeField] GameState m_state;
    [SerializeField] float m_gameTimer;
    [SerializeField] Vector2 m_vertical;
    [SerializeField] Vector2 m_horizontal;

    private int m_score;
    private int m_bestScore;
    private float m_currentTime;

    public GameState State => m_state;
    public Vector2 Vertical => m_vertical;
    public Vector2 Horizontal => m_horizontal;

    private void Awake()
    {
        if (m_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        m_instance = this;

        CalculateScreenSize();
    }
    private void Update()
    {
        if (m_state != GameState.Playing) return;

        if (m_currentTime > 0)
        {
            m_currentTime -= Time.deltaTime;
            MenuManager.Instance.UpdateTimer(m_currentTime / m_gameTimer);
        }
        else
        {
            EndGame();
        }
    }

    private void ResetGameData()
    {
        m_currentTime = m_gameTimer;
        m_score = 0;
    }
    private void CalculateScreenSize()
    {
        var bottomLeft = Camera.main.ScreenToWorldPoint(Vector2.zero);
        var upperRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        m_vertical = new Vector2(bottomLeft.y, upperRight.y);
        m_horizontal = new Vector2(bottomLeft.x, upperRight.x);
    }
    public void ScorePoint(int point = 1)
    {
        m_score += point;
        m_bestScore = (m_bestScore < m_score) ? m_score : m_bestScore;

        m_currentTime += point / 50;

        SoundManager.Instance.PlaySound("Score");
        MenuManager.Instance.SetScore(m_score, m_bestScore);
    }

    public void StartGame()
    {
        Debug.LogWarning("Start Game");

        ResetGameData();
        m_state = GameState.Playing;
        MapManager.Instance.DisposeMap();
        MapManager.Instance.InitializeMap();
    }
    public void PauseGame()
    {
        Debug.LogWarning("Pause Game");

        m_state = GameState.Pausing;
    }
    public void ResumeGame()
    {
        Debug.LogWarning("Resume Game");

        m_state = GameState.Playing;
    }
    public void EndGame()
    {
        Debug.LogWarning("End Game");

        m_state = GameState.End;

        ResetGameData();
        MenuManager.Instance.EndGame();
        SoundManager.Instance.PlaySound("GameOver");
    }

    #region IENumerator
    public static IEnumerator IE_Translate(Transform obj, Vector3 start, Vector3 end, float duration, System.Action callbacks = null)
    {
        float t = 0;
        while (t < duration)
        {
            obj.position = Vector3.Lerp(start, end, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        obj.position = end;
        callbacks?.Invoke();
    }
    public static IEnumerator IE_Scale(Transform obj, Vector3 start, Vector3 end, float duration, System.Action callbacks = null)
    {
        float t = 0;
        while (t < duration)
        {
            obj.localScale = Vector3.Lerp(start, end, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        obj.localScale = end;
        callbacks?.Invoke();
    }
    #endregion
}
