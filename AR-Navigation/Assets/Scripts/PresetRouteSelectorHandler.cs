using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresetRouteSelectorHandler : MonoBehaviour
{
    private RouteVisualizer routeVisualizer;
    [SerializeField] private List<TextAsset> routeFiles;
    [SerializeField] private Button expandButton;
    [SerializeField] private GameObject routesPanel;

    [SerializeField] private GameObject routeEntryPrefab;
    private List<PresetRouteEntry> entries;

    private Dictionary<int, PresetRoute> routesDict = new Dictionary<int, PresetRoute>();

    private bool isExpanded = false;
    private RectTransform expandButtonGraphicTransform;

    // Start is called before the first frame update
    void Start()
    {
        routeVisualizer = GameObject.FindObjectOfType<RouteVisualizer>();
        if(routeVisualizer == null)
        {
            throw new NullReferenceException("RouteVisualizer not found");
        }

        if (expandButton != null)
        {
            expandButton.onClick.AddListener(ExpandButtonClicked);
            expandButtonGraphicTransform = expandButton.transform.GetChild(0) as RectTransform;
        }

        routesPanel.SetActive(false);

        ParseRouteFiles();

        foreach (PresetRoute presetRoute in routesDict.Values)
        {
            CreateEntry(presetRoute);
        }
    }

    private void OnDestroy()
    {
        if (expandButton != null)
            expandButton.onClick.RemoveListener(ExpandButtonClicked);

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            PresetRouteEntry entry = child.GetComponent<PresetRouteEntry>();
            DestroyEntry(entry);
        }
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
        routesPanel.SetActive(isExpanded);
    }

    private void ParseRouteFiles()
    {
        for (int i = 0; i < routeFiles.Count; i++)
        {
            TextAsset routeFile = routeFiles[i];
            List<Vector2> route = ParseRouteFile(routeFile);
            PresetRoute presetRoute = new PresetRoute(routeFile.name, i, route);
            routesDict.Add(i, presetRoute);
        }
    }

    private List<Vector2> ParseRouteFile(TextAsset routeFile)
    {
        List<Vector2> route = new List<Vector2>();

        string[] lines = routeFile.text.Split("\n");
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            var coors = line.Split(',');
            float x, y;
            if (float.TryParse(coors[0], out x) && float.TryParse(coors[1], out y))
            {
                route.Add(new Vector2(x, y));
            }
        }

        return route;
    }

    private void CreateEntry(PresetRoute presetRoute)
    {
        var entryGo = Instantiate(routeEntryPrefab, routesPanel.transform);
        PresetRouteEntry presetRouteEntry = entryGo.GetComponent<PresetRouteEntry>();
        presetRouteEntry.SetLabel(presetRoute.name);
        presetRouteEntry.SetValue(presetRoute.index);
        presetRouteEntry.onEntryClicked += PresetRouteEntry_onEntryClicked;
    }

    private void DestroyEntry(PresetRouteEntry entry)
    {
        entry.onEntryClicked -= PresetRouteEntry_onEntryClicked;
        entries.Remove(entry);
        Destroy(entry.gameObject);
    }

    private async void PresetRouteEntry_onEntryClicked(object sender, System.EventArgs e)
    {
        PresetRouteEntry entry = sender as PresetRouteEntry;
        List<Vector2> route = routesDict[entry.index].waypoints;
        await routeVisualizer.PrepareWaypointsWithElevations(route);
    }
}

public class PresetRoute
{
    public string name { get; private set; }
    public int index { get; private set; }
    public List<Vector2> waypoints { get; private set; }

    public PresetRoute(string name, int index, List<Vector2> waypoints)
    {
        this.name = name;
        this.index = index;
        this.waypoints = waypoints;
    }
}