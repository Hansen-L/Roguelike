using UnityEngine;
using System.Threading;

public class EnemyProjectiling : IState
{
	// This should only be used on the cat enemy for now. Otherwise, make another abstract class for AEnemyCanDash

	private Animator _animator;
	private AProjectileEnemy _enemy;
	private Rigidbody2D _rb;

	private float attackTimer;
	private Vector2 attackDirection;

	private bool hasFired = false;

	public EnemyProjectiling(AProjectileEnemy enemy, Animator animator, Rigidbody2D rb)
	{
		_enemy = enemy;
		_animator = animator;
		_rb = rb;
	}

	public void OnEnter()
	{
		_animator.SetTrigger("charging");
		_enemy.isAttacking = true;

		attackTimer = 0f;
		attackDirection = (GameManager.GetMainPlayerRb().position - _rb.position).normalized;
	}

	public void Tick()
	{
		attackTimer += Time.deltaTime;

		if (attackTimer <= _enemy.ProjectileChargeTime)
		{
			_rb.velocity = new Vector2(0f, 0f);
		}
		else if (attackTimer > _enemy.ProjectileChargeTime)
		{
			if (!hasFired)
			{
				hasFired = true;

				_enemy.LaunchProjectile(attackDirection);

				_enemy.isAttacking = false;
				_enemy.ResetAttackCooldown();
			}
		}
	}

	public void FixedTick()
	{
	}


	public void OnExit()
	{
		_rb.velocity = new Vector2(0, 0);
		_animator.ResetTrigger("charging");
		hasFired = false;
	}
}
