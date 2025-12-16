using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeHeadController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 180f;

    private InputAction _steerAction;

    private void Awake()
    {
        // Ensure Physics Binding (Kinematic RB + Collider)
        if (GetComponent<Rigidbody2D>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;
        }
        if (GetComponent<Collider2D>() == null)
        {
            var col = gameObject.AddComponent<CircleCollider2D>();
            col.isTrigger = true; // Player is a trigger? Or Solid? 
                                  // If solid, Enemies (Kinematic) might push it?
                                  // Let's make it Trigger to be safe from physics forces.
                                  // Enemy has Trigger Logic.
        }

        // Bootstrap dependencies
        EnemySpawner.EnsureInstance();
        UIManager.EnsureInstance(); // Bootstrap UI
        CameraFollow.EnsureInstance(); // Bootstrap Camera

        // Setup input action with bindings for Keyboard and Gamepad
        _steerAction = new InputAction("Steer", binding: "<Gamepad>/leftStick/x");
        _steerAction.AddCompositeBinding("Axis")
            .With("Negative", "<Keyboard>/a")
            .With("Positive", "<Keyboard>/d")
            .With("Negative", "<Keyboard>/leftArrow")
            .With("Positive", "<Keyboard>/rightArrow");
    }

    private void OnEnable()
    {
        _steerAction.Enable();
    }

    private void OnDisable()
    {
        _steerAction.Disable();
    }

    private void Update()
    {
        // Continuous forward movement
        transform.position += transform.right * moveSpeed * Time.deltaTime;

        // Steering via New Input System
        float turnInput = _steerAction.ReadValue<float>();

        if (Mathf.Abs(turnInput) > 0.01f)
        {
            transform.Rotate(Vector3.forward, -turnInput * turnSpeed * Time.deltaTime);
        }
    }
}
