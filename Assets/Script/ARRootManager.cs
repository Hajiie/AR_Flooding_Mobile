using UnityEngine;
using System.Collections;

public class ARRootManager : MonoBehaviour
{
    public Transform ARRoot;
    private (double x, double y) geoOrigin;

    public static ARRootManager Instance;
    public bool useFakeGPS = true;
    public double fakeLat = 37.5665, fakeLon = 126.9780;

    void Awake() => Instance = this;

    void Start()
    {
        StartCoroutine(InitARRoot());
    ***REMOVED***

    IEnumerator InitARRoot()
    {
#if UNITY_EDITOR
        if (useFakeGPS)
        {
            geoOrigin = ConvertWGS84ToEPSG5181(fakeLat, fakeLon);
            ARRoot.position = Vector3.zero;
            yield break;
        ***REMOVED***
#endif

        Input.location.Start();
        yield return new WaitForSeconds(2);

        if (Input.location.status == LocationServiceStatus.Running)
        {
            var loc = Input.location.lastData;
            geoOrigin = ConvertWGS84ToEPSG5181(loc.latitude, loc.longitude);
        ***REMOVED***

        ARRoot.position = Vector3.zero;
    ***REMOVED***

    public Vector3 ConvertGeoToUnity(double geoX, double geoY)
    {
        float scale = 0.01f;
        float dx = (float)((geoX - geoOrigin.x) * scale);
        float dz = (float)((geoY - geoOrigin.y) * scale);
        return ARRoot.TransformPoint(new Vector3(dx, 0, dz));
    ***REMOVED***

    (double x, double y) ConvertWGS84ToEPSG5181(double lat, double lon)
    {
        double x = lon * 20037508.34 / 180.0;
        double y = System.Math.Log(System.Math.Tan((90 + lat) * System.Math.PI / 360.0)) / (System.Math.PI / 180.0);
        y = y * 20037508.34 / 180.0;
        return (x, y);
    ***REMOVED***
***REMOVED***