using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangExplosion : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake()
    {
        CinemachineImpulseManager.Play("Weak Impulse");
        AudioManager.Instance.PlayOneShot("Explosion");
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (ps && !ps.IsAlive()) // Once the particles are done playing, delete this gameobject
			Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
        ABasicEnemy enemyScript = otherCollider.gameObject.GetComponent<ABasicEnemy>();
        // Check if the object is an enemy by looking for an enemy script
        if (enemyScript != null && !enemyScript.IsDead()) // If enemy isn't already dead, do damage
            enemyScript.TakeDamage(Player.BoomerangExplosionDamage);
    }
}
