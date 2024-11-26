using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController cc;

    public Transform cameraTransform;

    public float speed = 6.0f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    
    [FormerlySerializedAs("alignRotationSpeed")] public float rotationSpeed = 10.0f;

    private bool isGrounded;

    public Quaternion targetRotation;

    public float wheelRotationSpeed = 300f;
    public Transform frontWheel;
    public Transform rearWheel;

    private Vector3 velocity;
    private Vector3 currentVelocity;
    
    public float acceleration = 10.0f;
    public float deceleration = 10.0f;
    
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = cc.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 cameraForward = cameraTransform.TransformDirection(Vector3.forward);
        Vector3 cameraRight = cameraTransform.TransformDirection(Vector3.right);

        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 inputDirection = (cameraForward * moveZ + cameraRight * moveX).normalized;

        float targetSpeed = speed;//Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        Vector3 targetVelocity = inputDirection * targetSpeed;

        if (inputDirection.magnitude > 0)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        Vector3 move = currentVelocity;

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        move += velocity;

        cc.Move(move * Time.deltaTime);
        
        if (inputDirection.magnitude > 0)
        {
            frontWheel.transform.Rotate(Vector3.right * wheelRotationSpeed);
            rearWheel.transform.Rotate(Vector3.right * wheelRotationSpeed);
            
            targetRotation = cameraTransform.rotation;
            var eulerAngles = targetRotation.eulerAngles;
            eulerAngles.x = 0;
            eulerAngles.z = 0;
            targetRotation.eulerAngles = eulerAngles;
        
            float angle = Mathf.Atan2(moveX, moveZ) * Mathf.Rad2Deg;
            targetRotation *= Quaternion.Euler(0, angle, 0);
        
            // var rotationSpeed = Mathf.Abs(horizontalInput) > 0 ? inputRotationSpeed : alignRotationSpeed; 
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);    
        }
    }
}
