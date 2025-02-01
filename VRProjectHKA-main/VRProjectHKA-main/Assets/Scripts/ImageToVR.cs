using UnityEngine;
using UnityEngine.UI;

public class ImageToVR : MonoBehaviour
{
    public Image vrImage;
    public Vector2 targetSize = new Vector2(300, 300);
    
    // Variable para llevar el control del estado actual de la imagen
    private bool imageVisible = false;

    void Start()
    {
        if (vrImage != null)
        {
            // Ajusta el tamaño del RectTransform según targetSize
            vrImage.rectTransform.sizeDelta = targetSize;
            // Inicialmente la imagen estará oculta
            vrImage.enabled = imageVisible;
        }
        else
        {
            Debug.LogWarning("No se asignó la imagen (vrImage) en el Inspector.");
        }
    }

    void Update()
    {
        // Detecta la pulsación del botón B del controlador derecho
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            // Alterna el estado de visibilidad
            imageVisible = !imageVisible;
            vrImage.enabled = imageVisible;
            Debug.Log("Estado de la imagen: " + (imageVisible ? "Visible" : "Oculta"));
        }
    }
}
