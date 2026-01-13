// Invulnerabilité.cs
using UnityEngine;
using System.Collections;

public class Invulnerabilité : MonoBehaviour
{
    [Header("Invincibility")]
    public float invincibleAlpha = 0.5f;

    private const int PLAYER_LAYER = 6;    // Tortue layer
    private const int PIPE_LAYER = 7;      // Pipes/obstacles layer

    private Renderer rend;
    private Color originalColor;

    private Coroutine routine;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
            originalColor = rend.material.color;
    }

    public void Activate(float duration)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(InvulnerabilityRoutine(duration));
    }

    private IEnumerator InvulnerabilityRoutine(float duration)
    {
        // Pass through pipes
        Physics.IgnoreLayerCollision(PLAYER_LAYER, PIPE_LAYER, true);

        yield return new WaitForSeconds(duration);

        // Restore collisions
        Physics.IgnoreLayerCollision(PLAYER_LAYER, PIPE_LAYER, false);

        // Restore opacity
        if (rend != null)
            rend.material.color = originalColor;

        routine = null;
    }
}
