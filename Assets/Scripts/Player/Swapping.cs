using UnityEngine;
using System.Threading;

public class Swapping : IState
{
	private Animator _animator;
	private Player _player;
	private Rigidbody2D _rb;

	private float swapTimer;

	public Swapping(Player player, Animator animator, Rigidbody2D rb)
	{
		_player = player;
		_animator = animator;
		_rb = rb;
	}

	public void OnEnter()
	{
		//_animator.SetTrigger("dash");
		_rb.velocity = new Vector2(0f, 0f);
		swapTimer = 0f;

		if (!_player.isShadow)
			SwapPositions();
	}

	public void Tick()
	{
		swapTimer += Time.deltaTime;

		if (swapTimer >= Player.swapTime)
		{
			_player.isSwapping = false;
		}
	}

	public void FixedTick()
	{
	}


	public void OnExit()
	{
		//_animator.ResetTrigger("dash");
	}

	private void SwapPositions()
	{
		Vector2 tempVector = _player.mainPlayer.transform.position;
		_player.mainPlayer.transform.position = _player.shadowPlayer.transform.position;
		_player.shadowPlayer.transform.position = tempVector;
	}
}

