using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PresetRouteEntry : MonoBehaviour
{
    public event EventHandler onEntryClicked;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Image entryBackgroundImage;

    public int index { get; private set; }

    public void SetLabel(string labelText) => label.text = labelText;

    public void SetValue(int index) => this.index = index;

    public void EntryClicked()
    {
        onEntryClicked?.Invoke(this, EventArgs.Empty);
    }
}
