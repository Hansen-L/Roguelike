using UnityEngine;
using System.Threading;

public class RandomMoving : IState
{
	private Animator _animator;
	private IEnemy _enemy;
	private Rigidbody2D _rb;

	private float moveTimer;
	private Vector2 moveDirection;

	public RandomMoving(IEnemy enemy, Animator animator, Rigidbody2D rb)
	{
		_enemy = enemy;
		_animator = animator;
		_rb = rb;
	}

	public void OnEnter()
	{
		_animator.SetTrigger("moving");

		moveTimer = 0f;

		moveDirection = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized;
	}

	public void Tick()
	{
		moveTimer += Time.deltaTime;
		//_rb.velocity = _enemy.moveSpeed * moveDirection;


		//if (moveTimer >= Player.dashTime)
		//{
		//	_enemy.isMoving = false;
			//_player.isStunned = true; // Stun at the end of dash
		//}
	}

	public void FixedTick()
	{
	}


	public void OnExit()
	{
		_rb.velocity = new Vector2(0, 0);
		_animator.ResetTrigger("dash");
	}
}
