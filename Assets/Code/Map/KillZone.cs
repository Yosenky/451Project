using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
public class KillZone : MonoBehaviour
{
    public Transform respawnPoint; // Assign the respawn point in the Inspector

    void OnTriggerEnter(Collider other)
    {
        CharacterController characterController = other.GetComponent<CharacterController>();
        if (characterController != null)
        {
            if (respawnPoint != null)
            {
                // Disable the CharacterController before teleporting
                characterController.enabled = false;

                // Move the player to the respawn point
                other.transform.position = respawnPoint.position;

                // Re-enable the CharacterController
                characterController.enabled = true;
            }
            else
            {
                UnityEngine.Debug.LogError("Respawn point not assigned in the Inspector!");
            }
        }
    }
}