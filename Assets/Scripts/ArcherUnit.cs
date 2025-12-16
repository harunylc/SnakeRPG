using UnityEngine;

public class ArcherUnit : MonoBehaviour
{
    [Header("Combat Stats")]
    [SerializeField] private float fireRate = 1.0f;
    [SerializeField] private float range = 10f; // Range is needed for detection
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifetime = 2f;

    private float _fireTimer;

    private void Update()
    {
        _fireTimer += Time.deltaTime;

        if (_fireTimer >= 1f / fireRate)
        {
            // Find target before firing
            Enemy target = EnemyManager.GetNearest(transform.position, range);
            
            if (target != null)
            {
                Fire(target);
                _fireTimer = 0f;
            }
        }
    }

    private void Fire(Enemy target)
    {
        // Calculate direction to target
        Vector3 direction = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Projectile p = ProjectilePool.Get();
        p.Initialize(transform.position, rotation, projectileSpeed, projectileLifetime);
    }
}
