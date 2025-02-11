using UnityEngine;

public class HoverColor : MonoBehaviour
{
    private Renderer cubeRenderer; // Reference to the cube's Renderer
    private Color originalColor; // Original color of the cube
    public Color hoverColor = Color.yellow; // Cube color when the mouse hovers over it

    void Start()
    {
        // Get the cube's Renderer to change the color
        cubeRenderer = GetComponent<Renderer>();
        if (cubeRenderer != null)
        {
            originalColor = cubeRenderer.material.color; // Save the original color
        }
    }

    void OnMouseEnter()
    {
        // Change the cube's color when the mouse hovers over it
        if (cubeRenderer != null)
        {
            cubeRenderer.material.color = hoverColor;
        }
    }

    void OnMouseExit()
    {
        // Restore the original color when the mouse exits the cube
        if (cubeRenderer != null)
        {
            cubeRenderer.material.color = originalColor;
        }
    }
}
