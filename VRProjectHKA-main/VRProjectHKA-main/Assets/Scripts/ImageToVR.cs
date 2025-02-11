using UnityEngine;
using UnityEngine.UI;

public class ImageToVR : MonoBehaviour
{
    public Image[] images;

    public Vector2 targetSize = new Vector2(200, 200); // Size of the images
    
    // -1 = none 
    private int currentIndex = -1;

    void Start()
    {
        // Sets the size and hides all images
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
            Debug.LogWarning("No images have been assigned in the array.");
        }
    }

 void Update()
    {
        // When pressing the B button on the right controller (OVRInput.Button.Two)
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            // If there is any visible image, hide it
            if (currentIndex != -1)
            {
                images[currentIndex].enabled = false;
            }

            // Increment the index cyclically
            currentIndex++;

            // If we reach the end of the array, reset the index to -1
            if (currentIndex >= images.Length)
            {
                currentIndex = -1;
            }

            // If we are not in state -1, show the image at the current index
            if (currentIndex != -1)
            {
                images[currentIndex].enabled = true;
                Debug.Log("Displaying image " + (currentIndex + 1));
            }
            else
            {
                Debug.Log("No visible image.");
            }
        }
    }
}
