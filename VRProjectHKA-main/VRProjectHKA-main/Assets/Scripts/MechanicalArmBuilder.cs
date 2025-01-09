using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using BioIK;
using Oculus.Interaction;

public class MechanicalArmBuilder : MonoBehaviour
{
    // Referencia al RayInteractor
    [SerializeField]
    public RayInteractor rayInteractor;
    public GameObject jointPrefab;          // Prefab for the joint (e.g., a sphere)
    public GameObject armSegmentPrefab;     // Prefab for the arm segment (e.g., a cylinder)

    public GameObject circlePrefab;         // Prefab for the circle visualization
    
    public float interactionPlaneDistance = 0.1f; // Distance of the interaction plane from the camera
    private float zCoord; 
    public Color normalColor = Color.white;      // Color when joint is not selected
    public Color selectableColor = Color.green; // Color when joint is selectable
    public Color hoverColor = Color.yellow;     // Color when hovering over the joint
    public bool isCreationMode = true;

    public GameObject firstJoint;
    public GameObject lastJoint;

    public Vector3 direction2; 

    private GameObject currentJoint;        // The currently selected joint
    private GameObject newJoint;            // The new joint being created
    private GameObject armSegment;          // The arm segment connecting the joints
    private Renderer currentJointRenderer;  // Renderer for the current joint to change its color
    private bool isDragging = false;        // Is the user dragging a new joint?
    private bool isHovering = false;        // Is the pointer hovering over the joint?
    public Button endArmCreationButton;
    public GameObject target;

    private BioIK.BioIK bioIK;

    public List<GameObject> joints = new List<GameObject>();        // List to store joints
    public List<GameObject> armSegments = new List<GameObject>();   // List to store arm segments

    Vector3 direction;

    [SerializeField] 
    private Text _title;
    [SerializeField] 
    private Text _title2;

    void Start()
    {   
        if (rayInteractor == null)
        {
            Debug.LogError("No se ha asignado un RayInteractor al script.");
        }

        bioIK = GetComponent<BioIK.BioIK>();
        if (bioIK == null)
        {
            Debug.LogError("BioIK component not found on the GameObject.");
        }

        // Initialize with the first joint in the center of the screen
        currentJoint = Instantiate(jointPrefab, Vector3.zero, Quaternion.identity);
        currentJoint.transform.SetParent(this.transform); // Nest the first joint inside the GameObject with this script
        currentJointRenderer = currentJoint.GetComponent<Renderer>();
        currentJointRenderer.material.color = selectableColor; // Set the initial selectable color
        endArmCreationButton.onClick.AddListener(EndArmCreationMode);
        joints.Add(currentJoint);
        firstJoint = currentJoint;
        // Detectar si el botón trasero derecho fue presionado
        
    }

    public void EndArmCreationMode(){


        isCreationMode = false;
        target.transform.position= (currentJoint.transform.position);
        currentJointRenderer = currentJoint.GetComponent<Renderer>();
        currentJointRenderer.material.color = normalColor;
        Destroy(endArmCreationButton.gameObject);

        if(bioIK){

            // activates bioIK for the whole arm
            bioIK.Root.AddJoint();
            var currentSegment = bioIK.Root;
            var currentGameObject = this.transform;
            Transform lastGameObject = null;
            while(currentSegment.Childs.Length>0){
                currentSegment = currentSegment.Childs[0];
                currentGameObject = currentGameObject.GetChild(0);
                var properties = currentGameObject.GetComponent<JointProperties>();
                properties.correspondingBioSegment = currentSegment;
                if(currentSegment.Childs.Length>0){
                    var currentJoint = currentSegment.AddJoint();
                    currentJoint.X.SetEnabled(true);
                    currentJoint.X.Constrained = false;
                    currentJoint.Y.SetEnabled(true);
                    currentJoint.Y.Constrained = false;
                }

                if(lastGameObject != null){
                    properties.setPreviousJoint(lastGameObject);

                    if (currentSegment.Childs.Length>0){
                        Transform NextJoint= null;
                        NextJoint= currentGameObject.GetChild(0);
                        Transform PreviousJoint= lastGameObject;
                        //VisualizeAngleDinamic(PreviousJoint, currentGameObject, NextJoint);
                    }
                    
                }
                
                lastGameObject = currentGameObject;
            }
            
            UpdateJointAngles();
            currentSegment.AddObjective(ObjectiveType.Position);
            Position objective = (Position)currentSegment.Objectives[0];
            objective.SetTargetTransform(target.transform);

        }
    }

