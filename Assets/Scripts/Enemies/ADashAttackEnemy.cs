using System;

public abstract class ADashAttackEnemy : ABasicEnemy
{
	public abstract float DashChargeTime { get; }
	public abstract float DashSpeed { get; }
	public abstract float DashTime { get; }

	public bool hitboxActive = false;
}