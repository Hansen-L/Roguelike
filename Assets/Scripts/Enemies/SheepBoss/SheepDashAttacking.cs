using UnityEngine;
using System.Threading;

public class SheepDashAttacking : IState
{
	private Animator _animator;
	private SheepBoss _sheep;
	private Rigidbody2D _rb;

	private float dashTimer;
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
		_animator.SetTrigger("isDashing");
		_sheep.isDashing = true;

		dashTimer = 0f;
	}

	public void Tick()
	{
		dashTimer += Time.deltaTime;

		if (dashTimer <= _sheep.DashChargeTime)
		{
			_rb.velocity = new Vector2(0f, 0f);
		}
		else if ((dashTimer > _sheep.DashChargeTime) && (dashTimer <= _sheep.DashChargeTime + _sheep.DashTime))
		{
			if (!hasDashed)
			{
				_sheep.StartCoroutine(_sheep.SpawnDashTrail());
				attackDirection = (GameManager.GetMainPlayerRb().position - _rb.position).normalized;
				hasDashed = true;
				_sheep.dashHitboxActive = true;
				_rb.velocity = _sheep.DashSpeed * attackDirection;
			}
		}
		else if (dashTimer > _sheep.DashChargeTime + _sheep.DashTime) // If done charging and dashing
		{
			_sheep.isDashing = false;
			_sheep.dashHitboxActive = false;

			_sheep.nextState = SheepBossStatesEnum.SheepProjectiling; // Transition into projectile after dashing
		}
	}

	public void FixedTick()
	{
	}


	public void OnExit()
	{
		_rb.velocity = new Vector2(0, 0);
		_animator.ResetTrigger("isDashing");
		hasDashed = false;
	}
}
