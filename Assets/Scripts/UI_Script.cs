using UnityEngine;

public class UIHandler : MonoBehaviour
{
    // =========================
    // UI SCREENS (MainMenu et SecondMenu)
    // =========================
    [Header("UI Screens")]
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject loseScreen;

    void Start()
    {
    
        if (startScreen != null)
            startScreen.SetActive(true);

        if (loseScreen != null)
            loseScreen.SetActive(false);
    }

    // =========================
    // PUBLIC FUNCTIONS
    // =========================

    public void DisplayStartScreen()
    {
        if (startScreen != null)
            startScreen.SetActive(true);

        if (loseScreen != null)
            loseScreen.SetActive(false);
    }

    public void DisplayLoseScreen()
    {
        if (loseScreen != null)
            loseScreen.SetActive(true);
    }

    public void HideStartScreen()
    {
        if (startScreen != null)
            startScreen.SetActive(false);
    }
}
