using System;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.Timeline;

[Serializable]
public class ConfigData
{
    public string googleMapApiKey;
}

public class MapManager : MonoBehaviour
{
    public RawImage mapRawImage;

    public RawImage userMarkerPrefab; // 프리팹으로 만든 UI RawImage
    private RawImage userMarkerInstance; // 생성된 인스턴스

    [Header("Map SET")]
    
    public string strBaseURL = "https://maps.googleapis.com/maps/api/staticmap?";
    
    public int zoom = 14;
    public int mapWidth;
    public int mapHeight;
    public string strAPIKey;

    public ARRootManager GPSlocation;
    private double latitude = 0;
    private double longitude = 0;

    private double save_latitude = 0;
    private double save_longitude = 0;

    // Start is called before the first frame update
    void Start()
    {
        TextAsset configText = Resources.Load<TextAsset>("config");
        if (configText == null)
        {
            Debug.LogError("config.json not found in Resources folder.");
            return;
        }
        ConfigData config = JsonUtility.FromJson<ConfigData>(configText.text);
        strAPIKey = config.googleMapApiKey;
        // if (mapRawImage == null)
        // {
        //     mapRawImage = GetComponent<RawImage>();
        //     if (mapRawImage == null)
        //     {
        //         Debug.LogError("RawImage component not found on the GameObject.");
        //         return;
        //     }
        // }
        StartCoroutine(WaitForSecond());
    }
    

    private void Update()
    {
        if (GPSlocation == null)
        {
            GPSlocation = FindObjectOfType<ARRootManager>();
            if (GPSlocation == null)
            {
                Debug.LogError("ARRootManager not found in the scene.");
                return;
            }
        }
        if (GPSlocation.originLat == 0 && GPSlocation.originLon == 0)
        {
            // 아직 초기 GPS 정보가 설정되지 않았음 → 대기
            return;
        }

        if (latitude == 0 && longitude == 0)
        {
            latitude = GPSlocation.originLat;
            longitude = GPSlocation.originLon;

            // 초기화 이후 지도 로드 트리거
            save_latitude = 0;
            save_longitude = 0;
        }
        else
        {
            latitude = GPSlocation.originLat;
            longitude = GPSlocation.originLon;
        }
        
        if (SystemInfo.supportsGyroscope && userMarkerInstance != null)
        {
            Input.gyro.enabled = true;
            float deviceYaw = Input.gyro.attitude.eulerAngles.y;
            userMarkerInstance.rectTransform.rotation = Quaternion.Euler(0, 0, -deviceYaw);
            Debug.Log("Gyroscope enabled, yaw: " + deviceYaw);
        }
        //print("location" + latitude + " " + longitude);
    }

    void UpdateUserMarkerPosition()
    {
        // 지도 중심 좌표
        double centerLat = latitude;
        double centerLon = longitude;

        // 내 위치
        double myLat = GPSlocation.originLat;
        double myLon = GPSlocation.originLon;

        // 지도 스케일 (대략적인 m/pixel, zoom이 클수록 좁은 영역)
        float metersPerPixel = 156543.03392f * Mathf.Cos((float)(centerLat * Mathf.Deg2Rad)) / Mathf.Pow(2, zoom);

        // 위도 경도 차이 → 거리(m)
        float dx = (float)((myLon - centerLon) * 111320 * Math.Cos(centerLat * Mathf.Deg2Rad));
        float dy = (float)((myLat - centerLat) * 111320);

        // 거리 → 픽셀
        float px = dx / metersPerPixel;
        float py = dy / metersPerPixel;

        // 중심이 (0,0)이므로 UI 좌표로 이동
        if(userMarkerInstance != null)
        {
            userMarkerInstance.rectTransform.anchoredPosition = new Vector2(px, py);
            userMarkerInstance.rectTransform.rotation = Quaternion.Euler(0, 0, -Input.gyro.attitude.eulerAngles.y);
        }
    }
    
    IEnumerator WaitForSecond()
    {
        while (true)
        {
            
            if (save_latitude != latitude || save_longitude != longitude)
            {
                save_latitude = latitude;
                save_longitude = longitude;
                StartCoroutine(LoadMap());
            }
            print("3초");
            yield return new WaitForSeconds(3f);
        }
        yield return new WaitForSeconds(1f);
    }
    
    IEnumerator LoadMap()
    {
        //string marker = "&markers=color:red%7Clabel:M%7C" + latitude + "," + longitude;
        string url = strBaseURL + "center=" + latitude + "," + longitude +
                     "&zoom=" + zoom.ToString() + 
                     "&size=" + mapWidth.ToString() + "x" + mapHeight.ToString() +
                     "&key=" + strAPIKey; 

        Debug.Log("URL : " + url);

        url = UnityWebRequest.UnEscapeURL(url); 
        UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);

        yield return req.SendWebRequest(); // 기다림

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Map download failed: " + req.error);
        }
        else
        {
            mapRawImage.texture = DownloadHandlerTexture.GetContent(req); // 성공 시 텍스처 적용
            if (userMarkerInstance == null)
            {
                userMarkerInstance = Instantiate(userMarkerPrefab, mapRawImage.transform);
            }
            UpdateUserMarkerPosition();
        }
    }
}
