using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    private Player _player;
    private Rigidbody2D boomerangRb;

    private int numExplosions = 2;
    private float minAngle = 20f;

	private void Awake()
	{
        boomerangRb = GetComponent<Rigidbody2D>();
	}

	public void SetPlayer(Player player) 
    {
        _player = player;
    }

	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
        AEnemy enemyScript = otherCollider.gameObject.GetComponent<AEnemy>();
        // Check if the object is an enemy by looking for an enemy script
        if (enemyScript!=null && !enemyScript.IsDead()) // If enemy isn't already dead, do damage
            enemyScript.TakeDamage(Player.boomerangDamage);

        Boomerang boomerangScript = otherCollider.gameObject.GetComponent<Boomerang>();
        if ((boomerangScript != null) && (numExplosions > 0)) // If object is another boomerang
        {
            Vector2 vel = boomerangRb.velocity;
            float thisAngle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
            float otherAngle = Mathf.Atan2(otherCollider.GetComponent<Rigidbody2D>().velocity.y, otherCollider.GetComponent<Rigidbody2D>().velocity.x) * Mathf.Rad2Deg;

            if (Mathf.Abs(otherAngle - thisAngle) > minAngle) // Check that the angle between the two boomerangs' velocity is bigger than 10
            {
                Vector2 offset = vel * (0.55f / vel.magnitude); // Offsetting it so that the explosion is at the intersection of the two boomerangs
                Instantiate(_player.boomerangExplosionEffect, this.transform.position + new Vector3(offset.x, offset.y, 0f), Quaternion.AngleAxis(thisAngle, Vector3.forward));
                numExplosions -= 1;
            }
        }
    }

}
