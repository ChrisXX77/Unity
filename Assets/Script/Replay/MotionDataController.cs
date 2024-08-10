using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MyTest
{
    public class MotionDataController : MonoBehaviour
    {
        public string JsonFilePath = "D://temp//test1.json";
        public string ReplaceJsonFilePath = "D://temp//replace.json";
        
        public Animator OriginalAnimator;
        private HumanPoseHandler OriginalPoseHandler;
        private HumanPose OriginalPose;

        public Animator ReplaceAnimator;
        private HumanPoseHandler ReplacePoseHandler;
        private HumanPose ReplacePose;

        private List<MuscleValues> replayData;
        private List<MuscleValues> replaceData;
        private int replayFrameIndex = 0;
        private float FPS = 30;
        private float _fpsDeltaTime;
        private float _updateTimer = 0f;
        private bool isPaused = false; // Track whether playback is paused

        void Start()
        {
            _fpsDeltaTime = 1.0f / FPS;

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

            // Initialize replace pose handler
            if (ReplaceAnimator != null && ReplaceAnimator.avatar != null)
            {
                ReplacePoseHandler = new HumanPoseHandler(ReplaceAnimator.avatar, ReplaceAnimator.transform);
                ReplacePose = new HumanPose();
                ReplacePoseHandler.GetHumanPose(ref ReplacePose);

                if (ReplacePose.muscles == null)
                {
                    Debug.LogError("ReplacePose.muscles is null after initialization.");
                }
            }
            else
            {
                Debug.LogError("Replace animator or avatar is missing.");
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

            if(Input.GetKeyDown("space")){
                // Continue to next frame
                replayFrameIndex += 1;
                if (replayFrameIndex >= Mathf.Min(replayData.Count, replaceData.Count))
                    replayFrameIndex = 0;
                isPaused = false;
            }
        }

        void OnGUI()
        {
            // Display the current frame index
            GUILayout.Label($"Current Frame: {replayFrameIndex + 1}/{Mathf.Min(replayData?.Count ?? 0, replaceData?.Count ?? 0)}", GUILayout.Width(300), GUILayout.Height(40));

            
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

            // Load replace data
            if (File.Exists(ReplaceJsonFilePath))
            {
                string replaceJsonString = File.ReadAllText(ReplaceJsonFilePath);
                MotionData replaceMotionData = JsonUtility.FromJson<MotionData>(replaceJsonString);
                if (replaceMotionData != null && replaceMotionData.motionFrames != null)
                {
                    replaceData = replaceMotionData.motionFrames;
                    Debug.Log($"Loaded {replaceData.Count} frames from {ReplaceJsonFilePath}");
                }
                else
                {
                    Debug.LogError("Failed to load replace data from " + ReplaceJsonFilePath);
                }
            }
            else
            {
                Debug.LogError("No data found at " + ReplaceJsonFilePath);
            }

            replayFrameIndex = 0;
        }

        private void DoReplay()
        {
            if ((replayData == null || replayData.Count == 0) && (replaceData == null || replaceData.Count == 0))
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

            // Replay for Replace Animator
            if (replaceData != null && replaceData.Count > 0 && replayFrameIndex < replaceData.Count)
            {
                if (ReplacePose.muscles != null)
                {
                    for (int i = 0; i < ReplacePose.muscles.Length; ++i)
                    {
                        ReplacePose.muscles[i] = replaceData[replayFrameIndex].muscleValues[i];
                    }

                    ReplaceAnimator.gameObject.transform.localPosition = replaceData[replayFrameIndex].position;
                    ReplaceAnimator.gameObject.transform.localRotation = replaceData[replayFrameIndex].rotation;

                    ReplacePoseHandler.SetHumanPose(ref ReplacePose);
                }
            }

            // Check if the current frame is in the replaced frames and pause if necessary
            if (AnimationEditController.ReplacedFrameIndices.Contains(replayFrameIndex))
            {
                isPaused = true; // Pause the playback
                Debug.Log($"Playback paused at frame {replayFrameIndex + 1}");
            }
            else
            {
                // Continue to next frame
                replayFrameIndex += 1;
                if (replayFrameIndex >= Mathf.Min(replayData.Count, replaceData.Count))
                    replayFrameIndex = 0;
            }
        }
    }
}
