using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Positioning")]
    public Vector3 offset;
    public float smoothSpeed = 0.125f;

    [Header("Map Boundaries")]
    public float mapMinX;
    public float mapMaxX;
    public float mapMinY;
    public float mapMaxY;

    private Camera cam;
    private float camHeight;
    private float camWidth;

    void Start()
    {
        cam = GetComponent<Camera>();
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;
    }
    void LateUpdate()
    {
        // 1. Calculate the camera's desired position
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 2. Calculate the actual boundary limits for the camera's center point
        //    This subtracts half the camera's size from the map edge.
        float clampedX = Mathf.Clamp(smoothedPosition.x, mapMinX + camWidth, mapMaxX - camWidth);
        float clampedY = Mathf.Clamp(smoothedPosition.y, mapMinY + camHeight, mapMaxY - camHeight);

        // 3. Apply the final, clamped position
        transform.position = new Vector3(clampedX, clampedY, smoothedPosition.z);
    }
}
