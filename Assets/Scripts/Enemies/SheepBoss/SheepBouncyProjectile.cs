using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepBouncyProjectile : MonoBehaviour
{
    private Rigidbody2D _rb;
    private SheepBoss _sheep;
    private int bouncesRemaining;

	public void Start()
	{
        _rb = GetComponent<Rigidbody2D>();
	}

	public void SetEnemy(SheepBoss sheep)
    {
        _sheep = sheep;
        bouncesRemaining = _sheep.ProjectileBounces;
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        Player playerScript = otherCollider.gameObject.GetComponent<Player>();
        if (playerScript != null && !playerScript.isShadow) // If we hit the main player
        {
            playerScript.TakeDamage(_sheep.ProjectileDamage);

            GetComponent<CircleCollider2D>().enabled = false; // Disable collider
            Destroy(gameObject);
            // TODO: Play explosion animation, destroy fireball object
        }
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
        Debug.Log(bouncesRemaining);
        if (bouncesRemaining > 0)
            bouncesRemaining -= 1;
        else
            Destroy(gameObject);
    }
}
