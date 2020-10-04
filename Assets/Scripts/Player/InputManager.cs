﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class handles all player input, and calls _player.SetState() to change the player state
public class InputManager : MonoBehaviour
{
	private BufferSystem bufferSystem = new BufferSystem();
	private Player _player;

	void Start()
	{
		_player = GetComponent<Player>();
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

		if (Input.GetKeyDown("space"))
			StartCoroutine(SetStateOrEnqueueWithDelay(StatesEnum.Dashing));
		if (Input.GetMouseButtonDown(0))
			StartCoroutine(SetStateOrEnqueueWithDelay(StatesEnum.Attacking));
		if (Input.GetMouseButtonDown(1))
		{
			if (_player.IsIdleOrMoving()()) { _player.SetState(StatesEnum.Boomeranging); }
			else { bufferSystem.Enqueue(StatesEnum.Boomeranging); }
		}
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
		_player.xInput = xInput; // -1 is left
		_player.yInput = yInput; // -1 is down

		// Store previous inputs
		if (_player.xInput != 0 || _player.yInput != 0)
		{
			_player.prevxInput = _player.xInput;
			_player.prevyInput = _player.yInput;
		}
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
			case StatesEnum.Boomeranging:
				if (_player.IsIdleOrMoving()()) { _player.SetState(StatesEnum.Boomeranging); }
				else { bufferSystem.Enqueue(StatesEnum.Boomeranging); }
				break;
			default:
				break;
		}
	}
}
