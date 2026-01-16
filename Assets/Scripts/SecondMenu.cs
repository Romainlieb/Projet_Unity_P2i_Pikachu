using UnityEngine;
using UnityEngine.SceneManagement;

public class SecondMenu : MonoBehaviour
{
    public string gameSceneName = "GameScene";

    public void PlayAgain()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
