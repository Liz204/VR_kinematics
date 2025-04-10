using UnityEngine;

public class ArmSegmentUpdater : MonoBehaviour
{
    public Transform startJoint; // Reference to the starting joint
    public Transform endJoint;   // Reference to the ending joint

    void Update()
    {
        if (startJoint != null && endJoint != null)
        {
            UpdateArmSegment();
        }
    }

    private void UpdateArmSegment()
    {
        Vector3 direction = endJoint.position - startJoint.position;
        float distance = direction.magnitude;

        // Position the arm segment between the two joints
        transform.position = startJoint.position + direction / 2;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);

        // Scale the arm segment to fit the distance between joints, keeping x and z constant
        transform.localScale = new Vector3(0.1f, distance / 2f, 0.1f);
    }
}