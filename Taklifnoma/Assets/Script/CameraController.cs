using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // Kuzatish markazi (jism)
    public float rotationSpeed = 5.0f;
    public float zoomSpeed = 2.0f;
    public float panSpeed = 0.5f;
    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;

    private Vector3 offset;
    private float distance;
    private Vector2 lastTouchPosition;
    private bool isRotating = false;
    private bool isPanning = false;

    void Start()
    {
        distance = Vector3.Distance(transform.position, target.position);
        offset = transform.position - target.position;
    }

    void Update()
    {
        if (Input.touchCount == 1) // Aylantirish (bitta barmoq)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                isRotating = true;
            }
            else if (touch.phase == TouchPhase.Moved && isRotating)
            {
                float h = (touch.position.x - lastTouchPosition.x) * rotationSpeed * 0.01f;
                float v = (lastTouchPosition.y - touch.position.y) * rotationSpeed * 0.01f;

                Quaternion rotation = Quaternion.Euler(v, h, 0);
                offset = rotation * offset;
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isRotating = false;
            }
        }
        else if (Input.touchCount == 2) // Zoom va Pan qilish (ikki barmoq)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
            float touchDeltaMag = (touch0.position - touch1.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            distance += deltaMagnitudeDiff * zoomSpeed * 0.01f;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            if (touch0.phase == TouchPhase.Moved && touch1.phase == TouchPhase.Moved)
            {
                Vector2 touchDelta = (touch0.deltaPosition + touch1.deltaPosition) / 2;
                Vector3 move = new Vector3(-touchDelta.x, -touchDelta.y, 0) * panSpeed * 0.01f;
                target.position += transform.right * move.x + transform.up * move.y;
            }
        }

        transform.position = target.position + offset.normalized * distance;
        transform.position = new Vector3(transform.position.x, Mathf.Max(transform.position.y, target.position.y), transform.position.z);
        transform.LookAt(target);
    }
}
