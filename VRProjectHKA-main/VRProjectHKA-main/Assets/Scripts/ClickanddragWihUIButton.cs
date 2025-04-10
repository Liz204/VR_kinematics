using UnityEngine;
using UnityEngine.UI;

public class ClickAndDragWithUI : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    public Button endArmCreationButton;
    public bool isDraggable = false;

    private Vector3 offset;
    private float zCoord;

    // Reference to the object's renderer for visual feedback
    private Renderer objectRenderer;

    void Start()
    {
        mainCamera = Camera.main;
        endArmCreationButton.onClick.AddListener(StartDraggableMode);

        objectRenderer = GetComponent<Renderer>();
        objectRenderer.enabled = false;
    }

    void StartDraggableMode()
    {
        objectRenderer.enabled = true;
        isDraggable = true;
    }

    void Update()
    {
        if (isDraggable)
        {
            HandleInput();
        }

        if (isDragging)
        {
            // Move the object to the pointer position
            Vector3 pointerPosition = GetPointerPosition();
            transform.position = pointerPosition + offset;
        }
    }

    private void HandleInput()
    {
        // When the select button is pressed
        if (IsSelectButtonDown())
        {
            Ray ray = GetPointerRay();

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    // Begin dragging
                    isDragging = true;

                    // Save the offset between the object and the pointer position
                    zCoord = mainCamera.WorldToScreenPoint(transform.position).z;
                    offset = transform.position - GetPointerWorldPosition();
                }
            }
        }

        // When the select button is released
        if (IsSelectButtonUp())
        {
            isDragging = false;
        }
    }

    private Ray GetPointerRay()
    {
        // TODO: Replace with VR controller's ray when implementing VR
        return mainCamera.ScreenPointToRay(Input.mousePosition);
    }

    private bool IsSelectButtonDown()
    {
        // TODO: Replace with VR controller's select button down check
        return Input.GetMouseButtonDown(0);
    }

    private bool IsSelectButtonUp()
    {
        // TODO: Replace with VR controller's select button up check
        return Input.GetMouseButtonUp(0);
    }

    private Vector3 GetPointerPosition()
    {
        Vector3 pointerScreenPosition = Input.mousePosition;
        pointerScreenPosition.z = zCoord;
        return mainCamera.ScreenToWorldPoint(pointerScreenPosition);
    }

    private Vector3 GetPointerWorldPosition()
    {
        Vector3 pointerScreenPosition = Input.mousePosition;
        pointerScreenPosition.z = zCoord;
        return mainCamera.ScreenToWorldPoint(pointerScreenPosition);
    }
}