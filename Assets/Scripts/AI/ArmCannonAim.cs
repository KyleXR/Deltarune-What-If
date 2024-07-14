using UnityEngine;

public class ArmCannonAim : MonoBehaviour
{
    public GameObject player;
    public GameObject armCannon;  // The transform of the arm cannon
    public ConfigurableJoint armJoint;

    public Vector3 armUp = Vector3.up;

    private Quaternion startLocalRotation;

    void Start()
    {
        // Cache the start local rotation of the arm joint
        startLocalRotation = armJoint.transform.localRotation;

        // Set up the joint to behave like a CharacterJoint
        armJoint.SetupAsCharacterJoint();

        // Configure the joint drives
        JointDrive drive = new JointDrive
        {
            positionSpring = 15000f,
            positionDamper = 50f,
            maximumForce = Mathf.Infinity
        };

        armJoint.angularXDrive = drive;
        armJoint.angularYZDrive = drive;
        armJoint.slerpDrive = drive;
    }

    void Update()
    {
        AimAtPlayer();
    }

    void AimAtPlayer()
    {
        if (player == null || armCannon == null || armJoint == null)
            return;

        // Calculate the direction to the player
        Vector3 directionToPlayer = player.transform.position - armCannon.transform.position;
        Vector3 localDirectionToPlayer = transform.InverseTransformDirection(directionToPlayer);

        // Calculate the rotation needed to aim at the player
        Quaternion targetRotation = Quaternion.LookRotation(localDirectionToPlayer, armUp);

        // Convert the target rotation from world space to local space
        //Quaternion targetLocalRotation = Quaternion.Inverse(armJoint.transform.rotation) * targetRotation;

        // Apply the target local rotation to the joint
        //armJoint.SetTargetRotationLocal(Quaternion.Inverse(targetRotation), startLocalRotation);
        armJoint.targetRotation = targetRotation;
        // Debug drawing
        DebugDraw(localDirectionToPlayer, targetRotation);
    }

    void DebugDraw(Vector3 directionToPlayer, Quaternion targetRotation)
    {
        // Draw the direction to the player
        Debug.DrawLine(armCannon.transform.position, player.transform.position, Color.red);

        // Draw the current direction of the arm cannon
        Debug.DrawRay(armCannon.transform.position, armCannon.transform.up * 2, Color.green);

        // Draw the target direction of the arm cannon
        Vector3 targetDirection = targetRotation * Vector3.forward;
        Debug.DrawRay(armCannon.transform.position, targetDirection * 2, Color.blue);

        // Draw the target rotation as set in the joint
        Quaternion currentJointRotation = armJoint.transform.rotation;
        Vector3 jointTargetDirection = currentJointRotation * Vector3.up;
        Debug.DrawRay(armCannon.transform.position, jointTargetDirection * 2, Color.yellow);
    }
}
