using UnityEngine;
using System.Threading;

// Sheep boss moves towards player
public class SheepMoving : IState
{
	private Animator _animator;
	private SheepBoss _sheep;
	private Rigidbody2D _rb;
	private Rigidbody2D _playerRb;

	private float moveTimer;
	private Vector2 moveDirection;

	public SheepMoving(SheepBoss sheep, Animator animator, Rigidbody2D rb)
	{
		_sheep = sheep;
		_animator = animator;
		_rb = rb;
		_playerRb = GameManager.GetMainPlayerRb();
	}

	public void OnEnter()
	{
		_sheep.LaunchBouncyProjectiles(); // Launch bouncy projectiles when entering move state

		AudioManager.Instance.Play("Stomp");
		_animator.SetBool("isMoving", true);

		moveTimer = Random.Range(-2f, 0f); // initializing timer with a bit of randomness
	}

	public void Tick()
	{
		moveDirection = (_playerRb.position - _rb.position).normalized; // Unit vector towards player

		moveTimer += Time.deltaTime;
		_rb.velocity = _sheep.MoveSpeed * moveDirection;

		if (moveTimer >= _sheep.MoveTime)
		{
			_sheep.isMoving = false;
			_sheep.isDashing = true; // Go into dash after moving
		}
	}

	public void FixedTick()
	{
	}


	public void OnExit()
	{
		AudioManager.Instance.Stop("Stomp");
		_animator.SetBool("isMoving", false);
		_rb.velocity = new Vector2(0, 0);
	}
}
