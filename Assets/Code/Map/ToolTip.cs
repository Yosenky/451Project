using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTip3D : MonoBehaviour
{
    public GameObject playerCamera; 
    public Canvas canvas;
    public TextMeshProUGUI tipText; 

    private bool hasSeenTip = false;

    void Start()
    {
        if (canvas != null)
        {
            //canvas.enabled = false; // hides it first 
            // problem with this is that it is hard to trigger it from all directions. 
        }
    }

    void Update()
    {
        // Make the canvas face the player
        if (playerCamera != null)
        {
            //transform.LookAt(playerCamera.transform);
            //transform.Rotate(0, 180, 0); // Flip if it's backward
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasSeenTip)
        {
            Debug.Log("player entered tip zone");
            canvas.enabled = true;
            hasSeenTip = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player exited tip zone");
            canvas.enabled = false;
        }
    }
}
