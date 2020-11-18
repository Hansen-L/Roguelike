using UnityEngine;

public abstract class AProjectileEnemy : ABasicEnemy
{
	public abstract float ProjectileChargeTime { get; }
	public abstract float ProjectileDuration { get; }
	public abstract float ProjectileSpeed { get; }

	public GameObject projectilePrefab;
	public Transform projectileFirePoint;

	public abstract void LaunchProjectile(Vector2 attackDirection);
}