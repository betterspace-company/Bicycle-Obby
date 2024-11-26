using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player; // Ссылка на персонажа
    public Vector3 offset = new Vector3(0f, 2f, -4f); // Смещение камеры от персонажа
    public float rotationSpeed = 100f; // Скорость вращения камеры
    public float minYAngle = -20f; // Минимальный угол наклона камеры
    public float maxYAngle = 80f; // Максимальный угол наклона камеры

    private float yaw = 0f;   // Вращение по горизонтали
    private float pitch = 0f; // Вращение по вертикали

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player Transform не назначен.");
        }

        // Инициализация углов на основе текущего положения камеры
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void LateUpdate()
    {
        if (player == null)
            return;

        // Проверяем, удерживается ли правая кнопка мыши
        if (Input.GetMouseButton(1)) // 1 - правая кнопка мыши
        {
            // Получаем ввод мыши
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            // Обновляем углы
            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle);
        }

        // Создаем вращение камеры
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = player.position + rotation * offset;

        // Применяем позицию и вращение камеры
        transform.position = desiredPosition;
        transform.LookAt(player.position + Vector3.up * 1.5f); // Смотрим на персонажа
    }
}
