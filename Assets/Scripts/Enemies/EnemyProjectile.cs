using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private AProjectileEnemy _enemy;

    public void SetEnemy(AProjectileEnemy enemy)
    {
        _enemy = enemy;
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        Player playerScript = otherCollider.gameObject.GetComponent<Player>();
        if (playerScript != null && !playerScript.isShadow) // If we hit the main player
        {
            playerScript.TakeDamage(_enemy.AttackDamage);

            GetComponent<CircleCollider2D>().enabled = false; // Disable collider
            Destroy(gameObject);
            // TODO: Play explosion animation, destroy fireball object
        }

        if (otherCollider.gameObject.layer == LayerMask.NameToLayer(LayersEnum.Walls.ToString()))
            Destroy(gameObject);
    }
}
