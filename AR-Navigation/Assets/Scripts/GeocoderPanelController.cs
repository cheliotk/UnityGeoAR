using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GeocoderPanelController : MonoBehaviour
    {
        [SerializeField] private Button expandButton;
        [SerializeField] private GameObject geocoderPanel;
        [SerializeField] private GameObject searchPanel;
        [SerializeField] private GameObject currentResultPanel;
        [SerializeField] private GeocoderResultsVisualizer resultsVisualizer;
        [SerializeField] private GeocoderSelectedResultPanel selectedResultPanel;

        private bool isExpanded = false;
        private RectTransform expandButtonGraphicTransform;

        // Use this for initialization
        void Start()
        {
            if (expandButton != null)
            {
                expandButton.onClick.AddListener(ExpandButtonClicked);
                expandButtonGraphicTransform = expandButton.transform.GetChild(0) as RectTransform;
            }

            if(resultsVisualizer != null)
            {
                resultsVisualizer.onEntrySelected += ResultsVisualizer_onEntrySelected;
            }

            searchPanel.SetActive(true);
            currentResultPanel.SetActive(false);
            geocoderPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (expandButton != null)
                expandButton.onClick.RemoveListener(ExpandButtonClicked);


            if (resultsVisualizer != null)
            {
                resultsVisualizer.onEntrySelected -= ResultsVisualizer_onEntrySelected;
            }
        }

        private void ResultsVisualizer_onEntrySelected(object sender, Auxiliary.Nominatim.Feature e)
        {
            if (selectedResultPanel.selectedFeature != e)
                selectedResultPanel.SetSelectedFeature(e);

            searchPanel.SetActive(false);
            currentResultPanel.SetActive(true);
        }

        public void ReturnToSearch()
        {
            searchPanel.SetActive(true);
            currentResultPanel.SetActive(false);
        }

        private void ExpandButtonClicked()
        {
            isExpanded = !isExpanded;
            
            Vector3 graphicRotation = new Vector3(0f, 0f, isExpanded ? 180f : 0f);
            expandButtonGraphicTransform.rotation = Quaternion.Euler(graphicRotation);

            TogglePanel();
        }

        private void TogglePanel()
        {
            geocoderPanel.SetActive(isExpanded);
        }
    }
}