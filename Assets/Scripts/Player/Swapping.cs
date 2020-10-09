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
		if (!_player.isShadow)
			SwapPositions();

		_rb.velocity = new Vector2(0f, 0f);
		swapTimer = 0f;

		if (!_player.isShadow)
			_animator.SetTrigger("swapMain");
		else
			_animator.SetTrigger("swapShadow");
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
		_animator.ResetTrigger("swapMain");
		_animator.ResetTrigger("swapShadow");
	}

	private void SwapPositions()
	{
		Vector2 tempVector = GameManager.GetMainPlayer().transform.position;
		GameManager.GetMainPlayer().transform.position = GameManager.GetShadowPlayer().transform.position;
		GameManager.GetShadowPlayer().transform.position = tempVector;
		ShadowMovementManager.ResetPositionQueue();
	}
}

