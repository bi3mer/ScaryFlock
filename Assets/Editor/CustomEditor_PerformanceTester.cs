using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PerformanceTester))]
public class CustomEditor_PerformanceTester : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PerformanceTester pt = (PerformanceTester)target;
        GUILayout.Label($"Flock Size: {pt.FlockSize}");

        if (GUILayout.Button("Increase Flock Size"))
        {
            pt.IncreaseFlockSize();
        }
    }
}
