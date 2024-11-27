using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using BioIK;
public class JointProperties : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public enum JointType
    {   
        Rotational,    // Rotates around a fixed axis
        Translational, // Moves along a linear path
        
    }

    [Header("Joint Settings")]
    public JointType jointType = JointType.Rotational;

    [Header("Restrictions")]
    public bool enabledX = false;
    public bool constrainedX = false;
    public float lowerLimitX = 0f;
    public float upperLimitX = 0f;
    public bool enabledY = false;
    public bool constrainedY = false;
    public float lowerLimitY = 0f;
    public float upperLimitY = 0f;

    [Header("Additional Properties")]
    public bool isFixed = false; // If true, the joint is fixed and cannot move or rotate
    public BioSegment correspondingBioSegment = null;

    [Header("UI Elements")]
    public GameObject jointSettingsPanelPrefab; // Assign your prefab in the Inspector
    private GameObject jointSettingsPanelInstance;
    
    private Renderer sphereRenderer; // Renderer to change color on hover
    public Color hoverColor = Color.yellow; // Color when hovering
    public Color normalColor = Color.white; // Default color
    private bool isHovering = false;

    public Transform previousJoint;
    public Transform nextJoint;

    private static JointProperties currentlySelectedJoint = null;

    void Start()
    {
        sphereRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (correspondingBioSegment)
        {
            HandlePointerHover();

            if (IsRightClick() && isHovering)
            {
                ShowJointSettingsMenu();
            }

            // Close the panel if clicking outside
            if (jointSettingsPanelInstance != null && Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
            {
                // Only close the menu if the click is not over the menu itself
                if (!isHoveringOverMenu)
                {
                    CloseJointSettingsMenu();
                }
            }
        }
    }

    private bool isHoveringOverMenu = false;

    private void ShowJointSettingsMenu()
    {
        if (jointSettingsPanelInstance == null)
        {
            // Close any previously open panel
            if (currentlySelectedJoint != null && currentlySelectedJoint != this)
            {
                currentlySelectedJoint.CloseJointSettingsMenu();
            }

            // Instantiate the panel
            jointSettingsPanelInstance = Instantiate(jointSettingsPanelPrefab);
            

            // Set the panel's parent to the Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            jointSettingsPanelInstance.transform.SetParent(canvas.transform, false);

            // Position the panel near the joint
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPosition,
                canvas.worldCamera,
                out Vector2 localPoint);

            jointSettingsPanelInstance.GetComponent<RectTransform>().localPosition = localPoint;

            // Initialize the UI elements
            InitializeUIPanel();

            // Set the current selected joint
            currentlySelectedJoint = this;
        }
        else
        {
            // Close the panel if it's already open
            CloseJointSettingsMenu();
        }
    }

    private void CloseJointSettingsMenu()
    {
        if (jointSettingsPanelInstance != null)
        {
            Destroy(jointSettingsPanelInstance);
            jointSettingsPanelInstance = null;
        }

        if (currentlySelectedJoint == this)
        {
            currentlySelectedJoint = null;
        }
    }

    private void InitializeUIPanel()
    {
        // Find UI components in the panel

        var jointsettings = jointSettingsPanelInstance.transform;
        //add debug log to check
        TMP_Dropdown typeDropdown = jointSettingsPanelInstance.transform.Find("Type").GetComponent<TMP_Dropdown>();
        Toggle enableXToggle = jointSettingsPanelInstance.transform.Find("EnableX").GetComponent<Toggle>();
        Toggle enableYToggle = jointSettingsPanelInstance.transform.Find("EnableY").GetComponent<Toggle>();
        Toggle constrainedXToggle = jointSettingsPanelInstance.transform.Find("ConstrainedX").GetComponent<Toggle>();
        Toggle constrainedYToggle = jointSettingsPanelInstance.transform.Find("ConstrainedY").GetComponent<Toggle>();
        Slider lowerLimitXSlider = jointSettingsPanelInstance.transform.Find("LowerLimitX").GetComponent<Slider>();
        Slider upperLimitXSlider = jointSettingsPanelInstance.transform.Find("UpperLimitX").GetComponent<Slider>();
        Slider lowerLimitYSlider = jointSettingsPanelInstance.transform.Find("LowerLimitY").GetComponent<Slider>();
        Slider upperLimitYSlider = jointSettingsPanelInstance.transform.Find("UpperLimitY").GetComponent<Slider>();

        // Remove previous listeners to prevent duplicate calls
        typeDropdown.onValueChanged.RemoveAllListeners();
        enableXToggle.onValueChanged.RemoveAllListeners();
        enableYToggle.onValueChanged.RemoveAllListeners();
        constrainedXToggle.onValueChanged.RemoveAllListeners();
        constrainedYToggle.onValueChanged.RemoveAllListeners();
        lowerLimitXSlider.onValueChanged.RemoveAllListeners();
        upperLimitXSlider.onValueChanged.RemoveAllListeners();
        lowerLimitYSlider.onValueChanged.RemoveAllListeners();
        upperLimitYSlider.onValueChanged.RemoveAllListeners();

        // Set the UI components to reflect the current joint settings
        typeDropdown.value = (int)jointType;
        enableXToggle.isOn = enabledX;
        enableYToggle.isOn = enabledY;
        constrainedXToggle.isOn = constrainedX;
        constrainedYToggle.isOn = constrainedY;
        lowerLimitXSlider.value = lowerLimitX;
        upperLimitXSlider.value = upperLimitX;
        lowerLimitYSlider.value = lowerLimitY;
        upperLimitYSlider.value = upperLimitY;

        // Add listeners to UI components to update joint properties when changed
        typeDropdown.onValueChanged.AddListener(OnTypeChanged);
        enableXToggle.onValueChanged.AddListener(OnEnableXChanged);
        enableYToggle.onValueChanged.AddListener(OnEnableYChanged);
        constrainedXToggle.onValueChanged.AddListener(onConstrainedXChanged);
        constrainedYToggle.onValueChanged.AddListener(onConstrainedYChanged);
        lowerLimitXSlider.onValueChanged.AddListener(OnLowerLimitXChanged);
        upperLimitXSlider.onValueChanged.AddListener(OnUpperLimitXChanged);
        lowerLimitYSlider.onValueChanged.AddListener(OnLowerLimitYChanged);
        upperLimitYSlider.onValueChanged.AddListener(OnUpperLimitYChanged);

        // Add event triggers to detect when the pointer is over the menu
        EventTrigger trigger = jointSettingsPanelInstance.AddComponent<EventTrigger>();
        EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((eventData) => { isHoveringOverMenu = true; });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((eventData) => { isHoveringOverMenu = false; });
        trigger.triggers.Add(entryExit);
    }

    private void OnTypeChanged(int value)
    {
        jointType = (JointType)value;
        Debug.Log($"Joint type changed to: {jointType}");
        // Additional logic if needed
    }

    private void OnEnableXChanged(bool value)
    {
        enabledX = value;
        Debug.Log($"Enable X changed to: {enabledX}");
        if (correspondingBioSegment != null)
        {
            correspondingBioSegment.Joint.X.Enabled = value;
            Debug.Log($"BioSegment.X.Enabled updated to: {value}");
        }
    }

    private void OnEnableYChanged(bool value)
    {
        enabledY = value;
        Debug.Log($"Enable Y changed to: {enabledY}");
        if (correspondingBioSegment != null)
        {
            correspondingBioSegment.Joint.Y.Enabled = value;
            Debug.Log($"BioSegment.Y.Enabled updated to: {value}");
        }
    }

    private void onConstrainedXChanged(bool value)
    {
        constrainedX = value;
        Debug.Log($"Constrained X changed to: {constrainedX}");
        if (correspondingBioSegment != null)
        {
            correspondingBioSegment.Joint.X.Constrained = value;
            Debug.Log($"BioSegment.X.Constrained updated to: {value}");
        }
    }

    private void onConstrainedYChanged(bool value)
    {
        constrainedY = value;
        Debug.Log($"Constrained Y changed to: {constrainedY}");
        if (correspondingBioSegment != null)
        {
            correspondingBioSegment.Joint.Y.Constrained = value;
            Debug.Log($"BioSegment.Y.Constrained updated to: {value}");
        }
    }

    private void OnLowerLimitXChanged(float value)
    {
        lowerLimitX = value;
        if (lowerLimitX > upperLimitX)
        {
            lowerLimitX = upperLimitX;
            Slider slider = jointSettingsPanelInstance.transform.Find("LowerLimitX").GetComponent<Slider>();
            slider.value = lowerLimitX;
        }
        Debug.Log($"Lower Limit X changed to: {lowerLimitX}");
        if (correspondingBioSegment != null)
        {
            correspondingBioSegment.Joint.X.LowerLimit = lowerLimitX;
            Debug.Log($"BioSegment.X.LowerLimit updated to: {lowerLimitX}");
        }
    }

    private void OnUpperLimitXChanged(float value)
    {
        upperLimitX = value;
        if (upperLimitX < lowerLimitX)
        {
            upperLimitX = lowerLimitX;
            Slider slider = jointSettingsPanelInstance.transform.Find("UpperLimitX").GetComponent<Slider>();
            slider.value = upperLimitX;
        }
        Debug.Log($"Upper Limit X changed to: {upperLimitX}");
        if (correspondingBioSegment != null)
        {
            correspondingBioSegment.Joint.X.UpperLimit = upperLimitX;
            Debug.Log($"BioSegment.X.UpperLimit updated to: {upperLimitX}");
        }
    }

    private void OnLowerLimitYChanged(float value)
    {
        lowerLimitY = value;
        if (lowerLimitY > upperLimitY)
        {
            lowerLimitY = upperLimitY;
            Slider slider = jointSettingsPanelInstance.transform.Find("LowerLimitY").GetComponent<Slider>();
            slider.value = lowerLimitY;
        }
        Debug.Log($"Lower Limit Y changed to: {lowerLimitY}");
        if (correspondingBioSegment != null)
        {
            correspondingBioSegment.Joint.Y.LowerLimit = lowerLimitY;
            Debug.Log($"BioSegment.Y.LowerLimit updated to: {lowerLimitY}");
        }
    }

    private void OnUpperLimitYChanged(float value)
    {
        upperLimitY = value;
        if (upperLimitY < lowerLimitY)
        {
            upperLimitY = lowerLimitY;
            Slider slider = jointSettingsPanelInstance.transform.Find("UpperLimitY").GetComponent<Slider>();
            slider.value = upperLimitY;
        }
        Debug.Log($"Upper Limit Y changed to: {upperLimitY}");
        if (correspondingBioSegment != null)
        {
            correspondingBioSegment.Joint.Y.UpperLimit = upperLimitY;
            Debug.Log($"BioSegment.Y.UpperLimit updated to: {upperLimitY}");
        }
    }

    private void HandlePointerHover()
    {
        // Raycast to check if the pointer is over this object
        Ray ray = GetPointerRay();

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == this.gameObject) // Check if this sphere is hit
            {
                if (!isHovering)
                {
                    isHovering = true;
                    sphereRenderer.material.color = hoverColor; // Change color on hover
                }
                return;
            }
        }

        // If not hovering, revert to the normal color
        if (isHovering)
        {
            isHovering = false;
            sphereRenderer.material.color = normalColor;
        }
    }

    private Ray GetPointerRay()
    {
        // TODO: Replace with VR controller's pointing direction when in VR mode
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    private bool IsRightClick()
    {
        // TODO: Replace with VR controller input when in VR mode
        return Input.GetMouseButtonDown(1);
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private void OnDestroy()
    {
        CloseJointSettingsMenu();
    }

    public void setPreviousJoint(Transform joint){
        previousJoint = joint;
        previousJoint.GetComponent<JointProperties>().nextJoint = this.transform;

    }

}
