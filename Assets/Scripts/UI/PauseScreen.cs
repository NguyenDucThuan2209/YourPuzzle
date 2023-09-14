using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : UIScreen
{
    [SerializeField] TMPro.TextMeshProUGUI[] m_englishText;
    [SerializeField] TMPro.TextMeshProUGUI[] m_germanText;

    public override void ShowScreen()
    {
        base.ShowScreen();
        switch (MenuManager.Instance.CurrentLanguage)
        {
            case Language.English:
                for (int i = 0; i < m_germanText.Length; i++)
                {
                    m_germanText[i].gameObject.SetActive(false);
                }
                for (int i = 0; i < m_englishText.Length; i++)
                {
                    m_englishText[i].gameObject.SetActive(true);
                }
                break;
            case Language.German:
                for (int i = 0; i < m_germanText.Length; i++)
                {
                    m_germanText[i].gameObject.SetActive(true);
                }
                for (int i = 0; i < m_englishText.Length; i++)
                {
                    m_englishText[i].gameObject.SetActive(false);
                }
                break;
        }
    }

    public void OnResumeButtonPressed()
    {
        SoundManager.Instance.PlaySound("Click");
        MenuManager.Instance.ResumeGame();
    }
    public void OnReplayButtonPressed()
    {
        SoundManager.Instance.PlaySound("Click");
        MenuManager.Instance.StartGame();
    }
    public void OnBackToMenuButtonPressed()
    {
        SoundManager.Instance.PlaySound("Click");
        MenuManager.Instance.BackToHome();
    }
}
