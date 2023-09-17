using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Language
{
    English,
    German
}

public class MenuManager : MonoBehaviour
{
    private static MenuManager m_instance;
    public static MenuManager Instance => m_instance;

    [SerializeField] MenuScreen m_menuScreen;
    [SerializeField] PauseScreen m_pauseScreen;
    [SerializeField] IngameScreen m_ingameScreen;
    [SerializeField] EndGameScreen m_endGameScreen;
    [Space]
    [SerializeField] Slider m_timeSlider;
    [SerializeField] Image m_menuBackground;
    [SerializeField] Image m_ingameBackground;

    private Language m_currentLanguage;
    public Language CurrentLanguage => m_currentLanguage;

    private void Awake()
    {
        if (m_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        m_instance = this;
    }
    
    private void HideAllScreen()
    {
        m_menuScreen.HideScreen();
        m_pauseScreen.HideScreen();
        m_ingameScreen.HideScreen();
        m_endGameScreen.HideScreen();
    }

    public void StartGame()
    {
        HideAllScreen();
        m_ingameScreen.ShowScreen();
        m_ingameScreen.SetScoreText(0);
        GameManager.Instance.StartGame();

        m_menuBackground.enabled = false;
        m_ingameBackground.enabled = true;
    }
    public void PauseGame()
    {
        //HideAllScreen();
        m_pauseScreen.ShowScreen();
        GameManager.Instance.PauseGame();

        m_menuBackground.enabled = true;
        m_ingameBackground.enabled = false;
    }
    public void ResumeGame()
    {
        HideAllScreen();
        m_ingameScreen.ShowScreen();
        GameManager.Instance.ResumeGame();

        m_menuBackground.enabled = false;
        m_ingameBackground.enabled = true;
    }
    public void EndGame()
    {
        //HideAllScreen();
        m_endGameScreen.ShowScreen();

        m_menuBackground.enabled = true;
        m_ingameBackground.enabled = false;
    }
    public void BackToHome()
    {
        HideAllScreen();
        m_menuScreen.ShowScreen();
        MapManager.Instance.DisposeMap();
    }

    public void SetScore(int score, int highScore)
    {
        m_ingameScreen.SetScoreText(score);
        m_menuScreen.SetHighScore(highScore);
    }
    public void SetLanguage(Language language)
    {
        m_currentLanguage = language;
    }
    public void UpdateTimer(float time)
    {
        m_timeSlider.value = time;
    }
}
