using UnityEngine;

public class Angles: MonoBehaviour
{
    public GameObject circlePrefab; // Reference to the flat circle prefab
    public GameObject cylinder1;   // Reference to the first cylinder
    public GameObject cylinder2;   // Reference to the second cylinder

    private float sphereDiameter;

    void Start()
    {
        // Get the diameter of the sphere based on its scale
        sphereDiameter = transform.localScale.x; // Assuming the sphere is uniformly scaled
        
        Debug.Log("Hola!");
    }

    void OnMouseDown()
    {
        Debug.Log("Clic!!!");
        // Calculate the angle between the two cylinders
        Vector3 direction1 = cylinder1.transform.position - transform.position; // Vector from sphere to cylinder1
        Vector3 direction2 = cylinder2.transform.position - transform.position; // Vector from sphere to cylinder2
        float angle = Vector3.Angle(direction1, direction2); // Calculate the angle in degrees

        // Instantiate the circle at the sphere's center
        GameObject circle = Instantiate(circlePrefab);
        circle.transform.position = transform.position;
        circle.transform.localScale = new Vector3(sphereDiameter + 1, 0.05f, sphereDiameter + 1);
        circle.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // Update the text on the circle prefab
        TextMesh textMesh = circle.GetComponentInChildren<TextMesh>();
        if (textMesh != null)
        {
            textMesh.text = $"Angle: {angle:F2}°";
        }
        Debug.Log($"Angle between the two cylinders: {angle:F2}°");
    }
}
