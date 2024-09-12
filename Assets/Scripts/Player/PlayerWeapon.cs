using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : MonoBehaviour
{
    // Get the prefab for the projectile and a cooldown time
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireCoolDown;
    // Record whether the player is currently firing and the next available time after cooldown
    private float nextFireTime = 0;
    private bool firing = false;

    private void Update()
    {
        // Deal with firing projectiles
        if (firing)
        {
            FireProjectiles();
        }
    }

    // Deals with firing of bullets
    private void FireProjectiles()
    {
        if (Time.time > nextFireTime)
        {
            // Instantiate a projectile facing towards the mouse
            ProjectileManager.Instance.Fire(ProjectileManager.Pattern.SINGLE, projectilePrefab, transform.position, transform.rotation);
            // When can we fire next?
            nextFireTime = Time.time + fireCoolDown;
        }
    }

    // Handles the input from the user when clicking "Fire"
    public void Fire(InputAction.CallbackContext context)
    {
        // Determine the state of whether we are firing from whether the button is pressed
        if (context.performed)
        {
            firing = true;
        }
        if (context.canceled)
        {
            firing = false;
        }
    }



}
