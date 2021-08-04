using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingletonMonoBehavior<StageManager>
{
    public SaveInt highScore;
    public int score;

    new private void Awake()
    {
        base.Awake();
        highScore = new SaveInt("highScore");
        ScoreUIRefresh();
    }

    public void AddScore(int addScore)
    {
        score += addScore;

        if (highScore.Value < score)
            highScore.Value = score;

        ScoreUIRefresh();
    }

    private void ScoreUIRefresh()
    {
        ScoreUI.Instance.UpdateUI(score, highScore.Value);
    }
}