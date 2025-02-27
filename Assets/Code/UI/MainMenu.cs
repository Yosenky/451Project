using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void Start()
    {
        Cursor.lockState = CursorLockMode.None; // make sure cursor is unlocked
    }
}
