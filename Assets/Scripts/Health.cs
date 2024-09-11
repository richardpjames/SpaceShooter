using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // The characters maximum health and whether their death ends the game
    [SerializeField] protected int maxHealth;
    [SerializeField] protected MMF_Player hitEffects;
    protected int _currentHealth;

    private void Start()
    {
        // Initialise current health on spawn
        _currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        // Play any effects that are present
        if (hitEffects != null)
        {
            hitEffects.PlayFeedbacks();
        }
        _currentHealth = _currentHealth - damage;
        _currentHealth = Math.Max(_currentHealth, 0);
        // Check if the current health is less than zero (leading to death)
        if (_currentHealth <= 0 )
        {
            // Then destroy the object
            Destroy(gameObject);
        }
    }
}
