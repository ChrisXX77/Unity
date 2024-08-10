using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MyTest
{
    public class AnimationController : MonoBehaviour
    {
        public string JsonFilePath = "D://temp//test1.json";
        //public string ReplaceJsonFilePath = "D://temp//replace.json";
        
        public Animator OriginalAnimator;
        private HumanPoseHandler OriginalPoseHandler;
        private HumanPose OriginalPose;


        private List<MuscleValues> replayData;
        private List<MuscleValues> replaceData;
        private int replayFrameIndex = 0;
        private float FPS = 30;
        private float _fpsDeltaTime;
        private float _updateTimer = 0f;

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
            
            // Load the JSON files
            LoadData();
        }

        void Update()
        {
            // Replay the motion
            DoReplay();
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

        

            replayFrameIndex += 1;
            if (replayFrameIndex >= replayData.Count)
                replayFrameIndex = 0;
        }
    }

    
}