    void Update()
    {   
        if(Input.GetKeyDown(KeyCode.M)){
            Debug.LogError(bioIK.Root);
        }
        
        if(isCreationMode){
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
            lastJoint = currentJoint;
            currentJoint = newJoint;  // Make the new joint the current joint for the next segment
            currentJointRenderer = currentJoint.GetComponent<Renderer>();
            currentJointRenderer.material.color = selectableColor; // Set the color back to selectable
            newJoint = null;
        }
        if(!isDragging && OVRInput.GetDown(OVRInput.Button.One))
        {
            EndArmCreationMode();
        }
        //Debug.Log("UPDATE");
        //UpdateJointAngles();
        if(OVRInput.GetDown(OVRInput.Button.Four))
        {
            Debug.Log("yCLICKED");
            UpdateJointAngles();
        }
    }
    
    }

    private void HandlePointerHover()
    {
        // Raycast to check if the pointer is over the current joint
        if (rayInteractor == null) return;

        Ray ray = rayInteractor.Ray;

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
        newJoint.transform.SetParent(currentJoint.transform); // Nest the new joint inside the current joint
        isDragging = true;

        // Create the arm segment between the current joint and new joint
        armSegment = Instantiate(armSegmentPrefab);
        var segmentUpdater = armSegment.GetComponent<ArmSegmentUpdater>();

        // Assign the start and end joints to the arm segment updater
        segmentUpdater.startJoint = currentJoint.transform;
        segmentUpdater.endJoint = newJoint.transform;

        joints.Add(newJoint);
        armSegments.Add(armSegment);
        _title2.text = (_title2.text+_title.text);
        
    }

    private Vector3 GetPointerPositionOnInteractionPlane()
    {
        // Create a plane at a fixed distance from the camera
        Plane interactionPlane = new Plane(Camera.main.transform.forward, currentJoint.transform.position);

        // Visualize the plane for debugging
        //DrawDebugPlane(interactionPlane, currentJoint.transform.position, Color.green, 10, 1.0f);

        // Get the pointer ray
        Ray ray = rayInteractor.Ray; // Obtain the ray from the RayInteractor

        // Find where the ray intersects with the interaction plane
        if (interactionPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero; // Fallback in case of an error
    }

    // Helper function to draw the plane
    private void DrawDebugPlane(Plane plane, Vector3 center, Color color, int gridSize, float cellSize)
    {
        Vector3 planeNormal = plane.normal;
        Vector3 tangent = Vector3.Cross(planeNormal, Vector3.up).normalized;
        Vector3 bitangent = Vector3.Cross(planeNormal, tangent);

        for (int x = -gridSize; x <= gridSize; x++)
        {
            for (int z = -gridSize; z <= gridSize; z++)
            {
                Vector3 start = center + (tangent * x * cellSize) + (bitangent * z * cellSize);
                Vector3 end1 = start + (tangent * cellSize);
                Vector3 end2 = start + (bitangent * cellSize);

                Debug.DrawLine(start, end1, color); // Draw horizontal lines
                Debug.DrawLine(start, end2, color); // Draw vertical lines
            }
        }
    }


    private Ray GetPointerRay()
    {
        // TODO VR: Replace this with VR controller's pointing direction when in VR mode
        // For now, use mouse position ray
        //return Camera.main.ScreenPointToRay(Input.mousePosition);
        // Obtén la posición y dirección del rayo desde el controlador derecho
        // Crear un rayo basado en la posición y dirección del controlador
        return new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        // Obtener la posición y rotación del controlador derecho
        //Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        //Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        // Crear un rayo basado en la posición y dirección del controlador
        //return new Ray(controllerPosition, controllerRotation * Vector3.forward);
    }

    private bool IsSelectButtonDown()
    {
        // TODO VR: Replace this with VR controller's select button down check
        //return Input.GetMouseButtonDown(0);
        return OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);
    }

    private bool IsSelectButtonUp()
    {
        // TODO VR: Replace this with VR controller's select button up check
        //return Input.GetMouseButtonUp(0);
        return OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger);
        
    }

    private void UpdateArmSegment()
    {
        if (armSegment != null && currentJoint != null && newJoint != null)
        {
            direction = newJoint.transform.position - currentJoint.transform.position;
            float distance = direction.magnitude;

            // Position the arm segment between the two joints
            armSegment.transform.position = currentJoint.transform.position + direction / 2;
            armSegment.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);

            // Scale the arm segment to fit the distance between joints, keeping x and z constant
            armSegment.transform.localScale = new Vector3(0.1f, distance / 2f, 0.1f);


            // Trigger angle visualization on a specific key or VR input
        if ( direction2!= Vector3.zero)
        {
            //Debug.Log("Button Four Pressed");
            //VisualizeAngleBetweenLastTwoJoints(direction,direction2);

            
            if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            Debug.Log("Botón trasero presionado");
            EndArmCreationMode();
        }
        
        }
            direction2 = direction;
        }
    }
      private void VisualizeAngleBetweenLastTwoJoints(Vector3 direction1,Vector3 directiontwo)
    {
        // Asegúrate de que el currentJoint tiene asignado su JointProperties
    JointProperties currentProperties = currentJoint.GetComponent<JointProperties>();

        GameObject joint1 = firstJoint;
        GameObject joint2 = currentJoint;
        GameObject joint3 = lastJoint;
 if (joint1== null || joint3 == null)
    {
        Debug.LogWarning("No hay suficientes joints conectados para calcular el ángulo.");
        return;
    }

        //Vector3 direction1 = joint2.transform.position - joint1.transform.position;

        directiontwo = joint3.transform.position - joint2.transform.position;
        float angle = Vector3.Angle(direction1, directiontwo);
        Debug.Log(direction1);
        Debug.Log(directiontwo);
        Debug.Log(joint1.transform.position);

        /*GameObject angleVisualizer = Instantiate(circlePrefab);
        angleVisualizer.transform.position = joint2.transform.position;
        angleVisualizer.transform.localScale = new Vector3(1f, 0.05f, 1f); // Adjust scale as needed
        angleVisualizer.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // Update the text on the circle prefab
        TextMeshPro textMesh = angleVisualizer.GetComponentInChildren<TextMeshPro>();
        // Busca el componente TextMeshPro en el hijo llamado "Text (TMP)"
        //TextMeshPro textMesh = angleVisualizer.transform.Find("Text (TMP)")?.GetComponent<TextMeshPro>();
        textMesh.text = $"Angle: {angle:F2}°";
        Debug.Log($"TextMesh updated: {textMesh.text}");
        if (textMesh != null)
        {
            textMesh.text = $"Angle: {angle:F2}°";
            Debug.Log($"TextMesh updated: {textMesh.text}");
        }*/

        //Debug.Log($"Angle between the last two joints: {angle:F2}°");
        _title.text = ( $"Angle: {angle:F2}°     ");
    }
    private void VisualizeAngleDinamic(Transform firstJoint, Transform currentJoint,Transform lastJoint)
    {
        // Asegúrate de que el currentJoint tiene asignado su JointProperties
    JointProperties currentProperties = currentJoint.GetComponent<JointProperties>();

        Transform joint1 = firstJoint;
        Transform joint2 = currentJoint;
        Transform joint3 = lastJoint;

 if (joint1== null || joint3 == null)
    {
        Debug.LogWarning("No hay suficientes joints conectados para calcular el ángulo.");
        return;
    }

        Vector3 direction1 = joint2.position - joint1.position;

        Vector3 directiontwo = joint3.position - joint2.position;
        float angle = Vector3.Angle(direction1, directiontwo);
        Debug.Log(direction1);
        Debug.Log(directiontwo);
        Debug.Log(joint1.transform.position);

        /*GameObject angleVisualizer = Instantiate(circlePrefab);
        angleVisualizer.transform.position = joint2.transform.position;
        angleVisualizer.transform.localScale = new Vector3(1f, 0.05f, 1f); // Adjust scale as needed
        angleVisualizer.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // Update the text on the circle prefab
        TextMeshPro textMesh = angleVisualizer.GetComponentInChildren<TextMeshPro>();
        // Busca el componente TextMeshPro en el hijo llamado "Text (TMP)"
        //TextMeshPro textMesh = angleVisualizer.transform.Find("Text (TMP)")?.GetComponent<TextMeshPro>();
        textMesh.text = $"Angle: {angle:F2}°";
        Debug.Log($"TextMesh updated: {textMesh.text}");
        if (textMesh != null)
        {
            textMesh.text = $"Angle: {angle:F2}°";
            Debug.Log($"TextMesh updated: {textMesh.text}");
        }*/

        Debug.Log($"Angle is: {angle:F2}°");
        //_title.text = ( $"Angle: {angle:F2}°     ");
    }

        // Nueva función para calcular y visualizar los ángulos
public void UpdateJointAngles()
{
var currentGameObject = this.transform;
Transform lastGameObject = null;
while (currentGameObject.childCount > 0)
{
    Transform nextGameObject = currentGameObject.GetChild(0);

    if (lastGameObject != null)
    {
        VisualizeAngleDinamic(lastGameObject, currentGameObject, nextGameObject);
    }

    lastGameObject = currentGameObject;
    currentGameObject = nextGameObject;
}
} 

}
