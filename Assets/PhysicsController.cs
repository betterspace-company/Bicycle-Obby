using Unity.Mathematics;
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

    private float currentRot;
    private float velocityRot;
    public float smoothRotTime = 0.5f;

    public ForceMode forceMode;
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

    // TODO(voven): fix freeze rotation
    // TODO(voven): clamp rotation by x and z
    // private void HandleSteering()
    // {
    //     float rotationY = steerInput * rotationSpeed * math.abs(rb.velocity.magnitude * 1.3f) * Time.fixedDeltaTime;
    //     currentRot = Mathf.SmoothDampAngle(currentRot, rotationY, ref velocityRot, smoothRotTime * Time.fixedDeltaTime);
    //     transform.eulerAngles += new Vector3(0, currentRot, 0);
    //     transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    // }
    
    private void HandleSteering()
    {
        // Calculate the desired rotation amount based on input and speed
        float rotationY = steerInput * rotationSpeed * Mathf.Abs(rb.velocity.magnitude * 1.3f) * Time.fixedDeltaTime;

        // Calculate the target rotation angle
        float targetRotationY = rb.rotation.eulerAngles.y + rotationY;

        // Smoothly interpolate to the target rotation angle
        currentRot = Mathf.SmoothDampAngle(rb.rotation.eulerAngles.y, targetRotationY, ref velocityRot, smoothRotTime);

        // Create the new rotation quaternion
        Quaternion smoothedRotation = Quaternion.Euler(0f, currentRot, 0f);

        // Apply the rotation to the Rigidbody
        rb.MoveRotation(smoothedRotation);
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
