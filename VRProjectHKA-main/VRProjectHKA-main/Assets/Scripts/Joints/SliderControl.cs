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

    [Header("Rotation Settings")]
    public float rotationFactor = 45f; // Degrees per unit of "other" movement (now along z-axis)

    private float railLength;          // Length of the rail in world space
    private float railStart;           // World-space start X-position of the slider range
    private float railEnd;             // World-space end X-position of the slider range

    // Interaction states
    private bool isDragging = false;
    private bool useVR = false; 
    private bool isHovering = false; 

    // Dragging references
    private Vector3 initialDragPoint;  // The world point on the plane where dragging started
    private float initialSliderValue;
    private float initialRotationX; 

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Determine if we are using VR based on RayInteractor availability
        useVR = (rayInteractor != null);

        // Rail assumed along X-axis
        railLength = rail.localScale.y;
        railStart = rail.position.x - (railLength * 0.5f);
        railEnd = rail.position.x + (railLength * 0.5f);

        // Initialize the slider position
        SetSliderValue(sliderValue);
    }

    void Update()
    {
        HandleHover();

        if (!isDragging)
        {
            // Start dragging if select pressed and hovering
            if (IsSelectButtonDown() && isHovering)
            {
                StartDragging();
            }
        }
        else
        {
            // If dragging, update or stop based on input
            if (IsSelectButtonPressed())
            {
                UpdateDragging();
            }
            else
            {
                StopDragging();
            }
        }
    }

    private void HandleHover()
    {
        Ray ray = GetPointerRay();
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                if (!isHovering)
                {
                    isHovering = true;
                    // (Optional) Change appearance on hover
                    // Change color to yellow
                    GetComponent<Renderer>().material.color = Color.yellow;
                }
                return;
            }
        }

        if (isHovering)
        {
            isHovering = false;
            // (Optional) Revert hover appearance
            GetComponent<Renderer>().material.color = Color.gray;
        }
    }

    private void StartDragging()
    {
        isDragging = true;
        Vector3? hitPoint = GetPointerWorldPoint();
        if (hitPoint.HasValue)
        {
            initialDragPoint = hitPoint.Value;
            initialSliderValue = sliderValue;
            initialRotationX = transform.eulerAngles.x;
        }
    }

    private void StopDragging()
    {
        isDragging = false;
    }

    private void UpdateDragging()
{
    Vector3? hitPoint = GetPointerWorldPoint();
    if (!hitPoint.HasValue) return;

    Vector3 currentPoint = hitPoint.Value;
    Vector3 delta = currentPoint - initialDragPoint;

    // Adjust slider value based on horizontal movement
    float horizontalDeltaX = delta.x;
    float newSliderValue = initialSliderValue + (horizontalDeltaX / (railEnd - railStart));
    SetSliderValue(newSliderValue);

    // // Adjust rotation based on vertical movement
    // float verticalMove = delta.z; 
    // float newRotationX = initialRotationX + (verticalMove * rotationFactor);

    // // Directly set rotation using known desired axes:
    // transform.rotation = Quaternion.Euler(newRotationX, 0f, 90f);
}

    public void SetSliderValue(float value)
    {
        sliderValue = Mathf.Clamp01(value);
        float xPos = Mathf.Lerp(railStart, railEnd, sliderValue);
        Vector3 newPos = transform.position;
        newPos.x = xPos;
        transform.position = newPos;
    }

    #region Input and Ray Methods

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
        // Instead of using camera forward, use a stable world direction:
        // For example, a horizontal plane defined by Vector3.up:
        Plane plane = new Plane(Camera.main.transform.forward, transform.position);

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
            // VR button down
            if(OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger)){
                 Debug.Log("Fisrt");
            }
            return OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);
        }
        else
        {
            // Mouse button down
            return Input.GetMouseButtonDown(0);
        }
    }

    private bool IsSelectButtonPressed()
    {
        if (useVR)
        {
            // VR button pressed
            if(OVRInput.Get(OVRInput.Button.SecondaryHandTrigger)){
                 Debug.Log("Second");
            }
            return OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
        }
        else
        {
            // Mouse button held
            return Input.GetMouseButton(0);
        }
    }

    #endregion
}