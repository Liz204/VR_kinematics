using UnityEngine;
using UnityEngine.XR;

public class Menu : MonoBehaviour
{
    public GameObject panel; // Panel that will be shown or hidden
    private bool isVRMode;

    void Start()
    {
        // Detect if we are in VR
        isVRMode = XRSettings.isDeviceActive;
        Debug.Log(isVRMode ? "VR mode detected." : "Computer mode detected.");
        
        // Make sure the panel is assigned
        if (panel == null)
        {
            Debug.LogError("The panel is not assigned in the Inspector.");
        }
    }

    void Update()
    {
        if (isVRMode)
        {
            // Check if the "A" button on the right VR controller is pressed
            if (OVRInput.GetDown(OVRInput.Button.One) || OVRInput.GetDown(OVRInput.Button.Two))
            {
                TogglePanelVisibility();
            }
        }
        else
        {
            // Check if the "M" key is pressed on the computer
            if (Input.GetKeyDown(KeyCode.M))
            {
                TogglePanelVisibility();
            }
        }
    }

    private void TogglePanelVisibility()
    {
        if (panel != null)
        {
            bool isActive = panel.activeSelf;
            panel.SetActive(!isActive); // Toggle the visibility state of the panel
            Debug.Log(isActive ? "Panel hidden." : "Panel shown.");
        }
    }
}
