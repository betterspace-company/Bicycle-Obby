using Unity.Mathematics;
using UnityEngine;

public class PhysicsController : MonoBehaviour
{
    public float moveForce = 50f;
    public float maxSpeed = 15f;
    public float rotationSpeed = 100f;

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

    public Animator bicycleAnimator;
    private static readonly int verticalId = Animator.StringToHash("Vertical");
    public float xRotSpeed = 200f;
    public float xDifference = 3f;

    public float gravity = 39.5f;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
        jumpKeyPressed = Input.GetKey(KeyCode.Space);
        
        bicycleAnimator.SetFloat(verticalId, math.abs(verticalInput));
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleSteering();
        HandleJumping();
        RotateWheels();
        Tilt();
    }

    public float acceleration = 30f;
    
    private void HandleMovement()
    {
        Vector3 force = transform.forward * verticalInput * moveForce;
        force.y = -gravity;

        // rb.velocity = Vector3.Lerp(rb.velocity, force, Time.fixedDeltaTime * acceleration);
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

    // TODO(voven): clamp rotation by x and z
    private void HandleSteering()
    {
        float rotationY = steerInput * rotationSpeed * Mathf.Abs(rb.velocity.magnitude * 1.3f) * Time.fixedDeltaTime;
        float targetRotationY = rb.rotation.eulerAngles.y + rotationY;

        currentRot = Mathf.SmoothDampAngle(rb.rotation.eulerAngles.y, targetRotationY, ref velocityRot, smoothRotTime);
        
        Quaternion smoothedRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, currentRot, 0f);
        transform.rotation = smoothedRotation;
    }

    private RaycastHit hit;
    private float t;

    public float maxTiltAngle = 45f; 
    public float smoothing = 5f;     
    public float raycastDistance = 50f;

    public void Tilt3()
    {
        RaycastHit frontHit;
        bool frontWheelHit = Physics.Raycast(frontWheel.position, Vector3.down, out frontHit, raycastDistance);

        // Выполняем рейкаст вниз от заднего колеса
        RaycastHit backHit;
        bool backWheelHit = Physics.Raycast(rearWheel.position, Vector3.down, out backHit, raycastDistance);

        if (frontWheelHit && backWheelHit)
        {
            // Вычисляем разницу высот между точками соприкосновения
            float heightDifference = frontHit.point.y - backHit.point.y;

            // Вычисляем расстояние между колесами по горизонтали
            float horizontalDistance = Vector3.Distance(
                new Vector3(frontWheel.position.x, 0, frontWheel.position.z),
                new Vector3(rearWheel.position.x, 0, rearWheel.position.z)
            );

            // Вычисляем угол наклона в градусах
            float tiltAngle = Mathf.Atan2(heightDifference, horizontalDistance) * Mathf.Rad2Deg;

            // Ограничиваем угол наклона максимальным значением
            tiltAngle = Mathf.Clamp(tiltAngle, -maxTiltAngle, maxTiltAngle);

            // Создаем желаемую ротацию
            Quaternion targetRotation = Quaternion.Euler(-tiltAngle, transform.eulerAngles.y, transform.eulerAngles.z);

            // Плавно интерполируем текущую ротацию к желаемой
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothing);
        }
    }
    
    public void Tilt2()
    {
        float minDist = 0.7f;
        RaycastHit frontHit;
        bool frontWheelHit = Physics.Raycast(frontWheel.position, Vector3.down, out frontHit, raycastDistance);

        RaycastHit backHit;
        bool backWheelHit = Physics.Raycast(rearWheel.position, Vector3.down, out backHit, raycastDistance);

        float frontDistance = Vector3.Distance(frontWheel.position, frontHit.point);
        float backDistance = Vector3.Distance(rearWheel.position, backHit.point);

        // Debug.Log($"Front: {frontDistance}, Back: {backDistance}");

        var frontDifference = frontDistance - minDist; 
        var backDifference = backDistance - minDist;
        
        if (frontDistance > minDist && frontDifference > backDifference)
        {
            float tiltAngle = transform.eulerAngles.x + xDifference;

            tiltAngle = Mathf.Clamp(tiltAngle, -maxTiltAngle, maxTiltAngle);

            Quaternion targetRotation = Quaternion.Euler(tiltAngle, transform.eulerAngles.y, transform.eulerAngles.z);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothing);
        }
        else if (backDistance > minDist && backDifference > frontDifference)
        {
            float tiltAngle = transform.eulerAngles.x - xDifference;

            tiltAngle = Mathf.Clamp(tiltAngle, -maxTiltAngle, maxTiltAngle);

            Quaternion targetRotation = Quaternion.Euler(tiltAngle, transform.eulerAngles.y, transform.eulerAngles.z);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothing);
        }
    }

    public float bikeTiltIncrement;
    public void Tilt()
    {
        if (Physics.Raycast(transform.position + originOffset, Vector3.down, out hit, 3f))
        {
            float xRot = (Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation).eulerAngles.x;
            Quaternion targetRot = Quaternion.Slerp(transform.rotation, Quaternion.Euler(xRot, transform.eulerAngles.y, transform.eulerAngles.z), bikeTiltIncrement);
            Quaternion newRot = Quaternion.Euler(targetRot.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
            transform.rotation = newRot;
        }
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
