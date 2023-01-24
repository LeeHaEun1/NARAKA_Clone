using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSword : MonoBehaviour
{
    public Transform swordBone;
    public Transform backBonePos;
    public Transform HandBonePos;
    void AttachToBack()
    {
        swordBone.parent = backBonePos;
        swordBone.localPosition = Vector3.zero;
        swordBone.localRotation = Quaternion.identity;
    }

    void AttachToHand()
    {
        swordBone.parent = HandBonePos;
        swordBone.localPosition = Vector3.zero;
        swordBone.localRotation = Quaternion.identity;
    }
}
