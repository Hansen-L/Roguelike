using UnityEngine;
using Utils;
using System;
using System.Collections;

public class BrownWolf : AProjectileEnemy
{
	#region Gameplay Constants
	// These override properties in AEnemy
	public override int MaxHealth { get { return 100; } }
	public override float DeathAnimationTime { get { return 0.84f; } }

	public override float IdleTime { get { return 2f; } }
	public override float MoveTime { get { return 4f; } } // How long enemy moves for
	public override float MoveSpeed { get { return 1f; } }
	public override float AttackRange { get { return 6f; } }
	public override float AttackCooldown { get { return 4f; } }
	public override int AttackDamage { get { return 10; } }

	public override float ProjectileChargeTime { get { return 0.5f; } }
	public override float ProjectileDuration { get { return 0.5f; } }
	public override float ProjectileSpeed { get { return 3f; } }

	public int ProjectileNumber { get { return 3; } } // How many projectiles to shoot in succession for brown wolf
	public float DelayBetweenProjectiles { get { return 0.3f; } }
	public float SpreadAngle { get { return 20f; } } // Range that projectiles can spread
	#endregion

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
		var enemyProjectiling = new EnemyProjectiling(this, _animator, _rb);

		// Assigning transitions
		At(randomMoving, enemyIdle, IsIdle());
		At(enemyIdle, randomMoving, IsMoving());

		_stateMachine.AddAnyTransition(enemyProjectiling, IsInRangeAndAttackReady());
		At(enemyProjectiling, enemyIdle, IsNotAttacking());

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
			FlipSprite();
		}
	}

	private void FlipSprite()
	{
		if (_rb.velocity.x > 0) // moving right
		{
			transform.localScale = new Vector3(1, 1, 1);
			transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1); // Flipping the health bar
		}
		else if (_rb.velocity.x < 0) // moving left
		{
			transform.localScale = new Vector3(-1, 1, 1);
			transform.GetChild(0).transform.localScale = new Vector3(-1, 1, 1);
		}
	}

	public override void LaunchProjectile(Vector2 attackDirection)
	{
		StartCoroutine(LaunchMultipleProjectiles(attackDirection));
	}

	private IEnumerator LaunchMultipleProjectiles(Vector2 attackDirection)
	{
		int count = 0;
		while (count < ProjectileNumber)
		{
			count += 1;
			Vector2 rotatedAttackDirection = rotateVector(attackDirection, UnityEngine.Random.Range(-SpreadAngle, SpreadAngle));
			Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * rotatedAttackDirection; // Adding random rotation to spread
			Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

			GameObject projectileObject = GameObject.Instantiate(projectilePrefab, projectileFirePoint.position, targetRotation);
			EnemyProjectile projectileScript = projectileObject.GetComponent<EnemyProjectile>();
			Rigidbody2D projectileRb = projectileObject.GetComponent<Rigidbody2D>();

			projectileScript.SetEnemy(this);
			projectileRb.velocity = ProjectileSpeed * rotatedAttackDirection;

			yield return new WaitForSeconds(DelayBetweenProjectiles);
		}
	}

	private static Vector2 rotateVector(Vector2 v, float delta)
	{
		delta = Mathf.Deg2Rad*delta;
		return new Vector2(
			v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
			v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
		);
	}

	#region Health methods
	#endregion

	//private void OnDrawGizmos()
	//{
	//	Gizmos.DrawWireSphere(this.transform.position, AttackRange);
	//}
}
