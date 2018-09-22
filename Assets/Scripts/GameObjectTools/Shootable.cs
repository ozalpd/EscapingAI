using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes the game object to be shootable by other game objects those are tagged as Projectile.
/// </summary>
public class Shootable : MonoBehaviour
{
    [Tooltip("Point that will be added when player makes a hit to this object.")]
    public int hitPoint = 0;

    [Tooltip("Point that will be added when player kills this object.")]
    public int killPoint = 5;

    [Tooltip("Damage amount that enough to kill this object.")]
    public float maxDamage = 20;

    [Header("Effects")]
    [Tooltip("VFX game object when a hit by a bullet etc.")]
    public GameObject impactVFX;

    [Tooltip("VFX game object when charcter dies.")]
    public GameObject killVFX;

    [Header("Loot System")]
    public PlayerItem lootableItem;
    private int _lootableItemId;
    private GameObject _lootableGO;

    [Range(0f, 1f)]
    [Tooltip("Item drop possibility when object killed. Zero is never. 0.5. 50% chance. One is certain.")]
    public float lootPossibility = 0;

    public delegate void DamageChange(float damage, float maxDamage);

    private int _explosionId;
    private int _impactId;
    private const string tagProjectile = "Projectile";

    private void Awake()
    {
        if (killVFX != null)
        {
            _explosionId = killVFX.GetInstanceID();
            ObjectPool.GetOrInitPool(killVFX);
        }

        if (impactVFX != null)
        {
            _impactId = impactVFX.GetInstanceID();
            ObjectPool.GetOrInitPool(impactVFX);
        }

        if (lootableItem != null)
        {
            _lootableGO = lootableItem.gameObject;
            _lootableItemId = _lootableGO.GetInstanceID();
            ObjectPool.GetOrInitPool(_lootableGO);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckIsHitByProjectile(collision.gameObject, collision.contacts[0].point);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckIsHitByProjectile(collision.gameObject, collision.contacts[0].point);
    }

    public float Damage
    {
        get { return _damage; }
        set
        {
            if (!Mathf.Approximately(_damage, value))
            {
                _damage = value;
            }
            if (DamageChanged != null)
                DamageChanged(_damage, maxDamage);
        }
    }
    protected float _damage = 0;
    public event DamageChange DamageChanged;

    protected virtual void Die(Vector3 contactPoint)
    {
        if (killVFX != null)
        {
            ObjectPool.GetInstance(_explosionId, contactPoint);
        }

        GameManager.Score += killPoint;
        DropItem();
        //DamageChanged = null; //No need to release subscribers when using ObjectPool
        ObjectPool.Release(gameObject);
    }

    protected virtual void DropItem()
    {
        if (lootableItem == null || Mathf.Approximately(lootPossibility, 0))
            return;

        if (Random.Range(0f, 1f) < lootPossibility)
            ObjectPool.GetInstance(_lootableItemId, transform.position, transform.rotation);
    }

    protected virtual void CheckIsHitByProjectile(GameObject collider, Vector3 contactPoint)
    {
        if (!collider.gameObject.tag.Equals(tagProjectile))
            return;

        var p = collider.GetComponent<Projectile>();
        if (p != null)
        {
            Damage += p.damageFactor;
        }
        else
        {
            Debug.LogWarning("Hit by " + collider.name + " which is tagged as '" + tagProjectile + "' but no Projectile component found!");
        }

        ObjectPool.Release(collider.gameObject);

        if (Damage < maxDamage)
        {
            if (hitPoint > 0)
                GameManager.Score += hitPoint;

            if (impactVFX != null)
                ObjectPool.GetInstance(_impactId, contactPoint);
        }
        else
        {
            Die(contactPoint);
        }
    }
}