using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingUIItem : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private Image icon;
    private Button button;

    // Start is called before the first frame update
    private void Awake()
    {
        scoreText = transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        icon = transform.Find("Icon").GetComponent<Image>();
        button = transform.Find("Button").GetComponent<Button>();
    }

    internal void SetData(int score)
    {
        scoreText.text = score.ToNumber();
    }
}