using UnityEngine;
using System.Collections;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.IO.CoordinateSystems;

public class ARRootManager : MonoBehaviour
{
    public static ARRootManager I;
    public Transform ARRoot;
    public bool Ready { get; private set; ***REMOVED***

    [Tooltip("1m 를 Unity 몇 unit 로? (1 = 1m, 0.01 = 1cm, 0.001 = 1mm)")]
    public float worldScale = 0.01f;

    public double originLat = 0, originLon = 0;
    public (double x, double y) geoOrigin; // EPSG‑5181 origin in meters
    const double Deg2Rad = System.Math.PI / 180;

    private static readonly ICoordinateTransformation _wgs84To5181;
    private static readonly ICoordinateTransformation _wgs84To5179;

    static ARRootManager()
    {
        var ctFactory = new CoordinateTransformationFactory();

        // WGS84
        var wgs84 = GeographicCoordinateSystem.WGS84;

        var csFactory = new CoordinateSystemFactory();

        // ----- datum & geographic CRS -----
        var grs80 = Ellipsoid.GRS80;
        var datum = csFactory.CreateHorizontalDatum(
            "Korean 2000",
            DatumType.HD_Geocentric,
            grs80,
            null);

        var geoKorea2000 = csFactory.CreateGeographicCoordinateSystem(
            "GEO_Korea_2000", AngularUnit.Degrees, datum,
            PrimeMeridian.Greenwich,
            new AxisInfo("Lon", AxisOrientationEnum.East),
            new AxisInfo("Lat", AxisOrientationEnum.North));

        // ----- projection -----
        var projParams5181 = new System.Collections.Generic.List<ProjectionParameter>
        {
            new ProjectionParameter("latitude_of_origin", 38.0),
            new ProjectionParameter("central_meridian", 127),
            new ProjectionParameter("scale_factor", 1.0),
            new ProjectionParameter("false_easting", 200000.0),
            new ProjectionParameter("false_northing", 500000.0)
        ***REMOVED***;

        var proj5181 = csFactory.CreateProjection(
            "Transverse_Mercator", "Transverse_Mercator", projParams5181);

        var epsg5181 = csFactory.CreateProjectedCoordinateSystem(
            "EPSG_5181", geoKorea2000, proj5181, LinearUnit.Metre,
            new AxisInfo("Easting", AxisOrientationEnum.East),
            new AxisInfo("Northing", AxisOrientationEnum.North));

        _wgs84To5181 = ctFactory.CreateFromCoordinateSystems(wgs84, epsg5181);
        
        var projParams5179 = new System.Collections.Generic.List<ProjectionParameter>
        {
            new ProjectionParameter("latitude_of_origin", 38.0),
            new ProjectionParameter("central_meridian", 127.5),
            new ProjectionParameter("scale_factor", 0.9996),
            new ProjectionParameter("false_easting", 1000000.0),
            new ProjectionParameter("false_northing", 2000000.0)
        ***REMOVED***;
        
        var proj5179 = csFactory.CreateProjection(
            "Transverse_Mercator", "Transverse_Mercator", projParams5179);
        
        
        // EPSG:5179 (Korea 2000 / Central Belt) for reference
        var epsg5179 = csFactory.CreateProjectedCoordinateSystem(
            "EPSG_5179", geoKorea2000, proj5179, LinearUnit.Metre,
            new AxisInfo("Easting", AxisOrientationEnum.East),
            new AxisInfo("Northing", AxisOrientationEnum.North));
        
        _wgs84To5179 = ctFactory.CreateFromCoordinateSystems(wgs84, epsg5179);
    ***REMOVED***

    void Awake() => I = this;

    IEnumerator Start()
    {
#if UNITY_EDITOR
        // originLat = 37.479860; // 테스트용
        // originLon = 126.856831;
        // originLat = 37.483639;
        // originLon = 126.907523;
        originLat = 37.484258;
        originLon = 126.916069;
        //geoOrigin = ConvertWGS84ToEPSG5181(originLat, originLon);
        geoOrigin = ConvertWGS84ToEPSG5179(originLat, originLon);
        Debug.Log("Using editor origin: " +
                  $"Lat {originLat***REMOVED***, Lon {originLon***REMOVED*** → " +
                  $"EPSG:5179 ({geoOrigin.x***REMOVED***, {geoOrigin.y***REMOVED***)");
        //37.549555, 126.923029
        // 37.553062, 126.924621
        // 37.551688, 126.927683
        // 37.548120, 126.926407
        Ready = true;
        yield break;
#else
        Input.location.Start();
        yield return new WaitUntil(() => Input.location.status == LocationServiceStatus.Running);
        originLat = Input.location.lastData.latitude;
        originLon = Input.location.lastData.longitude;
        //geoOrigin = ConvertWGS84ToEPSG5181(originLat, originLon);
        geoOrigin = ConvertWGS84ToEPSG5179(originLat, originLon);
        Ready = true;
#endif
    ***REMOVED***

    /** 위경도 → 장비 기준 (m) → Unity 좌표 */
    public Vector3 GeoToUnity(double latDeg, double lonDeg)
    {
        // ── 차이(도) → 미터 근사 ──────────────────
        double dLat = (latDeg - originLat) * 111320.0; // 위도 1° ≈ 111.32 km
        double dLon = (lonDeg - originLon) * 111320.0 * // 경도 1° ≈ 111.32 km × cosφ
                      System.Math.Cos(originLat * Deg2Rad);

        return new Vector3((float)(dLon * worldScale), 0f,
            (float)(dLat * worldScale));
    ***REMOVED***

    /// <summary>
    /// EPSG‑5181 projected meters → Unity world position.
    /// </summary>
    public Vector3 EpsgToUnity(double xMeter, double yMeter)
    {
        double dx = (xMeter - geoOrigin.x) * worldScale;
        double dz = (yMeter - geoOrigin.y) * worldScale;
        return ARRoot.TransformPoint(new Vector3((float)dx, 0f, (float)dz));
    ***REMOVED***

    /// <summary>
    /// WGS‑84 latitude/longitude → EPSG:5181 (Korea 2000 / Central TM) meters.
    /// Uses GRS80 ellipsoid parameters.  Central meridian 127.5°E, origin lat 38°N.
    /// Accuracy: ±2cm, sufficient for AR overlay.
    /// </summary>
    public (double x, double y) ConvertWGS84ToEPSG5181(double latDeg, double lonDeg)
    {
        // ProjNET transformation
        var xy = _wgs84To5181.MathTransform.Transform(new[] { lonDeg, latDeg ***REMOVED***);
        return (xy[0], xy[1]); // (Easting, Northing) in metres
    ***REMOVED***

    public (double x, double y) ConvertWGS84ToEPSG5179(double latDeg, double lonDeg)
    {
        // ProjNET transformation
        var xy = _wgs84To5179.MathTransform.Transform(new[] { lonDeg, latDeg ***REMOVED***);
        return (xy[0], xy[1]); // (Easting, Northing) in metres
    ***REMOVED***

***REMOVED***