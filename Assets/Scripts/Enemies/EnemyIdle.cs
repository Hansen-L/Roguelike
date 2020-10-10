using UnityEngine;
using System.Threading;

public class EnemyIdle : IState
{
	private Animator _animator;
	private AEnemy _enemy;
	private Rigidbody2D _rb;

	private float idleTimer;

	public EnemyIdle(AEnemy enemy, Animator animator, Rigidbody2D rb)
	{
		_enemy = enemy;
		_animator = animator;
		_rb = rb;
	}

	public void OnEnter()
	{
		//_animator.SetTrigger("idle");

		idleTimer = Random.Range(-2f, 0f);
		_rb.velocity = new Vector2(0f, 0f);
	}

	public void Tick()
	{
		idleTimer += Time.deltaTime;

		if (idleTimer >= _enemy.IdleTime)
			_enemy.isMoving = true;
	}

	public void FixedTick()
	{
	}


	public void OnExit()
	{
	}
}
