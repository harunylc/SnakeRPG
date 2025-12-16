using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private static EnemySpawner _instance;
    public static EnemySpawner Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("EnemySpawner_AutoCreated");
                _instance = go.AddComponent<EnemySpawner>();
            }
            return _instance;
        }
    }

    [Header("Settings")]
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private float spawnInterval = 1f; // Reverting to faster spawn as per recent user preference
    [SerializeField] private float minSpawnDistance = 7f;
    [SerializeField] private float maxSpawnDistance = 12f;

    private float _timer;

    public static void EnsureInstance()
    {
        var i = Instance; // Triggers getter logic
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        EnsurePrefab();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= spawnInterval)
        {
            SpawnEnemy();
            _timer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        EnsurePrefab();
        
        // Find Player to spawn around
        Vector3 center = Vector3.zero;
        var player = FindObjectOfType<SnakeHeadController>();
        if (player != null) center = player.transform.position;

        // Spawn in a donut shape around the center
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector3 spawnPos = center + (Vector3)(randomDir * distance);

        Enemy instance = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        instance.gameObject.SetActive(true);
    }

    private void EnsurePrefab()
    {
        if (enemyPrefab == null)
        {
            // === TEMP / DEBUG ONLY ===
            GameObject fallbackGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fallbackGO.name = "Fallback_Enemy";
            fallbackGO.transform.localScale = Vector3.one * 0.8f;
            
            // Color Blue
            var renderer = fallbackGO.GetComponent<Renderer>();
            if (renderer != null) renderer.material.color = Color.blue;

            // Fix Collider (Remove 3D, add 2D)
            DestroyImmediate(fallbackGO.GetComponent<Collider>());
            BoxCollider2D col = fallbackGO.AddComponent<BoxCollider2D>();
            col.isTrigger = true; // Enemies usually triggers in this genre? Or solid? 
                                  // Projectile checks OnTriggerEnter, so this needs to be a Trigger OR Projectile needs Rigidbody. 
                                  // Projectiles have isTrigger=true usually. 
                                  // If Projectile is Trigger, Enemy should be RigidBody or Trigger?
                                  // If Projectile has Trigger Collider and RB(Kinematic), it collides with Static Trigger? No.
                                  // Simple setup: Enemy = Trigger + RB(Kinematic).
            
            var rb = fallbackGO.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;

            enemyPrefab = fallbackGO.AddComponent<Enemy>();
            
            fallbackGO.SetActive(false);
            // =========================
        }
    }
}
