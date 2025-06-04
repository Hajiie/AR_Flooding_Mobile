using Newtonsoft.Json.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ShapefileLoader : MonoBehaviour
{
    public TextAsset geoJsonFile;  // Unity Inspector에 GeoJSON 파일 연결

    void Start()
    {
        JObject geoJson = JObject.Parse(geoJsonFile.text);
        var features = geoJson["features"];

        foreach (var feature in features)
        {
            var props = feature["properties"];
            float depth = props["F_SHIM"]?.Value<float>() ?? 0f;

            var geometry = feature["geometry"];
            var type = geometry["type"].ToString();

            if (type == "Polygon")
            {
                var rings = geometry["coordinates"].First; // 외곽만 처리
                List<Vector3> points = new();

                foreach (var pt in rings)
                {
                    float x = pt[0].Value<float>();
                    float z = pt[1].Value<float>();
                    float y = -depth;  // 수심을 Y값으로 적용

                    points.Add(new Vector3(x, y, z));
                    print($"Point: {x***REMOVED***, {y***REMOVED***, {z***REMOVED***");  // 디버그 출력
                ***REMOVED***

                DrawPolygon(points.ToArray(), depth);  // 시각화 함수 호출
            ***REMOVED***
        ***REMOVED***
    ***REMOVED***

    void DrawPolygon(Vector3[] points, float depth)
    {
        var go = new GameObject("FloodArea");
        var lr = go.AddComponent<LineRenderer>();
        lr.positionCount = points.Length;
        lr.SetPositions(points);
        lr.loop = true;
        lr.widthMultiplier = 1.0f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = Color.Lerp(Color.cyan, Color.blue, depth / 3f);  // 수심에 따라 색상 변경
    ***REMOVED***
***REMOVED***