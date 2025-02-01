using UnityEngine;
using UnityEngine.UI;

public class ImageToVR : MonoBehaviour
{
    public Image image1;
    public Image image2;

    public Vector2 targetSize = new Vector2(300, 300); // Size of the images
    
    // 0 = none, 1 = image1, 2 = image2.
    private int state = 0;

    void Start()
    {
        // Configuramos el tamaño y ocultamos ambas imágenes inicialmente
        if (image1 != null)
        {
            image1.rectTransform.sizeDelta = targetSize;
            image1.enabled = false;
        }
        else
        {
            Debug.LogWarning("No se asignó la imagen1 en el Inspector.");
        }

        if (image2 != null)
        {
            image2.rectTransform.sizeDelta = targetSize;
            image2.enabled = false;
        }
        else
        {
            Debug.LogWarning("No se asignó la imagen2 en el Inspector.");
        }
    }

    void Update()
    {
        // Al presionar el botón B del controlador derecho (OVRInput.Button.Two)
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            // Incrementa el estado de forma cíclica (0 -> 1 -> 2 -> 0 ...)
            state = (state + 1) % 3;
            ActualizarVisibilidad();
            Debug.Log("Estado actual: " + state);
        }
    }
    
    // Función que actualiza la visibilidad de las imágenes según el estado
    void ActualizarVisibilidad()
    {
        if (state == 0)
        {
            // Ninguna imagen visible
            if (image1 != null) image1.enabled = false;
            if (image2 != null) image2.enabled = false;
        }
        else if (state == 1)
        {
            // Se muestra la imagen 1 y se oculta la imagen 2
            if (image1 != null) image1.enabled = true;
            if (image2 != null) image2.enabled = false;
        }
        else if (state == 2)
        {
            // Se oculta la imagen 1 y se muestra la imagen 2
            if (image1 != null) image1.enabled = false;
            if (image2 != null) image2.enabled = true;
        }
    }
}
