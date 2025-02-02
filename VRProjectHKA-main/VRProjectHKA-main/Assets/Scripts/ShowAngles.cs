using UnityEngine;
using TMPro;

public class UpdateAngleText : MonoBehaviour
{
    private TextMeshProUGUI title;  // Referencia automática al TextMeshPro
    private Transform cameraTransform; // Referencia automática a la cámara principal
    private MechanicalArmBuilder armBuilder;

    private float angle;

    void Start()
    {
        title = GetComponent<TextMeshProUGUI>();
        armBuilder = FindObjectOfType<MechanicalArmBuilder>();

        cameraTransform = Camera.main?.transform;

        if (cameraTransform == null)
        {
            Debug.LogWarning("No se encontró una cámara principal en la escena.");
        }
        if (armBuilder == null)
        {
            Debug.LogWarning("No se encontró el script MechanicalArmBuilder en la escena.");
        }
    }

    void Update()
    {
        if (armBuilder != null)
        {
            float newAngle = armBuilder.newAngle; // Accede a la variable desde el otro script
            title.text = $"{newAngle:F2}°"; // Muestra solo el valor numérico
        }

        if (cameraTransform != null)
        {
            transform.LookAt(cameraTransform);
            transform.Rotate(0, 180, 0); // Evita que el texto se vea al revés
        }
    }
}
