using UnityEngine;
using System;
using Utils;
using System.Collections;

public class SheepBoss : AEnemy
{
	#region Gameplay Constants
	public override int MaxHealth { get { return 1000; } }
	public int Phase1Health { get { return 5; } }
	public int Phase2Health { get { return MaxHealth - Phase1Health; } }

	public override float DeathAnimationTime { get { return 0.84f; } }

	public float IdleTime { get { return 1f; } }

	public float MoveTime { get { return 4f; } }
	public float MoveSpeed { get { return 2.5f; } }

	public float ScatterProjectileChargeTime { get { return 1f; } }
	public float ScatterProjectileAnimationTime { get { return 1.85f; } }
	public float ScatterProjectileSpeed { get { return 6f; } }
	public int ScatterProjectileDamage { get { return 15; } }
	public float ScatterRange { get {  // Range of angles that the scatter fires in
			if (curPhase == 1)
				return 40f;
			return 60f;
		} }
	public int ScatterProjectileNumber { get {
			if (curPhase == 1)
				return 45;
			return 70;
		} }

	public float BouncyProjectileSpeed { get { return 3f; } }
	public int BouncyProjectileNumber { get { return 20; } }
	public int BouncyProjectileDamage { get { return 15; } }
	public int BouncyProjectileBounces { get { return 1; } }

	public float DashChargeTime { get { return 1f; } }
	public float DashTime { get { return 1f; } }
	public float DashSpeed { get { return 16f; } }
	public int DashDamage { get { return 20; } }
	public float DashTrailSpawnRate { get { return 0.15f; } } // How often to spawn the trail effect
	public float DashTrailDuration { get { return 0.6f; } }

	public float LaunchExplodingSheepChargeTime { get { return 1f; } }
	public float LaunchExplodingSheepShootingTime { get { return 5f; } } // How long it takes to shoot out all exploding sheep
	public int NumExplodingSheepLaunched { get { return 6; } }
	public float ExplodingSheepLaunchSpeed { get { return 16f; } }
	// For small exploding sheep
	public float ExplodingSheepTimeBeforeExploding { get { return 10f; } }
	public int ExplodingSheepBouncyProjectileNumber { get { return 10; } }
	#endregion

	private Collider2D _collider;
	public GameObject bouncyProjectilePrefab;
	public GameObject scatterProjectilePrefab;
	public GameObject explodingSheepPrefab;
	public GameObject dashTrailPrefab;
	public GameObject angryVeinParticle;

	public bool dashHitboxActive = false;

	public String prevState = SheepBossStatesEnum.SheepMoving.ToString();
	public SheepBossStatesEnum? nextState = null;

	public int curPhase = 1; // Boss phase. Should be 1 or 2.

	#region Boolean methods for state transitions
	public bool isMoving = true;
	public bool isProjectiling = false;
	public bool isDashing = false;
	public bool isLaunchingExplodingSheep = false;

	// Note: When calling these methods, we use IsMoving()() or IsMoving().Invoke()
	public Func<bool> IsMoving() => () => (isMoving);
	public Func<bool> IsNotMoving() => () => (!isMoving);
	public Func<bool> IsProjectiling() => () => (isProjectiling);
	public Func<bool> IsNotProjectiling() => () => (!isProjectiling);
	public Func<bool> IsDashing() => () => (isDashing);
	public Func<bool> IsNotDashing() => () => (!isDashing);
	public Func<bool> IsLaunchingExplodingSheep() => () => (isLaunchingExplodingSheep);
	public Func<bool> IsNotLaunchingExplodingSheep() => () => (!isLaunchingExplodingSheep);

	// Only becomes true once, when we switch phases.
	public Func<bool> IsPhaseChanging() => () => ((health <= (MaxHealth - Phase1Health)) && (curPhase == 1));

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
		var sheepLaunchingExplodingSheep = new SheepLaunchingExplodingSheep(this, _animator, _rb);
		var sheepPhaseChangeState = new SheepPhaseChangeState(this, _animator, _spriteRenderer);

		//// Assigning transitions
		At(sheepCentralState, sheepProjectiling, IsProjectiling());
		At(sheepCentralState, sheepMoving, IsMoving());
		At(sheepCentralState, sheepDashing, IsDashing());
		At(sheepCentralState, sheepLaunchingExplodingSheep, IsLaunchingExplodingSheep());

		At(sheepProjectiling, sheepCentralState, IsNotProjectiling());
		At(sheepMoving, sheepCentralState, IsNotMoving());
		At(sheepDashing, sheepCentralState, IsNotDashing());
		At(sheepLaunchingExplodingSheep, sheepCentralState, IsNotLaunchingExplodingSheep());

		// Can go into phase change state from any state. Only triggers once.
		_stateMachine.AddAnyTransition(sheepPhaseChangeState, IsPhaseChanging());
		At(sheepPhaseChangeState, sheepLaunchingExplodingSheep, IsLaunchingExplodingSheep()); 

		//// Starting state
		_stateMachine.SetState(sheepMoving);

		// Method to assign transitions easily
		void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
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

