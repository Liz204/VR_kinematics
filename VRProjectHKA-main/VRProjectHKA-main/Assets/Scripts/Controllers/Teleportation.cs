using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


public class Teleportation : MonoBehaviour
{
    public XRController controller; // O controlador VR
    public GameObject teleportIndicator; // Um indicador visual para onde o jogador será teletransportado
    public LayerMask teleportLayer; // Defina o que o controlador pode atingir (por exemplo, o chão)

    private bool isTeleporting = false;
    private Vector3 targetPosition;

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(controller.transform.position, controller.transform.forward);

        // Verifica se o raio atinge o chão ou um objeto específico
        if (Physics.Raycast(ray, out hit, 100f, teleportLayer))
        {
            targetPosition = hit.point;
            teleportIndicator.SetActive(true);
            teleportIndicator.transform.position = targetPosition;

            // Verifica o estado do gatilho (Trigger)
            if (controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool isPressed) && isPressed)
            {
                TeleportToTarget();
            }
        }
        else
        {
            teleportIndicator.SetActive(false);
        }
    }

    void TeleportToTarget()
    {
        // Teletransporta o jogador para a posição indicada
        transform.position = targetPosition;
    }
}
