using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    private Player _player;

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
        if (boomerangScript != null)
        {
            GameObject boomerangExplosionEffect = Instantiate(_player.boomerangExplosionEffect, this.transform);
        }
    }

}
