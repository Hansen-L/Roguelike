using UnityEngine;
using System.Threading;

public class EnemyDashAttacking : IState
{
	// This should only be used on the cat enemy for now. Otherwise, make another abstract class for AEnemyCanDash

	private Animator _animator;
	private Cat _cat;
	private Rigidbody2D _rb;

	private float attackTimer;
	private Vector2 attackDirection;

	private bool hasDashed = false;

	public EnemyDashAttacking(Cat cat, Animator animator, Rigidbody2D rb)
	{
		_cat = cat;
		_animator = animator;
		_rb = rb;
	}

	public void OnEnter()
	{
		_animator.SetTrigger("charging");
		_cat.isAttacking = true;

		attackTimer = 0f;
		attackDirection = (GameManager.GetMainPlayerRb().position - _rb.position).normalized;
	}

	public void Tick()
	{
		attackTimer += Time.deltaTime;

		if (attackTimer <= _cat.ChargeTime)
		{
			_rb.velocity = new Vector2(0f, 0f);
		}
		else if ((attackTimer > _cat.ChargeTime) && (attackTimer <= _cat.ChargeTime + _cat.DashTime))
		{
			if (!hasDashed)
			{
				hasDashed = true;
				_rb.velocity = _cat.DashSpeed * attackDirection;

				if (attackDirection.x > 0)
					_animator.SetTrigger("dashing right");
				else
					_animator.SetTrigger("dashing left");
			}
		}
		else if (attackTimer > _cat.ChargeTime + _cat.DashTime) // If done charging and dashing
		{
			_cat.isAttacking = false;
			_cat.ResetAttackCooldown();
		}
	}

	public void FixedTick()
	{
	}


	public void OnExit()
	{
		_cat.transform.localScale = new Vector3(1, 1, 1);
		_rb.velocity = new Vector2(0, 0);
		_animator.ResetTrigger("dashing right");
		_animator.ResetTrigger("dashing left");
		_animator.ResetTrigger("charging");
		hasDashed = false;
	}
}
