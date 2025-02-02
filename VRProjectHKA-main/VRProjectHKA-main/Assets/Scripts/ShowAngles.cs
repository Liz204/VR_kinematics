using UnityEngine;
using TMPro;

public class UpdateAngleText : MonoBehaviour
{
    private TextMeshProUGUI title2;  // Referencia automática al TextMeshPro
    private Transform cameraTransform; // Referencia automática a la cámara principal

    void Start()
    {
        title2 = GetComponent<TextMeshProUGUI>();
        cameraTransform = Camera.main?.transform;
    }

    void Update()
    {
        if (cameraTransform != null)
        {
            transform.LookAt(cameraTransform);
            transform.Rotate(0, 180, 0); // Evita que el texto se vea al revés
        }
    }
}
