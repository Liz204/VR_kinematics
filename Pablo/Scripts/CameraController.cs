using UnityEngine;
using UnityEngine.XR;

public class CameraController : MonoBehaviour
{
    public Transform vrCamera; // The main camera in the XR Rig
    public float mouseSensitivity = 2.0f; // Mouse sensitivity
    private float xRotation = 0.0f; // Accumulated vertical rotation
    private bool isVRMode; // Active mode: VR or computer

    void Start()
    {
        // Detect if we are in VR
        isVRMode = XRSettings.isDeviceActive;

        if (isVRMode)
        {
            Debug.Log("VR mode activated: The camera will be controlled by the headset.");
        }
        else
        {
            Debug.Log("Computer mode activated: The camera will be controlled by the mouse.");
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        }
    }

    void Update()
    {
        if (isVRMode)
        {
            // The camera automatically follows the headset movement in VR mode
            return;
        }

        // Camera control with the mouse in computer mode
        HandleMouseMovement();
    }

    private void HandleMouseMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the camera vertically (clamped to avoid excessive rotation)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply the rotations
        vrCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Vertical
        transform.Rotate(Vector3.up * mouseX); // Horizontal
    }
}
