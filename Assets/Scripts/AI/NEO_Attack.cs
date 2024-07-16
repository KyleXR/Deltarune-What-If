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
    private NEO_AttackHandler handler;
    
    public virtual void InitializeAttack(NEO_AttackHandler handler, Transform spawnTransform, Transform targetTransform, float currentUrgency = 0)
    {
        urgency = currentUrgency;
        this.spawnTransform = spawnTransform;
        this.targetTransform = targetTransform;
        this.handler = handler;
    }
    public enum AttackType
    {
        Cannon,
        BulletPattern
    }
    public void OnDestroy()
    {
        handler.RemoveAttack(this);
    }
}
