using UnityEngine;

public class Son_jellyFish : MonoBehaviour
{
    public static Son_jellyFish Instance;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton simple
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound
    }

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        audioSource.PlayOneShot(clip, volume);
    }
}
