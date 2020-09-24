using UnityEngine;

public interface IHealth
{
    int GetHealth();
    void TakeDamage(int damageAmount);
    void GainHealth(int healAmount);
    bool IsDead();
}