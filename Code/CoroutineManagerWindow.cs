using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CoroutineManagerWindow : EditorWindow
{
    private Vector2 scrollPosition;

    [MenuItem("Tools/Coroutine Manager")]
    public static void ShowWindow()
    {
        GetWindow<CoroutineManagerWindow>("Coroutine Manager");
    }

    private void OnEnable()
    {
        EditorApplication.update += Repaint;
    }

    private void OnDisable()
    {
        EditorApplication.update -= Repaint;
    }

    private void OnGUI()
    {

        GUILayout.Label("Ver. 1.0.0", EditorStyles.boldLabel);

        DrawLine();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        if (CoroutineManager.Instance != null)
        {
            var coroutineStates = CoroutineManager.GetRunningCoroutineStates();

            GUILayout.Label($"Running Coroutines ({coroutineStates.Count})", EditorStyles.boldLabel);

            GUILayout.Space(10);

            if (coroutineStates.Count == 0)
            {
                GUILayout.Label("No coroutines are currently running.");
            }
            else
            {
                foreach (var pair in coroutineStates)
                {
                    string status = pair.Value == CoroutineManager.CoroutineState.Running ? "Running" : "Paused";
                    if (GUILayout.Button($"{pair.Key} - {status}"))
                    {
                        CoroutineActionWindow.ShowWindow(pair.Key);
                    }
                }
            }
        }
        else
        {
            GUILayout.Label("CoroutineManager is not present in the scene.");
        }

        GUILayout.EndScrollView();
    }

    private void DrawLine()
    {
        var rect = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
        EditorGUI.DrawRect(rect, Color.gray);
    }

}