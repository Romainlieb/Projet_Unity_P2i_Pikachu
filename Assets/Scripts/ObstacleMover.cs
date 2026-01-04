using UnityEngine;


public class ObstacleMover : MonoBehaviour
{
    public float speed = 3f;
    public float leftLimitX = -12f;   
    public float respawnX = 12f;      

    [Header("Vertical random")]
    public bool randomizeY = true;
    public float minY = -2f;
    public float maxY = 2f;

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= leftLimitX)
        {
            float y = transform.position.y;
            if (randomizeY) y = Random.Range(minY, maxY);

            transform.position = new Vector3(respawnX, y, transform.position.z);
        }
    }
}

