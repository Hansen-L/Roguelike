using UnityEngine;
using System.Threading;
using System;

public class Attacking : MonoBehaviour, IState 
{
    private Animator _animator;
    private Player _player;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;

    private float attackTimer;

    public Attacking(Player player, Animator animator, Rigidbody2D rb, SpriteRenderer spriteRenderer)
    {
        _player = player;
        _animator = animator;
        _rb = rb;
        _spriteRenderer = spriteRenderer;
    }

    public void OnEnter() 
	{
		//AudioManager.Instance.Play("run");
		_animator.SetTrigger("attack");
        _rb.velocity = new Vector2(0f, 0f);

        _player.comboTimer = 0f; // Reset combo timer
        _player.comboCount += 1;
        attackTimer = 0f;

        Debug.Log(_player.comboCount);
        if (_player.comboCount >= 3)
        {
            _player.comboCount = 0;
            BigAttack();
        }
        else { Attack(); }
    }

    public void Tick() 
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= Player.attackTime)
        {
            _player.isAttacking = false;
        }
	}

    public void FixedTick()
    {
    }


    public void OnExit() 
	{
        //AudioManager.Instance.Stop("run");
        _animator.ResetTrigger("attack");
    }

    private void Attack()
    {
        float offsetX = 0.6f;
        float offsetY = 0.12f;

        if (_spriteRenderer.flipX == true) // Moving right
        {
            Vector2 slashEffectPosition = new Vector2(_player.transform.position.x + offsetX, _player.transform.position.y);
            GameObject slashEffectInstance = Instantiate(_player.slashEffect, slashEffectPosition, Quaternion.Euler(0, 0, 90));
            Destroy(slashEffectInstance, 2f);
        }
        else // Moving left
        {
            Vector2 slashEffectPosition = new Vector2(_player.transform.position.x - offsetX, _player.transform.position.y);
            GameObject slashEffectInstance = Instantiate(_player.slashEffect, slashEffectPosition, Quaternion.Euler(0, 180, 90));
            Destroy(slashEffectInstance, 2f);
        }
    }

    private void BigAttack()
    {
        float offsetX = 0.3f;
        float offsetY = 0.12f;
        Vector2 barkEffectPosition;

        if (_spriteRenderer.flipX == true) // Moving right
        {
            barkEffectPosition = new Vector2(_player.transform.position.x + offsetX, _player.transform.position.y + offsetY);
            GameObject barkEffectInstance = Instantiate(_player.barkEffect, barkEffectPosition, new Quaternion(0f, 180f, 0f, 1));
            barkEffectInstance.transform.parent = _player.gameObject.transform;
            Destroy(barkEffectInstance, 2f);
        }
        else // Moving left
        {
            barkEffectPosition = new Vector2(_player.transform.position.x - offsetX, _player.transform.position.y + offsetY);
            GameObject barkEffectInstance = Instantiate(_player.barkEffect, barkEffectPosition, new Quaternion(0f, 0f, 0f, 1));
            barkEffectInstance.transform.parent = _player.gameObject.transform;
            Destroy(barkEffectInstance, 2f);
        }

    }
}
