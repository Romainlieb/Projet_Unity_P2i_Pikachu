using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Tortue : MonoBehaviour
{
    [Header("Jump")]
    public float jumpForce = 5f;

    // Layers
    private const int PLAYER_LAYER = 6;
    private const int PIPE_LAYER = 7;

    private Rigidbody rb;
    private bool jumpRequested;

    // Shield state
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

        // Reset vertical velocity for jump
        var v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        jumpRequested = false;
    }

    private void OnCollisionEnter(Collision collision)
{
    // Detect collision with on Pipes layer 
    if (collision.gameObject.layer != LayerMask.NameToLayer("Pipes"))
        return;

    // If shield is active, ignore damage
    if (isInvincible) return;

    // Apply damage
    SystemeVie systemeVie = FindFirstObjectByType<SystemeVie>();
    if (systemeVie != null)
    {
        systemeVie.ChangeHealth(-1);
    }

    }

    public void ActivateShield(float duration)
    {
        if (shieldRoutine != null) StopCoroutine(shieldRoutine);
        shieldRoutine = StartCoroutine(ShieldCoroutine(duration));
    }

    private IEnumerator ShieldCoroutine(float duration)
    {
        isInvincible = true;

        // Pass through pipes during shield
        Physics.IgnoreLayerCollision(PLAYER_LAYER, PIPE_LAYER, true);

        // Semi-transparent ?? to see exactly with the tortue player how it work !! 
        if (rend != null)
        {
            Color c = rend.material.color;
            c.a = 0.5f;
            rend.material.color = c;
        }

        yield return new WaitForSeconds(duration);

        // Restore collisions
        Physics.IgnoreLayerCollision(PLAYER_LAYER, PIPE_LAYER, false);

        // Restore visibility
        if (rend != null)
            rend.material.color = originalColor;

        isInvincible = false;
        shieldRoutine = null;
    }
}
