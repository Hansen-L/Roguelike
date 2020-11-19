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
		_animator.SetTrigger("isProjectiling");
		_sheep.isProjectiling = true;

		attackTimer = 0f;
	}

	public void Tick()
	{
		attackTimer += Time.deltaTime;

		if (attackTimer <= _sheep.ScatterProjectileChargeTime)
		{
			_rb.velocity = new Vector2(0f, 0f);
		}
		else if (attackTimer > _sheep.ScatterProjectileChargeTime  &&  attackTimer <= _sheep.ScatterProjectileAnimationTime)
		{
			if (!hasFired)
			{
				AudioManager.Instance.PlayOneShot("Sheep1");
				hasFired = true;

				_sheep.LaunchProjectilesScatter();
			}
		}
		else if (attackTimer > _sheep.ScatterProjectileAnimationTime) // Once animation has ended, exit this state
			_sheep.isProjectiling = false;

	}

	public void FixedTick()
	{
	}


	public void OnExit()
	{
		_rb.velocity = new Vector2(0, 0);
		_animator.ResetTrigger("isProjectiling");
		hasFired = false;
	}
}
