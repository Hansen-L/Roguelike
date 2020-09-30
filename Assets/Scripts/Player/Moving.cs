using UnityEngine;
using System.Threading;

public class Moving : IState 
{
    private Animator _animator;
    private Player _player;

    public Moving(Player player, Animator animator)
    {
        _player = player;
        _animator = animator;
    }

    public void OnEnter() 
	{
		//AudioManager.Instance.Play("run");
		_animator.SetBool("isMoving", true);
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
		//AudioManager.Instance.Stop("run");
		_animator.SetBool("isMoving", false);
	}
}
