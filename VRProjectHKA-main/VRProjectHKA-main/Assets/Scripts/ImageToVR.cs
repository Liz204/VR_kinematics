using UnityEngine;
using UnityEngine.UI;

public class ImageToVR : MonoBehaviour
{
    public Image[] images;

    public Vector2 targetSize = new Vector2(300, 300); // Size of the images
    
    // -1 = none 
    private int currentIndex = -1;

    void Start()
    {
        // Configura el tamaño y oculta todas las imágenes
        if (images != null && images.Length > 0)
        {
            foreach (Image img in images)
            {
                if (img != null)
                {
                    img.rectTransform.sizeDelta = targetSize;
                    img.enabled = false;
                }
            }
        }
        else
        {
            Debug.LogWarning("No se han asignado imágenes en el arreglo.");
        }
    }

 void Update()
    {
        // Al presionar el botón B del controlador derecho (OVRInput.Button.Two)
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            // Si hay alguna imagen visible, la ocultamos
            if (currentIndex != -1)
            {
                images[currentIndex].enabled = false;
            }

            // Incrementamos el índice de forma cíclica
            currentIndex++;

            // Si llegamos al final del arreglo, reiniciamos el índice a -1
            if (currentIndex >= images.Length)
            {
                currentIndex = -1;
            }

            // Si no estamos en el estado -1, mostramos la imagen en el índice actual
            if (currentIndex != -1)
            {
                images[currentIndex].enabled = true;
                Debug.Log("Mostrando imagen " + (currentIndex + 1));
            }
            else
            {
                Debug.Log("Ninguna imagen visible.");
            }
        }
    }
}
