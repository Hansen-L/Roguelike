﻿using UnityEngine;
using System;
using Utils;
using System.Collections;

public class Player : MonoBehaviour, IHealth
{
	#region Gameplay constants
	// Health and taking damage
	public const int MaxHealth = 100;
	public const float DamageInvulnDuration = 0.5f; // How long player is invuln for after getting hit

	// Movement
	public const float Acceleration = 1f;
    public const float MaxSpeed = 6f;
    public const float Friction = 0.4f;

	// Dash
	public const float DashSpeed = 20f;
	public const float DashTime = 0.15f;
	public const float DashTrailSpawnRate = 0.03f;
	public const float DashTrailDuration = 0.6f;
	public const float DashCooldown = 0.3f;

	// Attack
	public const float AttackTime = 0.2f;
	public const float ComboWindow = 0.7f; // Time between attacks for combo
	public const int SlashDamage = 10;
	public const int BarkDamage = 30;
	public const float BarkMovementSpeed = 7f; // How much the player moves after barking

	// Boomerang
	public const float BoomerangTime = 0.1f; // Time for animation to play out, and to pause movement inputs
	public const int BoomerangDamage = 15;
	public const int BoomerangExplosionDamage = 10;
	public const float BoomerangStartSpeed = 25f;
	public const float BoomerangStartSpeedShadow = 25.5f;
	public const float BoomerangTorque = 1000f;
	public const float BoomerangSlowdownFactor = 1.5f; // Governs how quickly the boomerang reverses (smaller number means faster reversal)
	public const float BoomerangReturnAcceleration = 1/BoomerangSlowdownFactor; // Affects how fast the boomerang accelerates when returning. Not actual acceleration units though.

	// Swap
	public const float SwapTime = 0.2f;

	public const float BufferWindow = 0.7f; // Buffer window for player combos
	public const float ShadowDelay = 0.5f; // Delay before shadow copies player input
	public const float ShadowSpeedRatio = 0.8f; // What percent of player speed the shadow can move at
	#endregion

	#region Public Non-Constant Variables
	// TODO: Maybe these gameobject references should live in a game manager
	public GameObject mainPlayer; // GameObject that isn't the shadow
	public Camera mainCamera;
	public GameObject healthBar;

	public bool isShadow = false; // If isShadow is true, process inputs with a delay
	public bool shadowCanMove = true;

	public GameObject barkEffect;
	public GameObject slashEffect;
	public ParticleSystem slashParticle;
	public Collider2D barkCollider; // TODO: Reorganize these references to be more robust
	public Collider2D slashCollider;

	public GameObject boomerangPrefab;
	public GameObject boomerangExplosionEffect;
	public Transform projectileFirePoint;
	public GameObject gameOverCanvas;

	public bool canBoomerang = true; // Determines if the player can throw a boomerang
	public bool canDash = true;

	public int comboCount = 0; // Track which hit of combo we are on
	public float comboTimer = 0f;

	public float dashCooldownTimer = 0f;

	public float prevDamageTime = 0f; // Last time player took damage. Used for invuln calculations
	
	public float xInput = 0f;
	public float yInput = 0f;
	public float prevxInput = -1f;
	public float prevyInput = 0f;

	// Used for state transitions
	public bool isAttacking = false;
	public bool isBoomeranging = false;
	public bool isDashing = false;
	public bool isSwapping = false;
	#endregion


	#region Private Non-Constant Variables
	private StateMachine _stateMachine; // Using underscores to note instance variables where it could be ambiguous
	private Animator _animator;
	private SpriteRenderer _spriteRenderer;
	private Rigidbody2D _rb;
	private Collider2D _collider;

	public GameObject dashTrailPrefab;
	public GameObject dashWindPrefab;

	private int baseLayer;

	private float xVelocity;
	private float yVelocity;

	private int health = MaxHealth;

	#endregion

	#region Boolean methods for state transitions
	// Note: When calling these methods, we use IsMoving()() or IsMoving().Invoke()
	public Func<bool> HasMovementInput() => () => (xInput != 0 || yInput != 0);
	public Func<bool> HasVelocity() => () => (_rb.velocity.magnitude > 0.001f);
	public Func<bool> IsMoving() => () => (HasMovementInput()() || HasVelocity()());
	public Func<bool> IsIdle() => () => (xInput == 0 && yInput == 0);
	public Func<bool> IsIdleOrMoving() => () => ( (CurrentState() == StatesEnum.Idle.ToString()) || (CurrentState() == StatesEnum.Moving.ToString()));

