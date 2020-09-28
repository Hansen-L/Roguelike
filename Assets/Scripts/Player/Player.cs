using UnityEngine;
using System;
using Utils;

public class Player : MonoBehaviour 
{
    #region Constants
	// Movement
    public const float acceleration = 1f;
    public const float maxSpeed = 7f;
    public const float friction = 0.4f;

	// Attack
	public const float attackTime = 0.35f;
	public const float comboWindow = 0.7f; // Time between attacks for combo
	public const float bufferWindow = 0.5f; // Buffer window for player combos
	public const int slashDamage = 10;
	public const int barkDamage = 30;
	#endregion

	#region Non-Constant Variables
	public GameObject barkEffect;
	public GameObject slashEffect;
	public ParticleSystem slashParticle;
	public Collider2D barkCollider; // TODO: Reorganize these references to be more robust
	public Collider2D slashCollider;

	public float xInput = 0f;
    public float yInput = 0f;
	public float prevxInput = 1f;
    public float prevyInput = 1f;

	public int comboCount = 0; // Track which hit of combo we are on
	public float comboTimer = 0f;

	public bool isAttacking = false;

    private StateMachine _stateMachine; // Using underscores to note instance variables where it could be ambiguous
	private Animator _animator;
	private SpriteRenderer _spriteRenderer;
	private Rigidbody2D _rb;

	private int baseLayer;

	private float xVelocity;
	private float yVelocity;
	#endregion

	private void Start()
	{
		#region Configuring State Machine

		_animator = GetComponent<Animator>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_rb = GetComponent<Rigidbody2D>();

		_stateMachine = new StateMachine();

		// Instantiating states
		var running = new Running(this, _animator);
		var idle = new Idle(this, _animator);
		var attacking = new Attacking(this, _animator, _rb, _spriteRenderer);

		// Assigning transitions
		At(running, idle, IsIdle());
		At(idle, running, IsMoving());
		At(idle, attacking, IsAttacking());
		At(running, attacking, IsAttacking());
		At(attacking, idle, IsNotAttacking());

		// Starting state
		_stateMachine.SetState(running);

		// Method to assign transitions easily
		void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);

		// Transition conditions
		Func<bool> IsAttacking() => () => (isAttacking);
		Func<bool> IsNotAttacking() => () => (!isAttacking);
		Func<bool> IsMoving() => () => (xInput != 0 || yInput != 0);
		Func<bool> IsIdle() => () => (xInput == 0 && yInput == 0);
		#endregion

		#region Instantiating instance variables
		// Base sorting layer
		baseLayer = GetComponent<SpriteRenderer>().sortingOrder;
		#endregion
	}

	private void Update()
    {
		ComboCheck(); // Check if attacks are fast enough to combo
		Utils.Utils.SetRenderLayer(gameObject, baseLayer);
		FlipPlayer();

		_stateMachine.Tick();
    }

	private void FixedUpdate()
	{
        _stateMachine.FixedTick();
	}


	#region Methods called every frame regardless of state

	public void ComboCheck()
	{
		if (comboCount != 0) { comboTimer += Time.deltaTime; } // Start the combo timer once the attack starts

		if (comboTimer >= Player.comboWindow) // If the player took too long to attack, reset the combo
		{
			comboCount = 0;
			comboTimer = 0;
		}
	}

	public void FlipPlayer()
	{
		if (_rb.velocity.x > 0) // moving right
			{ transform.localScale = new Vector3(-1, 1, 1); }
		else if (_rb.velocity.x < 0) // moving left
			{ transform.localScale = new Vector3(1, 1, 1); }
	}

	public void SetState(StatesEnum input) // Use the method to set the player state based on the input
	{
		switch (input)
		{
			case StatesEnum.Attacking:
				isAttacking = true;
				break;
			default:
				break;
		}
	}

	//public void ClampPosition()
	//{
	//	Vector2 position;
	//	position.x = Mathf.Clamp(this.transform.position.x, -xBoundary, xBoundary);
	//	position.y = Mathf.Clamp(this.transform.position.y, -yBoundary, yBoundary);
	//	this.transform.position = position;
	//}

	#endregion


	#region Methods called from states

	public void ProcessMovement() // Called from running and idle states
	{
		if (xInput * xVelocity <= 0)
		{
			xVelocity *= Player.friction;
		}
		if (yInput * yVelocity <= 0)
		{
			yVelocity *= Player.friction;
		}
		xVelocity = Mathf.Clamp(xVelocity + xInput * Player.acceleration, -Player.maxSpeed, Player.maxSpeed);
		yVelocity = Mathf.Clamp(yVelocity + yInput * Player.acceleration, -Player.maxSpeed, Player.maxSpeed);

		Vector2 total_velocity = new Vector2(xVelocity, yVelocity);

		_rb.velocity = Vector2.ClampMagnitude(total_velocity, Player.maxSpeed);
	}

	#endregion

	#region Miscellaneous Methods

	public String CurrentState()
	{
		return _stateMachine.CurrentState();
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
