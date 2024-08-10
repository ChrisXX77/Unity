using UnityEngine;

public class AnimationPauseController : MonoBehaviour
{
    public Animator animator;
    private bool isPaused = false;
    private float lastSpeed;

    void Start(){
        PauseAnimation();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // Press 'P' to toggle pause
        {
            if (isPaused)
            {
                ResumeAnimation();
            }
            else
            {
                PauseAnimation();
            }
        }
    }

    void PauseAnimation()
    {
        lastSpeed = animator.speed;
        animator.speed = 0f;
        isPaused = true;
    }

    void ResumeAnimation()
    {
        animator.speed = lastSpeed;
        isPaused = false;
    }
}
