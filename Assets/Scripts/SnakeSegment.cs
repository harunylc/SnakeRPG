using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
    public SnakeBodyManager Manager { get; set; }

    private void Awake()
    {
        if (GetComponent<Collider2D>() == null)
        {
            var col = gameObject.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
        }
    }

    public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);
    }

    public void Initialize()
    {
        // Setup logic for pooling if needed (e.g. resetting visuals)
        gameObject.SetActive(true);
    }
}
