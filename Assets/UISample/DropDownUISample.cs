using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UISample
{
    public class DropDownUISample : MonoBehaviour
    {
        private Dropdown dropdown;
        private SaveInt lastSelectedIndex;

        private void Start()
        {
            dropdown = transform.Find("Dropdown").GetComponent<Dropdown>();
            dropdown.onValueChanged.AddListener(OnValueChanged);
            lastSelectedIndex = new SaveInt("lastSelectedIndex");
            dropdown.value = lastSelectedIndex;
        }

        private void OnValueChanged(int selectedIndex)
        {
            lastSelectedIndex.Value = selectedIndex;
            string selectedText = dropdown.options[selectedIndex].text;
            print(selectedText + "선택됨");
            transform.Find("Selected/Text").GetComponent<Text>().text = selectedText;
        }
    }
}