using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public enum AttackType
    {
        Once,
        Continuous
    }

    public AttackType attackType;
    public float damage = 10f;
    //public float burstInterval = 0.2f; // Interval between burst attacks
    //public float continuousDuration = 2f; // Duration of continuous attack
    //public float attackRange = 10f; // Range of the attack (used for initialization)

    //private float attackEndTime = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (attackType == AttackType.Continuous)
        {
            ApplyDamage(other);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ApplyDamage(other);
    }

    //void Update()
    //{
    //    if (attackType == AttackType.Continuous)
    //    {
    //        // Continuous attack logic
    //        if (Time.time > attackEndTime)
    //        {
    //            StopAttack();
    //        }
    //    }
    //}

    //public void StartAttack()
    //{
    //    switch (attackType)
    //    {
    //        case AttackType.Once:
    //            ApplyDamageToTargets();
    //            break;
    //        case AttackType.Burst:
    //            StartCoroutine(BurstAttack());
    //            break;
    //        case AttackType.Continuous:
    //            StartContinuousAttack();
    //            break;
    //    }
    //}

    //void ApplyDamageToTargets()
    //{
    //    Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
    //    foreach (var hitCollider in hitColliders)
    //    {
    //        ApplyDamage(hitCollider);
    //    }
    //}

    void ApplyDamage(Collider collider)
    {
        Debug.Log(collider.name);
        Health health = collider.GetComponent<Health>();
        if (health == null) { health = collider.GetComponentInParent<Health>(); }
        if (health != null)
        {
            health.TakeDamage(damage, collider.ClosestPoint(transform.position));
        }
    }

    //IEnumerator BurstAttack()
    //{
    //    for (int i = 0; i < 3; i++) // Example burst count
    //    {
    //        ApplyDamageToTargets();
    //        yield return new WaitForSeconds(burstInterval);
    //    }
    //}

    //void StopAttack()
    //{
    //    isAttacking = false;
    //}
}
