using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private static CameraFollow _instance;
    public static CameraFollow Instance
    {
        get
        {
            if (_instance == null)
            {
                var existing = FindObjectOfType<CameraFollow>();
                if (existing != null)
                {
                    _instance = existing;
                }
                else
                {
                    // Attach to Main Camera by default
                    if (Camera.main != null)
                    {
                        _instance = Camera.main.gameObject.AddComponent<CameraFollow>();
                    }
                    else
                    {
                        // Fallback if no main camera (unlikely but safe)
                        GameObject go = new GameObject("CameraFollow_AutoCreated");
                        go.AddComponent<Camera>();
                        _instance = go.AddComponent<CameraFollow>();
                    }
                }
            }
            return _instance;
        }
    }

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f);
    
    [Header("Zoom")]
    [SerializeField] private float baseSize = 8f; // Slightly larger base view
    [SerializeField] private float zoomFactor = 0.2f; // Zoom out 0.2 units per segment
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float maxZoom = 20f;

    private Camera _cam;
    private SnakeBodyManager _bodyManager;

    public static void EnsureInstance()
    {
        var i = Instance;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this); // Don't destroy gameObject, it might be the Main Camera
            return;
        }
        _instance = this;
        
        _cam = GetComponent<Camera>();
        if (_cam == null) _cam = gameObject.AddComponent<Camera>();
    }

    private void Start()
    {
        if (target == null)
        {
            var player = FindObjectOfType<SnakeHeadController>();
            if (player != null) target = player.transform;
        }

        _bodyManager = FindObjectOfType<SnakeBodyManager>();
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            // Try finding target again if lost (e.g. respawn)
            var player = FindObjectOfType<SnakeHeadController>();
            if (player != null) target = player.transform;
            return; 
        }

        // 1. Follow Position
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        // Force Z to remain constant if 2D
        smoothedPosition.z = offset.z; 
        transform.position = smoothedPosition;

        // 2. Dynamic Zoom
        if (_bodyManager == null)
        {
            _bodyManager = FindObjectOfType<SnakeBodyManager>();
        }

        if (_bodyManager != null && _cam != null)
        {
            int segments = _bodyManager.SegmentCount;
            float targetSize = baseSize + (segments * zoomFactor);
            targetSize = Mathf.Clamp(targetSize, baseSize, maxZoom);

            _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
        }
    }
}
