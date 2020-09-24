using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour, IEnemy
{
    public int health = 100;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Check if Wolf still has health, if not then kill the wolf
        if (IsDead()) { Die(); }
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
        Destroy(this, 2f);
    }
}
