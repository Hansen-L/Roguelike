﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class BufferSystem
{
	private List<BufferItem> bufferList = new List<BufferItem>();

	public void Enqueue(InputsEnum input)  // If this item is not already in the list, enqueue it
	{
		BufferItem bufferItem = new BufferItem(input);
		if (!bufferList.Contains(bufferItem))
		{
			bufferList.Add(bufferItem);
		}
	}

	public InputsEnum Dequeue() // Dequeues the next BufferItem, and returns the associated input
	{
		if (bufferList.Count > 0)
		{
			BufferItem dequeuedItem = bufferList[0];
			bufferList.RemoveAt(0);

			return dequeuedItem.GetInput();
		}
		return InputsEnum.None; // If list is empty, return none
	}

	public void UpdateBufferTimers(float deltaTime) // Decrease the bufferTimer on each BufferItem. If the timer is less than 0, remove this item from the list
	{
		if (bufferList.Count <= 0) { return; }

		for (int i = bufferList.Count - 1; i >= 0; i--) // Iterate backwards over the list so we can remove items as they come
		{
			bufferList[i].ChangeBufferTimer(deltaTime);
			if (bufferList[i].IsExpired()) { bufferList.RemoveAt(i); } // If the timer has ran out on this input, remove it from the list
		}
	}

	public bool IsEmpty()
	{
		if (bufferList.Count <= 0) { return true; }
		return false;
	}
}