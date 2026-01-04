using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Tortue : MonoBehaviour
{
    [Header("Jump")]
    public float jumpForce = 5f;   // Jump strength

    // =========================
    // Private variables
    // =========================
    private Rigidbody rb;          // Player Rigidbody (3D)
    private bool jumpRequested;    // Input flag

    // =========================
    // Shield state
    // =========================
    private bool isInvincible = false;   // True while shield is active

    void Awake()
    {
        // Get the Rigidbody attached to the player
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Read input in Update
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpRequested = true;
    }

    void FixedUpdate()
    {
        // Apply physics in FixedUpdate
        if (!jumpRequested) return;

        // Reset vertical velocity for Flappy-like jump
        var v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;

        // Add upward impulse
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // Reset input flag
        jumpRequested = false;
    }

    // =========================
    // Collision with obstacles (pipes)
    // =========================
    private void OnCollisionEnter(Collision collision)
    {
        // Ignore damage if shield is active
        if (isInvincible) return;

        // Only detect pipes (optional but recommended)
        if (collision.collider.CompareTag("Pipe"))
        {
            Debug.Log("Hit pipe (no health system yet)");
        }
    }

    // =========================
    // Shield activation (called by Bouclier.cs)
    // =========================
    public void ActivateShield(float duration)
    {
        // Restart shield timer if already active
        StopAllCoroutines();
        StartCoroutine(ShieldCoroutine(duration));
    }

    private IEnumerator ShieldCoroutine(float duration)
    {
        // Enable invincibility
        isInvincible = true;

        // Wait for shield duration
        yield return new WaitForSeconds(duration);

        // Disable invincibility
        isInvincible = false;
    }
}
