using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



namespace MyTest
{
    public class AnimationEditController : MonoBehaviour
    {

        public ManageRigLayer manageRigLayer; // Reference to ManageRigLayer script

        [Header("===Config Setting===")]
        public float FPS = 30;
        public string JsonFilePath = "D://temp//animation_data.json"; // Use this file for loading
        public string ReplaceJsonFilePath = "D://temp//replace.json"; // Use this file for saving replaced data

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
        private bool isPaused = false; // Track pause state

        public Text frameText;  // UI Text for displaying the current frame
        public Text replacedFramesText;  // UI Text for displaying replaced frames
        public Button saveButton;  // UI Button to save the current frame
        public Button replaySceneButton;  // UI Button to load the next scene

        public Sprite SaveSprite;  // Assign this in the Inspector
        public Sprite SendSprite; // Assign this in the Inspector

        

        public static HashSet<int> ReplacedFrameIndices { get; private set; } = new HashSet<int>(); // Static property to expose the replaced frame indices

        void Start()
        {
            _fpsDeltaTime = 1.0f / FPS;

            sourcePose = new HumanPose();
            sourcePoseHandler = new HumanPoseHandler(sourceAnimator.avatar, sourceAnimator.transform);
            sourcePoseHandler.GetHumanPose(ref sourcePose);
            sourceMusclesValue = sourcePose.muscles;
            sourceMusclesName = HumanTrait.MuscleName;

            LoadData(); // Load data at the start from JsonFilePath

            // Add listeners to UI buttons
            saveButton.onClick.AddListener(ReplaceCurrentFrame);
            replaySceneButton.onClick.AddListener(() => SaveAndLoadScene(2));

            saveButton.image.sprite = SaveSprite;
            replaySceneButton.image.sprite = SendSprite;
        }

        void Update()
        {
            

            if (!isPaused && motionData != null && motionData.motionFrames.Count > 0)
            {
                // Update the current frame index based on time
                currentFrameIndex = (int)(Time.time * FPS) % motionData.motionFrames.Count;

                MuscleValues currentFrame = motionData.motionFrames[currentFrameIndex];
                sourceAnimator.gameObject.transform.localPosition = currentFrame.position;
                sourceAnimator.gameObject.transform.localRotation = currentFrame.rotation;

                for (int i = 0; i < sourceMusclesValue.Length; ++i)
                {
                    sourcePose.muscles[i] = currentFrame.muscleValues[i];
                }
                sourcePoseHandler.SetHumanPose(ref sourcePose);

                // Update the UI Text components
                frameText.text = $"Frame: {currentFrameIndex}/{motionData.motionFrames.Count}";
                replacedFramesText.text = $"Replaced Frames: {string.Join(", ", replacedFrames)}";
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
                // Create a temporary HumanPose to avoid modifying the internal state of sourcePoseHandler
                HumanPose tempPose = new HumanPose();
                sourcePoseHandler.GetHumanPose(ref tempPose);

                // Replace the current frame with the current pose data from tempPose
                MuscleValues updatedValue = new MuscleValues
                {
                    muscleValues = new float[tempPose.muscles.Length],
                    position = sourceAnimator.gameObject.transform.localPosition,
                    rotation = sourceAnimator.gameObject.transform.localRotation
                };

                for (int i = 0; i < tempPose.muscles.Length; ++i)
                {
                    updatedValue.muscleValues[i] = tempPose.muscles[i];
                }

                // Overwrite the current frame
                motionData.motionFrames[currentFrameIndex] = updatedValue;

                Debug.Log($"Frame {currentFrameIndex} replaced with the current data.");

                // Track the replaced frame index
                replacedFrames.Add(currentFrameIndex);
                ReplacedFrameIndices = replacedFrames; // Update the static property

                SaveData(ReplaceJsonFilePath); // Save to the ReplaceJsonFilePath
            }
            else
            {
                Debug.LogWarning("No frames to replace.");
            }
        }


        private void SaveData(string path)
        {
            if (motionData != null)
            {
                string jsonData = JsonUtility.ToJson(motionData);
                File.WriteAllText(path, jsonData);
                Debug.Log("Data saved to: " + path);
                Debug.Log("Saved JSON: " + jsonData);
            }
            else
            {
                Debug.LogError("No data to save.");
            }
        }

        private void SaveAndLoadScene(int sceneIndex)
        {
            SaveData(ReplaceJsonFilePath);
            isPaused = false;
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
