using System.Collections;
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
        if (_player.IsIdleOrMoving()()) // TODO: Find a smarter way to check if we should consume buffers. Maybe delegate to a function
        {
            StatesEnum nextInput = bufferSystem.Dequeue();
			_player.SetState(nextInput);
        }
    }

	public void ProcessInput()
	{
		// TODO: Don't access variables from player object directly

		// Gives a value between -1 and 1
		_player.xInput = Input.GetAxisRaw("Horizontal"); // -1 is left
		_player.yInput = Input.GetAxisRaw("Vertical"); // -1 is down
		
		// Store previous inputs
		if (_player.xInput != 0 || _player.yInput != 0)
		{
			_player.prevxInput = _player.xInput;
			_player.prevyInput = _player.yInput;
		}

		if (Input.GetKeyDown("space"))
		{
			if (_player.IsIdleOrMoving()() || _player.IsAttacking()()) { _player.SetState(StatesEnum.Dashing); }
			else { bufferSystem.Enqueue(StatesEnum.Dashing); }
		}
		if (Input.GetMouseButtonDown(0)) // Enqueue Attacking state if we're in another action state. If we're idle/running, to the attack immediately
		{
			if (_player.IsIdleOrMoving()()) { _player.SetState(StatesEnum.Attacking); }
			else { bufferSystem.Enqueue(StatesEnum.Attacking); }
		}
		if (Input.GetMouseButtonDown(1))
		{
			if (_player.IsIdleOrMoving()()) { _player.SetState(StatesEnum.Boomeranging); }
			else { bufferSystem.Enqueue(StatesEnum.Boomeranging); }
		}
	}
}
