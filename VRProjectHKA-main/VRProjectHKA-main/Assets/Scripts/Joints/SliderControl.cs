using UnityEngine;

public class SliderControl : MonoBehaviour
{
    [Header("References")]
    public Transform rail;             // The rail (outer cylinder)
    public Camera mainCamera;          // Camera for raycasting if needed

    [Header("VR Settings")]
    public Transform vrHand;           // Assign your VR hand/controller transform here
    public bool isVRGrabbing = false;  // This would be set by your VR input system

    [Header("Slider Settings")]
    [Range(0f,1f)]
    public float sliderValue = 0.5f;   // 0 at the start, 1 at the end of the rail

    private float railLength;          // Length of the rail in world space
    private float railStart;           // World-space start X-position of the slider range
    private float railEnd;             // World-space end X-position of the slider range

    // Mouse dragging state
    private bool isMouseDragging = false;
    private float mouseDragOffset = 0f;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Assuming the cylinder is oriented along X-axis and its length is represented by localScale.y
        railLength = rail.localScale.y;
        railStart = rail.position.x - (railLength * 0.5f);
        railEnd = rail.position.x + (railLength * 0.5f);

        // Initialize the slider position based on sliderValue
        SetSliderValue(sliderValue);
    }

    void Update()
    {
        // If VR hand is assigned and we are grabbing, control via VR
        if (vrHand != null && isVRGrabbing)
        {
            HandleVRInput();
        }
        else
        {
            // Otherwise, use mouse input if no VR hand or not grabbing in VR
            HandleMouseInput();
        }
    }

    #region Mouse Input Methods
    void OnMouseDown()
    {
        if (vrHand == null || !isVRGrabbing)
        {
            isMouseDragging = true;
            var hitPoint = GetMouseWorldPoint();
            if (hitPoint.HasValue)
            {
                // Offset ensures smooth dragging from the click point
                mouseDragOffset = transform.position.x - hitPoint.Value.x;
            }
        }
    }

    void OnMouseUp()
    {
        if (vrHand == null || !isVRGrabbing)
        {
            isMouseDragging = false;
        }
    }

    private void HandleMouseInput()
    {
        if (isMouseDragging)
        {
            var hitPoint = GetMouseWorldPoint();
            if (hitPoint.HasValue)
            {
                // Apply offset to maintain smooth dragging
                float targetX = hitPoint.Value.x + mouseDragOffset;
                float newSliderValue = Mathf.InverseLerp(railStart, railEnd, Mathf.Clamp(targetX, railStart, railEnd));
                SetSliderValue(newSliderValue);
            }
        }
    }

    private Vector3? GetMouseWorldPoint()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);
        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return null;
    }
    #endregion

    #region VR Input Methods
    private void HandleVRInput()
    {
        // Take the VR hand's X position, clamp it to the rail, and set the slider
        float handX = vrHand.position.x;
        float clampedX = Mathf.Clamp(handX, railStart, railEnd);
        float newSliderValue = Mathf.InverseLerp(railStart, railEnd, clampedX);
        SetSliderValue(newSliderValue);
    }
    #endregion

    /// <summary>
    /// Sets the slider value (0 to 1) and updates the slider position along the rail.
    /// </summary>
    public void SetSliderValue(float value)
    {
        sliderValue = Mathf.Clamp01(value);
        float xPos = Mathf.Lerp(railStart, railEnd, sliderValue);
        Vector3 newPos = transform.position;
        newPos.x = xPos;
        transform.position = newPos;
    }
}