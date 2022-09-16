using Assets.Scripts.Auxiliary.Nominatim;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GeocoderResultEntry : MonoBehaviour
    {
        public event EventHandler onEntryClicked;
        [SerializeField] private TMP_Text label;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Image entryBackgroundImage;

        private bool isSelected = false;
        public Feature valueFeature { get; private set; }

        public void SetLabel(string labelText) => label.text = labelText;

        public void SetValue(Feature feature) => valueFeature = feature;

        public void EntryClicked()
        {
            onEntryClicked?.Invoke(this, EventArgs.Empty);
        }

        public void SetEntryIsSelected(bool isSelected)
        {
            this.isSelected = isSelected;
            entryBackgroundImage.color = isSelected ? selectedColor : defaultColor;
        }
    }
}