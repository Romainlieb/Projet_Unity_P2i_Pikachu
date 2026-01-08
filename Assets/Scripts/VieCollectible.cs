using UnityEngine;
using System.Collections;

public class VieCollectible : MonoBehaviour
{
    // =========================
    // MOVEMENT
    // =========================
    public float speed = 3f;
    public float leftLimitX = -12f;
    public float spawnX = 12f;

    // =========================
    // SPAWN TIME
    // =========================
    public float minSpawnTime = 40f;
    public float maxSpawnTime = 50f;

    // =========================
    // VERTICAL RANDOM
    // =========================
    public float minY = -2f;
    public float maxY = 2f;

    // =========================
    // AUDIO
    // =========================
    [Header("Audio")]
    public AudioClip collectSound;
    [Range(0f, 1f)] public float collectVolume = 1f;

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
            HideLife();
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

            float y = Random.Range(minY, maxY);
            transform.position = new Vector3(spawnX, y, transform.position.z);

            ShowLife();

            yield return new WaitUntil(() => isActive == false);
        }
    }

    void ShowLife()
    {
        isActive = true;
        if (rend) rend.enabled = true;
        if (col) col.enabled = true;
    }

    void HideLife()
    {
        isActive = false;
        if (rend) rend.enabled = false;
        if (col) col.enabled = false;
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

        if (systemeVie.health >= systemeVie.maxHealth)
            return;

        systemeVie.ChangeHealth(1);

        // 🔊 AUDIO JELLYFISH
        if (Son_jellyFish.Instance != null)
        {
            Son_jellyFish.Instance.PlaySound(collectSound, collectVolume);
        }

        HideLife();
    }
}
