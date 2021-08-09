using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldUI : SingletonMonoBehavior<GoldUI>
{
    private TextMeshProUGUI goldText;

    protected override void OnInit()
    {
        goldText = transform.Find("GoldText").GetComponent<TextMeshProUGUI>();
    }

    public void UpdateUI(int gold)
    {
        goldText.text = gold.ToNumber();
    }
}