using UnityEngine;
using System.Threading;

public class SheepLaunchingExplodingSheep : IState
{
	private Animator _animator;
	private SheepBoss _sheep;
	private Rigidbody2D _rb;

	private float stateTimer;
	private bool hasLaunched = false;

	public SheepLaunchingExplodingSheep(SheepBoss sheep, Animator animator, Rigidbody2D rb)
	{
		_sheep = sheep;
		_animator = animator;
		_rb = rb;
	}

	public void OnEnter()
	{
		_animator.SetTrigger("isLaunchingExplodingSheep");
		_sheep.isLaunchingExplodingSheep = true;

		stateTimer = 0f;
	}

	int count;
	public void Tick()
	{
		stateTimer += Time.deltaTime;

		// Wait a bit, then start shooting the exploding sheep
		if (stateTimer <= _sheep.LaunchExplodingSheepChargeTime)
		{
			_rb.velocity = new Vector2(0f, 0f);
		}
		else if ((stateTimer > _sheep.LaunchExplodingSheepChargeTime) && (stateTimer <= _sheep.LaunchExplodingSheepChargeTime + _sheep.LaunchExplodingSheepShootingTime))
		{
			if (!hasLaunched)
			{
				hasLaunched = true;
				_sheep.StartCoroutine(_sheep.LaunchExplodingSheep()); // Use the _sheep to start the coroutine to launch the exploders.
			}
		}
		else if (stateTimer > _sheep.LaunchExplodingSheepChargeTime + _sheep.LaunchExplodingSheepShootingTime)
		{
			_sheep.isLaunchingExplodingSheep = false;

			_sheep.nextState = SheepBossStatesEnum.SheepMoving; // Transition into projectile after dashing
		}
	}

	public void FixedTick()
	{
	}


	public void OnExit()
	{
		_sheep.isLaunchingExplodingSheep = false;
		hasLaunched = false;

		_rb.velocity = new Vector2(0, 0);
		_animator.ResetTrigger("isLaunchingExplodingSheep");
	}
}
