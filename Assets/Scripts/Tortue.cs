using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Tortue : MonoBehaviour
{
    [Header("Jump")]
    public float jumpForce = 5f;

    
    private const int PLAYER_LAYER = 6; // Tortue layer 
    private const int PIPE_LAYER = 7; // Pipes/ obstacles layer

    private Rigidbody rb;
    private bool jumpRequested;

    // Shield state
    private bool isInvincible = false;
    private Coroutine shieldRoutine;

    [Header("Shield Visual")]
    public GameObject shieldBubble; 

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Bubble off by default
        if (shieldBubble != null)
            shieldBubble.SetActive(false);
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

    private void OnCollisionEnter(Collision collision)
    {
        // Only react to layer pipes (obstacles/pipes)
        if (collision.gameObject.layer != LayerMask.NameToLayer("Pipes"))
            return;

        // If shield is active, ignore damage
        if (isInvincible) return;

        // Apply damage
        SystemeVie systemeVie = FindFirstObjectByType<SystemeVie>();
        if (systemeVie != null)
            systemeVie.ChangeHealth(-1);
    }

    // Called by BouclierMover when collected
    public void ActivateShield(float duration)
    {
        if (shieldRoutine != null) StopCoroutine(shieldRoutine);
        shieldRoutine = StartCoroutine(ShieldCoroutine(duration));
    }

    private IEnumerator ShieldCoroutine(float duration)
    {
        isInvincible = true;

        // Pass through obstacles during shield
        Physics.IgnoreLayerCollision(PLAYER_LAYER, PIPE_LAYER, true);

        // Bubble ON
        if (shieldBubble != null)
            shieldBubble.SetActive(true);

        yield return new WaitForSeconds(duration);

        // Restore collisions
        Physics.IgnoreLayerCollision(PLAYER_LAYER, PIPE_LAYER, false);

        // Bubble OFF
        if (shieldBubble != null)
            shieldBubble.SetActive(false);

        isInvincible = false;
        shieldRoutine = null;
    }
}
