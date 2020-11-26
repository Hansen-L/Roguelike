public enum StatesEnum // Enum of possible inputs (mapped to states)
{
	Idle,
	Moving,
	Dashing,
	Attacking,
	Boomeranging,
	Swapping,
	None
}

public enum SheepBossStatesEnum // Sheep boss states. If adding states, make sure to change the PickNextState() function
{
	SheepCentralState,
	SheepMoving,
	SheepProjectiling,
	SheepDashAttacking,
	SheepLaunchingExplodingSheep
}

public enum LayersEnum // layer names
{
	Walls,
}