using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.XR; // For VR input (if needed)

public class ClickAndDragWithUI : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    public Button endArmCreationButton;
    public bool isDraggable = false;

    private Vector3 offset;
    private float zCoord;

    void Start()
    {
        mainCamera = Camera.main; // Obtiene la cámara principal
        endArmCreationButton.onClick.AddListener(StartDraggableMode);
        var rend = GetComponent<Renderer>();
        rend.enabled = false;
    }

    void StartDraggableMode(){
        var rend = GetComponent<Renderer>();
        rend.enabled = true;
        isDraggable = true;
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
        if (isDragging && isDraggable)
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