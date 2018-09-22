using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contact damager weapon against player, decreases GameManager.Health.
/// </summary>
public class ContactDamager : Weapon
{
    [Range(1, 100)]
    public int damageFactor = 5;

    protected override void OnAttacking(Vector3 target, float prevAttackTime)
    {
        GameManager.Health -= damageFactor;
    }
}
