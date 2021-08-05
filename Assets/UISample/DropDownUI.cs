using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownUI : BaseUI<DropDownUI>
{
    private Dropdown dropdown;

    protected override void OnInit()
    {
        dropdown = transform.Find("Dropdown").GetComponent<Dropdown>();
        dropdown.AddListener(this, Selected);
        lastSelectedIndex = new SaveInt("lastSelectedIndex");
    }

    private SaveInt lastSelectedIndex;

    protected override void OnShow()
    {
        dropdown.value = lastSelectedIndex;
    }

    private void Selected(int selectedIndex)
    {
        string selectedText = dropdown.options[selectedIndex].text;
        print($"선택된 인덱스 : {selectedIndex} , {selectedText}");

        lastSelectedIndex.Value = selectedIndex;
    }
}