using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Add this for scene management

namespace MyTest
{
    public class PlayMocopi : MonoBehaviour
    {
        public string JsonFilePath = "D://temp//test1.json";
        
        public Animator OriginalAnimator;
        private HumanPoseHandler OriginalPoseHandler;
        private HumanPose OriginalPose;

        private List<MuscleValues> replayData;
        private int replayFrameIndex = 0;
        private float FPS = 30;
        private float _fpsDeltaTime;
        private float _updateTimer = 0f;
        private bool isPaused = false; // Track whether playback is paused

        public Button pauseButton; // Add a public Button field
        public Button goBackButton; // Add another Button field for going back
        public Button SendButton;
        public Sprite PlaySprite;  
        public Sprite PauseSprite;

        public Sprite GoBackSprite;
        public Sprite SendSprite;

        public Text frameCountText; // Add this field


        void Start()
        {
            _fpsDeltaTime = 1.0f / FPS;
            Time.timeScale = 1f;

            // Ensure the button is set up if assigned
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(OnPauseButtonClick); // Add listener to pause button click
                pauseButton.image.sprite = PauseSprite; // Initialize with PlaySprite
            }
            
            if (goBackButton != null)
            {
                goBackButton.onClick.AddListener(OnGoBackButtonClick); // Add listener to go back button click
                goBackButton.image.sprite = GoBackSprite;
            }

            if (SendButton != null)
            {
                SendButton.onClick.AddListener(OnSendButtonClick); // Add listener to send button click
                SendButton.image.sprite = SendSprite;
            }

            // Initialize original pose handler
            if (OriginalAnimator != null && OriginalAnimator.avatar != null)
            {
                OriginalPoseHandler = new HumanPoseHandler(OriginalAnimator.avatar, OriginalAnimator.transform);
                OriginalPose = new HumanPose();
                OriginalPoseHandler.GetHumanPose(ref OriginalPose);

                if (OriginalPose.muscles == null)
                {
                    Debug.LogError("OriginalPose.muscles is null after initialization.");
                }
            }
            else
            {
                Debug.LogError("Original animator or avatar is missing.");
            }

            // Load the JSON files
            LoadData();
        }

        void Update()
        {
            // Replay the motion
            if (!isPaused) // Only replay if not paused
            {
                DoReplay();
            }

            // Update the frame count display
            if (frameCountText != null)
            {
                frameCountText.text = $"Frame: {replayFrameIndex}/{replayData?.Count ?? 0}";
            }
        }

        public void OnPauseButtonClick()  // Public method to be called by the pause button
        {
            // Toggle pause state
            isPaused = !isPaused;

            // Update the button icon based on the current state
            pauseButton.image.sprite = isPaused ? PlaySprite : PauseSprite;

            // Continue to the next frame if resuming
            if (!isPaused)
            {
                replayFrameIndex += 1;
                if (replayFrameIndex >= replayData.Count)
                    replayFrameIndex = 0;
            }
        }

        public void OnGoBackButtonClick() // Public method to be called by the go back button
        {
            SceneManager.LoadScene(0); // Load Scene 3
        }

        public void OnSendButtonClick() // Public method to be called by the send button
        {
            SceneManager.LoadScene(1); // Load Scene 1
        }

        private void LoadData()
        {
            if (File.Exists(JsonFilePath))
            {
                string jsonString = File.ReadAllText(JsonFilePath);
                MotionData motionData = JsonUtility.FromJson<MotionData>(jsonString);
                if (motionData != null && motionData.motionFrames != null)
                {
                    replayData = motionData.motionFrames;
                    Debug.Log($"Loaded {replayData.Count} frames from {JsonFilePath}");
                }
                else
                {
                    Debug.LogError("Failed to load motion data from " + JsonFilePath);
                }
            }
            else
            {
                Debug.LogError("No data found at " + JsonFilePath);
            }

            replayFrameIndex = 0;
        }

        private void DoReplay()
        {
            if (replayData == null || replayData.Count == 0)
            {
                return;
            }

            _updateTimer -= Time.deltaTime;
            if (_updateTimer > 0)
                return;
            _updateTimer += _fpsDeltaTime;

            // Replay for Original Animator
            if (replayData != null && replayData.Count > 0)
            {
                for (int i = 0; i < OriginalPose.muscles.Length; ++i)
                {
                    OriginalPose.muscles[i] = replayData[replayFrameIndex].muscleValues[i];
                }

                OriginalAnimator.gameObject.transform.localPosition = replayData[replayFrameIndex].position;
                OriginalAnimator.gameObject.transform.localRotation = replayData[replayFrameIndex].rotation;

                OriginalPoseHandler.SetHumanPose(ref OriginalPose);
            }

            // Check if the current frame is in the replaced frames and pause if necessary
            if (AnimationEditController.ReplacedFrameIndices.Contains(replayFrameIndex))
            {
                isPaused = true; // Pause the playback
                Debug.Log($"Playback paused at frame {replayFrameIndex}");
            }

            // Continue to next frame if not paused
            if (!isPaused)
            {
                replayFrameIndex += 1;
                if (replayFrameIndex >= replayData.Count)
                {
                    // Stop replaying when reaching the end of the data
                    isPaused = true;
                    pauseButton.image.sprite = PlaySprite;
                    Debug.Log("Replay ended.");
                }
            }
        }
    }
}
