using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _speed;
    private float _lifetime;
    private float _timer;

    public void Initialize(Vector3 position, Quaternion rotation, float speed, float lifetime)
    {
        transform.position = position;
        transform.rotation = rotation;
        _speed = speed;
        _lifetime = lifetime;
        _timer = 0f;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);

        _timer += Time.deltaTime;
        if (_timer >= _lifetime)
        {
            ProjectilePool.Return(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.Die();
            ProjectilePool.Return(this);
        }
    }
}
