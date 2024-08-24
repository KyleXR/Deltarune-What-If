using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Attack))]
public abstract class NEO_Attack : MonoBehaviour
{
    public int ID = 0;
    public int baseSpawnAmount = 1;
    public float urgency = 0;
    public float damage = 10;
    public bool ignoresInvulability = false;
    public bool isUnique = false; //if unique, then cannot spawn multiple copies at same time
    public float duration = 10; //probably will only use for special attacks
    public AttackType attackType = AttackType.Cannon;
    protected Transform spawnTransform;
    protected Transform targetTransform;
    protected NEO_AttackHandler handler;

    public Vector3 spawnBounds = Vector3.zero;
    public bool randomSpawn = false;

    // string targetTag = "Player";
    private Attack attack;

    private void Start()
    {
        attack = GetComponent<Attack>();
        attack.damage = damage;
        if (attack.targetTags.Length == 0 || attack.targetTags == null)
        {
            attack.targetTags = new string[1];
            attack.targetTags[0] = "Player";
        }
        attack.ignoresDamageCooldown = ignoresInvulability;
    }

    public virtual void InitializeAttack(NEO_AttackHandler handler, Transform spawnTransform, Transform targetTransform, float currentUrgency = 0)
    {
        urgency = currentUrgency;
        this.spawnTransform = spawnTransform;
        this.targetTransform = targetTransform;
        this.handler = handler;

        if (randomSpawn)
        {
            Vector3 tempPos = new Vector3(Random.Range(-spawnBounds.x, spawnBounds.x), Random.Range(-spawnBounds.y, spawnBounds.y), Random.Range(-spawnBounds.z, spawnBounds.z));
            transform.localPosition = tempPos;
        }
    }
    public void SetTarget(Transform target)
    {
        targetTransform = target;
    }
    public enum AttackType
    {
        Cannon,
        BulletPattern,
        Special
    }
    public void OnDestroy()
    {
        if(handler != null) handler.RemoveAttack(this);
    }
}
