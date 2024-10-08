using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    // Magnitude sets how big the side to side and frequency how often
    [SerializeField] private float sinMagnitude;
    [SerializeField] private float sinFrequency;
    [SerializeField] private float jitter;
    private Rigidbody2D rb;
    private Vector3 _baseVelocityUp;
    private Vector3 _baseVelocityRight;
    private float _startTime;

    // Start is called before the first frame update
    void Start()
    {
        // Get the rigidbody
        rb = GetComponent<Rigidbody2D>();
        // Move the projectile based on our firing direction
        Transform fireDirection = transform;
        fireDirection.Rotate(0, 0, -90);
        // Setting the velocity will keep this moving
        _baseVelocityUp = (fireDirection.up * speed);
        _baseVelocityRight = fireDirection.right;
        // When was the object created
        _startTime = Time.time;
        // Destroy after the lifetime
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        // Take our direction and adjust by sin to give any side to side movement
        rb.velocity = _baseVelocityUp;
        // Now add sideways movement based on a sin wave
        rb.velocity += (Vector2) _baseVelocityRight * Mathf.Sin((Time.time - _startTime) * sinFrequency) * sinMagnitude;
        // Then add jitter to the sideways movement
        rb.velocity += (Vector2)_baseVelocityRight * Random.Range(-1f, 1f) * jitter;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get the health script from whatever we hit
        Health health = collision.gameObject.GetComponent<Health>();
        if(health != null)
        {
            health.TakeDamage(1);
        }
        // Finally, desroy the projectile itself
        Destroy(gameObject);
    }
}
