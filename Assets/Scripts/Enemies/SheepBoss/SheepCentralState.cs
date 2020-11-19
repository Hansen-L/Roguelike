using UnityEngine;
using System.Threading;

public class SheepCentralState : IState // Empty state to make transitioning a bit cleaner.
{
	private SheepBoss _sheep;

	public SheepCentralState(SheepBoss sheep)
	{
		_sheep = sheep;
	}

	public void OnEnter()
	{
		_sheep.PickNextState();
	}

	public void Tick()
	{

	}

	public void FixedTick()
	{
	}


	public void OnExit()
	{
	}
}
