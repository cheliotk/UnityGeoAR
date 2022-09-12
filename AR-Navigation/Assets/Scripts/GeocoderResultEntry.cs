using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class GeocoderResultEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;

        public void SetLabel(string labelText)
        {
            label.text = labelText;
        }
    }
}