using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBall : NEO_Attack
{
    [SerializeField] private GameObject attackPrefab;
    public float pulseDuration = 1f;   // Duration of one pulse cycle
    public float maxScale = 2f;        // Maximum scale factor
    public float minScale = 1f;        // Minimum scale factor
    public float attackPulseScale = 4f; // Scale factor during attack pulse
    public int initialPulses = 4;
    public int spawnPulses = 2;
    public int attackAmount = 2;
    public int attackRange = 5;
    public float maxSpeed = 2f;        // Maximum speed of movement
    public float forceMagnitude = 10f; // Magnitude of the force to be applied to the projectiles
    private bool isFiring = false;
    private bool isAttacking = false;
    public bool cannonFire = true;
    private Vector3 originalScale = Vector3.zero;
    private Vector3 targetPosition = Vector3.zero;
    private Vector3 originalLocalPosition = Vector3.zero;
    private float originalDistance = 0;
    private Rigidbody rb; // Reference to the Rigidbody component

    void Start()
    {
        originalScale = transform.localScale;
        
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        originalLocalPosition = transform.localPosition;
        originalDistance = Vector3.Distance(transform.position, targetPosition);
        transform.rotation = Quaternion.identity;

        UpdateTargetPosition();

        if (cannonFire) StartCoroutine(ChargeUp());
        else
        {
            StartCoroutine(PulseScale());
            ApplyImpulseToTarget(targetPosition);
        }
    }

    private void Update()
    {
        if (!isFiring && cannonFire) transform.position = spawnTransform.position;

        if (Vector3.Distance(transform.localPosition, originalLocalPosition) >= originalDistance)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public override void InitializeAttack(NEO_AttackHandler handler, Transform spawnTransform, Transform targetTransform, float currentUrgency = 0)
    {
        base.InitializeAttack(handler, spawnTransform, targetTransform, currentUrgency);
    }

    IEnumerator ChargeUp()
    {
        if (cannonFire)
        {
            handler.UpdateCannonAim(targetPosition, false);
            yield return StartCoroutine(LerpScale(0, minScale, pulseDuration * 2));
        }
        else
        {
            StartCoroutine(LerpScale(0, minScale, pulseDuration * 2));
        }
        StartCoroutine(PulseScale());
        yield return new WaitForSeconds(1);
        if (cannonFire) handler.StopAiming();
        isFiring = true;
        transform.rotation = Quaternion.identity;

        UpdateTargetPosition();

        ApplyImpulseToTarget(targetPosition);
        if (cannonFire) Invoke("FinishFiring", 2);
    }

    IEnumerator PulseScale()
    {
        int currentPulses = 0;
        int attackCount = 0;

        while (attackCount < attackAmount)
        {
            // Scale up
            yield return StartCoroutine(LerpScale(minScale, maxScale, pulseDuration));

            // If it's time to spawn an attack
            if (currentPulses >= spawnPulses && isAttacking && attackCount < attackAmount)
            {
                // Additional pulse for attack spawn
                yield return StartCoroutine(LerpScale(maxScale, attackPulseScale, pulseDuration * 0.5f));
                SpawnAttacks(6);
                attackCount++;
                yield return StartCoroutine(LerpScale(attackPulseScale, maxScale, pulseDuration * 0.5f));
            }

            // Scale down
            yield return StartCoroutine(LerpScale(maxScale, minScale, pulseDuration));

            if (currentPulses >= initialPulses && !isAttacking)
            {
                isAttacking = true;
                currentPulses = 0;
            }

            if (currentPulses >= spawnPulses && isAttacking)
            {
                currentPulses = 0;
            }

            currentPulses++;
        }
        yield return StartCoroutine(LerpScale(minScale, 0, pulseDuration * 2));
        Destroy(this.gameObject);
    }

    IEnumerator LerpScale(float startScale, float endScale, float duration)
    {
        float currentScale = 0;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            currentScale = Mathf.Lerp(startScale, endScale, t / duration);
            transform.localScale = originalScale * currentScale;
            yield return null;
        }
        transform.localScale = originalScale * endScale; // Ensure the scale is set to the final value
    }

    void ApplyImpulseToTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.AddForce(direction * forceMagnitude, ForceMode.Impulse);
    }

    // Method to spawn multiple attacks with evenly spread starting rotations
    void SpawnAttacks(int amount)
    {
        float angleStep = 360f / amount;
        float angle = 0f;

        for (int i = 0; i < amount; i++)
        {
            // Calculate the direction of the attack in local space
            float attackDirX = Mathf.Sin((angle * Mathf.PI) / 180f);
            float attackDirY = 0;
            float attackDirZ = Mathf.Cos((angle * Mathf.PI) / 180f);

            Vector3 attackDirectionLocal = new Vector3(attackDirX, attackDirY, attackDirZ);

            // Transform the local direction to world space
            Vector3 attackDirection = transform.TransformDirection(attackDirectionLocal);

            // Instantiate the attack prefab with the calculated rotation
            GameObject attack = Instantiate(attackPrefab, transform.position, Quaternion.LookRotation(attackDirection));
            attack.transform.parent = transform.parent;

            // Apply force to the Rigidbody of the attack prefab with deceleration
            Rigidbody attackRb = attack.GetComponent<Rigidbody>();
            attackRb.AddForce(attackDirection * forceMagnitude, ForceMode.Impulse);

            // Increment the angle for the next attack
            angle += angleStep;
        }
        transform.Rotate(Vector3.up, angleStep * 0.5f);
    }

    void FinishFiring()
    {
        if (cannonFire)
        {
            handler.StopAiming();
            handler.ToggleCannon(true, false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground")) rb.velocity = Vector3.zero;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) rb.velocity = Vector3.zero;
    }

    public void UpdateTargetPosition()
    {
        targetPosition = new Vector3(Random.Range(-spawnBounds.x, spawnBounds.x), Random.Range(-spawnBounds.y, spawnBounds.y) + 1, Random.Range(-spawnBounds.z, spawnBounds.z));
        targetPosition += targetTransform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 0.5f);
    }
}
