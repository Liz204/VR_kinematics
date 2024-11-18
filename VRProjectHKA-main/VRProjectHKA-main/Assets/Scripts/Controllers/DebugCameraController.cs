using UnityEngine;

public class DebugCameraController : MonoBehaviour
{
    public float moveSpeed = 10f; // Speed for WASD movement
    public float lookSpeed = 2f;  // Sensitivity for mouse rotation
    public float maxYAngle = 80f; // Maximum up/down angle for camera rotation

    private Vector2 rotation = Vector2.zero;
    private bool isCameraControlActive = true; // Toggle for camera control mode

    void Start()
    {
        EnableCameraControl(); // Start with the camera control mode enabled
    }

    void Update()
    {
        // Toggle camera control mode with the Tab key
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isCameraControlActive = !isCameraControlActive;

            if (isCameraControlActive)
            {
                EnableCameraControl();
            }
            else
            {
                DisableCameraControl();
            }
        }

        // Only allow camera movement and rotation if camera control is active
        if (isCameraControlActive)
        {
            HandleMovement();
            HandleRotation();
        }
    }

    // Method to handle WASD movement
    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); // A and D keys
        float moveVertical = Input.GetAxis("Vertical");     // W and S keys
        Vector3 direction = new Vector3(moveHorizontal, 0, moveVertical).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.Self);
    }

    // Method to handle mouse rotation
    private void HandleRotation()
    {
        rotation.x += Input.GetAxis("Mouse X") * lookSpeed;
        rotation.y -= Input.GetAxis("Mouse Y") * lookSpeed;
        rotation.y = Mathf.Clamp(rotation.y, -maxYAngle, maxYAngle);
        transform.rotation = Quaternion.Euler(rotation.y, rotation.x, 0);
    }

    // Enable camera control and lock the cursor
    private void EnableCameraControl()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Disable camera control and unlock the cursor
    private void DisableCameraControl()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}