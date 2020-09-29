using UnityEngine;
using System.Threading;
using System;

public class Boomeranging : IState
{
    private Animator _animator;
    private Player _player;
    private Rigidbody2D _rb;

    private float boomerangTimer;

    public Boomeranging(Player player, Animator animator, Rigidbody2D rb)
    {
        _player = player;
        _animator = animator;
        _rb = rb;
    }

    public void OnEnter()
    {
        //AudioManager.Instance.Play("run");
        _animator.SetTrigger("attack"); // TODO: Replace with projectile animation
        _rb.velocity = new Vector2(0f, 0f);

        boomerangTimer = 0f;

        LaunchBoomerang();
    }

    public void Tick()
    {
        boomerangTimer += Time.deltaTime;
        if (boomerangTimer >= Player.boomerangTime)
        {
            _player.isBoomeranging = false;
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

    #region Private Methods

    private void BarkEffect() // Animates the bark attack by spawning an effect
    {
    }

    private void LaunchBoomerang()
    {
        GameObject boomerang = GameObject.Instantiate(_player.boomerangPrefab, _player.projectileFirePoint.position, _player.projectileFirePoint.rotation);

        boomerang.GetComponent<Rigidbody2D>().velocity =  _player.GetPlayerDir() * Player.boomerangStartSpeed;
    }
    #endregion
}
