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
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireCoolDown = 0.25f;
    // Private variables
    private float _nextEvaluationTime = 0;
    private Vector2 _direction;
    private Rigidbody2D _rb;
    private Vector3 _lastDirection = Vector3.zero;
    private bool _active = false;
    private float _nextFireTime = 0;
    // How many directions will we look?
    private const float VIEW_DIRECTIONS = 60;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
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
            if (Vector3.Distance(transform.position, player.transform.position) < activationDistance)
            {
                _active = true;
            }
        }
        // If the cooldown has expired for our AI evaluation, then evaluate again (must be active)
        if (Time.time > _nextEvaluationTime && _active)
        {
            _direction = DetermineDirection();
            // Update the timer for next evaluation
            _nextEvaluationTime = Time.time + evaluationCooldown;
        }
        // If we can fire a projectile and can see the player, then attack
        if (Time.time > _nextFireTime && _active)
        {
            _nextFireTime = Time.time + fireCoolDown;
        }
    }

    private void FixedUpdate()
    {
        // Apply a force in the direction from the evaluation
        _rb.velocity = _direction * speed;
    }

    private Vector2 DetermineDirection()
    {
        // Constants for how things are weighted
        const float CLOSER_TO_PLAYER = 2f;
        const float TOO_CLOSE_TO_PLAYER = -50f;
        const float HIT_ENVIRONMENT = -1000f;
        // Store all of the weights in a dictionary
        Dictionary<Vector3, float> weights = new Dictionary<Vector3, float>();
        // Get a reference to the player to determine distance
        Player player = Object.FindObjectOfType<Player>();
        float currentDistanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        // We're going to go around a circle (360 degrees) in the number of view directions
        for(float i = 0; i <= 360f; i += 360f / VIEW_DIRECTIONS)
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
            if(newDistanceToPlayer < currentDistanceToPlayer && currentDistanceToPlayer > desiredPlayerDistanceMax)
            {
                weight += CLOSER_TO_PLAYER;
            }
            // See if we are too close!
            if(newDistanceToPlayer < desiredPlayerDistanceMin)
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
        if(_lastDirection != Vector3.zero && weights.Values.Max() == weights[_lastDirection])
        {
            return _lastDirection;
        }
        // Retreive the highest weighted direction from the dictionary (and update _lastDirection) based on random list ordering
        _lastDirection = weights.OrderBy(entry => Random.value).FirstOrDefault(entry => entry.Value == weights.Values.Max()).Key;
        return _lastDirection;  
    }

    private void OnDrawGizmosSelected()
    {
        // Find the player
        Player player = Object.FindObjectOfType<Player>();
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
