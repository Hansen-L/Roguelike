using UnityEngine;
using System;
using Utils;
using System.Collections;

public class Cat : AEnemy
{
	#region Gameplay Constants
	// These override properties in AEnemy
	public override int MaxHealth { get { return 100; } }
	public override float DeathAnimationTime { get { return 0.84f; } }

	public override float IdleTime { get { return 2f; } }
	public override float MoveTime { get { return 4f; } } // How long enemy moves for
	public override float MoveSpeed { get { return 1f; } }
	public override float AttackRange { get { return 3f; } }

	// Cat specific constants
	public float ChargeTime { get { return 0.5f; } }
	public float DashTime { get { return 0.5f; } }
	public float DashSpeed { get { return 7f; } }
	public float AttackCooldown { get { return 1f; } }
	#endregion


	#region Boolean methods for state transitions
	// Note: When calling these methods, we use IsMoving()() or IsMoving().Invoke()
	public Func<bool> IsMoving() => () => (isMoving);
	public Func<bool> IsIdle() => () => (!isMoving);
	
	public Func<bool> IsInRange() => () => (Vector2.Distance(_rb.position, GameManager.GetMainPlayerRb().position) < AttackRange);
	public Func<bool> IsInRangeAndCanAttack() => () => (IsInRange()() && canAttack);

	public Func<bool> IsAttacking() => () => (isAttacking);
	public Func<bool> IsNotAttacking() => () => (!isAttacking);
	#endregion

	public GameObject healthBar;
	public float attackCooldownTimer = 0f;


	// Start is called before the first frame update
	void Start()
	{
		#region Configuring State Machine

		_animator = GetComponent<Animator>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_rb = GetComponent<Rigidbody2D>();
		_stateMachine = new StateMachine();

		var randomMoving = new RandomMoving(this, _animator, _rb);
		var enemyIdle = new EnemyIdle(this, _animator, _rb);
		var enemyDashAttacking = new EnemyDashAttacking(this, _animator, _rb);

		// Assigning transitions
		At(randomMoving, enemyIdle, IsIdle());
		At(enemyIdle, randomMoving, IsMoving());

		_stateMachine.AddAnyTransition(enemyDashAttacking, IsInRangeAndCanAttack());
		At(enemyDashAttacking, enemyIdle, IsNotAttacking());

		// Starting state
		_stateMachine.SetState(enemyIdle);

		// Method to assign transitions easily
		void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);
		#endregion

		health = MaxHealth;
	}

	// Update is called once per frame
	void Update()
	{
		_stateMachine.Tick();
		CheckIfDead();
		CheckAttackCooldown();
	}

	private void CheckAttackCooldown()
	{
		attackCooldownTimer += Time.deltaTime;
		if (attackCooldownTimer >= AttackCooldown)
			canAttack = true;
	}

	public void ResetAttackCooldown()
	{
		canAttack = false;
		attackCooldownTimer = 0f;
	}

	#region Health methods
	protected override void CheckIfDead()
	{
		// Check if enemy still has health, if not then kill the enemy
		if (IsDead())
		{
			if (isDead == false) { Die(); } // Enemy hasn't already died. TODO: Make this cleaner
		}
		else { UpdateHealthBar(); }
	}

	protected override void Die()
	{
		isDead = true;
		_animator.SetTrigger("die");
		_rb.velocity = new Vector2(0f, 0f);
		GetComponent<BoxCollider2D>().enabled = false; // disable collisions
		Destroy(transform.GetChild(0).gameObject);
		Destroy(gameObject, DeathAnimationTime); // Destroy when death animation is done
	}

	private void UpdateHealthBar()
	{
		healthBar.transform.localScale = new Vector3(Mathf.Lerp(healthBar.transform.localScale.x, (float)GetHealth() / (float)MaxHealth, 0.7f), 1f);
	}
	#endregion

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(this.transform.position, AttackRange);
	}
}
