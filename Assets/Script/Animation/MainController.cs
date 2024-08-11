using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyTest
{
    public class MainController : MonoBehaviour
    {
        [Header("===Config Setting===")]
        public float FPS = 30;
        public string JsonFilePath = "D://temp//test1.json";
        public string ReplaceJsonFilePath = "D://temp//replace.json";

        [Header("===Source Avatar===")]
        public Animator sourceAnimator;
        private HumanPoseHandler sourcePoseHandler;
        private HumanPose sourcePose;
        public float[] sourceMusclesValue;
        public string[] sourceMusclesName;

        [Header("===UI Elements===")]
        public Button recordButton;
        public Button editSceneButton;
        public Button ReplayButton;

        public Sprite recordOnSprite;  // Assign this in the Inspector
        public Sprite recordOffSprite; // Assign this in the Inspector
        public Sprite SendSprite; // Assign this in the Inspector
        public Sprite ReplaySprite;


        private MotionData motionData;
        private bool onRecording = false;
        private float _fpsDeltaTime = 0f;
        private float _updateTimer = 0f;

        void Start()
        {
            _fpsDeltaTime = 1.0f / FPS;

            sourcePose = new HumanPose();
            sourcePoseHandler = new HumanPoseHandler(sourceAnimator.avatar, sourceAnimator.transform);
            sourcePoseHandler.GetHumanPose(ref sourcePose);
            sourceMusclesValue = sourcePose.muscles;
            sourceMusclesName = HumanTrait.MuscleName;

            // Attach methods to button click events
            recordButton.onClick.AddListener(ToggleRecording);
            editSceneButton.onClick.AddListener(SaveAndLoadEditScene);
            recordButton.image.sprite = recordOffSprite;
            editSceneButton.image.sprite = SendSprite;
            ReplayButton.image.sprite = ReplaySprite;


        }

        void Update()
        {
            // Control source avatar
            for (int i = 0; i < sourceMusclesValue.Length; ++i)
                sourcePose.muscles[i] = sourceMusclesValue[i];

            sourcePoseHandler.GetHumanPose(ref sourcePose);

            // Recording source avatar
            if (onRecording)
                DoRecording();
        }

        public void ToggleRecording()
        {
            if (onRecording)
            {
                onRecording = false;
                SaveData();
                recordButton.image.sprite = recordOffSprite;
            }
            else
            {
                onRecording = true;
                motionData = new MotionData();
                _updateTimer = _fpsDeltaTime;
                recordButton.image.sprite = recordOnSprite;
            }
        }

        public void SaveAndLoadEditScene()
        {
            SaveData();
            SceneManager.LoadScene(1); // Replace with your next scene name
        }

        private bool DoRecording()
        {
            _updateTimer -= Time.deltaTime;
            if (_updateTimer > 0)
                return true;
            _updateTimer += _fpsDeltaTime;

            MuscleValues tmpValue = new MuscleValues
            {
                muscleValues = new float[sourcePose.muscles.Length],
                position = sourceAnimator.gameObject.transform.localPosition,
                rotation = sourceAnimator.gameObject.transform.localRotation
            };
            for (int i = 0; i < sourcePose.muscles.Length; ++i)
                tmpValue.muscleValues[i] = sourcePose.muscles[i];

            motionData.motionFrames.Add(tmpValue);
            return false;
        }

        private void SaveData()
        {
            // Save Data to JsonFile
            if (motionData != null)
            {
                File.WriteAllText(JsonFilePath, JsonUtility.ToJson(motionData));
                Debug.Log("Data saved to: " + JsonFilePath);

                File.WriteAllText(ReplaceJsonFilePath, JsonUtility.ToJson(motionData));
                Debug.Log("Data saved to: " + ReplaceJsonFilePath);
            }
        }
    }

    [System.Serializable]
    public class MuscleValues
    {
        public Vector3 position;
        public Quaternion rotation;
        public float[] muscleValues;
    }

    [System.Serializable]
    public class MotionData
    {
        public List<MuscleValues> motionFrames = new List<MuscleValues>();
    }
}
