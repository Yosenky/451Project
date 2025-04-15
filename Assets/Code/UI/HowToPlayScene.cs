using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToPlayScene : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }
}