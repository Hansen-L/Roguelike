﻿using UnityEngine;

public abstract class AProjectileEnemy : AEnemy
{
	public abstract float ProjectileChargeTime { get; }
	public abstract float ProjectileDuration { get; }
	public abstract float ProjectileSpeed { get; }

	public GameObject projectilePrefab;
	public Transform projectileFirePoint;
}