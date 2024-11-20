using UnityEngine;
using BioIK;
public class JointProperties : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public enum JointType
    {
        Translational, // Moves along a linear path
        Rotational,    // Rotates around a fixed axis
        Multidirectional // Moves and rotates freely in multiple directions
    }

    [Header("Joint Settings")]
    public JointType jointType = JointType.Rotational;

    [Header("Restrictions")]
    public bool enabledX = false;
    public float lowerLimitX = 0f;
    public float upperLimitX = 0f;
    public bool enabledY = false;
    public float lowerLimitY = 0f;
    public float upperLimitY = 0f;

    [Header("Additional Properties")]
    public bool isFixed = false; // If true, the joint is fixed and cannot move or rotate
    public BioSegment correspondingBioSegment = null;
    
    private Renderer sphereRenderer; // Renderer to change color on hover
    public Color hoverColor = Color.yellow; // Color when hovering
    public Color normalColor = Color.white; // Default color
    private bool isHovering = false;

    void Start()
    {
        sphereRenderer = GetComponent<Renderer>();
    }

    void Update()
    {   if(correspondingBioSegment){
        HandlePointerHover();

        if (Input.GetMouseButtonDown(1) && isHovering) // Right-click check
        {
            Debug.Log("Sphere selected!");
        }
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
        // TODO VR: Replace this with VR controller's pointing direction when in VR mode
        // For now, use mouse position ray
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }
}
