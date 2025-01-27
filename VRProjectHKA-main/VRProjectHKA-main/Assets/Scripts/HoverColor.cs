using UnityEngine;

public class HoverColor : MonoBehaviour
{
    private Renderer cubeRenderer; // Referencia al Renderer del cubo
    private Color originalColor; // Color original del cubo
    public Color hoverColor = Color.yellow; // Color del cubo al pasar el mouse

    void Start()
    {
        // Obtener el Renderer del cubo para cambiar el color
        cubeRenderer = GetComponent<Renderer>();
        if (cubeRenderer != null)
        {
            originalColor = cubeRenderer.material.color; // Guardar el color original
        }
    }

    void OnMouseEnter()
    {
        // Cambiar el color del cubo al pasar el mouse
        if (cubeRenderer != null)
        {
            cubeRenderer.material.color = hoverColor;
        }
    }

    void OnMouseExit()
    {
        // Restaurar el color original cuando el mouse sale del cubo
        if (cubeRenderer != null)
        {
            cubeRenderer.material.color = originalColor;
        }
    }
}
