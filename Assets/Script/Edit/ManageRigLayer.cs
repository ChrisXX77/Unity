using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI; // Make sure to include this for UI components

public class ManageRigLayer : MonoBehaviour
{
    public Rig rigLayer;
    public bool isPaused = false;

    public Animator animator;
    public RigBuilder rigbuilder;
    private bool isAnimatorOn = false;
    private bool isRigBuilderOn = false;

    public Button pauseButton; // Add a public Button field
    public Sprite PlaySprite;  
    public Sprite PauseSprite; 

    void Start()
    {
        rigLayer.weight = 0f;
        
        // Ensure the button is set up if assigned
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(OnButtonClick); // Add listener to button click
            pauseButton.image.sprite = PlaySprite;

        }


    }
    

    public void OnButtonClick()  // Public method to be called by the button
    {
        TogglePause();
        ToggleAnimator();
    }

    void TogglePause()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
            rigLayer.weight = 0f;
            pauseButton.image.sprite = PlaySprite;

        }
        else
        {
            Time.timeScale = 0f;
            rigLayer.weight = 1f;
            pauseButton.image.sprite = PauseSprite;

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

    public void SetPauseState(bool paused)
    {
        if (isPaused != paused)
        {
            isPaused = paused;
            if (isPaused)
            {
                Time.timeScale = 0f;
                rigLayer.weight = 1f;
            }
            else
            {
                Time.timeScale = 1f;
                rigLayer.weight = 0f;
            }
        }
    }
}
