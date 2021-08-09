using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUI : SingletonMonoBehavior<ScoreUI>
{
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI highSoreText;

    protected override void OnInit()
    {
        scoreText = transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        highSoreText = transform.Find("highSoreText").GetComponent<TextMeshProUGUI>();
    }

    internal void UpdateUI(int score, int highScore)
    {
        scoreText.text = score.ToNumber();
        highSoreText.text = highScore.ToNumber();
    }
}