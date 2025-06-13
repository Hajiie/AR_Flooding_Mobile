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

    public float overrideDuration = 0f; // if >0, overrides default 5 min rise

    public void SetFastRise()
    {
        overrideDuration = 10f; // Rise over 10 seconds
    ***REMOVED***
    
    public void ResetWaterLevel()
    {
        if (waterPlane != null)
        {
            waterPlane.transform.position = new Vector3(
                waterPlane.transform.position.x,
                groundY.HasValue ? groundY.Value : 0f,
                waterPlane.transform.position.z);
        ***REMOVED***
    ***REMOVED***
    
#if UNITY_EDITOR
    public TMP_Text depthText; // optional text to show depth
#endif
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
    // Start is called before the first frame update

    IEnumerator Start()
    {
        // Wait for ARRootManager initialisation
        yield return new WaitUntil(() =>
            ARRootManager.I != null && ARRootManager.I.Ready);

        ParseGeoJson();
#if UNITY_EDITOR
        depthText.gameObject.SetActive(true);
#endif
        // Plane prefab is already horizontal (XZ), so no rotation needed
        //Debug.Log("Instantiating water plane at AR root position.");
        waterPlane = Instantiate(
            waterPlanePrefab,
            ARRootManager.I.ARRoot.position + Vector3.up * 0.02f,
            Quaternion.identity,
            ARRootManager.I.ARRoot.transform); // parent to AR root
    ***REMOVED***


    void ParsePolygon(JArray coordsArray, float depth)
    {
        foreach (var ringToken in coordsArray)
        {
            if (ringToken is not JArray ringArray) continue;

            var p = new PolyInfo { depth = depth ***REMOVED***;
            foreach (var point in ringArray)
            {
                if (point is not JArray pt || pt.Count < 2) continue;
                if (!float.TryParse(pt[0]?.ToString(), out float x)) continue;
                if (!float.TryParse(pt[1]?.ToString(), out float y)) continue;

                p.verts.Add(new Vector2d(x, y));
            ***REMOVED***

            if (p.verts.Count > 0)
            {
                polys.Add(p);
            ***REMOVED***
        ***REMOVED***
    ***REMOVED***

    void ParseGeoJson()
    {
        var data = JObject.Parse(geoJson.text);
        foreach (var f in data["features"])
        {
            float d = f["properties"]["F_SHIM"]?.Value<float>() ?? 0f;
            if (d <= 0f)
            {
                d = f["properties"]["INDD"]?.Value<float>() ?? 0f;
                Debug.Log("depth : " + d );
            ***REMOVED***
            if (d <= 0f) continue;

            string geomType = f["geometry"]?["type"]?.ToString();
            var coordsToken = f["geometry"]?["coordinates"];

            if (geomType == "Polygon" && coordsToken is JArray coordsArray)
            {
                ParsePolygon(coordsArray, d);
            ***REMOVED***
            else if (geomType == "MultiPolygon" && coordsToken is JArray multiCoordsArray)
            {
                foreach (var poly in multiCoordsArray)
                {
                    if (poly is JArray polyArray)
                        ParsePolygon(polyArray, d);
                ***REMOVED***
            ***REMOVED***
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
        var (xMeter, yMeter) = ARRootManager.I.ConvertWGS84ToEPSG5179(lat, lon);
        Vector3 worldPos = ARRootManager.I.EpsgToUnity(xMeter, yMeter);

        
        // point‑in‑polygon test
        //float depthHere = GetDepthAt(yMeter, xMeter);
        float depthHere = GetDepthAt(xMeter, yMeter);
        //Debug.Log($"Depth at {xMeter***REMOVED***, {yMeter***REMOVED***: {depthHere:F2***REMOVED*** m");
#if UNITY_EDITOR
        depthText.text = $"Depth: {depthHere:F2***REMOVED*** m";
#endif
        if (depthHere <= 0f)
        {
            if (waterPlane.activeSelf) waterPlane.SetActive(false);
            return;
        ***REMOVED***

        // show & colourise
        if (!waterPlane.activeSelf) waterPlane.SetActive(true);
        
        
        // position plane at camera (XZ only)
        //Debug.Log("Setting water plane position at: " + worldPos);
        float currentY = waterPlane.transform.position.y;
        float targetY = groundY.Value + depthHere;
        float riseSpeed = maxDepth / (overrideDuration > 0f ? overrideDuration : 5 * 60); // Allow override via button
        float newY = Mathf.MoveTowards(currentY, targetY, riseSpeed * Time.deltaTime);
        
        // If the newY is very close to targetY, reset overrideDuration
        if (Mathf.Approximately(newY, targetY) && overrideDuration > 0f)
        {
            overrideDuration = 0f;
        ***REMOVED***

        waterPlane.transform.position = new Vector3(
            worldPos.x,
            newY,
            worldPos.z);
        

        //float ratio = Mathf.Clamp01(depthHere / maxDepth);
        // Get original materials
        var meshRenderer = waterPlane.GetComponent<MeshRenderer>();

        // Select material based on current water level height above ground
        float currentDepth = newY - groundY.Value;

        int materialIndex = 0;
        switch (currentDepth)
        {
            case <= 0.2f:
                materialIndex = 0;
                break;
            case <= 0.4f:
                materialIndex = 1;
                break;
            case <= 0.6f:
                materialIndex = 2;
                break;
            case <= 0.8f:
                materialIndex = 3;
                break;
            default:
                materialIndex = 4;
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
            // if (intersect)
            // {
            //     Debug.Log("Checking edge: " + xi + "," + yi + " to " + xj + "," + yj + "intersect: " + intersect);
            // ***REMOVED***

            if (intersect) inside = !inside;
        ***REMOVED***
        return inside;
    ***REMOVED***
***REMOVED***

   