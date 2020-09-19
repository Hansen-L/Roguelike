using UnityEngine;
using System.Threading;

public class Stunned : IState
{
	private Animator _animator;
	private Dog _dog;
	private Rigidbody2D _rb;

	private float stunTimer;

	public Stunned(Dog dog, Rigidbody2D rb)
	{
		_dog = dog;
		_rb = rb;
	}

	public void OnEnter()
	{
		stunTimer = 0f;
	}

	public void Tick()
	{
		stunTimer += Time.fixedDeltaTime;
		_rb.velocity = new Vector2(0, 0);


		if (stunTimer >= Dog.stunTime)
		{
			_dog.isStunned = false;
		}
	}

	public void FixedTick()
	{
	}


	public void OnExit()
	{
	}
}
