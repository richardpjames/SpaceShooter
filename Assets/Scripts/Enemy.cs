using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private bool startActive = true;
    [SerializeField] private bool boss = false;
    [SerializeField] private bool followPlayer = true;
    [SerializeField] private float rotationSpeed = 0f;
    [Header("AI")]
    [SerializeField] private float activationDistance = 50f;
    [SerializeField] private float viewDistance = 50f;
    [SerializeField] private float evaluationDistance = 5f;
    [SerializeField] private float evaluationCooldown = 1f;
    [SerializeField] private LayerMask evaluationLayerMask;
    [SerializeField] private LayerMask playerOnlyLayerMask;
    [SerializeField] private float desiredPlayerDistanceMin = 5f;
    [SerializeField] private float desiredPlayerDistanceMax = 10f;
    [Header("Projectiles")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private ProjectileManager.Pattern attackPattern;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireCoolDown = 0.25f;
    [SerializeField] private float burstCooldown = 2f;
    [SerializeField] private int burstNumber = 5;
    [SerializeField] private float startupTime = 1f;
    // Private variables
    private float _nextEvaluationTime = 0;
    private Vector2 _direction;
    private Rigidbody2D _rb;
    private Vector3 _lastDirection = Vector3.zero;
    private bool _active = false;
    private float _nextFireTime = 0;
    private float _nextBurtstTime = 0;
    private int _currentBurstCounter = 0;
    private bool _attacking = false;
    // How many directions will we look?
    private const float VIEW_DIRECTIONS = 60;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _active = startActive;
        _nextBurtstTime = Time.time + startupTime;
    }

    private void OnDestroy()
    {
        EventManager.OnEnemyKilled?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        // If we get close enough to the enemy then we want it to activate
        if (!_active)
        {
            // Find the player
            Player player = Object.FindObjectOfType<Player>();
            // Check if the player is within the activation area
            if (player != null && Vector3.Distance(transform.position, player.transform.position) < activationDistance)
            {
                _active = true;
            }
        }
        // If this enemy points towards the player then do so now
        if (_active && followPlayer)
        {
            // Rotate to face the player
            PointToPlayer();
        }
        // Otherwise rotate at the rotation speed
        else if (_active) 
        {
            // Rotate the transform around the z axis (third vector argument) added to existing angle
            transform.rotation *= Quaternion.Euler(0, 0, rotationSpeed * Time.deltaTime);
        }
        // If the cooldown has expired for our AI evaluation, then evaluate again (must be active)
        if (Time.time > _nextEvaluationTime && _active)
        {

            // Check which direction to move
            _direction = DetermineDirection();
            // Update the timer for next evaluation
            _nextEvaluationTime = Time.time + evaluationCooldown;
        }
        // If we can fire a projectile and can see the player, then attack
        if (Time.time > _nextFireTime && Time.time > _nextBurtstTime && _active && (CanSeePlayer() || boss))
        {
            // Set to be attacking
            _attacking = true;
            // Fire a projectile
            ProjectileManager.Instance.Fire(attackPattern, projectilePrefab, firePoint.position, transform.rotation);
            // Reset the cooldown clock
            _nextFireTime = Time.time + fireCoolDown;
            // The number of bullets fired in this burst
            _currentBurstCounter++;
            // If we have hit the limit for this burst
            if (_currentBurstCounter == burstNumber)
            {
                // Set the next fire time and the burst counter back to zero
                _nextBurtstTime = Time.time + burstCooldown;
                _currentBurstCounter = 0;
                // After a burst is complete, the attack is over
                _attacking = false;
            }
        }
    }

    private void FixedUpdate()
    {
        // Enemies are still while attacking
        if (!boss && !_attacking)
        {
            // Apply a force in the direction from the evaluation
            _rb.velocity = _direction * speed;
        }
        else if (boss)
        {
            // If this is a boss then they simply stay static at the center
            _rb.velocity = Vector3.zero;
            transform.position = Vector3.zero;
        }
        else
        {
            _rb.velocity = Vector3.zero;
        }
    }

    private Vector2 DetermineDirection()
    {
        // For boss characters no need to calculate
        if (boss) return Vector2.zero;
        // Constants for how things are weighted
        const float CLOSER_TO_PLAYER = 2f;
        const float TOO_CLOSE_TO_PLAYER = -50f;
        const float HIT_ENVIRONMENT = -1000f;
        // Store all of the weights in a dictionary
        Dictionary<Vector3, float> weights = new Dictionary<Vector3, float>();
        // Get a reference to the player to determine distance
        Player player = Object.FindObjectOfType<Player>();
        if (player == null)
        {
            return Vector3.zero;
        }
        float currentDistanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        // We're going to go around a circle (360 degrees) in the number of view directions
        for (float i = 0; i <= 360f; i += 360f / VIEW_DIRECTIONS)
        {
            // The destination vector for our ray trace (up represents y axis)
            Vector3 destination = Vector3.up;
            // Rotate around the z axis the amount of i
            destination = Quaternion.Euler(0, 0, i) * destination;
            // Perform a raycast along that direction
            RaycastHit2D hit = Physics2D.Raycast(transform.position, destination, evaluationDistance, evaluationLayerMask);
            // How far will we be from the player after a small movement
            float newDistanceToPlayer = Vector3.Distance(transform.position + (destination * evaluationDistance), player.transform.position);
            float weight = 5f;
            // See if this takes us closer to the player and we are too far away
            if (newDistanceToPlayer < currentDistanceToPlayer && currentDistanceToPlayer > desiredPlayerDistanceMax)
            {
                weight += CLOSER_TO_PLAYER;
            }
            // See if we are too close!
            if (newDistanceToPlayer < desiredPlayerDistanceMin)
            {
                weight += TOO_CLOSE_TO_PLAYER;
            }
            // See if we hit something other than the player (e.g the environment)
            if (hit.collider != null)
            {
                if (hit.collider.tag != "Player")
                {
                    weight += HIT_ENVIRONMENT;
                }
            }
            // Store the result
            weights.Add(destination, weight);
            // Draw a line representing the direction and weight for the duation before next evaluation
            Debug.DrawLine(transform.position, transform.position + (destination * weight), Color.white, evaluationCooldown);
        }
        // If the last direction we headed is still the max, then return the same to avoid jittering
        if (_lastDirection != Vector3.zero && weights.Values.Max() == weights[_lastDirection])
        {
            return _lastDirection;
        }
        // Retreive the highest weighted direction from the dictionary (and update _lastDirection) based on random list ordering
        _lastDirection = weights.OrderBy(entry => Random.value).FirstOrDefault(entry => entry.Value == weights.Values.Max()).Key;
        return _lastDirection;
    }

    private void PointToPlayer()
    {
        // Rotate the transform to point towards the mouse
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, GetAngleToPlayer()));
    }

    private float GetAngleToPlayer()
    {
        Player player = Object.FindObjectOfType<Player>();
        if (player == null)
        {
            return 0;
        }
        // Get the position of the mouse
        Vector3 playerPosition = player.transform.position;
        // Subtract from the mouseposition to account for direction
        playerPosition.x -= transform.position.x;
        playerPosition.y -= transform.position.y;
        // Apply the correct rotation
        return Mathf.Atan2(playerPosition.y, playerPosition.x) * Mathf.Rad2Deg;
    }

    private bool CanSeePlayer()
    {
        // Get a reference to the player
        Player player = Object.FindObjectOfType<Player>();
        if (player == null)
        {
            return false;
        }
        // Cast a ray towards the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, viewDistance, evaluationLayerMask);
        // If we hit anything then we can see the player
        if (hit.collider && hit.collider.tag != "Player")
        {
            return false;
        }
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        // Find the player
        Player player = Object.FindObjectOfType<Player>();
        if (player == null)
        {
            return;
        }
        // Draw the desired area around the player
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.transform.position, desiredPlayerDistanceMin);
        Gizmos.DrawWireSphere(player.transform.position, desiredPlayerDistanceMax);
        // Draw the activation circle
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
        // Draw the evaluation circle
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, evaluationDistance);
        // Draw the view distance
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}
