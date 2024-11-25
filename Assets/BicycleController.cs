using Unity.Mathematics;
using UnityEngine;

public class BicycleController : MonoBehaviour
{
    public Transform camera;
    
    public float MoveSpeed = 50;
    public float MaxSpeed = 15;
    public float Drag = 0.98f;
    public float SteerAngle = 20;
    public float Traction = 1;

    public Transform frontWheel;
    public Transform rearWheel;
    
    private Vector3 MoveForce;
    public float wheelRotationSpeed = 10f;

    public CharacterController cc;

    private Vector3 jumpVelocity;
    public float jumpHeight = 3;
    public float gravity = -9.81f;
    
    void Update()
    {
        var isGrounded = cc.isGrounded;
        
        if (isGrounded && MoveForce.y < 0)
        {
            jumpVelocity.y = -2f; 
        }
        
        jumpVelocity.y += gravity * Time.deltaTime;
        // Moving
        MoveForce += transform.forward * MoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        cc.Move((MoveForce + jumpVelocity) * Time.deltaTime);
        // transform.position += MoveForce * Time.deltaTime;

        // Steering
        float steerInput = Input.GetAxis("Horizontal");

        transform.Rotate(Vector3.up * steerInput * MoveForce.magnitude * SteerAngle * Time.deltaTime);

        var angles = transform.rotation.eulerAngles;
        angles.x = 0;
        angles.z = 0;

        // transform.rotation = Quaternion.Euler(angles);
        
        // Drag and max speed limit
        MoveForce *= Drag;
        MoveForce = Vector3.ClampMagnitude(MoveForce, MaxSpeed);

        // Traction
        Debug.DrawRay(transform.position, MoveForce.normalized * 3);
        Debug.DrawRay(transform.position, transform.forward * 3, Color.blue);
        MoveForce = Vector3.Lerp(MoveForce.normalized, transform.forward, Traction * Time.deltaTime) * MoveForce.magnitude;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
        if (math.abs(Input.GetAxis("Vertical")) > 0)
        {
            frontWheel.transform.Rotate(Vector3.right * wheelRotationSpeed);
            rearWheel.transform.Rotate(Vector3.right * wheelRotationSpeed);
        }
    }
}
