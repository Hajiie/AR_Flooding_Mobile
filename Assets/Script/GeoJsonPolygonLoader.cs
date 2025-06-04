using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GeoJsonPolygonLoader : MonoBehaviour
{
    public string geoJsonFileName = "inundation"; // Resources에 있는 파일명 (확장자 제외)

    void Start()
    {
        // 1. GeoJSON 파일 로드
        TextAsset jsonFile = Resources.Load<TextAsset>(geoJsonFileName);
        if (jsonFile == null)
        {
            Debug.LogError("GeoJSON 파일을 찾을 수 없습니다.");
            return;
        ***REMOVED***

        // 2. 파싱
        JObject geoData = JObject.Parse(jsonFile.text);
        JArray features = (JArray)geoData["features"];

        foreach (JObject feature in features)
        {
            string geomType = feature["geometry"]["type"].ToString();
            var coords = feature["geometry"]["coordinates"];

            if (geomType == "Polygon")
            {
                foreach (var ring in coords) // 각 Polygon의 외곽선 (및 구멍)
                {
                    List<Vector3> points = new List<Vector3>();
                    foreach (var point in ring)
                    {
                        float lon = point[0].Value<float>();
                        float lat = point[1].Value<float>();
                        points.Add(new Vector3(lon, 0, lat)); // 단순히 위도경도를 Unity 좌표로 투영
                    ***REMOVED***

                    DrawLine(points.ToArray());
                ***REMOVED***
            ***REMOVED***
        ***REMOVED***
    ***REMOVED***

    void DrawLine(Vector3[] points)
    {
        GameObject lineObj = new GameObject("GeoPolygon");
        var lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
        lineRenderer.widthMultiplier = 1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    ***REMOVED***
***REMOVED***