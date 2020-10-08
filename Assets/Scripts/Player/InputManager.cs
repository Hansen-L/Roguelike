using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class handles all player input, and calls _player.SetState() to change the player state
public class InputManager : MonoBehaviour
{
	private BufferSystem bufferSystem = new BufferSystem();
	private Player _player;
	private Rigidbody2D _rb;

	void Start()
	{
		_player = GetComponent<Player>();
		_rb = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		ProcessInput(); // Read player input
		bufferSystem.UpdateBufferTimers(-Time.deltaTime);
		ConsumeBuffer();
	}

	public void ConsumeBuffer()
	{
		if (!bufferSystem.IsEmpty() && _player.IsIdleOrMoving()())
		{
			StatesEnum nextInput = bufferSystem.Dequeue();
			_player.SetState(nextInput);
		}
	}

	public void ProcessInput()
	{
		StartCoroutine(SetMovementInputsWithDelay(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));

		if (Input.GetKeyDown(KeyCode.Space))
			if (_player.CanMove())
				StartCoroutine(SetStateOrEnqueueWithDelay(StatesEnum.Dashing));

		if (Input.GetMouseButtonDown(0))
			StartCoroutine(SetStateOrEnqueueWithDelay(StatesEnum.Attacking));

		if (Input.GetMouseButtonDown(1)) // No delay for boomerang
		{
			if (_player.IsIdleOrMoving()()) { _player.SetState(StatesEnum.Boomeranging); }
			else { bufferSystem.Enqueue(StatesEnum.Boomeranging); }
		}

		if (Input.GetKeyDown(KeyCode.Q)) // Freeze/unfreeze shadow in place
		{
			_player.shadowCanMove = !_player.shadowCanMove;
			if (_player.isShadow)
				_rb.velocity = new Vector2(0f, 0f);
			// TODO: This code is a hacky way of preventing the 'moving' state when the shadow is frozen. Find a way to refactor.
			_player.xInput = 0;
			_player.yInput = 0;
		}

		if (Input.GetKeyDown(KeyCode.LeftShift))
			_player.SetState(StatesEnum.Swapping);
	}

	private IEnumerator SetMovementInputsWithDelay(float xInput, float yInput)
	// If this is shadow, wait before modifying the xInput/yInput. 
	// This is so that the attacks will go in the right direction, and so that the animations match up with the motion of the shadow.
	{
		if (!_player.isShadow)
			yield return null;
		else
			yield return new WaitForSeconds(Player.shadowDelay);

		// TODO: Don't access variables from player object directly
		// Gives a value between -1 and 1
		if (_player.CanMove()) // If the player is a shadow and isn't frozen, or if the player is not a shadow
			_player.SetCurrentInput(new Vector2(xInput, yInput));

		// Store previous inputs. These are used to flip the sprite and to aim attacks
		if (xInput != 0 || yInput != 0)
			_player.SetPrevInput(new Vector2(xInput, yInput));
	}

	private IEnumerator SetStateOrEnqueueWithDelay(StatesEnum state)
	// If this is the shadow, wait for a delay before processing the input. Then, either change the state immediately or enqueue it.
	{
		if (!_player.isShadow)
			yield return null;
		else
			yield return new WaitForSeconds(Player.shadowDelay);

		switch (state)
		{
			case StatesEnum.Dashing:
				if (_player.IsIdleOrMoving()() || _player.IsAttacking()()) { _player.SetState(StatesEnum.Dashing); }
				else { bufferSystem.Enqueue(StatesEnum.Dashing); }
				break;
			case StatesEnum.Attacking:
				// Enqueue Attacking state if we're in another action state. If we're idle/running, to the attack immediately
				if (_player.IsIdleOrMoving()()) { _player.SetState(StatesEnum.Attacking); }
				else { bufferSystem.Enqueue(StatesEnum.Attacking); }
				break;
			//case StatesEnum.Boomeranging:
			//	if (_player.IsIdleOrMoving()()) { _player.SetState(StatesEnum.Boomeranging); }
			//	else { bufferSystem.Enqueue(StatesEnum.Boomeranging); }
			//	break;
			default:
				break;
		}
	}
}
