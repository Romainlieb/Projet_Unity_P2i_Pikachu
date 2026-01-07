using UnityEngine;
using System.Collections;

public class BouclierMover : MonoBehaviour
{
    // =========================
    // MOVEMENT 
    // =========================
    public float speed = 3f;          // speed moving left
    public float leftLimitX = -12f;   // when to hide after passing player
    public float spawnX = 12f;        // where it appears (right side)

    // =========================
    // SPAWN TIME
    // =========================
    public float minSpawnTime = 20f;  // minimum time before spawn
    public float maxSpawnTime = 30f;  // maximum time before spawn

    // =========================
    // VERTICAL RANDOM
    // =========================
    public float minY = -2f;
    public float maxY = 2f;

    // =========================
    // SHIELD EFFECT
    // =========================
    public float shieldDuration = 5f;

    // =========================
    // INTERNAL STATE
    // =========================
    private bool isActive = false;

    private Renderer rend;
    private Collider col;

    void Awake()
    {
        // cache components
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
    }

    void Start()
    {
        // hide at start
        HideShield();

        // start spawn loop
        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        // move only when active
        if (!isActive) return;

        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= leftLimitX)
        {
            HideShield();
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // wait 20–30 seconds
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            // random Y position
            float y = Random.Range(minY, maxY);
            transform.position = new Vector3(spawnX, y, transform.position.z);

            // show and activate
            ShowShield();

            // wait until collected or passed
            yield return new WaitUntil(() => isActive == false);
        }
    }

    void ShowShield()
    {
        isActive = true;

        if (rend != null) rend.enabled = true;
        if (col != null) col.enabled = true;
    }

    void HideShield()
    {
        isActive = false;

        if (rend != null) rend.enabled = false;
        if (col != null) col.enabled = false;
    }

    // =========================
    // COLLECTION
    // =========================
    private void OnTriggerEnter(Collider other)
    {
        Tortue player = other.GetComponent<Tortue>();
        if (player == null) return;

        // activate shield on player
        player.ActivateShield(shieldDuration);

        // hide shield after collection
        HideShield();
    }
}
