using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public new string name;
    [Range(0f, 10f)]
    [Tooltip("Attack per second. If set to zero should not attack automatically.")]
    public float attackSpeed = 1;
    private float _lastAttackTime;

    protected virtual void Awake()
    {
        if (string.IsNullOrEmpty(name))
            name = gameObject.name;
    }


    public void Attack(Vector3 target)
    {
        if (CanAttack)
        {
            OnAttacking(target, _lastAttackTime);
            _lastAttackTime = Time.time;
        }
    }

    /// <summary>
    /// Actual attack method which should be implemented
    /// </summary>
    /// <param name="target">Target vector</param>
    /// <param name="prevAttackTime">Previous attack time recorded from Time.time</param>
    protected abstract void OnAttacking(Vector3 target, float prevAttackTime);

    /// <summary>
    /// Intervals between attacks, calculated from attackSpeed
    /// </summary>
    public float AttackInterval
    {
        get
        {
            return attackSpeed > 0 ? 1 / attackSpeed : 0;
        }
    }

    /// <summary>
    /// Time remaining for ability to a new attack in seconds
    /// </summary>
    public float AttackTimeRemaining
    {
        get
        {
            return attackSpeed > 0 ? AttackInterval + _lastAttackTime - Time.time : 0;
        }
    }

    /// <summary>
    /// Can weapon attack, Attack method checks this
    /// </summary>
    public virtual bool CanAttack
    {
        get { return !(AttackTimeRemaining > 0); }
    }

    public bool IsAutomatic
    {
        get { return attackSpeed > 0; }
    }
}
