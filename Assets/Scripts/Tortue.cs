using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Tortue : MonoBehaviour
{
    [Header("Jump")]
    public float jumpForce = 5f;

    // Layers (your setup)
    private const int PLAYER_LAYER = 6;
    private const int PIPE_LAYER = 7;

    private Rigidbody rb;
    private bool jumpRequested;

    private bool isInvincible = false;

    private Renderer rend;
    private Color originalColor;
    private Coroutine shieldRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rend = GetComponent<Renderer>();
        if (rend != null)
            originalColor = rend.material.color;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpRequested = true;
    }

    void FixedUpdate()
    {
        if (!jumpRequested) return;

        var v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        jumpRequested = false;
    }

    public void ActivateShield(float duration)
    {
        if (shieldRoutine != null) StopCoroutine(shieldRoutine);
        shieldRoutine = StartCoroutine(ShieldCoroutine(duration));
    }

    private IEnumerator ShieldCoroutine(float duration)
    {
        isInvincible = true;

        // pass through pipes
        Physics.IgnoreLayerCollision(PLAYER_LAYER, PIPE_LAYER, true);

        // optional transparency
        if (rend != null)
        {
            Color c = rend.material.color;
            c.a = 0.5f;
            rend.material.color = c;
        }

        yield return new WaitForSeconds(duration);

        // restore collisions
        Physics.IgnoreLayerCollision(PLAYER_LAYER, PIPE_LAYER, false);

        // restore visibility
        if (rend != null)
            rend.material.color = originalColor;

        isInvincible = false;
        shieldRoutine = null;
    }
}
