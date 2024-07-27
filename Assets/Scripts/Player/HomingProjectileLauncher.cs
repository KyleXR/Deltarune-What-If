using UnityEngine;

public class HomingProjectileLauncher : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform launchPoint;
    public TargetingLogic targetingLogic;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Example: Left mouse button to launch
        {
            LaunchProjectile();
        }
    }

    void LaunchProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, launchPoint.rotation);
        HomingProjectile homingProjectile = projectile.GetComponent<HomingProjectile>();

        if (homingProjectile != null)
        {
            homingProjectile.target = targetingLogic.GetCurrentTarget();
        }
    }
}
