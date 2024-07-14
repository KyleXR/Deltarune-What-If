using UnityEngine;

public class AimAtPlayer : MonoBehaviour
{
    public GameObject player; // Reference to the player
    public ConfigurableJoint armJoint; // Reference to the configurable joint on the arm
    public GameObject arm; // Reference to the arm's transform
    public Vector3 armUp = Vector3.forward; // This should point along the arm's positive Z-axis

    private Quaternion initialLocalRotation;

    private void Start()
    {
        // Save the initial local rotation of the joint
        initialLocalRotation = arm.transform.localRotation;

        // Configure the joint drives
        JointDrive drive = new JointDrive
        {
            positionSpring = 1500f,
            positionDamper = 100f,
            maximumForce = Mathf.Infinity
        };

        armJoint.angularXDrive = drive;
        armJoint.angularYZDrive = drive;
        armJoint.slerpDrive = drive;
    }

    void Update()
    {
        if (player == null || armJoint == null || arm == null)
        {
            return;
        }

        // Calculate direction to the player
        Vector3 directionToPlayer = player.transform.position - arm.transform.position;

        // Calculate the rotation required to aim at the player
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, armUp);

        DebugDraw(targetRotation, Color.red);

        // Convert world space rotation to local space
        Quaternion localRotation = Quaternion.Inverse(arm.transform.parent.rotation) * targetRotation;

        DebugDraw(localRotation, Color.green);
        // Set the target rotation for the configurable joint in local space
        armJoint.SetTargetRotationLocal(localRotation, initialLocalRotation);
    }

    void DebugDraw(Quaternion targetRotation, Color color)
    {
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(targetRotation);

        // Calculate direction of the ray (assuming local space Z-axis is the direction)
        Vector3 direction = rotationMatrix.MultiplyVector(Vector3.up).normalized;

        // Draw debug ray
        Debug.DrawRay(arm.transform.position, direction * 3, color);
        Debug.DrawLine(arm.transform.position, player.transform.position);
    }
}
