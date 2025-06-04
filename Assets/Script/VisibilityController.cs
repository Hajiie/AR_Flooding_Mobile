using UnityEngine;

public class VisibilityController : MonoBehaviour
{
    public FloodInfoManager floodInfoManager;

    void Update()
    {
        if (Camera.main == null) return;

        foreach (var marker in floodInfoManager.markers)
        {
            Vector3 viewPos = Camera.main.WorldToViewportPoint(marker.transform.position);
            bool visible = viewPos.z > 0 &&
                           viewPos.x >= 0 && viewPos.x <= 1 &&
                           viewPos.y >= 0 && viewPos.y <= 1;

            marker.SetActive(visible);
        ***REMOVED***
    ***REMOVED***
***REMOVED***