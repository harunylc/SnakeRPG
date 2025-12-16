using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    private static ProjectilePool _instance;
    public static ProjectilePool Instance
    {
        get
        {
            if (_instance == null)
            {
                // Lazy instantiation: Create a new GameObject if no instance exists in the scene
                GameObject go = new GameObject("ProjectilePool_AutoCreated");
                _instance = go.AddComponent<ProjectilePool>();
            }
            return _instance;
        }
    }

    [SerializeField] private Projectile prefab;
    [SerializeField] private int initialSize = 20;

    private readonly Queue<Projectile> _pool = new Queue<Projectile>();
    private bool _initialized = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        InitializePool();
    }

    private void InitializePool()
    {
        if (_initialized) return;

        EnsurePrefab();

        // Prewarm
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewProjectile();
        }
        _initialized = true;
    }

    private Projectile CreateNewProjectile()
    {
        EnsurePrefab();
        Projectile p = Instantiate(prefab, transform);
        p.gameObject.SetActive(false);
        _pool.Enqueue(p);
        return p;
    }

    private void EnsurePrefab()
    {
        if (prefab == null)
        {
            // === TEMP / DEBUG ONLY ===
            // Create a runtime fallback if no prefab is assigned.
            // This is a safety net for development to prevent null reference exceptions.
            GameObject fallbackGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            fallbackGO.name = "Fallback_Projectile";
            fallbackGO.transform.localScale = Vector3.one * 0.5f;
            
            // Remove 3D collider, add 2D trigger
            DestroyImmediate(fallbackGO.GetComponent<Collider>());
            CircleCollider2D col = fallbackGO.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            
            prefab = fallbackGO.AddComponent<Projectile>();
            
            // Disable the template in scene so it doesn't float around
            fallbackGO.SetActive(false); 
            // =========================
            Debug.LogWarning("ProjectilePool: Prefab was null. Created fallback runtime prefab.");
        }
    }

    public static Projectile Get()
    {
        // Ensure instance exists (triggering lazy load if needed)
        var pool = Instance; 
        
        if (!pool._initialized) pool.InitializePool();

        if (pool._pool.Count > 0)
        {
            return pool._pool.Dequeue();
        }
        else
        {
            return pool.CreateNewProjectile();
        }
    }

    public static void Return(Projectile p)
    {
        p.gameObject.SetActive(false);
        if (Instance != null)
        {
            Instance._pool.Enqueue(p);
        }
        else
        {
            Destroy(p.gameObject);
        }
    }
}
