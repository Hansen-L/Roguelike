using UnityEngine;
using System.Threading;

public class SheepDashAttacking : IState
{
	private Animator _animator;
	private SheepBoss _sheep;
	private Rigidbody2D _rb;

	private float attackTimer;
	private Vector2 attackDirection;

	private bool hasDashed = false;

	public SheepDashAttacking(SheepBoss sheep, Animator animator, Rigidbody2D rb)
	{
		_sheep = sheep;
		_animator = animator;
		_rb = rb;
	}

	public void OnEnter()
	{
		_animator.SetTrigger("charging");
		_sheep.isDashing = true;

		attackTimer = 0f;
		attackDirection = (GameManager.GetMainPlayerRb().position - _rb.position).normalized;
	}

	public void Tick()
	{
		attackTimer += Time.deltaTime;

		if (attackTimer <= _sheep.DashChargeTime)
		{
			_rb.velocity = new Vector2(0f, 0f);
		}
		else if ((attackTimer > _sheep.DashChargeTime) && (attackTimer <= _sheep.DashChargeTime + _sheep.DashTime))
		{
			if (!hasDashed)
			{
				hasDashed = true;
				_sheep.dashHitboxActive = true;
				_rb.velocity = _sheep.DashSpeed * attackDirection;

				if (attackDirection.x > 0)
					_animator.SetTrigger("dashing right");
				else
					_animator.SetTrigger("dashing left");
			}
		}
		else if (attackTimer > _sheep.DashChargeTime + _sheep.DashTime) // If done charging and dashing
		{
			_sheep.isDashing = false;
			_sheep.dashHitboxActive = false;

			_sheep.isProjectiling = true; // Transition into projectile after dashing
		}
	}

	public void FixedTick()
	{
	}


	public void OnExit()
	{
		_rb.velocity = new Vector2(0, 0);
		_animator.ResetTrigger("charging");
		hasDashed = false;
	}
}
