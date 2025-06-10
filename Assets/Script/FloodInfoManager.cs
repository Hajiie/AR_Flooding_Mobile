using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Vector2d = UnityEngine.Vector2;

public class FloodInfoManager : MonoBehaviour
{
    public TextAsset geoJson;          // inundation.json (EPSG‑5181)
    public GameObject waterPlanePrefab;
    public float maxDepth = 2f;        // deepest water for colour ramp
    public TMP_Text depthText; // optional text to show depth
    private float? groundY = null;

    public Material[] waterPlaneMaterial; // optional material for water plane
    
    
    // follower plane instance
    GameObject waterPlane;

    // Parsed polygons in projected metres
    class PolyInfo
    {
        public List<Vector2d> verts = new();
        public float depth;
    ***REMOVED***
    readonly List<PolyInfo> polys = new();

    // colour ramp (shallow → deep)
    static readonly Color shallowCol = new(0f, 0.6f, 1f, 0.25f);
    static readonly Color deepCol    = new(0f, 0.15f, 0.6f, 0.70f);

    IEnumerator Start()
    {
        // Wait for ARRootManager initialisation
        yield return new WaitUntil(() =>
            ARRootManager.I != null && ARRootManager.I.Ready);

        ParseGeoJson();

        // Plane prefab is already horizontal (XZ), so no rotation needed
        Debug.Log("Instantiating water plane at AR root position.");
        waterPlane = Instantiate(
            waterPlanePrefab,
            ARRootManager.I.ARRoot.position + Vector3.up * 0.02f,
            Quaternion.identity,
            ARRootManager.I.ARRoot.transform); // parent to AR root
        Debug.Log("Instantiating water plane at AR root position.");
    ***REMOVED***



    void ParseGeoJson()
    {
        var data = JObject.Parse(geoJson.text);
        foreach (var f in data["features"])
        {
            float d = f["properties"]["F_SHIM"]?.Value<float>() ?? 0f;
            if (d <= 0f) continue;

            var p = new PolyInfo { depth = d ***REMOVED***;
            var ring = f["geometry"]["coordinates"][0] as JArray;
            foreach (var v in ring)
            {
                double x = v[0].Value<double>();
                double y = v[1].Value<double>();
                p.verts.Add(new Vector2d((float)x, (float)y));
            ***REMOVED***
            polys.Add(p);
        ***REMOVED***
        Debug.Log($"Parsed {polys.Count***REMOVED*** polygons.");
    ***REMOVED***

    void Update()
    {
        if (waterPlane == null || polys.Count == 0) return;
        // Determine ground Y if not already set
        if (!groundY.HasValue)
        {
            var planeManager = FindObjectOfType<ARPlaneManager>();
            if (planeManager != null)
            {
                float minDist = float.MaxValue;
                foreach (var plane in planeManager.trackables)
                {
                    if (plane.alignment != PlaneAlignment.HorizontalUp) continue;
                    float dist = Vector3.Distance(plane.transform.position, Camera.main.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        groundY = plane.transform.position.y;
                    ***REMOVED***
                ***REMOVED***
            ***REMOVED***

            if (!groundY.HasValue)
                return; // wait until groundY is determined
        ***REMOVED***
        // current GPS → EPSG
#if UNITY_EDITOR
        double lat = ARRootManager.I.originLat;
        double lon = ARRootManager.I.originLon;
#else
        if (Input.location.status != LocationServiceStatus.Running) return;
        double lat = Input.location.lastData.latitude;
        double lon = Input.location.lastData.longitude;
#endif
        //Debug.Log("Current GPS: " + lat + ", " + lon);
        var (xMeter, yMeter) = ARRootManager.I.ConvertWGS84ToEPSG5181(lat, lon);
        Vector3 worldPos = ARRootManager.I.EpsgToUnity(xMeter, yMeter);

        
        // point‑in‑polygon test
        float depthHere = GetDepthAt(xMeter, yMeter);
        depthText.text = $"Depth: {depthHere:F2***REMOVED*** m";
        if (depthHere <= 0f)
        {
            if (waterPlane.activeSelf) waterPlane.SetActive(false);
            return;
        ***REMOVED***

        // show & colourise
        if (!waterPlane.activeSelf) waterPlane.SetActive(true);
        
        
        // position plane at camera (XZ only)
        //Debug.Log("Setting water plane position at: " + worldPos);
        waterPlane.transform.position = new Vector3(
            worldPos.x,
            groundY.Value + depthHere,
            worldPos.z);
        

        //float ratio = Mathf.Clamp01(depthHere / maxDepth);
        // Get original materials
        var meshRenderer = waterPlane.GetComponent<MeshRenderer>();

        // Select material based on depth ratio
        int materialIndex = 0;
        switch (depthHere)
        {
            case <= 0.2f:
                materialIndex = 0; // shallow
                break;
            case <= 0.4f:
                materialIndex = 1; // shallow-medium
                break;
            case <= 0.6f:
                materialIndex = 2; // medium
                break;
            case <= 0.8f:
                materialIndex = 3; // medium-deep
                break;
            default:
                materialIndex = 4; // deep
                break;
        ***REMOVED***

        meshRenderer.material = waterPlaneMaterial[materialIndex];
        
        // Debug.Log($"Material Index: {materialIndex***REMOVED***");
        // Debug.Log($"Depth: {depthHere:F2***REMOVED*** m");
        // Debug.Log($"Material: {meshRenderer.material.name***REMOVED***");

        // Plane primitive size is 10×10 units; scale 0.3 ≈ 3m
        waterPlane.transform.localScale = new Vector3(50f, 0.1f, 50f);
    ***REMOVED***

    // simple ray‑crossing point‑in‑polygon test
    float GetDepthAt(double x, double y)
    {
        foreach (var p in polys)
        {
            if (PointInPoly(p.verts, x, y))
                return p.depth;
        ***REMOVED***
        return 0f;
    ***REMOVED***

    static bool PointInPoly(List<Vector2d> verts, double px, double py)
    {
        bool inside = false;
        int n = verts.Count;
        for (int i = 0, j = n - 1; i < n; j = i++)
        {
            double xi = verts[i].x, yi = verts[i].y;
            double xj = verts[j].x, yj = verts[j].y;
            bool intersect = ((yi > py) != (yj > py)) &&
                (px < (xj - xi) * (py - yi) / (yj - yi + 1e-9) + xi);
            //Debug.Log("Checking edge: " + xi + "," + yi + " to " + xj + "," + yj +"intersect: " + intersect);

            if (intersect) inside = !inside;
        ***REMOVED***
        return inside;
    ***REMOVED***
***REMOVED***