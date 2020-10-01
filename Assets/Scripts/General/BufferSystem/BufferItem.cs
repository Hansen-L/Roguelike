using System;
using System.Collections.Generic;
using UnityEngine;

public class BufferItem
{
	public float bufferTimer = Player.bufferWindow; // Variable to hold how long this item can stay in buffer
	private StatesEnum _input; // String representing player input

	public BufferItem(StatesEnum state)
	{
		_input = state;
	}

	public void ChangeBufferTimer(float deltaTime)
	{
		bufferTimer += deltaTime;
	}

	public bool IsExpired() // Check if buffer time has ran out
	{
		if (bufferTimer <= 0) { return true; }
		return false;
	}

	public StatesEnum GetInput()
	{
		return _input;
	}

	public override bool Equals(object obj)
	{
		return this.Equals(obj as BufferItem);
	}

	public bool Equals(BufferItem otherItem) // Don't care about timer for comparison
	{
		if (this.GetInput() == otherItem.GetInput()) { return true; }
		else { return false; }
	}

	public override string ToString()
	{
		return (_input.ToString() + "  ->  " + bufferTimer);
	}
}