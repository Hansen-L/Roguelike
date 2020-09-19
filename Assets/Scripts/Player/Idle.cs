using UnityEngine;

public class Idle : IState
{
	private Animator _animator;
	private Player _player;

	public Idle(Player player, Animator animator)
	{
		_player = player;
		_animator = animator;
	}

	public void OnEnter()
	{
		//AudioManager.Instance.Stop("run");
		_animator.SetBool("isMoving", false);
	}

	public void Tick()
	{
	}

	public void FixedTick()
	{
		_player.ProcessMovement();
	}


	public void OnExit()
	{
	}
}
