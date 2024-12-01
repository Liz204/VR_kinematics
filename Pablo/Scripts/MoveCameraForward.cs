using UnityEngine;

public class MoveCameraForward : MonoBehaviour
{
    public float distanceToMove = 1f; // Distance the camera will move forward
    void Start()
    {
        
    }

    void Update()
    {
        
        // Detect left mouse click or VR controller back button
        if (Input.GetMouseButtonDown(0) || OVRInput.GetDown(OVRInput.Button.Back) || OVRInput.GetDown(OVRInput.Button.Two) )
        {
            MoveForward();
        }
    }

    void MoveForward()
    {
        // Move the XRRig forward in the direction of its forward vector
        transform.position += transform.forward * distanceToMove;
        Debug.Log("Moved forward by " + distanceToMove + " units.");
    }

}
