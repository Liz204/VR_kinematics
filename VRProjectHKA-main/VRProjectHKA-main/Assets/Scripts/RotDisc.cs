using UnityEngine;

public class RotationDisc : MonoBehaviour
{
    public enum Axis { X, Y, Z } // Enumeration for the three axes

    public Axis axisToRotate; // Axis in which the disc will control the rotation
    public GameObject cube; // Reference to the cube that will rotate
    private bool isDragging = false; // Drag state
    public float rotationSpeed = 1000f; // Rotation speed

    private void Update()
    {
        // Main interaction logic with VR
        if (IsSelectButtonDown()) // Detects if the controller button has been pressed
        {
            Ray pointerRay = GetPointerRay(); // Get the controller ray
            RaycastHit hit;

            // Check if the ray hits this disc
            if (Physics.Raycast(pointerRay, out hit))
            {
                if (hit.collider.gameObject == gameObject) // Verify if the touched object is this disc
                {
                    isDragging = true;
                }
            }
        }

        if (isDragging && cube != null)
        {
            // Detects the movement of the controller
            float rotationAmount = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x * rotationSpeed * Time.deltaTime;

            // Apply rotation on the selected axis
            if (axisToRotate == Axis.X)
            {
                cube.transform.Rotate(Vector3.right, -rotationAmount); // Opposite direction on X
            }
            else if (axisToRotate == Axis.Y)
            {
                cube.transform.Rotate(Vector3.up, -rotationAmount); // Opposite direction on Y
            }
            else if (axisToRotate == Axis.Z)
            {
                cube.transform.Rotate(Vector3.forward, rotationAmount); // No change in Z
            }
        }

        // If the select button is no longer pressed, stop dragging
        if (OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
        {
            isDragging = false;
        }
    }

    private Ray GetPointerRay()
    {
        // Create a ray based on the controller's position and direction
        return new Ray(Camera.main.transform.position, Camera.main.transform.forward);
    }

    private bool IsSelectButtonDown()
    {
        // Check if the select button is pressed
        return OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);
    }
}
