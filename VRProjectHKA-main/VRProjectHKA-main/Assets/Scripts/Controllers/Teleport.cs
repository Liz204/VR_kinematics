using Oculus.Interaction;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Teleport : MonoBehaviour
{
    [SerializeField] private XRController controller; // Controlador do jogador (esquerdo ou direito)
    [SerializeField] private LayerMask groundLayer;    // Camada do chão
    [SerializeField] private float teleportationHeight = 0.5f; // Altura de teletransporte (evitar que o jogador fique "atravessando" o chão)

    private bool isTeleporting = false;
    private Vector3 teleportLocation;

    // Referência ao botão de interação (o botão que o jogador usa para teleportar)
    [SerializeField] private InputHelpers.Button teleportButton = InputHelpers.Button.PrimaryButton;
    private bool isButtonPressed = false;

    void Update()
    {
        // Verifica se o jogador pressionou o botão de teletransporte
        isButtonPressed = IsButtonPressed();

        // Se o botão for pressionado, ativa o teletransporte
        if (isButtonPressed)
        {
            if (!isTeleporting)
            {
                TryTeleport();
            }
        }
    }

    // Verifica se o botão foi pressionado
    private bool IsButtonPressed()
    {
        InputHelpers.IsPressed(controller.inputDevice, teleportButton, out bool isPressed);
        return isPressed;
    }

    // Tenta realizar o teletransporte
    private void TryTeleport()
    {
        RaycastHit hit;
        Ray ray = new Ray(controller.transform.position, controller.transform.forward);

        // Verifica se o raio atinge o chão
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            teleportLocation = hit.point + Vector3.up * teleportationHeight; // Ajusta a altura de teletransporte
            isTeleporting = true;
            TeleportToLocation(teleportLocation);
        }
    }

    // Realiza o teletransporte para o local selecionado
    private void TeleportToLocation(Vector3 location)
    {
        // Muda a posição do jogador para o local selecionado
        transform.position = location;
        isTeleporting = false;
    }
}
