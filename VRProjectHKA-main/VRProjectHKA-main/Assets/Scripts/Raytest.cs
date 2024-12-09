using UnityEngine;

public class RayInteractionHandler : MonoBehaviour
{
    private Renderer objectRenderer;

    void Start()
    {
        // Obtener el Renderer para cambiar el color como feedback
        objectRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // Verificar si el botón trasero se presiona mientras el objeto está siendo apuntado
        if (IsBeingPointedAt() && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) // Gatillo derecho
        {
            Interact();
        }
    }

    private bool IsBeingPointedAt()
    {
        // Realizar un Raycast desde el controlador
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Verificar si el rayo está golpeando este objeto
            return hit.collider.gameObject == gameObject;
            //return True;
        }
        return false;
    }

    private void Interact()
    {
        Debug.Log("Objeto interactuado: " + gameObject.name);
        
        // Cambiar el color como ejemplo
        if (objectRenderer != null)
        {
            objectRenderer.material.color = Color.red;
        }
    }
}