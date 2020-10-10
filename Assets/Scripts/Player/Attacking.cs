using UnityEngine;
using System.Threading;
using System;

public class Attacking : IState
{
    private Animator _animator;
    private Player _player;
    private Rigidbody2D _rb;

    private float attackTimer;

    public Attacking(Player player, Animator animator, Rigidbody2D rb)
    {
        _player = player;
        _animator = animator;
        _rb = rb;
    }

    public void OnEnter() 
	{
		//AudioManager.Instance.Play("run");
		_animator.SetTrigger("attack");
        _rb.velocity = new Vector2(0f, 0f);

        _player.comboTimer = 0f; // Reset combo timer
        _player.comboCount += 1;
        attackTimer = 0f;

        // Decide which attack to launch based on the combo count
        if (_player.comboCount >= 3)
        {
            _player.comboCount = 0;
            BarkEffect();
            LaunchAttack(_player.barkCollider, Player.BarkDamage);
        }
        else {
            SlashEffect();
            LaunchAttack(_player.slashCollider, Player.SlashDamage);
        }
    }

    public void Tick() 
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= Player.AttackTime)
        {
            _player.isAttacking = false;
        }
	}

    public void FixedTick()
    {
    }


    public void OnExit() 
	{
        _player.isAttacking = false;
        //AudioManager.Instance.Stop("run");
        _animator.ResetTrigger("attack");
    }

	#region Private Methods
	private void SlashEffect() // Animates the slash attack by spawning an effect
    {
        float offsetX = 0.6f;
        GameObject slashEffectInstance;

        if (_player.transform.localScale.x == -1) // Moving right
        {
            Vector2 slashEffectPosition = new Vector2(_player.transform.position.x + offsetX, _player.transform.position.y);
            slashEffectInstance = GameObject.Instantiate(_player.slashEffect, slashEffectPosition, Quaternion.Euler(0, 0, 90));
            GameObject.Destroy(slashEffectInstance, 2f);
        }
        else // Moving left
        {
            Vector2 slashEffectPosition = new Vector2(_player.transform.position.x - offsetX, _player.transform.position.y);
            slashEffectInstance = GameObject.Instantiate(_player.slashEffect, slashEffectPosition, Quaternion.Euler(0, 180, 90));
            GameObject.Destroy(slashEffectInstance, 2f);
        }

        _player.SetColorToBlack(slashEffectInstance);
    }

    private void BarkEffect() // Animates the bark attack by spawning an effect
    {
        float offsetX = 0.3f;
        float offsetY = 0.12f;
        Vector2 barkEffectPosition;
        GameObject barkEffectInstance;

        if (_player.transform.localScale.x == -1) // Moving right
        {
            barkEffectPosition = new Vector2(_player.transform.position.x + offsetX, _player.transform.position.y + offsetY);
            barkEffectInstance = GameObject.Instantiate(_player.barkEffect, barkEffectPosition, new Quaternion(0f, 180f, 0f, 1));
            barkEffectInstance.transform.parent = _player.gameObject.transform;
            GameObject.Destroy(barkEffectInstance, 2f);
        }
        else // Moving left
        {
            barkEffectPosition = new Vector2(_player.transform.position.x - offsetX, _player.transform.position.y + offsetY);
            barkEffectInstance = GameObject.Instantiate(_player.barkEffect, barkEffectPosition, new Quaternion(0f, 0f, 0f, 1));
            barkEffectInstance.transform.parent = _player.gameObject.transform;
            GameObject.Destroy(barkEffectInstance, 2f);
        }

        _player.SetColorToBlack(barkEffectInstance);
    }

    private void LaunchAttack(Collider2D hitbox, int damageAmount)
    {
        Collider2D[] allColliders = Physics2D.OverlapBoxAll(hitbox.bounds.center, hitbox.bounds.size, 0);

        foreach (Collider2D collider in allColliders)
        {
            AEnemy enemyScript = collider.gameObject.GetComponent<AEnemy>();
            // Check if the enemy script exists to confirm that this is an enemy's collider
            if (enemyScript!=null && !enemyScript.IsDead()) // If enemy isn't already dead, do damage
            {
                _player.slashParticle.Play();
                enemyScript.TakeDamage(damageAmount);
            }
        }
    }
    #endregion
}
