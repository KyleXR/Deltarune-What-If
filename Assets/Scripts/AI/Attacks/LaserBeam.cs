using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class LaserBeam : NEO_Attack
{
    [SerializeField] private GameObject laserCharge;
    [SerializeField] private GameObject laserBeam;
    [SerializeField] private float laserLength = 10;
    [SerializeField] private float laserSpeed = 10;
    [SerializeField] private float laserDuration = 2;
    [SerializeField] private Vector3 directionAxis = Vector3.forward;
    [SerializeField] private float pulseDuration = 1f; // Duration of each pulse
    [SerializeField] private int pulseCount = 3; // Number of pulses
    [SerializeField] private bool isShooting = false;
    [SerializeField] private Transform aimTransform;
    [SerializeField] List<GameObject> fxPrefabs;

    [SerializeField] private bool checkForGround = true;
    //private Transform spawnTransform;
    private int laserComponents = 2;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShootLaser());
    }

    private void Update()
    {
        if (spawnTransform != null && targetTransform != null)
        {
            transform.position = spawnTransform.position;
            transform.LookAt(aimTransform);
        }
    }

    public override void InitializeAttack(NEO_AttackHandler handler, Transform spawnTransform, Transform targetTransform, float currentUrgency = 0)
    {
        base.InitializeAttack(handler, spawnTransform, targetTransform, currentUrgency);
        this.aimTransform.parent = targetTransform.parent;
        this.aimTransform.position = targetTransform.position;
        //Physics.Raycast(spawnTransform.position, targetTransform.position - spawnTransform.position, out hit, laserLength);
        RaycastHit hit;
        int layerMask = 1 << 0;
        if (Physics.Raycast(spawnTransform.position, targetTransform.position - spawnTransform.position, out hit, laserLength, layerMask))
        {
            Debug.Log("Hit");
            if (checkForGround) laserLength = hit.distance;
            aimTransform.position = hit.transform.position;
            //handler.UpdateCannonAim(hit.transform.position, false);
        }
        //else laserLength = Vector3.Distance(spawnTransform.position, target.position);
        transform.LookAt(aimTransform.position);
        handler.UpdateCannonAim(aimTransform.position, true);
    }

    // Coroutine to pulse the laser charge
    IEnumerator GrowLaserCharge()
    {
        var fx = Instantiate(fxPrefabs[0], laserCharge.transform);
        fx.transform.position = laserCharge.transform.position;
        Vector3 originalScale = laserCharge.transform.localScale;
        Vector3 prevScale = Vector3.zero;
        Vector3 maxScale = originalScale * 1.5f; // Maximum scale during the pulse

        laserCharge.transform.localScale = Vector3.zero;
        float growDuration = pulseDuration * pulseCount; // Total time to grow to the original size

        // Calculate the scale increments
        Vector3[] pulseScales = new Vector3[pulseCount];
        for (int i = 0; i < pulseCount; i++)
        {
            pulseScales[i] = originalScale * ((i + 1) / (float)pulseCount); // Incremental scale up to maxScale
        }

        // Pulse effect
        for (int i = 0; i < pulseCount; i++)
        {
            float elapsedTime = 0f;
            // Scale up
            while (elapsedTime < pulseDuration / 2)
            {
                float progress = elapsedTime / (pulseDuration / 2);
                laserCharge.transform.localScale = Vector3.Lerp(prevScale, pulseScales[i], progress);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            // Scale down
            elapsedTime = 0f;
            while (elapsedTime < pulseDuration / 2)
            {
                float progress = elapsedTime / (pulseDuration / 2);
                laserCharge.transform.localScale = Vector3.Lerp(pulseScales[i], i > 0 ? pulseScales[i - 1] : Vector3.zero, progress);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            prevScale = laserCharge.transform.localScale;
        }

        // Ensure the laser charge returns to its original scale
        laserCharge.transform.localScale = originalScale;

        Destroy(fx);
        // Start pulsing the laser charge
        StartCoroutine(PulseLaserCharge());
    }

    IEnumerator PulseLaserCharge()
    {
        Vector3 originalScale = laserCharge.transform.localScale;
        Vector3 pulseScale = originalScale * 1.5f;

        // Pulse effect
        while (isShooting)
        {
            float elapsedTime = 0f;
            // Scale up
            while (elapsedTime < pulseDuration / 2)
            {
                float progress = elapsedTime / (pulseDuration / 2);
                laserCharge.transform.localScale = Vector3.Lerp(originalScale, pulseScale, progress);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            // Scale down
            elapsedTime = 0f;
            while (elapsedTime < pulseDuration / 2)
            {
                float progress = elapsedTime / (pulseDuration / 2);
                laserCharge.transform.localScale = Vector3.Lerp(pulseScale, originalScale, progress);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        DestroyComponent(0);
    }

    IEnumerator PulseLaserBeam()
    {
        Vector3 originalScale = laserBeam.transform.localScale;
        Vector3 pulseAxis = (Vector3.one - directionAxis) * 1.25f;
        Vector3 pulseScale = Vector3.Scale(originalScale, pulseAxis + directionAxis);

        // Pulse effect
        while (isShooting)
        {
            float elapsedTime = 0f;
            // Scale up
            while (elapsedTime < pulseDuration / 2 && isShooting)
            {
                float progress = elapsedTime / (pulseDuration / 2);
                laserBeam.transform.localScale = Vector3.Lerp(originalScale, pulseScale, progress);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            // Scale down
            elapsedTime = 0f;
            while (elapsedTime < pulseDuration / 2)
            {
                float progress = elapsedTime / (pulseDuration / 2);
                laserBeam.transform.localScale = Vector3.Lerp(pulseScale, isShooting ? originalScale : Vector3.zero, progress);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    // Coroutine to scale the laser
    IEnumerator ShootLaser()
    {
        isShooting = true;

        // Calculate the target and midpoint positions
        Vector3 targetPosition = aimTransform.position;
        //Debug.Log(targetPosition);
        Vector3 midpointPosition = (spawnTransform.position + targetPosition) / 2f;

        // Convert the positions to local space
        Vector3 localTargetPosition = spawnTransform.InverseTransformPoint(targetPosition);
        Vector3 localMidpointPosition = spawnTransform.InverseTransformPoint(midpointPosition);

        Vector3 targetScale = (directionAxis * laserLength * 0.5f) + Vector3.Scale(laserBeam.transform.localScale, Vector3.one - directionAxis);

        laserBeam.transform.localScale = Vector3.zero;
        // Pulse the laser charge
        yield return StartCoroutine(GrowLaserCharge());
        StartCoroutine(PulseLaserCharge());
        handler.UpdateCannonAim(aimTransform.position, false);

        //particeles
        var fx = Instantiate(fxPrefabs[1], laserCharge.transform);
        fx.transform.position = laserCharge.transform.position;
        // Part 1: Grow the laser to the target scale
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            float progress = elapsedTime * laserSpeed;
            laserBeam.transform.localScale = Vector3.Lerp(laserBeam.transform.localScale, targetScale, progress);
            laserBeam.transform.localPosition = Vector3.Lerp(laserBeam.transform.localPosition, localMidpointPosition, progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Part 2: Grow the laser from the midpoint to the target
        //elapsedTime = 0f;
        //while (elapsedTime < 1f)
        //{
        //    float progress = elapsedTime * laserSpeed;
        //    laserBeam.transform.localScale = new Vector3(laserBeam.transform.localScale.x, laserBeam.transform.localScale.y, laserLength / 2f + progress * laserLength / 2f);
        //    laserBeam.transform.localPosition = Vector3.Lerp(localMidpointPosition, localTargetPosition, progress);
        //    elapsedTime += Time.deltaTime;
        //    yield return null;
        //}

        laserBeam.transform.localScale = targetScale;
        laserBeam.transform.localPosition = localMidpointPosition;

        // Maintain laser for the specified duration
        StartCoroutine(PulseLaserBeam());
        yield return new WaitForSeconds(laserDuration);
        StopCoroutine(PulseLaserBeam());
        isShooting = false;

        // Scale down and translate to end
        elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            float progress = elapsedTime * laserSpeed;
            laserBeam.transform.localScale = Vector3.Lerp(laserBeam.transform.localScale, Vector3.zero, progress);
            laserBeam.transform.localPosition = Vector3.Lerp(laserBeam.transform.localPosition, localTargetPosition, progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        laserBeam.transform.localScale = Vector3.zero;
        laserBeam.transform.localPosition = spawnTransform.localPosition;
        DestroyComponent(1);
        Destroy(fx);
    }

    public void DestroyComponent(int id)
    {
        laserComponents--;
        switch (id)
        {
            case 0:
                StopCoroutine(PulseLaserCharge());
                laserCharge.SetActive(false);
                break;
            case 1:
                StopCoroutine(PulseLaserBeam());
                laserBeam.SetActive(false);
                break;
        }
        if (laserComponents <= 0)
        {
            //Debug.Log("Finished");
            handler.StopAiming();
            StopAllCoroutines();
            Destroy(aimTransform.gameObject);
            Destroy(gameObject);
        }
    }
}
