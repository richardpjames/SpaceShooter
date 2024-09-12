using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Movement Speed
    [SerializeField] private float speed;
    // Rigidbody for moving
    private Rigidbody2D rb;

    // For setting the direction of the player
    private Vector2 inputDirection = Vector2.zero;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Movement is based on physics, so use fixed update
    private void FixedUpdate()
    {
        // Using an impulse gives us a slow down effect
        rb.AddForce(inputDirection * speed, ForceMode2D.Force);
    }

    // Handles the input from the user when moving
    public void SetDirectionFromInput(InputAction.CallbackContext context)
    {
        // Get the input from the user
        inputDirection = context.ReadValue<Vector2>().normalized;
    }
}
