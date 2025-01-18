using UnityEngine;
using UnityEngine.UI;
using Oculus.Interaction;
using UnityEngine.SceneManagement;

public class ButtonRay : MonoBehaviour
{
    [SerializeField] private RayInteractor rayInteractor; // Referencia al RayInteractor
    [SerializeField] private GameObject CubeObject;     // Objeto Canvas
    [SerializeField] private Color normalColor = Color.white; // Color por defecto
    [SerializeField] private Color selectableColor = Color.green; // Color seleccionable
    [SerializeField] private Color hoverColor = Color.yellow; // Color al pasar el puntero
    
    [SerializeField] public string sceneToLoad; // Nombre de la escena a cargar

    private bool isHovering = false; // Indica si el puntero está sobre el objeto
    private Image canvasImage;

 

    void Start()
    {

        if (rayInteractor == null)
        {
            Debug.LogError("No se ha asignado un RayInteractor al script.");
            return;
        }

        if ( CubeObject== null)
        {
            Debug.LogError("No se ha asignado ningún objeto al script.");
            return;
        }
        
        Renderer cubeRenderer = CubeObject.GetComponent<Renderer>();

        // Buscar automáticamente el componente Image en el Canvas
        //canvasImage = canvasObject.GetComponent<Image>();
        /*if (canvasImage == null)
        {
            Debug.LogError("El objeto Canvas no tiene un componente Image.");
            return;
        }*/

        // Establecer el color inicial
        cubeRenderer.material.color = normalColor;
    }

    void Update()
    {
        HandlePointerHover();

        if (IsSelectButtonDown() && isHovering)
        {
            ChangeScene();
        }
    }

    private bool IsSelectButtonDown()
    {
        // Verificar si se presiona el botón del VR controller
        return OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);
    }

    private void ChangeScene()
    {
        Debug.Log("Cambiando de escena...");
        SceneManager.LoadScene(sceneToLoad);
        // Aquí puedes agregar lógica para cambiar de escena
    }

    private void HandlePointerHover()
    {
        if (rayInteractor == null) return;

        Ray ray = rayInteractor.Ray;
        
        Renderer cubeRenderer = CubeObject.GetComponent<Renderer>();

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider != null && hit.collider.gameObject == CubeObject)
            {
                if (!isHovering)
                {
                    isHovering = true;
                    cubeRenderer.material.color = hoverColor;
                }
                return;
            }
        }

        // Si no está haciendo hover, revertir al color seleccionable
        if (isHovering)
        {
            isHovering = false;
            cubeRenderer.material.color = selectableColor;
        }
    }
}
