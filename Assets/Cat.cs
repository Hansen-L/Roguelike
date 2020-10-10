using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Cat : MonoBehaviour, IEnemy
{
    #region Gameplay Constants
    public const int maxHealth = 100;
    public const float deathAnimationTime = 0.84f;

    public const float idleTime = 0.5f;
    public const float moveTime = 1f; // How long enemy moves for
    public const float moveSpeed = 3f;
    public const float attackRange = 4f;
	#endregion

	public GameObject healthBar;

    public bool isDead = false;
    public bool isMoving = false;

    private int health = maxHealth;

    private StateMachine _stateMachine; // Using underscores to note instance variables where it could be ambiguous
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        #region Configuring State Machine

        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();

        _stateMachine = new StateMachine();

        //var randomMoving = new randomMoving(this, _animator);
        //var enemyIdle = new EnemyIdle(this, _animator);
		#endregion
	}

	// Update is called once per frame
	void Update()
    {
        CheckIfDead();

        _rb.position = Vector2.MoveTowards(_rb.position, GameManager.Instance.mainPlayer.GetComponent<Rigidbody2D>().position, 0.005f) ;
    }


    #region Health methods
    private void CheckIfDead()
    {
        // Check if enemy still has health, if not then kill the enemy
        if (IsDead())
        {
            if (isDead == false) { Die(); } // Enemy hasn't already died. TODO: Make this cleaner
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
        _rb.velocity = new Vector2(0f, 0f);
        GetComponent<BoxCollider2D>().enabled = false; // disable collisions
        Destroy(transform.GetChild(0).gameObject);
        Destroy(gameObject, deathAnimationTime); // Destroy when death animation is done
    }

    private void UpdateHealthBar()
    {
        healthBar.transform.localScale = new Vector3(Mathf.Lerp(healthBar.transform.localScale.x, (float)GetHealth() / (float)maxHealth, 0.7f), 1f);
    }
    #endregion
}
