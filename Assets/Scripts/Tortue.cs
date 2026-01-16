using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

    // Hearts UI
    private List<GameObject> hearts = new List<GameObject>();

    [Header("Shield Visual")]
    public GameObject shieldBubble;

    [Header("Vertical Limits")]
    public float minY = -4f;
    public float maxY = 4f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tortueCollider = GetComponent<Collider>();

        // Reset collision layers to ensure detection works after scene reload
        Physics.IgnoreLayerCollision(PLAYER_LAYER, PIPE_LAYER, false);

        // Reset invincibility state
        isInvincible = false;
        if (shieldRoutine != null)
        {
            StopCoroutine(shieldRoutine);
            shieldRoutine = null;
        }

        // Reset jump state
        jumpRequested = false;

        // Ensure collider is enabled
        if (tortueCollider != null)
            tortueCollider.enabled = true;

        // Bubble off by default
        if (shieldBubble != null)
            shieldBubble.SetActive(false);

        // Find all Heart(Clone) in the Canvas
        FindHearts();

        // Reactivate all hearts on scene start
        ReactivateAllHearts();
    }

    void Start()
    {
        // Reset velocity to zero when scene starts/reloads
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void FindHearts()
    {
        hearts.Clear();
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        // Find Panel in Canvas
        Transform panel = canvas.transform.Find("Panel");
        if (panel == null) return;

        // Find all Heart(Clone) children
        for (int i = 0; i < panel.childCount; i++)
        {
            Transform child = panel.GetChild(i);
            if (child.name.Contains("Heart(Clone)"))
            {
                hearts.Add(child.gameObject);
            }
        }

        // Sort by sibling index to maintain order
        hearts = hearts.OrderBy(h => h.transform.GetSiblingIndex()).ToList();
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

        // Disable one heart in UI
        DisableHeart();

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

    private void DisableHeart()
    {
        // Find the first active heart and disable it
        foreach (GameObject heart in hearts)
        {
            if (heart != null && heart.activeSelf)
            {
                heart.SetActive(false);
                return;
            }
        }
    }

    // Public method to enable a heart when health is recovered
    public void EnableHeart()
    {
        // Find the first inactive heart and enable it (from left to right)
        foreach (GameObject heart in hearts)
        {
            if (heart != null && !heart.activeSelf)
            {
                heart.SetActive(true);
                return;
            }
        }
    }

    private void ReactivateAllHearts()
    {
        // Reactivate all hearts when scene starts/reloads
        foreach (GameObject heart in hearts)
        {
            if (heart != null)
                heart.SetActive(true);
        }
    }
}
