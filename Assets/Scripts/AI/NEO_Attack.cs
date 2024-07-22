using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NEO_Attack : MonoBehaviour
{
    public int ID = 0;
    public int baseSpawnAmount = 1;
    public float urgency = 0;
    public float damage = 10;
    public bool ignoresInvulability = false;
    public bool isUnique = false; //if unique, then cannot spawn multiple copies at same time
    public AttackType attackType = AttackType.Cannon;
    protected Transform spawnTransform;
    protected Transform targetTransform;
    protected NEO_AttackHandler handler;

    public Vector3 spawnBounds = Vector3.zero;
    public bool randomSpawn = false;

    public virtual void InitializeAttack(NEO_AttackHandler handler, Transform spawnTransform, Transform targetTransform, float currentUrgency = 0)
    {
        urgency = currentUrgency;
        this.spawnTransform = spawnTransform;
        this.targetTransform = targetTransform;
        this.handler = handler;

        if (randomSpawn)
        {
            Vector3 tempPos = new Vector3(Random.Range(-spawnBounds.x, spawnBounds.x), Random.Range(-spawnBounds.y, spawnBounds.y), Random.Range(-spawnBounds.z, spawnBounds.z));
            transform.position = tempPos;
        }
    }
    public enum AttackType
    {
        Cannon,
        BulletPattern
    }
    public void OnDestroy()
    {
        if(handler != null) handler.RemoveAttack(this);
    }
}
