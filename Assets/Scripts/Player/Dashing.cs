﻿using UnityEngine;
using System.Threading;

public class Dashing : IState
{
	private Animator _animator;
	private Player _player;
	private Rigidbody2D _rb;
	private Collider2D _collider;

	private float dashTimer;
	private Vector2 dashDirection;

	public Dashing(Player player, Animator animator, Rigidbody2D rb, Collider2D collider)
	{
		_player = player;
		_animator = animator;
		_rb = rb;
		_collider = collider;
	}

	public void OnEnter()
	{
		if (!_player.isShadow)
		{
			// Start dash cooldown
			_player.canDash = false;
			_player.dashCooldownTimer = Player.DashCooldown;

			AudioManager.Instance.PlayOneShot("DogDash");
			_player.StartDashTrailCoroutine();

			// Spawn dashwind effect
			GameObject dashWind = GameObject.Instantiate(_player.dashWindPrefab, new Vector2(_rb.position.x, _rb.position.y + 0.5f), _player.transform.rotation);
			dashWind.transform.localScale =  new Vector3(Mathf.Sign(_player.transform.localScale.x)* dashWind.transform.localScale.x, dashWind.transform.localScale.y, 1);
		}

		_animator.SetTrigger("dash");
		_collider.enabled = false;

		dashTimer = 0f;

		if (_player.xInput != 0 || _player.yInput != 0) { dashDirection = new Vector2(_player.xInput, _player.yInput); }
		else { dashDirection = new Vector2(_player.prevxInput, _player.prevyInput); } // If we aren't holding a direction, dash in last direction

		dashDirection.Normalize();
	}

	public void Tick()
	{
		dashTimer += Time.deltaTime;
		_rb.velocity = Player.DashSpeed * dashDirection;


		if (dashTimer >= Player.DashTime)
		{
			_player.isDashing = false;
			//_player.isStunned = true; // Stun at the end of dash
		}
	}

	public void FixedTick()
	{
	}


	public void OnExit()
	{
		_rb.velocity = new Vector2(0, 0);
		_animator.ResetTrigger("dash");
		_collider.enabled = true;
	}
}
