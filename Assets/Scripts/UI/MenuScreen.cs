using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : UIScreen
{
    [SerializeField] TMPro.TextMeshProUGUI m_bestScoreEnglish;
    [SerializeField] TMPro.TextMeshProUGUI m_bestScoreGerman;
    [SerializeField] Sprite[] m_buttonEnglishSprites;
    [SerializeField] Sprite[] m_buttonGermanSprites;
    [SerializeField] Button m_buttonEnglish;
    [SerializeField] Button m_buttonGerman;
    [SerializeField] Button m_buttonSound;

    private bool m_isMuteSound = false;

    private void UpdateLanguage()
    {
        switch (MenuManager.Instance.CurrentLanguage)
        {
            case Language.English:
                m_bestScoreGerman.gameObject.SetActive(false);
                m_bestScoreEnglish.gameObject.SetActive(true);
                m_buttonGerman.image.sprite = m_buttonGermanSprites[1];
                m_buttonEnglish.image.sprite = m_buttonEnglishSprites[0];
                break;
            case Language.German:
                m_bestScoreGerman.gameObject.SetActive(true);
                m_bestScoreEnglish.gameObject.SetActive(false);
                m_buttonGerman.image.sprite = m_buttonGermanSprites[0];
                m_buttonEnglish.image.sprite = m_buttonEnglishSprites[1];
                break;
        }
    }

    public override void ShowScreen()
    {
        base.ShowScreen();
        UpdateLanguage();

        m_buttonSound.image.color = (SoundManager.Instance.MusicState && SoundManager.Instance.SoundState) ?
                                    new Color(115f / 255f, 115f / 255f, 115f / 255f) :
                                    Color.white;
    }

    public void OnStartGameButtonPressed()
    {
        SoundManager.Instance.PlaySound("Click");

        MenuManager.Instance.StartGame();
    }
    public void OnSoundButtonPressed()
    {
        m_isMuteSound = !m_isMuteSound;
        
        SoundManager.Instance.PlaySound("Click");
        SoundManager.Instance.SetAllSFXState(m_isMuteSound);

        m_buttonSound.image.color = (m_isMuteSound) ?
                                    new Color(115f / 255f, 115f / 255f, 115f / 255f) : 
                                    Color.white;
    }
    public void OnLanguageButtonPressed(string language)
    {
        switch (language)
        {
            case "English":
                MenuManager.Instance.SetLanguage(Language.English);
                break;

            case "German":
                MenuManager.Instance.SetLanguage(Language.German);
                break;
        }
        UpdateLanguage();
    }

    public void SetHighScore(int highScore)
    {
        m_bestScoreEnglish.text = "BEST " + highScore;
        m_bestScoreGerman.text = "BESTE " + highScore;
    }
}
