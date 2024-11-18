using UnityEngine;
using System.Collections.Generic;
// Add any necessary namespaces for VR
using UnityEngine.XR; // For VR input (if needed)

public class MechanicalArmBuilder : MonoBehaviour
{
    public GameObject jointPrefab;          // Prefab for the joint (e.g., a sphere)
    public GameObject armSegmentPrefab;     // Prefab for the arm segment (e.g., a cylinder)
    public float interactionPlaneDistance = 0.1f; // Distance of the interaction plane from the camera
    public Color normalColor = Color.white;      // Color when joint is not selected
    public Color selectableColor = Color.green; // Color when joint is selectable
    public Color hoverColor = Color.yellow;     // Color when hovering over the joint

    private GameObject currentJoint;        // The currently selected joint
    private GameObject newJoint;            // The new joint being created
    private GameObject armSegment;          // The arm segment connecting the joints
    private Renderer currentJointRenderer;  // Renderer for the current joint to change its color
    private bool isDragging = false;        // Is the user dragging a new joint?
    private bool isHovering = false;        // Is the pointer hovering over the joint?

    public List<GameObject> joints = new List<GameObject>();        // List to store joints
    public List<GameObject> armSegments = new List<GameObject>();   // List to store arm segments

    void Start()
    {
        // Initialize with the first joint in the center of the screen
        currentJoint = Instantiate(jointPrefab, Vector3.zero, Quaternion.identity);
        currentJointRenderer = currentJoint.GetComponent<Renderer>();
        currentJointRenderer.material.color = selectableColor; // Set the initial selectable color

        joints.Add(currentJoint);
    }

    void Update()
    {
        if (isDragging && newJoint != null)
        {
            // Update the position of the new joint based on pointer position on the interaction plane
            Vector3 targetPosition = GetPointerPositionOnInteractionPlane();
            newJoint.transform.position = targetPosition;

            // Update arm segment to connect currentJoint and newJoint
            UpdateArmSegment();
        }

        // Check for pointer hover to change color
        HandlePointerHover();

        // Check for input to select or create a joint
        if (IsSelectButtonDown() && !isDragging && isHovering)
        {
            CreateNewJoint();
        }

        // Release the new joint if the select button is released
        if (IsSelectButtonUp() && isDragging)
        {
            isDragging = false;
            currentJointRenderer.material.color = normalColor; // Reset the color of the current joint
            currentJoint = newJoint;  // Make the new joint the current joint for the next segment
            currentJointRenderer = currentJoint.GetComponent<Renderer>();
            currentJointRenderer.material.color = selectableColor; // Set the color back to selectable
            newJoint = null;
        }
    }

    private void HandlePointerHover()
    {
        // Raycast to check if the pointer is over the current joint
        Ray ray = GetPointerRay();

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == currentJoint)
            {
                // If hovering over the joint, change to hover color
                if (!isHovering)
                {
                    isHovering = true;
                    currentJointRenderer.material.color = hoverColor;
                }
                return;
            }
        }

        // If not hovering, revert to selectable color
        if (isHovering)
        {
            isHovering = false;
            currentJointRenderer.material.color = selectableColor;
        }
    }

    private void CreateNewJoint()
    {
        // Create a new joint and start dragging it
        Vector3 initialPosition = GetPointerPositionOnInteractionPlane();
        newJoint = Instantiate(jointPrefab, initialPosition, Quaternion.identity);
        isDragging = true;

        // Create the arm segment between the current joint and new joint
        armSegment = Instantiate(armSegmentPrefab);

        joints.Add(newJoint);
        armSegments.Add(armSegment);

        UpdateArmSegment();
    }

    private Vector3 GetPointerPositionOnInteractionPlane()
    {
        // Create a plane at a fixed distance from the camera
        Plane interactionPlane = new Plane(Camera.main.transform.forward, Camera.main.transform.position + Camera.main.transform.forward * interactionPlaneDistance);

        // Get the pointer ray
        Ray ray = GetPointerRay();

        // Find where the ray intersects with the interaction plane
        if (interactionPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero; // Fallback in case of an error
    }

    private Ray GetPointerRay()
    {
        // TODO VR: Replace this with VR controller's pointing direction when in VR mode
        // For now, use mouse position ray
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    private bool IsSelectButtonDown()
    {
        // TODO VR: Replace this with VR controller's select button down check
        return Input.GetMouseButtonDown(0);
    }

    private bool IsSelectButtonUp()
    {
        // TODO VR: Replace this with VR controller's select button up check
        return Input.GetMouseButtonUp(0);
    }

    private void UpdateArmSegment()
    {
        if (armSegment != null && currentJoint != null && newJoint != null)
        {
            Vector3 direction = newJoint.transform.position - currentJoint.transform.position;
            float distance = direction.magnitude;

            // Position the arm segment between the two joints
            armSegment.transform.position = currentJoint.transform.position + direction / 2;
            armSegment.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);

            // Scale the arm segment to fit the distance between joints, keeping x and z constant
            armSegment.transform.localScale = new Vector3(0.1f, distance / 2f, 0.1f);
        }
    }
}