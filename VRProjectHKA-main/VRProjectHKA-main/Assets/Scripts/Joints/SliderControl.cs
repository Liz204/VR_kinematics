using UnityEngine;
using Oculus.Interaction; // For RayInteractor and VR input handling

public class SliderControl : MonoBehaviour
{
    [Header("References")]
    public Transform rail;             // The rail (outer cylinder)
    public RayInteractor rayInteractor; // VR Ray Interactor (if available)
    public Camera mainCamera;          // Fallback camera for mouse input if VR not available

    [Header("Slider Settings")]
    [Range(0f, 1f)]
    public float sliderValue = 0.5f;   // 0 at the start, 1 at the end of the rail

    private float railLength;          // Length of the rail in world space
    private float railStart;           // World-space start X-position of the slider range
    private float railEnd;             // World-space end X-position of the slider range

    // Interaction states
    private bool isDragging = false;
    private float dragOffset = 0f;

    private bool useVR = false; // Will be true if VR input (RayInteractor) is available
    private bool isHovering = false; // Is the pointer hovering over the slider?

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Determine if we are using VR based on RayInteractor availability
        useVR = (rayInteractor != null);

        // Assuming the cylinder is oriented along the X-axis and is centered at rail.position
        railLength = rail.localScale.y;
        railStart = rail.position.x - (railLength * 0.82f);
        railEnd = rail.position.x + (railLength * 0.82f);

        // Initialize the slider position based on sliderValue
        SetSliderValue(sliderValue);
    }

    void Update()
    {
        HandleHover();

        if (!isDragging)
        {
            // If not currently dragging, check if the user tries to start dragging
            if (IsSelectButtonDown() && isHovering)
            {
                StartDragging();
            }
        }
        else
        {
            // If currently dragging, update slider position
            if (IsSelectButtonPressed())
            {
                UpdateDragging();
            }
            else
            {
                // If user released the button, stop dragging
                StopDragging();
            }
        }
    }

    private void HandleHover()
    {
        // Raycast to check if the pointer is over the slider
        Ray ray = GetPointerRay();
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                if (!isHovering)
                {
                    isHovering = true;
                    // You could change appearance here if you like, e.g. highlight the slider
                }
                return;
            }
        }

        if (isHovering)
        {
            isHovering = false;
            // Revert any hover appearance change
        }
    }

    private void StartDragging()
    {
        isDragging = true;
        Vector3? hitPoint = GetPointerWorldPoint();
        if (hitPoint.HasValue)
        {
            // Calculate offset to ensure smooth dragging
            dragOffset = transform.position.x - hitPoint.Value.x;
        }
    }

    private void StopDragging()
    {
        isDragging = false;
    }

    private void UpdateDragging()
    {
        Vector3? hitPoint = GetPointerWorldPoint();
        if (hitPoint.HasValue)
        {
            float targetX = hitPoint.Value.x + dragOffset;
            float newSliderValue = Mathf.InverseLerp(railStart, railEnd, Mathf.Clamp(targetX, railStart, railEnd));
            SetSliderValue(newSliderValue);
        }
    }

    public void SetSliderValue(float value)
    {
        sliderValue = Mathf.Clamp01(value);
        float xPos = Mathf.Lerp(railStart, railEnd, sliderValue);
        Vector3 newPos = transform.position;
        newPos.x = xPos;
        transform.position = newPos;
    }

    #region Input Helpers

    private Ray GetPointerRay()
    {
        if (useVR && rayInteractor != null)
        {
            // Use VR ray
            return rayInteractor.Ray;
        }
        else
        {
            // Fallback to mouse ray
            return mainCamera.ScreenPointToRay(Input.mousePosition);
        }
    }

    private Vector3? GetPointerWorldPoint()
    {
        Ray ray = GetPointerRay();

        // Create a plane perpendicular to the camera direction, passing through the slider's current position
        Plane plane = new Plane(mainCamera.transform.forward, transform.position);

        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return null;
    }

    private bool IsSelectButtonDown()
    {
        if (useVR)
        {
            // VR input: Check for trigger press down
            return OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);
        }
        else
        {
            // Mouse input
            return Input.GetMouseButtonDown(0);
        }
    }

    private bool IsSelectButtonPressed()
    {
        if (useVR)
        {
            // VR input: Check if trigger is still held
            return OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
        }
        else
        {
            // Mouse input
            return Input.GetMouseButton(0);
        }
    }

    #endregion
}
