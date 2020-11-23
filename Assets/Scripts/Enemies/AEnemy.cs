using UnityEngine;
using System;

// All enemies inherit from this. This class has health methods. Also has the instance variables for _stateMachine,_animator,_spirteRenderer, and _rb.
public abstract class AEnemy : MonoBehaviour, IHealth // Different abstract classes for enemy types will inherit from this
{
    #region Gameplay Constants
    // These are default values, should all be overriden in inheriting class
    public abstract int MaxHealth { get; }
    public abstract float DeathAnimationTime { get; }
    #endregion

    protected int health = 0;
    public GameObject currentHealthBar;
    public bool isDead = false;

    protected StateMachine _stateMachine; // Using underscores to note instance variables where it could be ambiguous
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    protected Rigidbody2D _rb;

    #region Health/death Methods
    public int GetHealth()
    {
        return health;
    }

    public void TakeDamage(int damageAmount)
    {
        WhiteFlashManager.FlashWhite(gameObject);
        health -= damageAmount;
        if (health < 0) { health = 0; }
    }

    public void GainHealth(int healthAmount)
    {
    }

    public bool IsDead()
    {
        // If health less than 0, return true
        return (health <= 0) ? true : false;
    }

    protected void UpdateHealthBar()
    {
        currentHealthBar.transform.localScale = new Vector3(Mathf.Lerp(currentHealthBar.transform.localScale.x, (float)GetHealth() / (float)MaxHealth, 0.7f), 1f);
    }

    protected void CheckIfDead()
    {
        // Check if enemy still has health, if not then kill the enemy
        if (IsDead())
        {
            if (isDead == false) { Die(); } // Enemy hasn't already died. TODO: Make this cleaner
        }
        else { UpdateHealthBar(); }
    }

    protected abstract void Die();
    #endregion
}