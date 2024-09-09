using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireCoolDown;
    [SerializeField] private GameObject playerDeathParticles;

    // For setting the direction of the player
    private Vector2 _direction = Vector2.zero;
    private float _nextFireTime = 0;
    private bool _firing = false;

    // Events
    private void Awake()
    {
        EventManager.OnPlayerDead += Die;
    }
    private void OnDestroy()
    {
        EventManager.OnPlayerDead -= Die;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Deal with firing projectiles
        if (_firing)
        {
            FireProjectiles();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Using an impulse gives us a slow down effect
        rb.AddForce(_direction * speed, ForceMode2D.Force);
        // Point it towards the mouse
        PointToMouse();
    }

    // Handles the input from the user when moving
    public void Move(InputAction.CallbackContext context)
    {
        // Get the input from the user
        _direction = context.ReadValue<Vector2>().normalized;
    }

    // Handles the input from the user when clicking "Fire"
    public void Fire(InputAction.CallbackContext context)
    {
        // Determine the state of whether we are firing from whether the button is pressed
        if (context.performed)
        {
            _firing = true;
        }
        if (context.canceled)
        {
            _firing = false;
        }
    }

    // Method for quitting the application TODO: move this to a more sensible place in the code
    public void Quit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            EventManager.OnQuitRequested?.Invoke();
        }
    }

    // Deals with firing of bullets
    private void FireProjectiles()
    {
        if (Time.time > _nextFireTime)
        {
            // Instantiate a projectile facing towards the mouse
            Instantiate(projectilePrefab, firePoint.position, transform.rotation);
            // When can we fire next?
            _nextFireTime = Time.time + fireCoolDown;
        }
    }

    private void PointToMouse()
    {
        // Rotate the transform to point towards the mouse
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, GetAngleToMouse()));
    }

    private float GetAngleToMouse()
    {
        // Get the position of the mouse
        Vector3 mousePosition = Input.mousePosition;
        // Get the position of the spaceship
        Vector3 objectPosition = Camera.main.WorldToScreenPoint(transform.position);
        // Subtract from the mouseposition to account for direction
        mousePosition.x -= objectPosition.x;
        mousePosition.y -= objectPosition.y;
        // Apply the correct rotation
        return Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;
    }

    // When taking damage simply emit a message (managed by Game Manager)
    public void TakeDamage()
    {
        // The game manager deals with this logic after an event
        EventManager.OnPlayerHit?.Invoke();
    }

    // When the game manager signals that the player is dead we trigger this
    private void Die()
    {
        // Plays particles and sets the object to inactive (stopping display and other interactions)
        Instantiate(playerDeathParticles, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
