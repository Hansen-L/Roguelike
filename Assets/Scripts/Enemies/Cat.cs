using UnityEngine;
using System;
using Utils;
using System.Collections;

public class Cat : ADashAttackEnemy
{
	#region Gameplay Constants
	// Abstract class overrides
	public override int MaxHealth { get { return 100; } }
	public override float DeathAnimationTime { get { return 0.84f; } }

	public override float IdleTime { get { return 2f; } }
	public override float MoveTime { get { return 4f; } } // How long enemy moves for
	public override float MoveSpeed { get { return 1f; } }
	public override float AttackRange { get { return 3f; } }
	public override float AttackCooldown { get { return 2f; } }
	public override int AttackDamage { get { return 10; } }

	public override float DashChargeTime { get { return 0.5f; } }
	public override float DashTime { get { return 0.5f; } }
	public override float DashSpeed { get { return 7f; } }
	#endregion

	public GameObject healthBar;

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

		_stateMachine.AddAnyTransition(enemyDashAttacking, IsInRangeAndAttackReady());
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
		if (!isDead)
		{
			_stateMachine.Tick();
			CheckIfDead();
			CheckAttackCooldown();
		}
	}

	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		bool hasDoneDamage = false; // Only hit player once
		if (hitboxActive && !hasDoneDamage) // Enable hitbox when attacking
		{
			Player playerScript = otherCollider.gameObject.GetComponent<Player>();
			if (playerScript != null && !playerScript.isShadow) // If we hit th emain player
				playerScript.TakeDamage(AttackDamage);
		}
	}

	#region Health methods

	protected override void UpdateHealthBar()
	{
		healthBar.transform.localScale = new Vector3(Mathf.Lerp(healthBar.transform.localScale.x, (float)GetHealth() / (float)MaxHealth, 0.7f), 1f);
	}
	#endregion

	//private void OnDrawGizmos()
	//{
	//	Gizmos.DrawWireSphere(this.transform.position, AttackRange);
	//}
}