	public Func<bool> IsDashing() => () => (isDashing);
	public Func<bool> IsNotDashing() => () => (!isDashing);

	public Func<bool> IsAttacking() => () => (isAttacking);
	public Func<bool> IsNotAttacking() => () => (!isAttacking);

	public Func<bool> IsBoomeranging() => () => (isBoomeranging);
	public Func<bool> IsNotBoomeranging() => () => (!isBoomeranging);

	public Func<bool> IsSwapping() => () => (isSwapping);
	public Func<bool> IsNotSwapping() => () => (!isSwapping);
	#endregion

	private void Start()
	{
		#region Configuring State Machine

		_animator = GetComponent<Animator>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_rb = GetComponent<Rigidbody2D>();
		_collider = GetComponent<Collider2D>();

		_stateMachine = new StateMachine();

		// Instantiating states
		var moving = new Moving(this, _animator);
		var idle = new Idle(this, _animator);
		var dashing = new Dashing(this, _animator, _rb, _collider);
		var attacking = new Attacking(this, _animator, _rb);
		var boomeranging = new Boomeranging(this, _animator, _rb);
		var swapping = new Swapping(this, _animator, _rb);

		// Assigning transitions
		At(moving, idle, IsIdle());
		At(idle, moving, IsMoving());

		// Dashing
		At(idle, dashing, IsDashing());
		At(moving, dashing, IsDashing());
		At(dashing, idle, IsNotDashing());

		// Attacking
		At(idle, attacking, IsAttacking());
		At(moving, attacking, IsAttacking());
		At(attacking, idle, IsNotAttacking());

		// Boomeranging
		At(idle, boomeranging, IsBoomeranging());
		At(moving, boomeranging, IsBoomeranging());
		At(boomeranging, idle, IsNotBoomeranging());

		// Swapping
		_stateMachine.AddAnyTransition(swapping, IsSwapping());
		At(swapping, idle, IsNotSwapping());

		// Starting state
		_stateMachine.SetState(moving);

		// Method to assign transitions easily
		void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);
		#endregion

