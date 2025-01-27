using UnityEngine;

public class RotationDisc : MonoBehaviour
{
    public enum Axis { X, Y, Z } // Enumeración para los tres ejes

    public Axis axisToRotate; // Eje en el que el disco controlará la rotación
    public GameObject cube; // Referencia al cubo que girará
    private bool isDragging = false; // Estado de arrastre
    public float rotationSpeed = 1000f; // Velocidad de rotación

    void OnMouseDown()
    {
        // Comienza a arrastrar el disco
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging && cube != null)
        {
            // Movimiento del ratón en el eje X
            float mouseX = Input.GetAxis("Mouse X");

            // Rotar el cubo mientras lo arrastras
            float rotationAmount = mouseX * rotationSpeed * Time.deltaTime;

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
    }

    void OnMouseUp()
    {
        // Deja de arrastrar el disco
        isDragging = false;
    }
}
