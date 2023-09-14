using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameScreen : UIScreen
{
    [SerializeField] TMPro.TextMeshProUGUI m_scoreText;

    public void SetScoreText(int score)
    {
        m_scoreText.text = score.ToString();
    }
    public void OnPauseButtonPressed()
    {
        SoundManager.Instance.PlaySound("Click");
        MenuManager.Instance.PauseGame();
    }
}
