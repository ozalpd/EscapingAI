using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : Weapon
{
    public Projectile projectile;

    [Range(5f, 50f)]
    public float firingRange = 20;
    public float projectileSpeed = 12;

    private int _projectileId;

    protected override void Awake()
    {
        _projectileId = projectile.gameObject.GetInstanceID();
        ObjectPool.GetOrInitPool(projectile.gameObject);
    }

    public Projectile loadedProjectile
    {
        get { return _loadedProjectile; }
    }
    protected Projectile _loadedProjectile = null;


    protected override void OnAttacking(Vector3 target, float prevAttackTime)
    {
        var go = ObjectPool.GetInstance(_projectileId, transform.position, transform.rotation);
        var p = go.GetComponent<Projectile>();
        p.range = firingRange;
        p.speed = projectileSpeed;
    }
}