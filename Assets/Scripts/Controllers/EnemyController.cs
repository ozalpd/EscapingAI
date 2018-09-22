using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : NavAgentController
{
    [Tooltip("VFX game object when enemy attacks by contact to player.")]
    public GameObject contactImpact;
    private int _contactImpactId;

    private ContactDamager contactDamager;

    protected override void Awake()
    {
        base.Awake();

        contactDamager = GetComponent<ContactDamager>();
        if (contactImpact != null)
        {
            _contactImpactId = contactImpact.GetInstanceID();
            ObjectPool.GetOrInitPool(contactImpact);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player") && contactDamager != null)
        {
            if (contactDamager.attackSpeed > 0)
            {
                InvokeRepeating("Attack", contactDamager.AttackTimeRemaining, contactDamager.AttackInterval);
            }
            else
            {
                Attack();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player") && contactDamager != null)
        {
            CancelInvoke("Attack");
        }
    }

    private void OnDestroy()
    {
        CancelInvoke("Attack");
    }

    private void OnDisable()
    {
        CancelInvoke("Attack");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player") && contactDamager != null)
        {
            if (contactDamager.attackSpeed > 0)
            {
                InvokeRepeating("Attack", contactDamager.AttackTimeRemaining, contactDamager.AttackInterval);
            }
            else
            {
                Attack();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player") && contactDamager != null)
        {
            CancelInvoke("Attack");
        }
    }


    protected virtual void Attack()
    {
        if (contactDamager != null)
        {
            contactDamager.Attack(transform.forward);
            if (contactImpact != null)
                ObjectPool.GetInstance(_contactImpactId, Target.position.With(y: 1));
        }
    }
}
