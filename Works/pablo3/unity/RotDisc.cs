using UnityEngine;
/*
RotDisc es el unico script que utilizo,
Va en el disco
Y su función es hacer girar al cubo en alguna de sus 3 coordinadas

Solo tienes que poner el script en el objeto "circulo" dentro del prefab
Crear 3 de estos prefabs ya en escena y relacionarlos con un gameobject
*/
public class RotationDisc : MonoBehaviour
{
    public enum Axis { X, Y, Z } // Enumeración para los tres ejes

    public Axis axisToRotate; // Eje en el que el disco controlará la rotación
    public GameObject cube; // Referencia al cubo que girará
    private bool isDragging = false; // Estado de arrastre
    public float rotationSpeed = 603f; // Velocidad de rotación
    
    void Start(){Debug.Log("RotDisc: 3");}

    void OnMouseDown()
    {
        // Comienza a arrastrar el disco
        isDragging = true;
    }


    void OnMouseDrag()
    {
        if (isDragging)
        {
            // Movimiento del ratón en el eje X
            float mouseX = Input.GetAxis("Mouse X");

            // Rotar el disco mientras lo arrastras
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
