using UnityEngine;
//using UnityEngine.XR; // For VR input (if needed)

public class MoverObjetoConRaton : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;

    private Vector3 offset;
    private float zCoord;

    void Start()
    {
        mainCamera = Camera.main; // Obtiene la cámara principal
    }

    void OnMouseDown()
    {
        // Guarda la distancia Z del objeto desde la cámara
        zCoord = mainCamera.WorldToScreenPoint(transform.position).z;

        // Calcula el offset entre el clic y la posición del objeto
        offset = transform.position - GetMouseWorldPosition();
        
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false; // Suelta el objeto
    }

    void Update()
    {
        if (isDragging)
        {
            // Mueve el objeto a la posición del ratón
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    // Calcula la posición del ratón en coordenadas del mundo
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;

        // Agrega la profundidad Z
        mousePoint.z = zCoord;

        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}