

using UnityEngine;

public class OrbitAround : MonoBehaviour
{
    public Transform target; // Aylanish markazi (jism)
    public float rotationSpeed = 5f; // Aylanish tezligi
    public float zoomSpeed = 2f; // Yaqinlashish va uzoqlashish tezligi
    public float minDistance = 2f; // Minimal masofa
    public float maxDistance = 10f; // Maksimal masofa
    public float distance = 5f; // Markazdan boshlang‘ich masofa
    private float rotationX = 0f; // X o'qi bo'ylab aylanish
    private float rotationY = 0f; // Y o'qi bo'ylab aylanish
    public float minVerticalAngle = 0f; // X o'qdagi minimal burchak
    public float maxVerticalAngle = 80f; // X o'qdagi maksimal burchak

    void Start()
    {
        // Kamerani boshlang‘ich joyda o‘rnatish
        if (target != null)
        {
            Vector3 direction = (transform.position - target.position).normalized;
            transform.position = target.position + direction * distance;
            transform.LookAt(target);
        }
    }

    void Update()
    {
        if (target != null)
        {
            HandleRotation();
            HandleZoom();
        }
    }

    void HandleRotation()
    {
        // Telefon uchun Touch input
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;

                // Y o'qi bo'ylab aylanish (gorizontal)
                rotationY += delta.x * rotationSpeed * Time.deltaTime;

                // X o'qi bo'ylab aylanish (vertikal) va cheklash
                rotationX -= delta.y * rotationSpeed * Time.deltaTime;
                rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

                // Kameraning pozitsiyasini yangilash
                UpdateCameraPosition();
            }
        }
        // Kompyuter uchun sichqoncha input
        else if (Input.GetMouseButton(0))
        {
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;
            float vertical = -Input.GetAxis("Mouse Y") * rotationSpeed;

            // Y o'qi bo'ylab aylanish (gorizontal)
            rotationY += horizontal;

            // X o'qi bo'ylab aylanish (vertikal) va cheklash
            rotationX += vertical;
            rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

            // Kameraning pozitsiyasini yangilash
            UpdateCameraPosition();
        }
    }

    void HandleZoom()
    {
        // Telefon uchun Pinch (ikkita barmoq) input
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // Oldingi va joriy masofani hisoblash
            float previousDistance = (touch1.position - touch1.deltaPosition - (touch2.position - touch2.deltaPosition)).magnitude;
            float currentDistance = (touch1.position - touch2.position).magnitude;

            // Masofa o'zgarishiga qarab yaqinlashish yoki uzoqlashish
            float zoomDelta = (previousDistance - currentDistance) * zoomSpeed * Time.deltaTime;
            distance = Mathf.Clamp(distance + zoomDelta, minDistance, maxDistance);

            // Kameraning pozitsiyasini yangilash
            UpdateCameraPosition();
        }
        // Kompyuter uchun g'ildirak input
        else
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);

            // Kameraning pozitsiyasini yangilash
            UpdateCameraPosition();
        }
    }

    void UpdateCameraPosition()
    {
        // Kameraning yangi pozitsiyasini hisoblash
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        Vector3 direction = rotation * Vector3.back * distance;
        transform.position = target.position + direction;

        // Kamerani markazga qarating
        transform.LookAt(target);
    }
}
