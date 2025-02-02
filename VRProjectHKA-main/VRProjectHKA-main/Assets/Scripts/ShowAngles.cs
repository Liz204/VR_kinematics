using UnityEngine;
using TMPro;

public class UpdateAngleText : MonoBehaviour
{
    private TextMeshProUGUI title2;  // Referencia automática al TextMeshPro
    private Transform cameraTransform; // Referencia automática a la cámara principal
    private MechanicalArmBuilder armBuilder;

    void Start()
    {
        title2 = GetComponent<TextMeshProUGUI>();
        armBuilder = FindObjectOfType<MechanicalArmBuilder>();

        cameraTransform = Camera.main?.transform;
    }

    void Update()
    {
        if (armBuilder != null)
        {
            title2.text = $"{armBuilder.newAngle:F2}°";
        }

        if (cameraTransform != null)
        {
            transform.LookAt(cameraTransform);
            transform.Rotate(0, 180, 0); // Evita que el texto se vea al revés
        }
    }
}
