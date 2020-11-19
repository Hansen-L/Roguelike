using UnityEngine;
using System;
using Utils;

public class SheepBoss : AEnemy
{
	#region Gameplay Constants
	public override int MaxHealth { get { return 2000; } }
	public override float DeathAnimationTime { get { return 0.84f; } }

	public float IdleTime { get { return 1f; } }

	public float MoveTime { get { return 4f; } }
	public float MoveSpeed { get { return 2.5f; } }

	public float ScatterProjectileChargeTime { get { return 1f; } }
	public float ScatterProjectileAnimationTime { get { return 1.85f; } }
	public float ScatterProjectileSpeed { get { return 8f; } }
	public int ScatterProjectileNumber { get { return 45; } }
	public int ScatterProjectileDamage { get { return 15; } }
	public float ScatterRange { get { return 40f; } } // Range of angles that the scatter fires in

	public float BouncyProjectileSpeed { get { return 4f; } }
	public int BouncyProjectileNumber { get { return 20; } }
	public int BouncyProjectileDamage { get { return 15; } }
	public int BouncyProjectileBounces { get { return 1; } }

	public float DashChargeTime { get { return 0.3f; } }
	public float DashTime { get { return 0.5f; } }
	public float DashSpeed { get { return 7f; } }
	public int DashDamage { get { return 20; } }
	#endregion

	private Collider2D _collider;
	public GameObject bouncyProjectilePrefab;
	public GameObject scatterProjectilePrefab;

	public bool dashHitboxActive = false;

	public String prevState = SheepBossStatesEnum.SheepMoving.ToString();

	#region Boolean methods for state transitions
	public bool isMoving = true;
	public bool isProjectiling = false;
	public bool isDashing = false;

	// Note: When calling these methods, we use IsMoving()() or IsMoving().Invoke()
	public Func<bool> IsMoving() => () => (isMoving);
	public Func<bool> IsNotMoving() => () => (!isMoving);
	public Func<bool> IsProjectiling() => () => (isProjectiling);
	public Func<bool> IsNotProjectiling() => () => (!isProjectiling);
	public Func<bool> IsDashing() => () => (isDashing);
	public Func<bool> IsNotDashing() => () => (!isDashing);

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

		var sheepCentralState = new SheepCentralState(this);
		var sheepMoving = new SheepMoving(this, _animator, _rb);
		var sheepProjectiling = new SheepProjectiling(this, _animator, _rb);
		var sheepDashing = new SheepDashAttacking(this, _animator, _rb);

		//// Assigning transitions
		At(sheepCentralState, sheepProjectiling, IsProjectiling());
		At(sheepCentralState, sheepMoving, IsMoving());
		At(sheepCentralState, sheepDashing, IsDashing());

		At(sheepProjectiling, sheepCentralState, IsNotProjectiling());
		At(sheepMoving, sheepCentralState, IsNotMoving());
		At(sheepDashing, sheepCentralState, IsNotDashing());

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
			UpdatePreviousState();
		}
	}

	private void OnTriggerEnter2D(Collider2D otherCollider) // To damage the player when the sheep dashes
	{
		bool hasDoneDamage = false; // Only hit player once
		if (dashHitboxActive && !hasDoneDamage) // Enable hitbox when attacking
		{
			Player playerScript = otherCollider.gameObject.GetComponent<Player>();
			if (playerScript != null && !playerScript.isShadow) // If we hit th emain player
				playerScript.TakeDamage(DashDamage);
		}
	}

	public void PickNextState() // Randomly pick next state
	{
		if (!isMoving && !isDashing && !isProjectiling) // Only pick next state if we haven't set one already
		{
			float rand = UnityEngine.Random.Range(0, 1f);

			if (rand < 0.5)
				isMoving = true;
			else
				isProjectiling = true;
		}
	}

	protected override void Die()
	{
		isDead = true;
		AudioManager.Instance.Stop("Stomp");
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

	public void LaunchProjectilesScatter() // Launch a scatter of projectiles at the player
	{
		Vector2 playerDir = (GameManager.GetMainPlayerRb().position - _rb.position).normalized;

		int count = 0;
		while (count < ScatterProjectileNumber)
		{
			count += 1;
			float angle = UnityEngine.Random.Range(-ScatterRange, ScatterRange); // Add some randomness to angles
			Vector2 projectileDir = (Quaternion.AngleAxis(angle, Vector3.forward) * playerDir).normalized;

			GameObject scatterProjectileObject = GameObject.Instantiate(scatterProjectilePrefab, _rb.position, Quaternion.identity);
			SheepScatterProjectile projectileScript = scatterProjectileObject.GetComponent<SheepScatterProjectile>();
			Rigidbody2D projectileRb = scatterProjectileObject.GetComponent<Rigidbody2D>();

			projectileScript.SetEnemy(this);
			projectileRb.velocity = ScatterProjectileSpeed * projectileDir;
			projectileRb.angularVelocity = UnityEngine.Random.Range(100f, 200f);
		}
	}

	public void LaunchBouncyProjectiles() // Launch a circle of projectiles that bounce on walls
	{
		Vector2 vec = new Vector2(1, 0); // Unit vector to use for rotations
										 // Launch projectiles in a circle
		int count = 0;
		while (count < BouncyProjectileNumber)
		{
			count += 1;
			float angle = 360 / BouncyProjectileNumber * count;
			//angle += UnityEngine.Random.Range(-5f, 5f); // Add some randomness to angles
			Vector2 projectileDir = (Quaternion.AngleAxis(angle, Vector3.forward) * vec).normalized;

			GameObject bouncyProjectileObject = GameObject.Instantiate(bouncyProjectilePrefab, _rb.position, Quaternion.identity);
			SheepBouncyProjectile projectileScript = bouncyProjectileObject.GetComponent<SheepBouncyProjectile>();
			Rigidbody2D projectileRb = bouncyProjectileObject.GetComponent<Rigidbody2D>();

			projectileScript.SetEnemy(this);
			projectileRb.velocity = BouncyProjectileSpeed * projectileDir;
			projectileRb.angularVelocity = UnityEngine.Random.Range(500f, 1000f);
		}
	}

	private void UpdatePreviousState() // Keeps track of previous state
	{
		string curState = _stateMachine.CurrentState().ToString();
		if (curState != prevState)
			prevState = curState;
	}
}
