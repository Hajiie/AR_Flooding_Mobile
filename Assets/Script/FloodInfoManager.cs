using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class FloodInfoManager : MonoBehaviour
{
    public GameObject markerPrefab;
    public TextAsset geoJsonFile;
    public float maxDepth = 2.0f;

    public List<GameObject> markers = new List<GameObject>();

    void Start()
    {
        LoadFloodData();
    ***REMOVED***

    void LoadFloodData()
    {
        JObject geoData = JObject.Parse(geoJsonFile.text);
        JArray features = (JArray)geoData["features"];

        foreach (var feature in features)
        {
            var properties = feature["properties"];
            double depth = properties["F_SHIM"]?.Value<double>() ?? 0.0;

            var coordinates = feature["geometry"]["coordinates"][0];
            double avgX = 0, avgY = 0;
            int count = 0;

            foreach (var coord in coordinates)
            {
                avgX += coord[0].Value<double>();
                avgY += coord[1].Value<double>();
                count++;
            ***REMOVED***

            avgX /= count;
            avgY /= count;

            Vector3 worldPos = ARRootManager.Instance.ConvertGeoToUnity(avgX, avgY);

            GameObject marker = Instantiate(markerPrefab, worldPos, Quaternion.identity);
            marker.SetActive(false);

            // 수심 색상 적용
            Color waterColor = Color.Lerp(new Color(0, 0, 1, 0), new Color(0, 0, 1, 0.6f), Mathf.Clamp01((float)depth / maxDepth));
            marker.GetComponent<Renderer>().material.color = waterColor;

            markers.Add(marker);
        ***REMOVED***
    ***REMOVED***
***REMOVED***