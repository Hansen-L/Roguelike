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

	public EnemyDashAttacking(Cat cat, Animator animator, Rigidbody2D rb)
	{
		_cat = cat;
		_animator = animator;
		_rb = rb;
	}

	public void OnEnter()
	{
		_animator.SetTrigger("charging");
		Debug.Log("charging");
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
			_rb.velocity = _cat.DashSpeed * attackDirection;
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
		_rb.velocity = new Vector2(0, 0);
		_animator.ResetTrigger("charging");
	}
}
