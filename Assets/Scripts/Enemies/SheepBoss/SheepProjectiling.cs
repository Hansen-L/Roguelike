using UnityEngine;
using System.Threading;

// Sheep boss sends out projectiles
public class SheepProjectiling : IState
{
	private Animator _animator;
	private SheepBoss _sheep;
	private Rigidbody2D _rb;

	private float attackTimer;
	private Vector2 attackDirection;

	private bool hasFired = false;

	public SheepProjectiling(SheepBoss sheep, Animator animator, Rigidbody2D rb)
	{
		_sheep = sheep;
		_animator = animator;
		_rb = rb;
	}

	public void OnEnter()
	{
		_animator.SetTrigger("charging");
		_sheep.isProjectiling = true;

		attackTimer = 0f;
	}

	public void Tick()
	{
		attackTimer += Time.deltaTime;

		if (attackTimer <= _sheep.ProjectileChargeTime)
		{
			_rb.velocity = new Vector2(0f, 0f);
		}
		else if (attackTimer > _sheep.ProjectileChargeTime)
		{
			if (!hasFired)
			{
				hasFired = true;

				_sheep.LaunchProjectile();

				_sheep.isProjectiling = false;
				_sheep.isMoving = true; //Transition to walking after projectiling
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
