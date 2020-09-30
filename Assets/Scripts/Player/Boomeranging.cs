using UnityEngine;
using System.Collections;

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
        _player.isBoomeranging = false;
        //AudioManager.Instance.Stop("run");
        _animator.ResetTrigger("attack");
    }

    #region Private Methods

    private void BarkEffect() // Animates the bark attack by spawning an effect
    {
    }

    private void LaunchBoomerang()
    {
        GameObject boomerangObject = GameObject.Instantiate(_player.boomerangPrefab, _player.projectileFirePoint.position, _player.projectileFirePoint.rotation);
        Rigidbody2D boomerangRb = boomerangObject.GetComponent<Rigidbody2D>();
        boomerangRb.velocity =  _player.GetPlayerDir() * Player.boomerangStartSpeed;
        boomerangRb.AddTorque(Player.boomerangTorque);

        _player.StartChildCoroutine(BoomerangPath(_player.GetPlayerDir(), boomerangRb));
    }

    IEnumerator BoomerangPath(Vector2 playerDir, Rigidbody2D boomerangRb) // Make the boomerang return to the player
    {
        while (boomerangRb)
        {
            boomerangRb.velocity -= playerDir / Player.boomerangSlowdownFactor;//new Vector2(Mathf.Lerp(boomerangRb.velocity.x, 0, 0.5f), Mathf.Lerp(boomerangRb.velocity.y, 0, 0.5f));
            yield return new WaitForFixedUpdate();
        }
    }
    
    #endregion
}
