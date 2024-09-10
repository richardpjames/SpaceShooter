using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager Instance { get; private set; }
    public enum Pattern { SINGLE, CIRCLE }
    // Set up the singleton
    private void Awake()
    {
        // Ensure that this is the only instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Fire(Pattern pattern, GameObject projectile, Vector2 startPosition, Quaternion rotation)
    {
        // Determine the pattern based on the input
        if(pattern == Pattern.SINGLE)
        {
            FireSingle(projectile, startPosition, rotation);
        }
        if(pattern == Pattern.CIRCLE)
        {
            FireCircle(projectile, startPosition, rotation);
        }
    }

    // Fires a single projectile from the start position (most basic attack)
    private void FireSingle(GameObject projectile, Vector2 startPosition, Quaternion rotation)
    {
        // Simply instantiate a projectile
        Instantiate(projectile, startPosition, rotation);
    }

    // Fires multiple projectiles in a circle from the start position (all at the same time)
    private void FireCircle(GameObject projectile, Vector2 startPosition, Quaternion rotation)
    {
        const int FIRE_DIRECTIONS = 16;
        // Loop through the number of directions and spawn projectiles
        for (float i = 0; i <= 360f; i += 360f / FIRE_DIRECTIONS)
        {
            // Spawn with a rotate around the z axis the amount of i
            Instantiate(projectile, startPosition, rotation * Quaternion.Euler(0,0,i));
        }
    }
}
