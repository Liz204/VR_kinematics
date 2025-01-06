using UnityEngine;

public class RotSphere : MonoBehaviour
{
    public GameObject discPrefab; // Prefab del disco
    private GameObject spawnedDisc; // Referencia al disco instanciado

    void Start(){Debug.Log("RotSphere: 1");}

    void OnMouseDown()
    {
        if (spawnedDisc == null)
        {
            // Crear el disco si aún no existe
            spawnedDisc = Instantiate(discPrefab, transform.position, Quaternion.identity);
            Debug.Log("Disco creado");
        }
    }
}
