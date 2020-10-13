using UnityEngine;
using System;

public abstract class AEnemy: MonoBehaviour, IHealth // Different abstract classes for enemy types will inherit from this
{
    #region Gameplay Constants
    // These are default values, should all be overriden in inheriting class
    public abstract int MaxHealth { get; }
    public abstract float DeathAnimationTime { get; }

    public abstract float IdleTime { get; }
    public abstract float MoveTime { get; }
    public abstract float MoveSpeed { get; }
    public abstract float AttackRange { get; }
    public abstract int AttackDamage { get; }
    public abstract float AttackCooldown { get; }
    #endregion

    protected int health = 0;
    protected float attackCooldownTimer = 0f;

    #region Boolean methods for state transitions
    public bool isDead = false;
	public bool isMoving = false;
    public bool isAttacking = false;
    public bool canAttack = true;

    // Note: When calling these methods, we use IsMoving()() or IsMoving().Invoke()
    public Func<bool> IsMoving() => () => (isMoving);
    public Func<bool> IsIdle() => () => (!isMoving);

    public Func<bool> IsInRange() => () => (Vector2.Distance(_rb.position, GameManager.GetMainPlayerRb().position) < AttackRange);
    public Func<bool> IsInRangeAndCanAttack() => () => (IsInRange()() && canAttack);

    public Func<bool> IsAttacking() => () => (isAttacking);
    public Func<bool> IsNotAttacking() => () => (!isAttacking);
    #endregion

    protected StateMachine _stateMachine; // Using underscores to note instance variables where it could be ambiguous
	protected Animator _animator;
	protected SpriteRenderer _spriteRenderer;
	protected Rigidbody2D _rb;


    public void ResetAttackCooldown()
    {
        canAttack = false;
        attackCooldownTimer = 0f;
    }

    #region Health/death Methods
    public int GetHealth()
    {
        return health;
    }

    public void TakeDamage(int damageAmount)
    {
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

    protected void CheckAttackCooldown()
    {
        attackCooldownTimer += Time.deltaTime;
        if (attackCooldownTimer >= AttackCooldown)
            canAttack = true;
    }

    protected abstract void UpdateHealthBar();

    protected void CheckIfDead()
    {
        // Check if enemy still has health, if not then kill the enemy
        if (IsDead())
        {
            if (isDead == false) { Die(); } // Enemy hasn't already died. TODO: Make this cleaner
        }
        else { UpdateHealthBar(); }
    }

    protected void Die()
    {
        isDead = true;
        _animator.SetTrigger("die");
        _rb.velocity = new Vector2(0f, 0f);
        GetComponent<BoxCollider2D>().enabled = false; // disable collisions
        Destroy(transform.GetChild(0).gameObject); // Destroy healthbar
        Destroy(gameObject, DeathAnimationTime); // Destroy when death animation is done
    }
	#endregion
}