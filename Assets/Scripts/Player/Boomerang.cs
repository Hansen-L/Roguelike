using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
        if (otherCollider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            IEnemy enemyScript = otherCollider.gameObject.GetComponent<IEnemy>();
            if (!enemyScript.IsDead()) // If enemy isn't already dead, do damage
            {
                enemyScript.TakeDamage(Player.boomerangDamage);
            }
        }
    }
}
