using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    public const int maxHealth = 100;
    public const float deathAnimationTime = 0.8f;

    public GameObject healthBar;

    private int health = maxHealth;
    private bool isDead = false;

    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if Wolf still has health, if not then kill the wolf
        if (IsDead())
        {
            if (isDead == false)  // Wolf hasn't already died. TODO: Make this cleaner
                Die();
        }
        else { UpdateHealthBar(); }
    }



    public int GetHealth()
    {
        return health;
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health < 0) { health = 0; }
    }

    public void GainHealth(int healthAmount)
    {
    }

    public bool IsDead()
    {
        // If health less than 0, return true
        return (health <= 0) ? true : false;
    }

    public void Die()
    {
        isDead = true;
        _animator.SetTrigger("die");
        GetComponent<BoxCollider2D>().enabled = false; // disable collisions
        Destroy(transform.GetChild(0).gameObject);
		Destroy(gameObject, deathAnimationTime);
	}

    private void UpdateHealthBar()
    {
        healthBar.transform.localScale = new Vector3(Mathf.Lerp(healthBar.transform.localScale.x, (float)GetHealth()/(float)maxHealth, 0.7f), 1f);
    }
}
