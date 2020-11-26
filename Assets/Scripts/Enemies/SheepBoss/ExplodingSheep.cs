using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingSheep : AEnemy
{
    public override int MaxHealth { get { return 1; } }
    public override float DeathAnimationTime { get { return 0.5f; } }

    public SheepBoss _sheepBoss;

    private Collider2D _collider;
    private float lifeTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // Only enable the hitbox once the sheep has slowed down
        if (_rb.velocity.magnitude < 0.5f)
            _collider.enabled = true;

        lifeTimer += Time.deltaTime;
        
        if (!isDead)
            if (lifeTimer > _sheepBoss.ExplodingSheepTimeBeforeExploding)
                Explode();
            else
                CheckIfDead();
    }

    private void Explode()
    {
        _sheepBoss.LaunchBouncyProjectiles(transform.position);
        Die();
    }

    protected override void Die()
    {
        AudioManager.Instance.PlayPitch("Sheep1", UnityEngine.Random.Range(1.8f, 2.5f));
        _animator.SetTrigger("die");
        isDead = true;
        GetComponent<Collider2D>().enabled = false; // disable collisions
        Destroy(transform.GetChild(0).gameObject); // Destroy healthbar
        Destroy(gameObject, DeathAnimationTime); // Destroy when death animation is done
    }
}
