using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnBackButton : MonoBehaviour
{
    public OVRInput.Button buttonToPress = OVRInput.Button.PrimaryIndexTrigger; // Botón trasero derecho
    public string sceneToLoad; // Nombre de la escena a cargar

    void Update()
    {
        // Detectar si el botón trasero derecho fue presionado
        if (OVRInput.GetDown(buttonToPress))
        {
            Debug.Log("Botón trasero presionado. Cambiando de escena...");
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}