	public void PickNextState() // If we have set a nextState, activate it. Otherwise, pick randomly. Set the nextstate by using sheep.nextState = ...
	{
		if (nextState != null)
		{
			if (nextState == SheepBossStatesEnum.SheepDashAttacking)
				isDashing = true;
			else if (nextState == SheepBossStatesEnum.SheepProjectiling)
				isProjectiling = true;
			else if (nextState == SheepBossStatesEnum.SheepMoving)
				isMoving = true;
			else if (nextState == SheepBossStatesEnum.SheepLaunchingExplodingSheep)
				isLaunchingExplodingSheep = true;
			nextState = null;
		}
		else if (!isMoving && !isDashing && !isProjectiling && !isLaunchingExplodingSheep) // Only pick next state if we haven't set one already
		{
			if (curPhase == 1)
			{
				float rand = UnityEngine.Random.Range(0, 1f);
				if (rand < 0.5)
					isMoving = true;
				else
					isProjectiling = true;
			}
			else if (curPhase == 2)
			{
				float rand = UnityEngine.Random.Range(0, 1f);
				if (prevState == SheepBossStatesEnum.SheepProjectiling.ToString()) // Projectile can loop or go into launchingExplodingSheep
				{
					if (rand < 0.5f)
						isProjectiling = true;
					else
						isLaunchingExplodingSheep = true;
				}
				else if (prevState == SheepBossStatesEnum.SheepDashAttacking.ToString())
				{
					if (rand < 0.5f)
						isDashing = true;
					else
						isProjectiling = true;
				}
				else
				{
					if (rand < 0.5f)
						isMoving = true;
					else
						isProjectiling = true;
				}
			}
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
		float safeAngle1 = UnityEngine.Random.Range(-ScatterRange, ScatterRange);
		float safeAngle2 = UnityEngine.Random.Range(-ScatterRange, ScatterRange);

		int count = 0;
		while (count < ScatterProjectileNumber)
		{
			count += 1;
			//float angle = ScatterRange * (count - ScatterProjectileNumber / 2) / (ScatterProjectileNumber/2);
			float angle = UnityEngine.Random.Range(-ScatterRange, ScatterRange); // Add some randomness to angles
			if (Mathf.Abs(angle - safeAngle1) < 5f  ||  Mathf.Abs(angle - safeAngle2) < 5f) // Leave a spot for the player to pass through
				continue;
			Vector2 projectileDir = (Quaternion.AngleAxis(angle, Vector3.forward) * playerDir).normalized;

			GameObject scatterProjectileObject = GameObject.Instantiate(scatterProjectilePrefab, _rb.position, Quaternion.identity);
			SheepScatterProjectile projectileScript = scatterProjectileObject.GetComponent<SheepScatterProjectile>();
			Rigidbody2D projectileRb = scatterProjectileObject.GetComponent<Rigidbody2D>();

			projectileScript.SetEnemy(this);
			projectileRb.velocity = ScatterProjectileSpeed * projectileDir;
			projectileRb.angularVelocity = UnityEngine.Random.Range(100f, 200f);
		}
	}

	// Launch a circle of projectiles that bounce on walls. Launches at boss location by default, but we can override it.
	public void LaunchBouncyProjectiles(Vector3? launchPosition = null)
	{
		if (launchPosition == null) // Default to sheep boss location
			launchPosition = transform.position;

		Vector2 vec = new Vector2(1, 0); // Unit vector to use for rotations
										 // Launch projectiles in a circle
		int count = 0;
		while (count < BouncyProjectileNumber)
		{
			count += 1;
			float angle = 360 / BouncyProjectileNumber * count;
			//angle += UnityEngine.Random.Range(-5f, 5f); // Add some randomness to angles
			Vector2 projectileDir = (Quaternion.AngleAxis(angle, Vector3.forward) * vec).normalized;

			GameObject bouncyProjectileObject = GameObject.Instantiate(bouncyProjectilePrefab, (Vector3)launchPosition, Quaternion.identity);
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

	public IEnumerator SpawnDashTrail()
	{
		int numTrails = (int)(DashTime / DashTrailSpawnRate);
		for (int i = 0; i < numTrails; i++)
		{
			GameObject dashTrailObject = GameObject.Instantiate(dashTrailPrefab, this.transform.position, Quaternion.identity);
			Destroy(dashTrailObject, DashTrailDuration);
			dashTrailObject.transform.localScale = this.transform.localScale;
			yield return new WaitForSeconds(DashTrailSpawnRate);
		}
	}

	public IEnumerator LaunchExplodingSheep()
	{
		Vector2 vec = new Vector2(1, 0); // Unit vector to use for rotations

		float timeBetweenLaunches = LaunchExplodingSheepShootingTime / NumExplodingSheepLaunched;
		for (int i = 0; i < NumExplodingSheepLaunched; i++)
		{
			AudioManager.Instance.PlayOneShot("Throw");
			GameObject explodingSheepObject = GameObject.Instantiate(explodingSheepPrefab, this.transform.position, Quaternion.identity);
			// Randomize launch direction
			float angle = UnityEngine.Random.Range(0f, 360f);
			Vector2 launchDir = (Quaternion.AngleAxis(angle, Vector3.forward) * vec).normalized;
			explodingSheepObject.GetComponent<Rigidbody2D>().velocity = launchDir * ExplodingSheepLaunchSpeed * UnityEngine.Random.Range(1f, 1.5f);
			yield return new WaitForSeconds(timeBetweenLaunches);
		}
	}
}
