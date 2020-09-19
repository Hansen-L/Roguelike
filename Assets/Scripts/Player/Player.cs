using UnityEngine;
using System;
using Utils;

public class Player : MonoBehaviour 
{
    #region Constants
    public const float acceleration = 1f;
    public const float maxSpeed = 7f;
    public const float friction = 0.4f;
	#endregion

    public float xInput = 0f;
    public float yInput = 0f;
	public float prevxInput = 1f;
    public float prevyInput = 1f;

	public bool isAttacking = false;

    private StateMachine _stateMachine;
	private Animator _animator;
	private SpriteRenderer _spriteRenderer;
	private Rigidbody2D _rb;

	private int baseLayer;

	private float xVelocity;
	private float yVelocity;

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
		var attacking = new Attacking(this, _animator);

		// Assigning transitions
		_stateMachine.AddAnyTransition(attacking, IsAttacking());
		At(running, idle, IsIdle());
		At(idle, running, IsMoving());
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
        ProcessInput(); // Read player input
		Utils.Utils.SetRenderLayer(gameObject, baseLayer);
		FlipSprite();

		_stateMachine.Tick();
    }

	private void FixedUpdate()
	{
        _stateMachine.FixedTick();
	}


	#region Methods called every frame regardless of state

	public void ProcessInput()
    {
        // Gives a value between -1 and 1
        xInput = Input.GetAxisRaw("Horizontal"); // -1 is left
        yInput = Input.GetAxisRaw("Vertical"); // -1 is down
		// Store previous inputs
		if (xInput != 0 || yInput != 0) {
			prevxInput = xInput;
			prevyInput = yInput;
		}

		if (Input.GetMouseButtonDown(0)) { isAttacking = true; }
	}

	//public void ClampPosition()
	//{
	//	Vector2 position;
	//	position.x = Mathf.Clamp(this.transform.position.x, -xBoundary, xBoundary);
	//	position.y = Mathf.Clamp(this.transform.position.y, -yBoundary, yBoundary);
	//	this.transform.position = position;
	//}

	public void FlipSprite()
	{
		if (_rb.velocity.x > 0) // moving right
			{ _spriteRenderer.flipX = true; }
		else if (_rb.velocity.x < 0) // moving left
			{ _spriteRenderer.flipX = false; }
	}

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
}
