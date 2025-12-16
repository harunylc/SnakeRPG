using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private XPGem xpGemPrefab;

    private Transform _target;

    private void OnEnable()
    {
        EnemyManager.Register(this);
        FindTarget();
    }

    private void OnDisable()
    {
        EnemyManager.Unregister(this);
    }

    private void Start()
    {
        FindTarget();
        EnsurePrefab();
    }

    private void EnsurePrefab()
    {
        if (xpGemPrefab == null)
        {
            // Fallback XP Gem (Yellow Sphere)
            // We can't create a Prefab asset at runtime easily, 
            // but we can create a runtime object template to instantiate from,
            // OR we just handle fallback instantiation in Die().
            // Let's rely on Die() checking.
        }
    }

    private void FindTarget()
    {
        if (_target == null)
        {
            SnakeHeadController player = FindObjectOfType<SnakeHeadController>();
            if (player != null) _target = player.transform;
        }
    }

    private void Update()
    {
        if (_target != null)
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
        }
        else
        {
            FindTarget();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Head Collision -> Game Over
        if (col.GetComponent<SnakeHeadController>())
        {
            // Game Over Logic
            Time.timeScale = 0f;
        }
        
        // Segment Collision -> Destroy Segment, Destroy Enemy
        var segment = col.GetComponent<SnakeSegment>();
        if (segment != null)
        {
            if (segment.Manager != null)
            {
                segment.Manager.RemoveSegment(segment);
                Destroy(gameObject); // Enemy dies
            }
        }
    }

    public void Die()
    {
        SpawnXP();
        Destroy(gameObject);
    }

    private void SpawnXP()
    {
        if (xpGemPrefab != null)
        {
            Instantiate(xpGemPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            // Runtime Fallback for XP Gem
            // Runtime Fallback for XP Gem
            GameObject xpFallback = new GameObject("Fallback_XP");
            xpFallback.transform.position = transform.position;
            xpFallback.transform.localScale = Vector3.one * 0.5f;

            // Add Visuals (Yellow Sphere) manually to avoid 3D Collider conflict
            // We create a temp primitive to steal the mesh/material references, then destroy it.
            // Note: DestroyImmediate might be blocked, so we use Destroy and just don't parenting.
            // Actually, to be safe from Physics Callback errors, we just won't assume we can steal easily without potential overhead.
            // EASIER: Just use a Cube? Or just create primitive and DISABLE the collider.
            
            // Re-attempt with Disable approach which is safest:
            GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            tempSphere.name = "Visuals";
            tempSphere.transform.SetParent(xpFallback.transform);
            tempSphere.transform.localPosition = Vector3.zero;
            
            // Disable 3D Collider immediately
            var c3d = tempSphere.GetComponent<Collider>();
            if (c3d != null) c3d.enabled = false;
            Destroy(c3d); // Clean up later

            // Color
            var renderer = tempSphere.GetComponent<Renderer>();
            if (renderer != null) renderer.material.color = Color.yellow;

            // Add 2D Collider to Root
            var col = xpFallback.AddComponent<CircleCollider2D>();
            col.isTrigger = true;

            // Add XPGem component to Root
            xpFallback.AddComponent<XPGem>();
        }
    }
}
