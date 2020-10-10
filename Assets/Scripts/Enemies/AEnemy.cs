using UnityEngine;

public abstract class AEnemy: MonoBehaviour, IHealth
{
    #region Gameplay Constants
    // These are default values, should all be overriden in inheriting class
    public abstract int MaxHealth { get; }
    public abstract float DeathAnimationTime { get; }

    public abstract float IdleTime { get; }
    public abstract float MoveTime { get; }
    public abstract float MoveSpeed { get; }
    public abstract float AttackRange { get; }
    #endregion

    protected StateMachine _stateMachine; // Using underscores to note instance variables where it could be ambiguous
	protected Animator _animator;
	protected SpriteRenderer _spriteRenderer;
	protected Rigidbody2D _rb;

    protected int health = 0;

	public bool isDead = false;
	public bool isMoving = false;
    public bool isAttacking = false;
    public bool canAttack = true;


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

    protected abstract void CheckIfDead();

    protected abstract void Die();
}