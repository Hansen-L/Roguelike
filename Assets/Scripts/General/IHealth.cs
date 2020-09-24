using UnityEngine;

public interface IHealth
{
    void TakeDamage(int damageAmount);
    void GainHealth(int healAmount);
    bool IsDead();
}