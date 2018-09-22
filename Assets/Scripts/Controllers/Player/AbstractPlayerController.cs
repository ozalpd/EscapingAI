using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PlayerMoveMechanism
{
    SetPositionAndAccelerate = 0,
    SetPosition = 10,
    SetVelocity = 20,
    AddForce = 30
}

public abstract class AbstractPlayerController : MonoBehaviour
{
    [Range(1f, 200f)]
    public float speed = 5f;
    public PlayerMoveMechanism moveMechanism;

    public Weapon defaultWeapon;
    public Transform weaponPosition;

    protected List<Weapon> weapons;

    protected virtual void Awake()
    {
        if (weaponPosition == null)
            weaponPosition = transform.Find("WeaponPosition");

        weapons = new List<Weapon>();
    }

    protected virtual void Start()
    {
        if (defaultWeapon != null)
            SwitchWeapon(defaultWeapon);
    }

    public Vector3? AimAt
    {
        get { return _aimAt; }
        set
        {
            if (_aimAt != value)
            {
                _aimAt = value;
                if (_aimAt.HasValue)
                    OnAimedAt(_aimAt.Value);
                else
                    OnAimToIdle();
            }
        }
    }
    private Vector3? _aimAt = null;
    protected virtual void OnAimedAt(Vector3 target) { }
    protected virtual void OnAimToIdle() { }

    public virtual bool Attacking
    {
        get { return _isAttacking; }
        set
        {
            if (_isAttacking != value)
            {
                _isAttacking = value;

                if (_isAttacking)
                    OnAttacking();
                else
                    OnAttackStopped();
            }
        }
    }
    private bool _isAttacking = false;
    protected virtual void OnAttacking() { }
    protected virtual void OnAttackStopped() { }


    public abstract void GetIdle();

    /// <summary>
    /// Moves player
    /// </summary>
    /// <param name="movement"></param>
    /// <param name="deltaTime">If this method called from Fixed update this should be Time.fixedDeltaTime, otherwise Time.deltaTime</param>
    public abstract void Move(Vector3 movement);

    public abstract bool Shielding { get; set; }

    public virtual void SwitchWeapon(Weapon weapon)
    {
        if (Weapon != null && Weapon.name.Equals(weapon.name))
            return;

        var inListWeapon = weapons.FirstOrDefault(w => w.name.Equals(weapon.name));
        if (weaponPosition == null)
            weaponPosition = transform;

        bool isAttacking = Attacking;
        if (Weapon != null && Attacking)
            Attacking = false;

        if (inListWeapon != null)
        {
            inListWeapon.gameObject.SetActive(true);
            Weapon = inListWeapon;
        }
        else
        {
            var weaponGO = Instantiate(weapon.gameObject, weaponPosition);
            weaponGO.transform.localRotation = Quaternion.identity;
            weaponGO.transform.localPosition = Vector3.zero;

            Weapon = weaponGO.GetComponent<Weapon>();
        }

        Attacking = isAttacking;
        //override this method if we need not to continue to attack after switching
    }
    public Weapon Weapon { get; protected set; }

    /// <summary>
    /// Resets properties, variables and transformations.
    /// </summary>
    public virtual void ResetPlayer()
    {
        ResetValues();
    }
    /// <summary>
    /// Sets properties and variables to default values.
    /// </summary>
    public abstract void ResetValues();
}