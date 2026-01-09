using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemeVie : MonoBehaviour
{
    public int maxHealth = 3;
    int currentHealth;

    // read-only property
    public int health { get { return currentHealth; } }

    [Header("Game Over")]
    public string gameOverSceneName = "SecondMenu-UI";

    void Start()
    {
        currentHealth = maxHealth;  
        Debug.Log(currentHealth + "/" + maxHealth);
    }

    public void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);

        // GAME OVER
        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(gameOverSceneName);
        }
    }
}
