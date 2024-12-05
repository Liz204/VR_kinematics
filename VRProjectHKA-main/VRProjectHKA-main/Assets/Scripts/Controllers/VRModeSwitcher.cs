using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
public class VRModeSwitcher : MonoBehaviour
{
    private bool isVRMode = false;

    void Start()
    {
        SetVRMode(isVRMode); // Start in non-VR mode
    }

    void Update()
    {
        // Toggle VR mode with a key press (e.g., 'V')
        if (Input.GetKeyDown(KeyCode.V))
        {
            isVRMode = !isVRMode;
            SetVRMode(isVRMode);
        }
    }

    private void SetVRMode(bool enableVR)
    {
        if (enableVR)
        {
            // Enable VR mode
            XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
            XRGeneralSettings.Instance.Manager.StartSubsystems();
        }
        else
        {
            // Disable VR mode
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
    }
}