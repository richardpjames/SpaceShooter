using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float viewDistance = 5f;
    [SerializeField] private float evaluationCooldown = 1f;
    [SerializeField] private LayerMask evaluationLayerMask;
    [SerializeField] private float desiredPlayerDistanceMin = 5f;
    private float _nextEvaluationTime = 0;
    private Vector2 _direction;
    private Rigidbody2D _rb;
    private Vector3 _lastDirection = Vector3.zero;
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
        // If the cooldown has expired for our AI evaluation, then evaluate again
        if (Time.time > _nextEvaluationTime)
        {
            _direction = DetermineDirection();
            // Update the timer for next evaluation
            _nextEvaluationTime = Time.time + evaluationCooldown;
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
        const float PLAYER_ALONG_RAYCAST = 5f;
        const float HIT_ENVIRONMENT = -5f;
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
            RaycastHit2D hit = Physics2D.Raycast(transform.position, destination, viewDistance, evaluationLayerMask);
            // How far will we be from the player after a small movement (don't take account of view distance)
            float newDistanceToPlayer = Vector3.Distance(transform.position + destination, player.transform.position);
            float weight = 5f;
            // See if this takes us closer to the player
            if(newDistanceToPlayer < currentDistanceToPlayer)
            {
                weight += CLOSER_TO_PLAYER;
            }
            // See if we are too close!
            if(newDistanceToPlayer < desiredPlayerDistanceMin)
            {
                weight += TOO_CLOSE_TO_PLAYER;
            }
            // See if we hit something
            if (hit.collider != null) {
                if(hit.collider.tag == "Player")
                {
                    weight += PLAYER_ALONG_RAYCAST;
                }
                else
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
        // Retreive the highest weighted direction from the dictionary (and update _lastDirection)
        _lastDirection = weights.FirstOrDefault(entry => entry.Value == weights.Values.Max()).Key;
        return _lastDirection;  
    }
}
