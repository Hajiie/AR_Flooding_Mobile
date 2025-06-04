using UnityEngine;

public class ARRootController : MonoBehaviour
{
    public static Transform Instance;

    private void Awake()
    {
        Instance = transform; // 전역에서 접근할 수 있도록
    ***REMOVED***
***REMOVED***