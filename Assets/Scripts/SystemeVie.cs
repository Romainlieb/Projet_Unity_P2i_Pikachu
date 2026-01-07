using UnityEngine;

public class SystemeVie : MonoBehaviour
{
    public int maxHealth = 3;
    int currentHealth;

    // read-only property (tutorial requirement)
    public int health { get { return currentHealth; } }

    void Start()
    {
        currentHealth = maxHealth;   // start game with full health
        Debug.Log(currentHealth + "/" + maxHealth);
    }

    void Update()
    {
    }

    public void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
    }
}
