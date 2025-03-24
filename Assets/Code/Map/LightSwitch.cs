using System;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public Light targetLight;
    public float interactDistance = 5f;

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            targetLight.enabled = !targetLight.enabled; // toggle the light
            Debug.Log("player toggled light");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
