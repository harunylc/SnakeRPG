using System.Collections.Generic;
using UnityEngine;

public enum SegmentType
{
    Normal,
    Archer
}

public class SnakeBodyManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float segmentSpacing = 1.5f;
    [SerializeField] private SnakeSegment segmentPrefab; // Normal
    [SerializeField] private SnakeSegment archerSegmentPrefab; // Archer
    [SerializeField] private int initialSegments = 5;

    // Buffer stores the path of the head
    private readonly List<Pose> _pathHistory = new List<Pose>();
    private readonly List<SnakeSegment> _activeSegments = new List<SnakeSegment>();
    
    // Pooling keys
    private readonly Dictionary<SegmentType, Queue<SnakeSegment>> _pools = new Dictionary<SegmentType, Queue<SnakeSegment>>();

    // Track type of each active segment to return to correct pool
    private readonly Dictionary<SnakeSegment, SegmentType> _segmentTypes = new Dictionary<SnakeSegment, SegmentType>();

    public int SegmentCount => _activeSegments.Count;

    private void Start()
    {
        EnsurePrefabs();

        // Initialize pools
        _pools[SegmentType.Normal] = new Queue<SnakeSegment>();
        _pools[SegmentType.Archer] = new Queue<SnakeSegment>();

        // Initialize path with current position to avoid immediate snapping issues
        _pathHistory.Add(new Pose(transform.position, transform.rotation));

        // Add Archer first (Head follower)
        AddSegment(SegmentType.Archer);

        // Add remaining segments as Normal
        for (int i = 0; i < initialSegments - 1; i++)
        {
            AddSegment(SegmentType.Normal);
        }
    }

    private void EnsurePrefabs()
    {
        // Normal Fallback
        if (segmentPrefab == null)
        {
            // Try loading from Resources first
            segmentPrefab = Resources.Load<SnakeSegment>("Normal");
            
            if (segmentPrefab == null)
            {
                GameObject fallbackGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                fallbackGO.name = "Fallback_Segment_Normal";
                fallbackGO.transform.localScale = Vector3.one * 0.4f;
                DestroyImmediate(fallbackGO.GetComponent<Collider>());
                var col = fallbackGO.AddComponent<BoxCollider2D>();
                col.isTrigger = true; // Segments are triggers
                segmentPrefab = fallbackGO.AddComponent<SnakeSegment>();
                fallbackGO.SetActive(false);
            }
        }

        // Archer Fallback
        if (archerSegmentPrefab == null)
        {
            // Try loading from Resources first
            archerSegmentPrefab = Resources.Load<SnakeSegment>("Archer");

            if (archerSegmentPrefab == null)
            {
                GameObject fallbackGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                fallbackGO.name = "Fallback_Segment_Archer";
                fallbackGO.transform.localScale = Vector3.one * 0.4f;
                
                // Color it Red
                var renderer = fallbackGO.GetComponent<Renderer>();
                if (renderer != null) renderer.material.color = Color.red;

                DestroyImmediate(fallbackGO.GetComponent<Collider>());
                var col = fallbackGO.AddComponent<BoxCollider2D>();
                col.isTrigger = true;
                
                // Add ArcherUnit
                fallbackGO.AddComponent<ArcherUnit>();
                archerSegmentPrefab = fallbackGO.AddComponent<SnakeSegment>();
                
                fallbackGO.SetActive(false);
            }
        }
    }

    private void Update()
    {
        RecordPath();
        UpdateSegments();
    }

    private void RecordPath()
    {
        if (_pathHistory.Count == 0 || Vector3.Distance(_pathHistory[_pathHistory.Count - 1].position, transform.position) > 0.01f)
        {
            _pathHistory.Add(new Pose(transform.position, transform.rotation));
        }

        if (_pathHistory.Count > 2000) 
        {
            _pathHistory.RemoveAt(0);
        }
    }

    private void UpdateSegments()
    {
        float currentDist = 0f;
        int lastHistoryIndex = _pathHistory.Count - 1;
        float accumulatedDistance = 0f;
        
        accumulatedDistance += Vector3.Distance(transform.position, _pathHistory[lastHistoryIndex].position);

        int segmentIndex = 0;
        
        for (int i = _pathHistory.Count - 2; i >= 0; i--)
        {
            float distToNext = Vector3.Distance(_pathHistory[i].position, _pathHistory[i + 1].position);
            accumulatedDistance += distToNext;

            while (segmentIndex < _activeSegments.Count)
            {
                float targetDist = (segmentIndex + 1) * segmentSpacing;
                
                if (accumulatedDistance >= targetDist)
                {
                    float overshoot = accumulatedDistance - targetDist;
                    float t = overshoot / distToNext; 
                    
                    Vector3 pos = Vector3.Lerp(_pathHistory[i + 1].position, _pathHistory[i].position, t);
                    Quaternion rot = Quaternion.Lerp(_pathHistory[i + 1].rotation, _pathHistory[i].rotation, t);
                    
                    _activeSegments[segmentIndex].SetPositionAndRotation(pos, rot);
                    segmentIndex++;
                }
                else
                {
                    break;
                }
            }
            
            if (segmentIndex >= _activeSegments.Count) break;
        }
    }

    public void AddSegment(SegmentType type = SegmentType.Normal)
    {
        SnakeSegment segment = GetFromPool(type);
        _activeSegments.Add(segment);
        _segmentTypes[segment] = type;
        
        if (_pathHistory.Count > 0)
        {
            segment.SetPositionAndRotation(_pathHistory[0].position, _pathHistory[0].rotation);
        }

        segment.Manager = this;
    }

    private SnakeSegment GetFromPool(SegmentType type)
    {
        EnsurePrefabs(); 

        Queue<SnakeSegment> targetPool = _pools.ContainsKey(type) ? _pools[type] : null;

        if (targetPool != null && targetPool.Count > 0)
        {
            SnakeSegment segment = targetPool.Dequeue();
            segment.Initialize();
            return segment;
        }
        else
        {
            SnakeSegment prefabToUse = (type == SegmentType.Archer) ? archerSegmentPrefab : segmentPrefab;
            return Instantiate(prefabToUse, transform.position, Quaternion.identity);
        }
    }

    public void ReturnToPool(SnakeSegment segment)
    {
        segment.gameObject.SetActive(false);
        if (_segmentTypes.ContainsKey(segment))
        {
            SegmentType type = _segmentTypes[segment];
            if (_pools.ContainsKey(type))
            {
                _pools[type].Enqueue(segment);
            }
        }
        else
        {
            Destroy(segment.gameObject);
        }
    }

    public void RemoveSegment(SnakeSegment segment)
    {
        if (_activeSegments.Contains(segment))
        {
            _activeSegments.Remove(segment);
            // Optionally remove from type map? No, needed for pool return.
            // But if segment is returned to pool, it is fine.
            ReturnToPool(segment);
        }
    }
}
