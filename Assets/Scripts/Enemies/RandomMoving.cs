﻿using UnityEngine;
using System.Threading;

public class RandomMoving : IState
{
	private Animator _animator;
	private AEnemy _enemy;
	private Rigidbody2D _rb;

	private float moveTimer;
	private Vector2 moveDirection;

	public RandomMoving(AEnemy enemy, Animator animator, Rigidbody2D rb)
	{
		_enemy = enemy;
		_animator = animator;
		_rb = rb;
	}

	public void OnEnter()
	{
		//_animator.SetTrigger("moving");

		moveTimer = 0f;
		moveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
		Debug.Log(moveDirection);
	}

	public void Tick()
	{
		moveTimer += Time.deltaTime;
		_rb.velocity = _enemy.MoveSpeed * moveDirection;

		if (moveTimer >= _enemy.MoveTime)
			_enemy.isMoving = false;
	}

	public void FixedTick()
	{
	}


	public void OnExit()
	{
		_rb.velocity = new Vector2(0, 0);
	}
}
