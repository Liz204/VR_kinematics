using UnityEngine;

public class RotSphere : MonoBehaviour
{
    public GameObject rotationSpherePrefab; // Prefab of the sphere with the discs
    private GameObject spawnedSphere; // Instance of the generated sphere

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            // If the generated sphere does not exist, create it
            if (spawnedSphere == null)
            {
                // Create the sphere with the discs over the cube
                spawnedSphere = Instantiate(rotationSpherePrefab, transform.position, Quaternion.identity);

                // Set the cube as the parent of the generated sphere
                spawnedSphere.transform.SetParent(transform);

                // Assign the cube to each disc inside the sphere, except the last one
                int childCount = spawnedSphere.transform.childCount;
                int index = 0;

                foreach (Transform child in spawnedSphere.transform)
                {
                    if (index < childCount - 1) // Avoid the last child
                    {
                        RotationDisc discScript = child.GetComponent<RotationDisc>();
                        if (discScript != null)
                        {
                            discScript.cube = gameObject; // Automatically assign the cube
                        }
                    }
                    index++;
                }
            }
        }
    }

    void OnMouseDown()
    {
        if (spawnedSphere == null)
        {
            // Create the sphere with the discs over the cube
            spawnedSphere = Instantiate(rotationSpherePrefab, transform.position, Quaternion.identity);

            // Set the cube as the parent of the generated sphere
            spawnedSphere.transform.SetParent(transform);

            // Debug.Log("RotationSphere generated over the cube.");

            // Assign the cube to each disc inside the sphere
            foreach (Transform child in spawnedSphere.transform)
            {
                RotationDisc discScript = child.GetComponent<RotationDisc>();
                if (discScript != null)
                {
                    discScript.cube = gameObject; // Automatically assign the cube (the cube is 'gameObject')
                }
            }
        }
        else
        {
            // If the sphere already exists, destroy it
            Destroy(spawnedSphere);
            // Debug.Log("RotationSphere destroyed.");
        }
    }
}
