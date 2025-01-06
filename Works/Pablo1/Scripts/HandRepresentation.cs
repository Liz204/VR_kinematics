using UnityEngine;
using TMPro;

public class HandRepresentation : MonoBehaviour
{
    public TMP_Text debugText; // Reference to the TextMeshPro text for displaying messages

    void Update()
    {
        // Variable to store the message
        string message = "";

        // Check if the right controller is connected
        if (OVRInput.IsControllerConnected(OVRInput.Controller.RTouch))
        {
            message += "Right controller detected.\n";

            // Check if any button on the right controller is pressed
            if (OVRInput.GetDown(OVRInput.Button.Any))
            {
                message += "A button on the right controller was pressed.";
            }
            else
            {
                message += "No button has been pressed.";
            }
        }
        else
        {
            message += "Right controller not detected.";
        }

        // Update the text in TMP
        if (debugText != null)
        {
            debugText.text = message;
        }
    }
}
