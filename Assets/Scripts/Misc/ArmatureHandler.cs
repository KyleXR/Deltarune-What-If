using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmatureHandler : MonoBehaviour
{
    public GameObject parentObject;
    public GameObject[] bones;
    public GameObject[] armBones;

    public void CopyArmatureTransform(ArmatureHandler other)
    {
        //parentObject.transform.position = bones[1].transform.position;
        //bones[1].transform.position = Vector3.zero;
        for (int i = 0; i < bones.Length; i++)
        {
            bones[i].transform.position = other.bones[i].transform.position;
            bones[i].transform.rotation = other.bones[i].transform.rotation;
        }
        armBones[0].transform.position = other.armBones[0].transform.position;

    }
}
