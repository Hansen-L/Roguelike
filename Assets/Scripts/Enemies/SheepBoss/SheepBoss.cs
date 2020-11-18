using UnityEngine;
using System;
using Utils;

public class SheepBoss : AEnemy
{
	#region Gameplay Constants
	public override int MaxHealth { get { return 200; } }
	public override float DeathAnimationTime { get { return 0.84f; } }

	public float IdleTime { get { return 1f; } }

	public float MoveTime { get { return 4f; } }
	public float MoveSpeed { get { return 2.5f; } }

	public float ProjectileChargeTime { get { return 1.33f; } }
	public float ProjectileSpeed { get { return 6f; } }
	public int ProjectileNumber { get { return 30; } }
	public int ProjectileDamage { get { return 15; } }
	public int ProjectileBounces { get { return 1; } }

	public float DashChargeTime { get { return 0.3f; } }
	public float DashTime { get { return 0.5f; } }
	public float DashSpeed { get { return 7f; } }
	public int DashDamage { get { return 20; } }
	#endregion

	private Collider2D _collider;
	public GameObject projectilePrefab;

	public bool hitboxActive = false;


	#region Boolean methods for state transitions
	public bool isMoving = false;
	public bool isProjectiling = false;

	// Note: When calling these methods, we use IsMoving()() or IsMoving().Invoke()
	public Func<bool> IsMoving() => () => (isMoving);
	public Func<bool> IsProjectiling() => () => (isProjectiling);

	//public Func<bool> IsAttacking() => () => (isAttacking);
	#endregion

	// Start is called before the first frame update
	void Start()
	{
		#region Configuring State Machine

		_animator = GetComponent<Animator>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_rb = GetComponent<Rigidbody2D>();
		_collider = GetComponent<Collider2D>();
		_stateMachine = new StateMachine();

		var sheepMoving = new SheepMoving(this, _animator, _rb);
		var sheepProjectiling = new SheepProjectiling(this, _animator, _rb);

		//// Assigning transitions
		At(sheepMoving, sheepProjectiling, IsProjectiling());
		At(sheepProjectiling, sheepMoving, IsMoving());

		//// Starting state
		_stateMachine.SetState(sheepMoving);

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
			FlipSprite();
		}
	}

	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		bool hasDoneDamage = false; // Only hit player once
		if (hitboxActive && !hasDoneDamage) // Enable hitbox when attacking
		{
			Player playerScript = otherCollider.gameObject.GetComponent<Player>();
			if (playerScript != null && !playerScript.isShadow) // If we hit th emain player
				playerScript.TakeDamage(DashDamage);
		}
	}

	protected override void Die()
	{
		isDead = true;
		_animator.SetTrigger("die");
		_rb.velocity = new Vector2(0f, 0f);
		GetComponent<Collider2D>().enabled = false; // disable collisions
		Destroy(transform.GetChild(0).gameObject); // Destroy healthbar
		Destroy(gameObject, DeathAnimationTime); // Destroy when death animation is done
	}

	private void FlipSprite()
	{
		float mag = Mathf.Abs(transform.localScale.x); // Magnitude of localscale
		if (_rb.velocity.x > 0) // moving right
		{
			transform.localScale = new Vector3(-mag, mag, 1);
			transform.GetChild(0).transform.localScale = new Vector3(-1, 1, 1); // Flipping the health bar
		}
		else if (_rb.velocity.x < 0) // moving left
		{
			transform.localScale = new Vector3(mag, mag, 1);
			transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1);
		}
	}

	public void LaunchProjectile()
	{
		Vector2 vec = new Vector2(1, 0); // Unit vector to use for rotations
		// Launch projectiles in a circle
		int count = 0;
		while (count < ProjectileNumber)
		{
			count += 1;
			float angle = 360 / ProjectileNumber * count;
			angle += UnityEngine.Random.Range(-5f, 5f); // Add some randomness to angles
			Vector2 projectileDir = (Quaternion.AngleAxis(angle, Vector3.forward) * vec).normalized;

			GameObject projectileObject = GameObject.Instantiate(projectilePrefab, _rb.position, Quaternion.identity);
			SheepBouncyProjectile projectileScript = projectileObject.GetComponent<SheepBouncyProjectile>();
			Rigidbody2D projectileRb = projectileObject.GetComponent<Rigidbody2D>();

			projectileScript.SetEnemy(this);
			projectileRb.velocity = ProjectileSpeed * projectileDir;
			projectileRb.angularVelocity = UnityEngine.Random.Range(5f, 10f);
		}
	}
}
