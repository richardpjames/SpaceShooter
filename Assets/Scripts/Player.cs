using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private float slowDownFactor;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireCoolDown;

    // For setting the direction of the player
    private Vector2 _direction = Vector2.zero;
    private float _nextFireTime = 0;
    private bool _firing = false;

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
        // If the button is being pressed then set the velocity
        if (_direction != Vector2.zero)
        {
            // Move the spaceship in the direction of the input
            rb.velocity = _direction * speed;
        }
        // Otherwise slow down gently
        else
        {
            rb.velocity *= slowDownFactor;
        }
        // Point it towards the mouse
        PointToMouse();
    }

    public void Move(InputAction.CallbackContext context)
    {
        // Get the input from the user
        _direction = context.ReadValue<Vector2>().normalized;
    }

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
}
