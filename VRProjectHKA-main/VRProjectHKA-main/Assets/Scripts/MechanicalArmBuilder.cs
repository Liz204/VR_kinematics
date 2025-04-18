using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using BioIK;
using Oculus.Interaction;
using TMPro;


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
    //public GameObject canva;

    private BioIK.BioIK bioIK;

    public List<GameObject> joints = new List<GameObject>();        // List to store joints
    public List<GameObject> armSegments = new List<GameObject>();   // List to store arm segments

    Vector3 direction;

    [SerializeField] 
    private Text _title;
    [SerializeField] 
    private Text _title2;

    private bool isDraggingTarget = false;     // Are we currently dragging the target?
    private bool isHoveringLastJoint = false;  // Are we hovering over the last joint?
    private Renderer lastJointRenderer;        // Renderer for the last joint

    private Dictionary<Transform, float> previousAngles = new Dictionary<Transform, float>();
    
    void Start()
    {   
        if (rayInteractor == null)
        {
            // Debug.LogError("No RayInteractor has been assigned to the script.");
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
    }

    public void EndArmCreationMode(){

        lastJoint = currentJoint;
        isCreationMode = false;
        lastJointRenderer = lastJoint.GetComponent<Renderer>();
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
                    }
                    
                }
                
                lastGameObject = currentGameObject;
            }
            
            currentSegment.AddObjective(ObjectiveType.Position);
            Position objective = (Position)currentSegment.Objectives[0];
            objective.SetTargetTransform(target.transform);
            //canva.SetActive(false);

        }
    }

    void Update()
    {   
        if(Input.GetKeyDown(KeyCode.M)){
            Debug.LogError(bioIK.Root);
        }

        if(isCreationMode){
            if (isDragging && newJoint != null){
                // Update the position of the new joint based on pointer position on the interaction plane
                Vector3 targetPosition = GetPointerPositionOnInteractionPlane();
                newJoint.transform.position = targetPosition;

                // Update arm segment to connect currentJoint and newJoint
                UpdateArmSegment();
            }

            // Check for pointer hover to change color
            HandlePointerHover();

            // Check for input to select or create a joint
            if (IsSelectButtonDown() && !isDragging && isHovering){
                CreateNewJoint();
            }

            // Release the new joint if the select button is released
            if (IsSelectButtonUp() && isDragging){
                isDragging = false;
                currentJointRenderer.material.color = normalColor; // Reset the color of the current joint
                lastJoint = currentJoint;
                currentJoint = newJoint;  // Make the new joint the current joint for the next segment
                currentJointRenderer = currentJoint.GetComponent<Renderer>();
                currentJointRenderer.material.color = selectableColor; // Set the color back to selectable
                newJoint = null;
            }

            // End Creation Mode
            if(!isDragging && OVRInput.GetDown(OVRInput.Button.One)){
                EndArmCreationMode();
            }
        }// End Creation Mode
        else {
            RemoveDiscsFromJoint(lastJoint.transform);
            HandleLastJointHoverAndDrag();
            if (IsSelectButtonUp()){
                MoveTargetWithRay();
            }
            UpdateJointAngles();
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
        // Create a ray based on the controller's position and direction
        return new Ray(Camera.main.transform.position, Camera.main.transform.forward);

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
                VisualizeAngleBetweenLastTwoJoints(direction,direction2);
            
            }
            direction2 = direction;
        }
    }

    private void HandleLastJointHoverAndDrag()
    {
        
        if (lastJoint == null)
        {
            //Debug.LogWarning("Cannot drag last joint because lastJoint is NULL!");
            return;
        }
        if (lastJointRenderer == null)
        {
            //Debug.LogWarning($"No renderer on {lastJoint.name}, cannot show hover color!");
            return;
        }
        if (rayInteractor == null)
        {
            //Debug.LogWarning("No rayInteractor assigned, cannot raycast to last joint!");
            return;
        }

        // Step 1: Check hover
        bool wasHovering = isHoveringLastJoint;
        isHoveringLastJoint = false;

        Ray ray = rayInteractor.Ray;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Debug.Log($"RaycastHit: {hit.collider.gameObject.name}");
            if (hit.collider.gameObject == lastJoint)
            {
                isHoveringLastJoint = true;
                //Debug.Log("Hovering over LAST JOINT!");
            }
        }
        else
        {
            //Debug.Log("Raycast did not hit anything.");
        }

        // Hover color changes
        if (!wasHovering && isHoveringLastJoint && !isDraggingTarget)
        {
            //Debug.Log("Just started hovering lastJoint. Changing color to hoverColor.");
            lastJointRenderer.material.color = hoverColor;
        }
        else if (wasHovering && !isHoveringLastJoint && !isDraggingTarget)
        {
            //Debug.Log("Stopped hovering lastJoint. Reverting color to normalColor.");
            lastJointRenderer.material.color = normalColor;
        }

        // Step 2: Handle “drag” input
        if (!isDraggingTarget && isHoveringLastJoint && IsSelectButtonDown())
        {
            //Debug.Log("Select button pressed while hovering lastJoint -> start dragging target.");
            isDraggingTarget = true;
        }

        if (isDraggingTarget)
        {
            MoveTargetWithRay();

            if (IsSelectButtonUp())
            {
                //Debug.Log("Select button released -> stop dragging target.");
                isDraggingTarget = false;
                lastJointRenderer.material.color = normalColor;
            }
        }
    }

    private void MoveTargetWithRay()
    {
        // We pick a plane near the last joint, oriented by camera's forward
        Plane plane = new Plane(Camera.main.transform.forward, lastJoint.transform.position);
        Ray ray = rayInteractor.Ray;

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 newPosition = ray.GetPoint(distance);
            Debug.Log($"Moving target to {newPosition}");
            target.transform.position = newPosition;
        }
        else
        {
            Debug.LogWarning("Plane.Raycast failed: no intersection between ray and plane!");
        }
    }

      private void VisualizeAngleBetweenLastTwoJoints(Vector3 direction1,Vector3 directiontwo)
    {
        // Make sure that currentJoint has its JointProperties assigned
        JointProperties currentProperties = currentJoint.GetComponent<JointProperties>();

        GameObject joint1 = firstJoint;
        GameObject joint2 = currentJoint;
        GameObject joint3 = lastJoint;
        if (joint1== null || joint3 == null)
        {
            Debug.LogWarning("There are not enough connected joints to calculate the angle.");
            return;
        }

        //Vector3 direction1 = joint2.transform.position - joint1.transform.position;

        directiontwo = joint3.transform.position - joint2.transform.position;
        float angle = Vector3.Angle(direction1, directiontwo);
        //Debug.Log(direction1);
        //Debug.Log(directiontwo);
        //Debug.Log(joint1.transform.position);

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
        //_title.text = ( $"Angle 2: {angle:F2}°     ");
    }

    public void UpdateJointAngles()
    {
        List<Transform> joints = CollectJoints();
        List<float> angles = CalculateAndDisplayAngles(joints);
        UpdateRotationSpheres(joints, angles);
        HighlightRotationSphereOnHover(joints);
    }

    // Function to collect all the joints
    private List<Transform> CollectJoints()
    {
        var currentGameObject = firstJoint.transform;
        List<Transform> joints = new List<Transform>();

        while (currentGameObject.childCount > 0)
        {
            joints.Add(currentGameObject); // Store the joint
            currentGameObject = currentGameObject.GetChild(0); // next joint
        }

        return joints;
    }

    // Function to calculate angles and display on screen
    private List<float> CalculateAndDisplayAngles(List<Transform> joints)
    {
        Transform lastGameObject = null;
        List<float> angles = new List<float>();
        string resultText = "";

        for (int i = 0; i < joints.Count; i++)
        {
            Transform currentGameObject = joints[i];
            Transform nextGameObject = (i + 1 < joints.Count - 2) ? joints[i + 1] : null;

            if (lastGameObject != null && nextGameObject != null)
            {
                float angle = VisualizeAngleDinamic(lastGameObject, currentGameObject, nextGameObject);
                angles.Add(angle);
                resultText += $"{i}: {angle:F0}°\n";
            }

            lastGameObject = currentGameObject;
        }

        _title2.text = resultText;
        return angles;
    }

    // Function to update the RotationSpheres with the calculated angles
    private void UpdateRotationSpheres(List<Transform> joints, List<float> angles)
    {
        int angleIndex = 0;

        for (int i = 1; i < joints.Count - 2; i++)
        {
            if (angleIndex >= angles.Count) break;

            Transform rotationSphere = joints[i].Find("RotationSphere(Clone)");
            if (rotationSphere != null)
            {
                Transform canvas = rotationSphere.Find("Canvas");
                if (canvas != null)
                {
                    TextMeshProUGUI textComponent = canvas.GetComponentInChildren<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        textComponent.text = $"{angles[angleIndex]:F0}°";
                        angleIndex++;
                    }
                }
            }
        }
    }

    // Change the color of the discs
    private void HighlightRotationSphereOnHover(List<Transform> joints)
    {
        if (rayInteractor == null) return;

        Ray ray = rayInteractor.Ray;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            foreach (Transform joint in joints)
            {
                Transform rotationSphere = joint.Find("RotationSphere(Clone)");
                if (rotationSphere != null)
                {

                    // Search for the discs inside the RotationSphere
                    Transform diskX = rotationSphere.Find("Disk X");
                    Transform diskY = rotationSphere.Find("Disk Y");
                    Transform diskZ = rotationSphere.Find("Disk Z");

                    // Check if the ray hit any disc
                    if (diskX != null && hit.collider.gameObject == diskX.gameObject)
                    {
                        // Change the color of the X disc when it is touched
                        Renderer diskXRenderer = diskX.GetComponent<Renderer>();
                        if (diskXRenderer != null)
                        {
                            diskXRenderer.material.color = Color.red; // Change color to red
                        }
                    }
                    else
                    {
                        // Return to original color
                        if (diskX != null)
                        {
                            Renderer diskXRenderer = diskX.GetComponent<Renderer>();
                            if (diskXRenderer != null)
                            {
                                diskXRenderer.material.color = Color.gray; // Original color
                            }
                        }
                    }

                    if (diskY != null && hit.collider.gameObject == diskY.gameObject)
                    {
                        // Change the color of the Y disc when it is touched
                        Renderer diskYRenderer = diskY.GetComponent<Renderer>();
                        if (diskYRenderer != null)
                        {
                            diskYRenderer.material.color = Color.green; // Change color to green
                        }
                    }
                    else
                    {
                        // Return to original color
                        if (diskY != null)
                        {
                            Renderer diskYRenderer = diskY.GetComponent<Renderer>();
                            if (diskYRenderer != null)
                            {
                                diskYRenderer.material.color = Color.gray;
                            }
                        }
                    }

                    if (diskZ != null && hit.collider.gameObject == diskZ.gameObject)
                    {
                        // Change the color of the Z disc when it is touched
                        Renderer diskZRenderer = diskZ.GetComponent<Renderer>();
                        if (diskZRenderer != null)
                        {
                            diskZRenderer.material.color = Color.blue; // Change color to blue
                        }
                    }
                    else
                    {
                        // Return to original color
                        if (diskZ != null)
                        {
                            Renderer diskZRenderer = diskZ.GetComponent<Renderer>();
                            if (diskZRenderer != null)
                            {
                                diskZRenderer.material.color = Color.gray;
                            }
                        }
                    }
                }
            }
        }
    }

    // Function to calculate the angle and return the value
    private float VisualizeAngleDinamic(Transform firstJoint, Transform currentJoint, Transform lastJoint)
    {
        if (firstJoint == null || lastJoint == null) return 0f;

        Vector3 direction1 = currentJoint.position - firstJoint.position;
        Vector3 direction2 = lastJoint.position - currentJoint.position;

        float angle = Vector3.Angle(direction1, direction2);
        return angle;
    }

    // Function to highlight the disc if there is a change, or restore it to gray if not
    private void ApplyRotationToAssociatedDiscs(Transform joint, float angleDifference)
    {
        RotationDisc[] discs = joint.GetComponentsInChildren<RotationDisc>();

        foreach (var disc in discs)
        {
            Renderer discRenderer = disc.GetComponent<Renderer>();
            if (discRenderer == null) continue;

            // If the difference is less than 0.1°, restore to gray
            if (Mathf.Abs(angleDifference) < 0.1f)
            {
                discRenderer.material.color = Color.gray;
            }
            else
            {
                // Apply color based on the axis
                ApplyColorBasedOnAxis(disc,angleDifference);
            }
        }
    }

    private void ApplyColorBasedOnAxis(RotationDisc disc, float angle)
    {
        if (disc.cube == null) return;

        Renderer discRenderer = disc.GetComponent<Renderer>();
        if (discRenderer != null)
        {
            switch (disc.axisToRotate)
            {
                case RotationDisc.Axis.X:
                    discRenderer.material.color = Color.red;
                    break;
                case RotationDisc.Axis.Y:
                    discRenderer.material.color = Color.green;
                    break;
                case RotationDisc.Axis.Z:
                    discRenderer.material.color = Color.blue;
                    break;
                default:
                    Debug.LogWarning("Unknown rotation axis: " + disc.axisToRotate);
                    break;
            }
        }
    }
    // Function to apply rotation based on the angle
    private void ApplyRotationFromAngle(RotationDisc disc, float angle)
    {
        if (disc.cube == null) return; // Make sure there is a cube associated

        float rotationAmount = angle * disc.rotationSpeed * Time.deltaTime; // Calculates how much to rotate

        // Apply rotation based on the selected axis on the disc
        if (disc.axisToRotate == RotationDisc.Axis.X)
        {
            disc.cube.transform.Rotate(Vector3.right, -rotationAmount);
        }
        else if (disc.axisToRotate == RotationDisc.Axis.Y)
        {
            disc.cube.transform.Rotate(Vector3.up, -rotationAmount);
        }
        else if (disc.axisToRotate == RotationDisc.Axis.Z)
        {
            disc.cube.transform.Rotate(Vector3.forward, rotationAmount);
        }
    }

    // Remove the disk from the last joint
    private void RemoveDiscsFromJoint(Transform joint)
    {
        // Get all the RotationDisc inside the joint
        RotationDisc[] discs = joint.GetComponentsInChildren<RotationDisc>();

        // Remove each disc found
        foreach (var disc in discs)
        {
            Destroy(disc.gameObject);
        }
    }


}