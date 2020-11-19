using UnityEngine;
using System.Threading;

public class EnemyDashAttacking : IState
{
	// This should only be used on the cat enemy for now. Otherwise, make another abstract class for AEnemyCanDash

	private Animator _animator;
	private ADashAttackEnemy _enemy;
	private Rigidbody2D _rb;

	private float dashTimer;
	private Vector2 attackDirection;

	private bool hasDashed = false;

	public EnemyDashAttacking(ADashAttackEnemy enemy, Animator animator, Rigidbody2D rb)
	{
		_enemy = enemy;
		_animator = animator;
		_rb = rb;
	}

	public void OnEnter()
	{
		_animator.SetTrigger("charging");
		_enemy.isAttacking = true;

		dashTimer = 0f;
		attackDirection = (GameManager.GetMainPlayerRb().position - _rb.position).normalized;
	}

	public void Tick()
	{
		dashTimer += Time.deltaTime;

		if (dashTimer <= _enemy.DashChargeTime)
		{
			_rb.velocity = new Vector2(0f, 0f);
		}
		else if ((dashTimer > _enemy.DashChargeTime) && (dashTimer <= _enemy.DashChargeTime + _enemy.DashTime))
		{
			if (!hasDashed)
			{
				hasDashed = true;
				_enemy.hitboxActive = true;
				_rb.velocity = _enemy.DashSpeed * attackDirection;

				if (attackDirection.x > 0)
					_animator.SetTrigger("dashing right");
				else
					_animator.SetTrigger("dashing left");
			}
		}
		else if (dashTimer > _enemy.DashChargeTime + _enemy.DashTime) // If done charging and dashing
		{
			_enemy.isAttacking = false;
			_enemy.hitboxActive = false;
			_enemy.ResetAttackCooldown();
		}
	}

	public void FixedTick()
	{
	}


	public void OnExit()
	{
		_enemy.transform.localScale = new Vector3(1, 1, 1);
		_rb.velocity = new Vector2(0, 0);
		_animator.ResetTrigger("dashing right");
		_animator.ResetTrigger("dashing left");
		_animator.ResetTrigger("charging");
		hasDashed = false;
	}
}
