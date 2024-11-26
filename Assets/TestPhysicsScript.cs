using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TestPhysicsScript : MonoBehaviour
{
    public float moveSpeed = 5f;          // Movement speed
    public float rotationSpeed = 200f;    // Rotation speed
    public float smoothTime = 0.1f;       // Smoothing time for movement and rotation

    private Rigidbody rb;
    private Vector3 movementInput;
    private float rotationInput;

    private Vector3 velocity = Vector3.zero;       // Velocity reference for movement smoothing
    private float rotationVelocity = 0f;           // Velocity reference for rotation smoothing

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // We handle rotation manually
    }

    void Update()
    {
        // Get movement input
        float moveHorizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right arrow keys
        float moveVertical = Input.GetAxis("Vertical");     // W/S or Up/Down arrow keys

        movementInput = new Vector3(moveHorizontal, 0f, moveVertical).normalized;

        // Get rotation input (e.g., using mouse X movement)
        rotationInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        // Smoothly move the cube
        Vector3 targetPosition = rb.position + movementInput * moveSpeed * Time.fixedDeltaTime;
        Vector3 smoothedPosition = Vector3.SmoothDamp(rb.position, targetPosition, ref velocity, smoothTime);
        rb.MovePosition(smoothedPosition);

        // Smoothly rotate the cube
        float targetYRotation = rb.rotation.eulerAngles.y + rotationInput * rotationSpeed * Time.fixedDeltaTime;
        float smoothedYRotation = Mathf.SmoothDampAngle(rb.rotation.eulerAngles.y, targetYRotation, ref rotationVelocity, smoothTime);
        Quaternion smoothedRotation = Quaternion.Euler(0f, smoothedYRotation, 0f);
        rb.MoveRotation(smoothedRotation);
    }
}