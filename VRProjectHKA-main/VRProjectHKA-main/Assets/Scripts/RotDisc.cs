using UnityEngine;

public class RotationDisc : MonoBehaviour
{
    public enum Axis { X, Y, Z } // Enumeración para los tres ejes

    public Axis axisToRotate; // Eje en el que el disco controlará la rotación
    public GameObject cube; // Referencia al cubo que girará
    private bool isDragging = false; // Estado de arrastre
    public float rotationSpeed = 1000f; // Velocidad de rotación

    private void Update()
    {
        // Lógica principal de interacción con VR
        if (IsSelectButtonDown()) // Detecta si se ha presionado el botón del controlador
        {
            Ray pointerRay = GetPointerRay(); // Obtén el rayo del controlador
            RaycastHit hit;

            // Comprueba si el rayo golpea este disco
            if (Physics.Raycast(pointerRay, out hit))
            {
                if (hit.collider.gameObject == gameObject) // Verifica si el objeto tocado es este disco
                {
                    isDragging = true;
                }
            }
        }

        if (isDragging && cube != null)
        {
            // Detecta el movimiento del controlador
            float rotationAmount = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x * rotationSpeed * Time.deltaTime;

            // Aplica la rotación en el eje seleccionado
            if (axisToRotate == Axis.X)
            {
                cube.transform.Rotate(Vector3.right, -rotationAmount); // Dirección opuesta en X
            }
            else if (axisToRotate == Axis.Y)
            {
                cube.transform.Rotate(Vector3.up, -rotationAmount); // Dirección opuesta en Y
            }
            else if (axisToRotate == Axis.Z)
            {
                cube.transform.Rotate(Vector3.forward, rotationAmount); // Sin cambio en Z
            }
        }

        // Si el botón de selección ya no está presionado, deja de arrastrar
        if (OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
        {
            isDragging = false;
        }
    }

    private Ray GetPointerRay()
    {
        // Crear un rayo basado en la posición y dirección del controlador
        return new Ray(Camera.main.transform.position, Camera.main.transform.forward);
    }

    private bool IsSelectButtonDown()
    {
        // Verifica si el botón de selección está presionado
        return OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);
    }
}
