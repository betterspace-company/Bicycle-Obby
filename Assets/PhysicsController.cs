using UnityEngine;

public class PhysicsController : MonoBehaviour
{
    public float moveForce = 50f;
    public float maxSpeed = 15f;
    public float rotationSpeed = 100f;
    public float steerAngle = 20f;

    public Transform frontWheel;
    public Transform rearWheel;
    public Vector3 originOffset;
    public float wheelRotationSpeed = 10f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private float verticalInput; 
    private float steerInput; 
    private bool jumpKeyPressed;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
        jumpKeyPressed = Input.GetKey(KeyCode.Space);
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleSteering();
        HandleJumping();
        RotateWheels();
    }

    private void HandleMovement()
    {
        Vector3 force = transform.forward * verticalInput * moveForce;

        // Apply force
        rb.AddForce(force);

        // Limit max speed
        if (rb.velocity.magnitude > maxSpeed)
        {
            var yVel = rb.velocity.y;
            var normalizedVelocity = rb.velocity.normalized * maxSpeed;
            normalizedVelocity.y = yVel;
            rb.velocity = normalizedVelocity;
        }
    }

    private void HandleSteering()
    {
        // Calculate turn angle
        float turn = steerInput * steerAngle * rotationSpeed * rb.velocity.magnitude * Time.fixedDeltaTime;

        // Apply rotation
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    private void HandleJumping()
    {
        if (jumpKeyPressed && IsGrounded())
        {
            // Apply upward force
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + originOffset, Vector3.down, 1.1f);
    }

    private void RotateWheels()
    {
        if (Mathf.Abs(verticalInput) > 0)
        {
            float rotationAmount = wheelRotationSpeed * verticalInput;
            frontWheel.Rotate(Vector3.right * rotationAmount);
            rearWheel.Rotate(Vector3.right * rotationAmount);
        }
    }
}