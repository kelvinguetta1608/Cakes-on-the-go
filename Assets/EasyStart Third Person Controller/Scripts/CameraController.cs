using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Camera movement script for third-person games.
/// This Script should not be applied to the camera! It is attached to an empty object, and inside
/// it (as a child object) should be your game's MainCamera.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Tooltip("Enable to move the camera by holding the right mouse button. Does not work with joysticks.")]
    public bool clickToMoveCamera = false;

    [Tooltip("Enable zoom in/out when scrolling the mouse wheel. Does not work with joysticks.")]
    public bool canZoom = true;

    [Tooltip("The tag of the object the camera should follow.")]
    public string targetTag = "Player";

    [Space]
    [Tooltip("The higher it is, the faster the camera moves. It is recommended to increase this value for games that use joysticks.")]
    public float sensitivity = 5f;

    [Tooltip("Camera Y rotation limits. The X axis is the maximum it can go up and the Y axis is the maximum it can go down.")]
    public Vector2 cameraLimit = new Vector2(-45, 40);

    private float mouseX;
    private float mouseY;
    private float offsetDistanceY;

    private Transform target; // The object to follow

    private void Start()
    {
        GameObject targetObject = GameObject.FindWithTag(targetTag);
        if (targetObject != null)
        {
            target = targetObject.transform;
        }
        else
        {
            Debug.LogWarning($"No object found with tag '{targetTag}'. Camera will not follow any object.");
        }

        offsetDistanceY = transform.position.y;

        if (!clickToMoveCamera)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (target != null)
        {
            FollowTarget();
            RotateCamera();
            HandleZoom();
        }
    }

    private void FollowTarget()
    {
        transform.position = target.position + new Vector3(0, offsetDistanceY, 0);
    }

    private void RotateCamera()
    {
        Vector3 forwardDirection = target.forward;
        Quaternion targetRotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * sensitivity);
    }

    private void HandleZoom()
    {
        if (canZoom && Mouse.current.scroll.y.ReadValue() != 0)
        {
            float zoomAmount = Mouse.current.scroll.y.ReadValue() * sensitivity * -0.1f;
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView + zoomAmount, 20, 60);
        }
    }
}
