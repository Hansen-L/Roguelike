using System;
using System.Collections.Generic;

public class BufferItem
{
	public float bufferTimer = Player.bufferWindow; // Variable to hold how long this item can stay in buffer
	private InputsEnum _input; // String representing player input

	public BufferItem(InputsEnum input)
	{
		_input = input;
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

	public InputsEnum GetInput()
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
}