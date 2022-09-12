using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class GeocoderInputHandler : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;

        private Geocoder geocoder;

        private void Start()
        {
            geocoder = Geocoder.Instance;

            if(inputField != null)
                inputField.onSubmit.AddListener(MakeQuery);
        }

        private void OnDestroy()
        {
            if (inputField != null)
                inputField.onSubmit.RemoveListener(MakeQuery);
        }

        public void MakeQueryWithCurrentString()
        {
            if (string.IsNullOrWhiteSpace(inputField?.text))
                return;

            MakeQuery(inputField.text);
        }

        public void MakeQuery(string queryText)
        {
            geocoder.MakeQuery(queryText);
        }
    }
}