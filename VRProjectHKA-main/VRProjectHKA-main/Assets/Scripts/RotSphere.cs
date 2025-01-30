using UnityEngine;

public class RotSphere : MonoBehaviour
{
    public GameObject rotationSpherePrefab; // Prefab de la esfera con los discos
    private GameObject spawnedSphere; // Instancia de la esfera generada

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            // Si no existe la esfera generada, crearla
            if (spawnedSphere == null)
            {
                // Crear la esfera con los discos sobre el cubo
                spawnedSphere = Instantiate(rotationSpherePrefab, transform.position, Quaternion.identity);

                // Establecer el cubo como padre de la esfera generada
                spawnedSphere.transform.SetParent(transform);

                // Asignar el cubo a cada disco dentro de la esfera, excepto el último
                int childCount = spawnedSphere.transform.childCount;
                int index = 0;

                foreach (Transform child in spawnedSphere.transform)
                {
                    if (index < childCount - 1) // Evita el último hijo
                    {
                        RotationDisc discScript = child.GetComponent<RotationDisc>();
                        if (discScript != null)
                        {
                            discScript.cube = gameObject; // Asignamos el cubo automáticamente
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
            // Crear la esfera con los discos sobre el cubo
            spawnedSphere = Instantiate(rotationSpherePrefab, transform.position, Quaternion.identity);

            // Establecer el cubo como padre de la esfera generada
            spawnedSphere.transform.SetParent(transform);

            //Debug.Log("RotationSphere generada sobre el cubo.");

            // Asignar el cubo a cada disco dentro de la esfera
            foreach (Transform child in spawnedSphere.transform)
            {
                RotationDisc discScript = child.GetComponent<RotationDisc>();
                if (discScript != null)
                {
                    discScript.cube = gameObject; // Asignamos el cubo automáticamente (el cubo es 'gameObject')
                }
            }
        }
        else
        {
            // Si ya existe la esfera, destruirla
            Destroy(spawnedSphere);
            //Debug.Log("RotationSphere destruida.");
        }
    }
}

