using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTargetController : MonoBehaviour
{

    //Hand
    public Transform LeftHandTransform; // Assign the hand bone transform in the Inspector
    public Transform LeftHandTarget;   
     private Vector3 LeftHandInitialOffset;
    private Quaternion LeftHandInitialRotation;     // Assign the target object in the Inspector
   
    public Transform RightHandTransform;
    public Transform RightHandTarget;
    private Vector3 RightHandInitialOffset;
    private Quaternion RightHandInitialRotation;


    //Arm
    public Transform LeftArmTransform; // Assign the hand bone transform in the Inspector
    public Transform LeftArmTarget;   
     private Vector3 LeftArmInitialOffset;
    private Quaternion LeftArmInitialRotation;     // Assign the target object in the Inspector

    public Transform RightArmTransform; // Assign the hand bone transform in the Inspector
    public Transform RightArmTarget;   
     private Vector3 RightArmInitialOffset;
    private Quaternion RightArmInitialRotation;     // Assign the target object in the Inspector
    //Leg
    //Left
    public Transform LeftFootTransform; // Assign the hand bone transform in the Inspector
    public Transform LeftFootTarget;   
     private Vector3 LeftFootInitialOffset;
    private Quaternion LeftFootInitialRotation;     // Assign the target object in the Inspector

      public Transform RightFootTransform; // Assign the hand bone transform in the Inspector
    public Transform RightFootTarget;   
     private Vector3 RightFootInitialOffset;
    private Quaternion RightFootInitialRotation;     // Assign the target object in the Inspector

    void Start()
    {
        // Calculate the initial offset and rotation
        //Hand
        LeftHandInitialOffset = CalculateInitialOffset(LeftHandTransform, LeftHandTarget, out LeftHandInitialRotation);
        RightHandInitialOffset = CalculateInitialOffset(RightHandTransform, RightHandTarget, out RightHandInitialRotation);
        //Arm
        LeftArmInitialOffset = CalculateInitialOffset(LeftArmTransform, LeftArmTarget, out LeftArmInitialRotation);
        RightArmInitialOffset = CalculateInitialOffset(RightArmTransform, RightArmTarget, out RightArmInitialRotation);

        //Foot
        LeftFootInitialOffset = CalculateInitialOffset(LeftFootTransform, LeftFootTarget, out LeftFootInitialRotation);
        RightFootInitialOffset = CalculateInitialOffset(RightFootTransform, RightFootTarget, out RightFootInitialRotation);

    }

    void Update()
    {
        // Only update the target's position and rotation if the game is not paused
        if (Time.timeScale > 0f)
        {
            //Hand
            UpdateTargetTransform(LeftHandTransform, LeftHandTarget, LeftHandInitialOffset, LeftHandInitialRotation);
            UpdateTargetTransform(RightHandTransform, RightHandTarget, RightHandInitialOffset, RightHandInitialRotation);
            //Arm
            UpdateTargetTransform(LeftArmTransform, LeftArmTarget, LeftArmInitialOffset, LeftArmInitialRotation);
            UpdateTargetTransform(RightArmTransform, RightArmTarget, RightArmInitialOffset, RightArmInitialRotation);

            //Foot
            UpdateTargetTransform(LeftFootTransform, LeftFootTarget, LeftFootInitialOffset, LeftFootInitialRotation);
            UpdateTargetTransform(RightFootTransform, RightFootTarget, RightFootInitialOffset, RightFootInitialRotation);

        }
    }

    private Vector3 CalculateInitialOffset(Transform boneTransform, Transform targetTransform, out Quaternion rotation)
    {
        rotation = Quaternion.Inverse(boneTransform.rotation) * targetTransform.rotation;
        return targetTransform.position - boneTransform.position;
    }

    private void UpdateTargetTransform(Transform boneTransform, Transform targetTransform, Vector3 offset, Quaternion rotation)
    {
        targetTransform.position = boneTransform.position + boneTransform.rotation * offset;
        targetTransform.rotation = boneTransform.rotation * rotation;
    }
    
}
