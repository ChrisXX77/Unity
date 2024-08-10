using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ManageRigLayer : MonoBehaviour
{
    public Rig rigLayer;
    private bool isPaused = false;

    public Animator animator;
    public RigBuilder rigbuilder;
    private bool isAnimatorOn = false;
    private bool isRigBuilderOn = false;

    void Start()
    {
        rigLayer.weight = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePause();
            ToggleAnimator();
            //ToggleRigBuilder();

        }
    }

    void TogglePause()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
            rigLayer.weight = 0f;
        }
        else
        {
            Time.timeScale = 0f;
            rigLayer.weight = 1f;
        }
        isPaused = !isPaused;
    }

    void ToggleAnimator()
    {
        isAnimatorOn = !isAnimatorOn;
        animator.enabled = isAnimatorOn;
    }
    void ToggleRigBuilder()
    {
        isRigBuilderOn = !isRigBuilderOn;
        rigbuilder.enabled = isRigBuilderOn;
    }
}
