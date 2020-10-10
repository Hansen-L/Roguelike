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
        if (!_player.canBoomerang) // If the player can't throw the boomerang, exit this state
            _player.isBoomeranging = false;
        else
        {
            _player.canBoomerang = false;
            //AudioManager.Instance.Play("run");
            _animator.SetTrigger("attack"); // TODO: Replace with projectile animation

            boomerangTimer = 0f;

            LaunchBoomerang();
        }
    }

    public void Tick()
    {
        boomerangTimer += Time.deltaTime;
        if (boomerangTimer >= Player.BoomerangTime)
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

    private void LaunchBoomerang()
    {
        GameObject boomerangObject = GameObject.Instantiate(_player.boomerangPrefab, _player.projectileFirePoint.position, _player.projectileFirePoint.rotation);
        boomerangObject.GetComponent<Boomerang>().SetPlayer(_player); // Pass the player object to the boomerang script
        Rigidbody2D boomerangRb = boomerangObject.GetComponent<Rigidbody2D>();
        boomerangRb.AddTorque(Player.BoomerangTorque);
        _player.SetColorToBlack(boomerangObject);

        // Calculate the unit vector towards the mouse, launch boomerang in that direction
        Vector2 mousePosition = _player.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 launchDir = (mousePosition - boomerangRb.position).normalized;
        _player.StartChildCoroutine(BoomerangPath(launchDir, boomerangRb, boomerangObject));
    }


    IEnumerator BoomerangPath(Vector2 launchDir, Rigidbody2D boomerangRb, GameObject boomerangObject) // Make the boomerang return to the player
    {
        float distToPlayer;
        float returningMaxDistDelta = 0; // Change in position when boomerang is returning

        if (_player.isShadow)
            boomerangRb.velocity = launchDir * Player.BoomerangStartSpeedShadow; // Initial velocity
        else
            boomerangRb.velocity = launchDir * Player.BoomerangStartSpeed;

        while (boomerangRb)
        {
            distToPlayer = Vector2.Distance(boomerangRb.position, _rb.position);

            if (Vector2.Dot(boomerangRb.velocity, launchDir) >= -0.5) // When the boomerang is being thrown
            {
                boomerangRb.velocity -= launchDir / Player.BoomerangSlowdownFactor;
                yield return new WaitForFixedUpdate();
            }
            else // When the boomerang is returning
            {
                returningMaxDistDelta += Player.BoomerangReturnAcceleration;
                boomerangRb.position = Vector2.MoveTowards(boomerangRb.position, _rb.position, Time.deltaTime*returningMaxDistDelta);

                // Destroy boomerang when it is returned
                if (distToPlayer < 1f)
                {
                    GameObject.Destroy(boomerangObject);
                    _player.canBoomerang = true;
                }

                yield return new WaitForFixedUpdate();
            }

        }
    }
    
    #endregion
}
