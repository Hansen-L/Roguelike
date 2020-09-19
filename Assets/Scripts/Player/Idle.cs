using UnityEngine;

public class Idle : IState
{
	private Animator _animator;
	private Dog _dog;

	public Idle(Dog dog, Animator animator)
	{
		_dog = dog;
		_animator = animator;
	}

	public void OnEnter()
	{
		AudioManager.Instance.Stop("run");
		_animator.SetBool("isMoving", false);
	}

	public void Tick()
	{
	}

	public void FixedTick()
	{
		_dog.ProcessMovement();
	}


	public void OnExit()
	{
	}
}
