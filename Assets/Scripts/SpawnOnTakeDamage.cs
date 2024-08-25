using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class SpawnOnTakeDamage : MonoBehaviour
{
    private Health health;
    [SerializeField] GameObject spawnPrefab;
    [SerializeField] float minDamage = 1;

    void Start()
    {
        health = GetComponent<Health>();
        health.OnTakeDamage += OnPlayerDamaged;
    }

    void OnPlayerDamaged(float damage)
    {
        if (damage >= minDamage) Instantiate(spawnPrefab, transform);
    }
}
