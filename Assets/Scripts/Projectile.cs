using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    [SerializeField] private GameObject particlePrefab;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        // Get the rigidbody
        rb = GetComponent<Rigidbody2D>();
        // Move the projectile based on our firing direction
        Transform fireDirection = transform;
        fireDirection.Rotate(0, 0, -90);
        // Setting the velocity will keep this moving
        rb.velocity = fireDirection.up * speed;
        // Destroy after the lifetime
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Create a screenshake
        FXManager.Instance.ShakeCamera();
        // Create sparks
        Instantiate(particlePrefab, transform.position, Quaternion.identity);
        // Check if we hit an enemy
        if (collision.gameObject.tag == "Enemy")
        {
            // Get the enemy script and execute the take damage method
            collision.gameObject.GetComponent<Enemy>().TakeDamage();
        }
        else if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().TakeDamage();
        }
        // Finally, desroy the projectile itself
        Destroy(gameObject);
    }
}
