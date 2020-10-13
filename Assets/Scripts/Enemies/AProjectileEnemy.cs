using UnityEngine;

public abstract class AProjectileEnemy : AEnemy
{
	public abstract float ChargeTime { get; }
	public abstract float ProjectileDuration { get; }
	public abstract float ProjectileSpeed { get; }
}