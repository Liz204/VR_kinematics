using UnityEngine;
using TMPro;

public class UpdateAngleText : MonoBehaviour
{
    private TextMeshProUGUI title2;  // Automatic reference to the TextMeshPro
    private Transform cameraTransform; // Automatic reference to the main camera

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
            transform.Rotate(0, 180, 0); // Prevents the text from appearing reversed
        }
    }
}
