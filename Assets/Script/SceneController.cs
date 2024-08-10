using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneController : MonoBehaviour
{
    void OnGUI()
    {
        GUI.skin.button.border = new RectOffset(8, 8, 8, 8);
        GUI.skin.button.margin = new RectOffset(0, 0, 0, 0);
        GUI.skin.button.padding = new RectOffset(0, 0, 0, 0);

        int BtnHeight = 40;
        int BtnWidth = 150;

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        // Button to go to the next scene
        if (GUILayout.Button("Replay Scene", GUILayout.Width(BtnWidth), GUILayout.Height(BtnHeight)))
        {
            // Load the next scene in the build settings
            //int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(2);
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}
