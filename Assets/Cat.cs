using UnityEngine;
using System;
using Utils;
using System.Collections;

public class Cat : AEnemy
{
	#region Gameplay Constants
	// TODO: These are overriding the AEnemy default values. Should I use properties for more robust polymorphism?
	public override int MaxHealth { get { return 100; } }
	public override float DeathAnimationTime { get { return 0.84f; } }

	public override float IdleTime { get { return 2f; } }
	public override float MoveTime { get { return 4f; } } // How long enemy moves for
	public override float MoveSpeed { get { return 0.5f; } }
	public override float AttackRange { get { return 4f; } }
	#endregion

	public GameObject healthBar;

	#region Boolean methods for state transitions
	// Note: When calling these methods, we use IsMoving()() or IsMoving().Invoke()
	public Func<bool> IsMoving() => () => (isMoving);
	public Func<bool> IsIdle() => () => (!isMoving);
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

		// Assigning transitions
		At(randomMoving, enemyIdle, IsIdle());
		At(enemyIdle, randomMoving, IsMoving());

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
}
