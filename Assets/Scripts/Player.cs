using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed;
    // For setting the direction of the player
    private Vector2 _direction = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Move the spaceship in the direction of the input
        rb.velocity = _direction * speed;
        // Point it towards the mouse
        PointToMouse();
    }

    public void Move(InputAction.CallbackContext context)
    {
        // Get the input from the user
        _direction = context.ReadValue<Vector2>().normalized;
    }

    private void PointToMouse()
    {
        // Get the position of the mouse
        Vector3 mousePosition = Input.mousePosition;
        // Get the position of the spaceship
        Vector3 objectPosition = Camera.main.WorldToScreenPoint(transform.position);
        // Subtract from the mouseposition to account for direction
        mousePosition.x -= objectPosition.x;
        mousePosition.y -= objectPosition.y;
        // Apply the correct rotation
        float angle = Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
