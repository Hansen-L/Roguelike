using UnityEngine;
using System;

// This class is for basic enemies, that move for a set amount of time, then attack if the player is in range. Various transition methods are defined, as well as checking attack cooldown.
// Die() is also implemented here.
public abstract class ABasicEnemy: AEnemy //Enemies that aren't bosses, with simple attack patterns
{
    #region Gameplay Constants
    // These are default values, should all be overriden in inheriting class
    public abstract float IdleTime { get; }
    public abstract float MoveTime { get; }
    public abstract float MoveSpeed { get; }
    public abstract float AttackRange { get; }
    public abstract int AttackDamage { get; }
    public abstract float AttackCooldown { get; }
    #endregion

    protected float attackCooldownTimer = 0f;

    #region Boolean methods for state transitions
	public bool isMoving = false;
    public bool isAttacking = false;
    public bool attackCooldownReady = true;

    // Note: When calling these methods, we use IsMoving()() or IsMoving().Invoke()
    public Func<bool> IsMoving() => () => (isMoving);
    public Func<bool> IsIdle() => () => (!isMoving);

    public Func<bool> IsInRange() => () => (Vector2.Distance(_rb.position, GameManager.GetMainPlayerRb().position) < AttackRange);
    public Func<bool> IsInRangeAndAttackReady() => () => (IsInRange()() && attackCooldownReady);

    public Func<bool> IsAttacking() => () => (isAttacking);
    public Func<bool> IsNotAttacking() => () => (!isAttacking);
    #endregion

    public void ResetAttackCooldown()
    {
        attackCooldownReady = false;
        attackCooldownTimer = 0f;
    }

    #region Health/death Methods
    protected void CheckAttackCooldown()
    {
        attackCooldownTimer += Time.deltaTime;
        if (attackCooldownTimer >= AttackCooldown)
            attackCooldownReady = true;
    }

    protected override void Die()
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