using UnityEngine;

public class DragObjectWithController : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Transform controllerTransform; // Posición del controlador (mano)

    void Update()
    {
        // Si estamos arrastrando, actualiza la posición del objeto
        if (isDragging && controllerTransform != null)
        {
            transform.position = controllerTransform.position + offset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detecta si el gatillo del controlador se presiona
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            isDragging = true;

            // Obtén la posición del controlador
            controllerTransform = GetControllerTransform();
            if (controllerTransform != null)
            {
                // Calcula la diferencia (offset) entre el objeto y el controlador
                offset = transform.position - controllerTransform.position;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Suelta el objeto cuando el gatillo deja de presionarse
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
        {
            isDragging = false;
            controllerTransform = null;
        }
    }

    // Obtiene la Transform del controlador derecho (RTouch) o izquierdo (LTouch)
    private Transform GetControllerTransform()
    {
        var rig = FindObjectOfType<OVRCameraRig>();
        if (rig != null)
        {
            // Por ejemplo, usaremos el controlador derecho (RTouch)
            return rig.rightHandAnchor;
        }
        return null;
    }
}