using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyTest
{
    
    public class AnimationEditController : MonoBehaviour
    {
        [Header("===Config Setting===")]
        public float FPS = 30;
        public string JsonFilePath = "D://temp//animation_data.json"; // Use the same file for loading and saving

        [Header("===Source Avatar===")]
        public Animator sourceAnimator;
        private HumanPoseHandler sourcePoseHandler;
        private HumanPose sourcePose;
        public float[] sourceMusclesValue;
        public string[] sourceMusclesName;

        private MotionData motionData;
        private float _fpsDeltaTime = 0f;
        private int currentFrameIndex = 0; // Track the current frame index

        private HashSet<int> replacedFrames = new HashSet<int>(); // Track replaced frames

        public static HashSet<int> ReplacedFrameIndices { get; private set; } = new HashSet<int>(); // Static property to expose the replaced frame indices

        void OnGUI()
        {
            GUI.skin.button.border = new RectOffset(8, 8, 8, 8);
            GUI.skin.button.margin = new RectOffset(0, 0, 0, 0);
            GUI.skin.button.padding = new RectOffset(0, 0, 0, 0);

            int BtnHeight = 40;
            int BtnWidth = 150;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Display the current frame index
            GUILayout.Label($"Current Frame: {currentFrameIndex}/{motionData?.motionFrames.Count ?? 0}", GUILayout.Width(BtnWidth), GUILayout.Height(BtnHeight));

            // Display the replaced frames
            GUILayout.Label($"Replaced Frames: {string.Join(", ", replacedFrames)}", GUILayout.Width(BtnWidth * 2), GUILayout.Height(BtnHeight));

            // Button to replace the current frame with the current data
            if (GUILayout.Button("Save", GUILayout.Width(BtnWidth), GUILayout.Height(BtnHeight)))
            {
                ReplaceCurrentFrame();
            }

            // Button to load the next scene
            if (GUILayout.Button("Replay Scene", GUILayout.Width(BtnWidth), GUILayout.Height(BtnHeight)))
            {
                SaveData();
                SceneManager.LoadScene(2); // Replace with your next scene name
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        void Start()
        {
            _fpsDeltaTime = 1.0f / FPS;

            sourcePose = new HumanPose();
            sourcePoseHandler = new HumanPoseHandler(sourceAnimator.avatar, sourceAnimator.transform);
            sourcePoseHandler.GetHumanPose(ref sourcePose);
            sourceMusclesValue = sourcePose.muscles;
            sourceMusclesName = HumanTrait.MuscleName;

            LoadData(); // Load data at the start
        }

        void Update()
        {
            // Update the avatar's pose
            if (motionData != null && motionData.motionFrames.Count > 0)
            {
                currentFrameIndex = (int)(Time.time * FPS) % motionData.motionFrames.Count; // Update the current frame index based on time

                MuscleValues currentFrame = motionData.motionFrames[currentFrameIndex];
                sourceAnimator.gameObject.transform.localPosition = currentFrame.position;
                sourceAnimator.gameObject.transform.localRotation = currentFrame.rotation;

                for (int i = 0; i < sourceMusclesValue.Length; ++i)
                {
                    sourcePose.muscles[i] = currentFrame.muscleValues[i];
                }
                sourcePoseHandler.SetHumanPose(ref sourcePose);
            }
        }

        private void LoadData()
        {
            if (File.Exists(JsonFilePath))
            {
                string jsonData = File.ReadAllText(JsonFilePath);
                motionData = JsonUtility.FromJson<MotionData>(jsonData);
                Debug.Log("Data loaded from: " + JsonFilePath);

                if (motionData != null && motionData.motionFrames != null)
                {
                    Debug.Log("Data loaded successfully.");
                }
                else
                {
                    Debug.LogError("Loaded data is null or has no frames.");
                }
            }
            else
            {
                Debug.LogError("JSON file not found at: " + JsonFilePath);
            }
        }

        private void ReplaceCurrentFrame()
        {
            if (motionData != null && motionData.motionFrames.Count > 0)
            {
                // Replace the current frame with the current pose data
                MuscleValues updatedValue = new MuscleValues
                {
                    muscleValues = new float[sourcePose.muscles.Length],
                    position = sourceAnimator.gameObject.transform.localPosition,
                    rotation = sourceAnimator.gameObject.transform.localRotation
                };

                for (int i = 0; i < sourcePose.muscles.Length; ++i)
                {
                    updatedValue.muscleValues[i] = sourcePose.muscles[i];
                }

                // Overwrite the current frame
                motionData.motionFrames[currentFrameIndex] = updatedValue;

                Debug.Log($"Frame {currentFrameIndex} replaced with the current data.");

                // Track the replaced frame index
                replacedFrames.Add(currentFrameIndex);
                ReplacedFrameIndices = replacedFrames; // Update the static property

                SaveData(); // Ensure the data is saved immediately after replacement
            }
            else
            {
                Debug.LogWarning("No frames to replace.");
            }
        }

        private void SaveData()
        {
            if (motionData != null)
            {
                string jsonData = JsonUtility.ToJson(motionData);
                File.WriteAllText(JsonFilePath, jsonData);
                Debug.Log("Data saved to: " + JsonFilePath);
                Debug.Log("Saved JSON: " + jsonData);
            }
            else
            {
                Debug.LogError("No data to save.");
            }
        }
    }
}
