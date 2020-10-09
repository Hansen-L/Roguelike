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
        IEnemy enemyScript = otherCollider.gameObject.GetComponent<IEnemy>();
        // Check if the object is an enemy by looking for an enemy script
        if (enemyScript!=null && !enemyScript.IsDead()) // If enemy isn't already dead, do damage
            enemyScript.TakeDamage(Player.boomerangDamage);

        Boomerang boomerangScript = otherCollider.gameObject.GetComponent<Boomerang>();
        if ((boomerangScript != null) && (numExplosions > 0)) // If object is another boomerang
        {
            Vector2 dir = boomerangRb.velocity;
            float thisAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float otherAngle = Mathf.Atan2(otherCollider.GetComponent<Rigidbody2D>().velocity.y, otherCollider.GetComponent<Rigidbody2D>().velocity.x) * Mathf.Rad2Deg;

            if (Mathf.Abs(otherAngle - thisAngle) > minAngle) // Check that the angle between the two boomerangs' velocity is bigger than 10
                Instantiate(_player.boomerangExplosionEffect, this.transform.position, Quaternion.AngleAxis(thisAngle, Vector3.forward));
        }
    }

}
