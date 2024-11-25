using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Цель следования")]
    [Tooltip("Трансформ велосипеда, за которым следует камера.")]
    public Transform target;

    [Header("Позиция относительно цели")]
    [Tooltip("Расстояние камеры от цели по оси Z.")]
    public float distance = 5.0f;

    [Tooltip("Высота камеры относительно цели по оси Y.")]
    public float height = 2.0f;

    [Header("Плавность движения")]
    [Tooltip("Скорость сглаживания позиции камеры.")]
    public float positionSmoothSpeed = 0.125f;

    [Tooltip("Скорость сглаживания поворота камеры.")]
    public float rotationSmoothSpeed = 5.0f;

    [Header("Угол обзора")]
    [Tooltip("Угол поворота камеры по оси X.")]
    public float pitch = 20.0f;

    [Tooltip("Максимальный угол поворота камеры по оси Y.")]
    public float yaw = 0.0f;

    private Vector3 desiredPosition;
    private Quaternion desiredRotation;
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null)
            return;

        // Вычисляем желаемую позицию камеры
        desiredPosition = target.position - target.forward * distance + Vector3.up * height;

        // Плавное перемещение камеры к желаемой позиции
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, positionSmoothSpeed);

        // Вычисляем желаемый поворот камеры
        desiredRotation = Quaternion.Euler(pitch, target.eulerAngles.y + yaw, 0);

        // Плавное вращение камеры к желаемому повороту
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSmoothSpeed);
    }
}