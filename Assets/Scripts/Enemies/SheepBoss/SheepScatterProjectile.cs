using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepScatterProjectile : MonoBehaviour
{
    private Rigidbody2D _rb;
    private SheepBoss _sheep;

	public void Start()
	{
        _rb = GetComponent<Rigidbody2D>();
	}

	public void SetEnemy(SheepBoss sheep)
    {
        _sheep = sheep;
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        Player playerScript = otherCollider.gameObject.GetComponent<Player>();
        if (playerScript != null && !playerScript.isShadow) // If we hit the main player
        {
            playerScript.TakeDamage(_sheep.BouncyProjectileDamage);

            GetComponent<CircleCollider2D>().enabled = false; // Disable collider
            Destroy(gameObject);
        }
    }

	private void OnCollisionEnter2D(Collision2D collision) // If wall collider hits a wall
	{
        Destroy(gameObject);
    }
}
