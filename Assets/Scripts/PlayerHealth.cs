using System;

public class PlayerHealth : Health
{
    public override void TakeDamage(int damage)
    {
        _currentHealth = _currentHealth - damage;
        _currentHealth = Math.Max(_currentHealth, 0);
        // Signal to other components that the player health is updated
        EventManager.OnHealthUpdated?.Invoke(_currentHealth, maxHealth);
        // Check if the current health is less than zero (leading to death)
        if (_currentHealth <= 0)
        {
            // Tell the game manager that the game is over
            GameManager.Instance.EndGame();
            // Then destroy the object
            Destroy(gameObject);
        }
    }
}
