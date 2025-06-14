using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

public class QuadTouchSpawner : MonoBehaviour
{
    public GameObject hazardPrefab;
    public Camera arCamera;
    
    public EventSystem eventSystem; // EventSystem 컴포넌트 (UI 터치 감지용)

    public ARRaycastManager raycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        if (raycastManager == null)
        {
            raycastManager = FindObjectOfType<ARRaycastManager>();
            if (raycastManager == null)
            {
                Debug.LogError("ARRaycastManager not found in the scene.");
            }
            return;
        }
        
        //Debug.Log("raycastMng : "+raycastManager.name);
    }

    void Update()
    {
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return; // UI 클릭 시 무시
        }
#if UNITY_EDITOR
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;
        Vector2 touchPosition = Mouse.current.position.ReadValue();
#else
    if (Touchscreen.current == null || Touchscreen.current.primaryTouch.press.wasPressedThisFrame == false)
        return;

    Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
#endif

        // Plane 위 터치 감지
        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            Vector3 hitPosition = hitPose.position;

            // 충돌체가 있는지 확인
            Ray ray = arCamera.ScreenPointToRay(touchPosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.collider != null && hitInfo.collider.gameObject.CompareTag("Hazard"))
                {
                    Destroy(hitInfo.collider.gameObject);
                    return;
                }
            }

            // Rotate the hazard prefab to normal based on hitPosition rotation
            Quaternion rotation = Quaternion.LookRotation(-hitPose.up, Vector3.up);
            //Quaternion rotation = Quaternion.Euler(0, -90f, 0);
            //Debug.Log("Hit position: " + hitPosition);
            //Adds the z-axis coordinate of the created newHazard's coordinate axis by 0.01.
            Vector3 offset = rotation * Vector3.forward * -0.01f;
            // Instantiate the hazard prefab at the hit position with the specified rotation
            GameObject newHazard = Instantiate(hazardPrefab, hitPosition + offset, rotation);
            newHazard.tag = "Hazard";  // 꼭 tag 설정!
        }
    }
    
    //button을 눌렀을 때 모든 Hazard 제거
    public void ClearHazards()
    {
        Debug.Log("Clearing Hazards");
        GameObject[] hazards = GameObject.FindGameObjectsWithTag("Hazard");
        foreach (GameObject hazard in hazards)
        {
            Destroy(hazard);
        }
    }
}