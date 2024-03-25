using UnityEditor;
using UnityEngine;

public class CoroutineActionWindow : EditorWindow
{
    private static string selectedCoroutineName;

    public static void ShowWindow(string coroutineName)
    {
        selectedCoroutineName = coroutineName;
        var window = GetWindow<CoroutineActionWindow>($"{coroutineName}", true);
        window.Show();
    }

    private void OnGUI()
    {
        if (!string.IsNullOrEmpty(selectedCoroutineName))
        {
            GUILayout.Label($"Selected Coroutine: {selectedCoroutineName}", EditorStyles.boldLabel);

            GUILayout.Space(5);
            DrawLine();
            GUILayout.Space(5);

            if (GUILayout.Button("Pause"))
            {
                CoroutineManager.PauseManagedCoroutine(selectedCoroutineName);
            }
            if (GUILayout.Button("Resume"))
            {
                CoroutineManager.ResumeManagedCoroutine(selectedCoroutineName);
            }
            if (GUILayout.Button("Stop"))
            {
                CoroutineManager.StopManagedCoroutine(selectedCoroutineName);
                selectedCoroutineName = null; // 선택된 코루틴 제거
                Close(); // 창 닫기
            }
        }
    }

    private void DrawLine()
    {
        var rect = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
        EditorGUI.DrawRect(rect, Color.gray);
    }
}