		#region Instantiating instance variables
		// Base sorting layer
		baseLayer = GetComponent<SpriteRenderer>().sortingOrder;
		#endregion
	}

	private void Update()
    {
		ComboCheck(); // Check if attacks are fast enough to combo
		DashCooldownCheck();
		Utils.Utils.SetRenderLayer(gameObject, baseLayer);
		FlipPlayer();

		// TODO: Make a health check method
		if (!isShadow)
		{
			if (IsDead())
				Die();
			UpdateHealthBar();
		}

		_stateMachine.Tick();
    }

	private void FixedUpdate()
	{
        _stateMachine.FixedTick();
	}

	#region Health functions
	// TODO: Could put this in a separate script?
	public int GetHealth()
	{
		return health;
	}

	// Checks if player is invulnerable. If not, reduces health by damageAmount.
	public void TakeDamage(int damageAmount)
	{
		if (Time.time - prevDamageTime > DamageInvulnDuration) // checking if invulnerability time is up
		{
			AudioManager.Instance.PlayPitch("Hurt", UnityEngine.Random.Range(1.5f, 2f));
			CinemachineImpulseManager.Play("Strong Impulse");
			WhiteFlashManager.FlashWhite(gameObject);

			prevDamageTime = Time.time;
			health -= damageAmount;
		}
		if (health < 0) { health = 0; }
	}

	public void GainHealth(int healthAmount)
	{
		health += healthAmount;
		if (health >= MaxHealth)
			health = MaxHealth;
	}

	public bool IsDead()
	{
		// If health less than 0, return true
		return (health <= 0) ? true : false;
	}

	private void UpdateHealthBar()
	{
		healthBar.transform.localScale = new Vector3(Mathf.Lerp(healthBar.transform.localScale.x, (float)GetHealth() / (float)MaxHealth, 0.3f), 1f);
	}

	private void Die()
	{
		AudioListener.volume = 0.2f;
		Invoke("StopTime", 2f); // Stop time after 2 seconds.
		gameOverCanvas.SetActive(true);
	}
	private void StopTime()
	{
		Time.timeScale = 0;
	}
	#endregion

	#region Methods called every frame regardless of state
	public void SetState(StatesEnum state) // Use the method to set the player state based on the input
	{
		switch (state)
		{
			case StatesEnum.Dashing:
				isDashing = true;
				isAttacking = false;
				isBoomeranging = false;
				break;
			case StatesEnum.Attacking:
				isAttacking = true;
				break;
			case StatesEnum.Boomeranging:
				isBoomeranging = true;
				break;
			case StatesEnum.Swapping:
				isSwapping = true;
				break;
			default:
				break;
		}
	}

	private void DashCooldownCheck()
	{
		dashCooldownTimer -= Time.deltaTime;
		if (dashCooldownTimer < 0)
			canDash = true;
	}

	private void ComboCheck()
	{
		if (comboCount != 0) { comboTimer += Time.deltaTime; } // Start the combo timer once the attack starts

		if (comboTimer >= Player.ComboWindow) // If the player took too long to attack, reset the combo
		{
			comboCount = 0;
			comboTimer = 0;
		}
	}

	private void FlipPlayer()
	{
		if (IsIdleOrMoving()())
		{
			if (_rb.velocity.x > 0) // moving right
				transform.localScale = new Vector3(-1, 1, 1);
			else if (_rb.velocity.x < 0) // moving left
				transform.localScale = new Vector3(1, 1, 1);
			else if (prevxInput > 0) // moving right
				transform.localScale = new Vector3(-1, 1, 1);
			else if (prevxInput < 0) // moving left
				transform.localScale = new Vector3(1, 1, 1);
		}
	}
	#endregion


	#region Methods called from states
	public void StartDashTrailCoroutine() // Necessary so that coroutine can start from states
	{
		StartCoroutine(SpawnDashTrail());
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

	public void ProcessMovement() // Called from moving and idle states
	{
		if (!isShadow) // If this is not the shadow gameobject, process movement as regular
		{
			if (xInput * xVelocity <= 0)
				xVelocity *= Player.Friction;
			if (yInput * yVelocity <= 0)
				yVelocity *= Player.Friction;

			xVelocity = Mathf.Clamp(xVelocity + xInput * Player.Acceleration, -Player.MaxSpeed, Player.MaxSpeed);
			yVelocity = Mathf.Clamp(yVelocity + yInput * Player.Acceleration, -Player.MaxSpeed, Player.MaxSpeed);

			Vector2 total_velocity = new Vector2(xVelocity, yVelocity);
			_rb.velocity = Vector2.ClampMagnitude(total_velocity, Player.MaxSpeed);
		}
	}
	#endregion

	#region Miscellaneous Methods

	public String CurrentState()
	{
		return _stateMachine.CurrentState();
	}

	public Vector2 GetPlayerDir() // Return normalized direction that player is moving towards
	{
		// If the player isn't currently giving a movement input, return the previous direction
		if (xInput == 0 || yInput == 0) { return new Vector2(prevxInput, prevyInput).normalized; }
		else { return new Vector2(xInput, yInput).normalized; }
	}

	public void StartChildCoroutine(IEnumerator coroutineMethod)
	{
		StartCoroutine(coroutineMethod);
	}

	public void SetColorToBlack(GameObject obj)
	{
		if (isShadow == true)
			obj.GetComponent<SpriteRenderer>().color = Color.black;
	}

	public void SetCurrentInput(Vector2 input)
	{
		xInput = input.x;
		yInput = input.y;
	}

	public void SetPrevInput(Vector2 prevInput)
	{
		prevxInput = prevInput.x;
		prevyInput = prevInput.y;
	}

	public bool CanMove() // Check if the player/shadow can move
	{
		return ((isShadow && shadowCanMove) || (!isShadow));
	}

	#endregion

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(barkCollider.bounds.center, barkCollider.bounds.size);
		Gizmos.DrawWireCube(slashCollider.bounds.center, slashCollider.bounds.size);
		//Gizmos.DrawWireCube(new Vector2(-barkCollider.bounds.center.x, barkCollider.bounds.center.y), barkCollider.bounds.size);
		//Gizmos.DrawWireCube(new Vector2(-slashCollider.bounds.center.x, slashCollider.bounds.center.y), slashCollider.bounds.size);
	}
}
