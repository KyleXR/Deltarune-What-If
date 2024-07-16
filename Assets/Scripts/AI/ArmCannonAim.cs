using System.Collections.Generic;
using UnityEngine;

public class ArmCannonAim : MonoBehaviour
{
    public GameObject playerTransform;
    public GameObject targetTransform;

    public ArmatureHandler ragdollBones;
    public ArmatureHandler ikBones;
    public ConfigurableJoint[] joints;
    private Rigidbody[] rb;
    public JointDrive aimDrive;
    public JointDrive idleDrive;
    private Quaternion[] startingRotations;

    public bool isAiming = false;

    private void Start()
    {
        startingRotations = new Quaternion[joints.Length];
        rb = new Rigidbody[joints.Length];
        for (int i = 0; i < joints.Length; i++)
        {
            if (joints[i] != null)
            {
                startingRotations[i] = joints[i].transform.localRotation;
                rb[i] = joints[i].GetComponent<Rigidbody>();
            }
        }
        idleDrive = new JointDrive
        {
            positionSpring = 0,
            positionDamper = 0,
            maximumForce = 0
        };
        aimDrive = new JointDrive
        {
            positionSpring = 1000,
            positionDamper = 250,
            maximumForce = 1500
        };
    }

    private void Update()
    {
        if (isAiming)
        {
            targetTransform.transform.position = playerTransform.transform.position + Vector3.up;
            targetTransform.transform.rotation = Quaternion.LookRotation(transform.position - playerTransform.transform.position);

            ikBones.CopyArmatureTransform(ragdollBones);
            //ikBones[0].transform.rotation = ragdollBones[0].transform.rotation;//Shoulder
            for (int i = 0; i < joints.Length; i++)
            {
                joints[i].SetTargetRotationLocal(ikBones.armBones[i].transform.localRotation, startingRotations[i]);
            }
        }
    }
    public void EnableAiming(bool enable = true)
    {
        isAiming = enable;
        if (isAiming)
        {
            for (int i = 0; i < joints.Length; i++)
            {
                if (joints[i] != null)
                {
                    joints[i].angularXDrive = aimDrive;
                    joints[i].angularYZDrive = aimDrive;
                    joints[i].slerpDrive = aimDrive;
                }
                if (rb[i] != null) rb[i].useGravity = false;
            }
        }
        else
        {
            for (int i = 0; i < joints.Length; i++)
            {
                if (joints[i] != null)
                {
                    joints[i].angularXDrive = idleDrive;
                    joints[i].angularYZDrive = idleDrive;
                    joints[i].slerpDrive = idleDrive;
                }
                if (rb[i] != null) rb[i].useGravity = true;
            }
        }
    }
    
}
