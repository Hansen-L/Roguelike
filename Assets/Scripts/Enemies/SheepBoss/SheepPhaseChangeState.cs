using UnityEngine;
using System.Threading;

public class SheepPhaseChangeState : IState // Empty state to make transitioning a bit cleaner.
{
	private Animator _animator;
	private SheepBoss _sheep;
	private SpriteRenderer _sr;

	public SheepPhaseChangeState(SheepBoss sheep, Animator animator, SpriteRenderer sr)
	{
		_sheep = sheep;
		_animator = animator;
		_sr = sr;
	}

	public void OnEnter()
	{
		_sheep.curPhase = 2;

		// Transition to phase 2
		_sheep.angryVeinParticle.SetActive(true);
		AudioManager.Instance.Play("SheepAngry");
		CinemachineImpulseManager.Play("Extra Strong Impulse");

		// Set color of sheep to red
		// We need to disable the RuntimeAnimatorController for the color to update properly.
		RuntimeAnimatorController runtimeAnimatorController = _animator.runtimeAnimatorController;
		_animator.runtimeAnimatorController = null;
		_sr.color = new Color(1, 0.25f, 0.25f);
		_animator.runtimeAnimatorController = runtimeAnimatorController;

		_sheep.nextState = SheepBossStatesEnum.SheepLaunchingExplodingSheep;
	}

	public void Tick()
	{
		_sheep.PickNextState();
	}

	public void FixedTick()
	{
	}


	public void OnExit()
	{
	}
}
