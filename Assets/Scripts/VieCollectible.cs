using UnityEngine;
using System.Collections;

public class VieMover : MonoBehaviour
{
    // =========================
    // MOVEMENT (like pipes)
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
    // INTERNAL STATE
    // =========================
    private bool isActive = false;

    private Renderer rend;
    private Collider col;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
    }

    void Start()
    {
        HideLife();
        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        if (!isActive) return;

        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= leftLimitX)
        {
            HideLife();
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            float y = Random.Range(minY, maxY);
            transform.position = new Vector3(spawnX, y, transform.position.z);

            ShowLife();

            yield return new WaitUntil(() => isActive == false);
        }
    }

    void ShowLife()
    {
        isActive = true;

        if (rend != null) rend.enabled = true;
        if (col != null) col.enabled = true;
    }

    void HideLife()
    {
        isActive = false;

        if (rend != null) rend.enabled = false;
        if (col != null) col.enabled = false;
    }

    // =========================
    // COLLECTION 
    // =========================
    void OnTriggerEnter(Collider other)
    {
        Tortue tortue = other.GetComponent<Tortue>();
        if (tortue == null) return;

        SystemeVie systemeVie = FindFirstObjectByType<SystemeVie>();
        if (systemeVie == null) return;

        systemeVie.ChangeHealth(1);

        // Instead of Destroy(gameObject), we hide it so it can respawn later
        HideLife();
    }
}
