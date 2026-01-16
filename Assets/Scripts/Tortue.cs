using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Tortue : MonoBehaviour
{
    [Header("Jump")]
    public float jumpForce = 5f;

    private const int PLAYER_LAYER = 6; // Tortue layer 
    private const int PIPE_LAYER = 7;   // Pipes / obstacles layer

    private Rigidbody rb;
    private bool jumpRequested;
    private Collider tortueCollider;

    // Shield state
    private bool isInvincible = false;
    private Coroutine shieldRoutine;

    [Header("Shield Visual")]
    public GameObject shieldBubble;

    [Header("Vertical Limits")]
    public float minY = -4f;
    public float maxY = 4f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tortueCollider = GetComponent<Collider>();

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

    //  LIMITES VERTICALES (
    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only react to obstacles
        if (collision.gameObject.layer != LayerMask.NameToLayer("Pipes"))
            return;

        // Ignore damage if shield is active
        if (isInvincible) return;

        SystemeVie systemeVie = FindFirstObjectByType<SystemeVie>();
        if (systemeVie != null)
            systemeVie.ChangeHealth(-1);

        if (tortueCollider != null)
            StartCoroutine(DisableColliderTemporarily());
    }

    private IEnumerator DisableColliderTemporarily()
    {
        tortueCollider.enabled = false;
        ActivateShield(3f);
        yield return new WaitForSeconds(3f);
        tortueCollider.enabled = true;
    }

    // Called by BouclierMover
    public void ActivateShield(float duration)
    {
        if (shieldRoutine != null) StopCoroutine(shieldRoutine);
        shieldRoutine = StartCoroutine(ShieldCoroutine(duration));
    }

    private IEnumerator ShieldCoroutine(float duration)
    {
        isInvincible = true;

        Physics.IgnoreLayerCollision(PLAYER_LAYER, PIPE_LAYER, true);

        if (shieldBubble != null)
            shieldBubble.SetActive(true);

        yield return new WaitForSeconds(duration);

        Physics.IgnoreLayerCollision(PLAYER_LAYER, PIPE_LAYER, false);

        if (shieldBubble != null)
            shieldBubble.SetActive(false);

        isInvincible = false;
        shieldRoutine = null;
    }
}
