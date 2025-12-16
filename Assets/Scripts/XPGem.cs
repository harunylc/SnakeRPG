using UnityEngine;

public class XPGem : MonoBehaviour
{
    [SerializeField] private int xpAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it's the snake head (using tag or component)
        // Assuming Player has SnakeHeadController
        if (other.GetComponent<SnakeHeadController>())
        {
            LevelExperienceManager.Instance.AddXP(xpAmount);
            
            // ToDo: Return to pool
            Destroy(gameObject);
        }
    }
}
