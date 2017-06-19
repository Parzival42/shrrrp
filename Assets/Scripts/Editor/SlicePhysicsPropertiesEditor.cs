using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SlicePhysicsProperties))]
public class SlicePhysicsPropertiesEditor : Editor
{
    private int thresholdAmount;
    private int newThresholdAmount;
    private bool once;


    public override void OnInspectorGUI()
    {
        SlicePhysicsProperties slicePhysicsProperties = (SlicePhysicsProperties) target;

        if (!once)
        {
            once = true;
            thresholdAmount = newThresholdAmount = slicePhysicsProperties.volumeThresholds.Length;
        }

        GUILayout.Label("Basic Mass Properties");
        EditorGUILayout.BeginVertical("Box");
        slicePhysicsProperties.baseMass = EditorGUILayout.FloatField("Base Mass", slicePhysicsProperties.baseMass);
        slicePhysicsProperties.referenceVolume = EditorGUILayout.FloatField("Reference Volume", slicePhysicsProperties.referenceVolume);
        EditorGUILayout.LabelField("Resulting Kilos Per Cubic Meters: " + slicePhysicsProperties.baseMass / slicePhysicsProperties.referenceVolume);
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);
        GUILayout.Label("Volume Mass Thresholds");
        EditorGUILayout.BeginVertical("Box");

        slicePhysicsProperties.useThresholds = EditorGUILayout.Toggle("Use Volume-Mass Thresholds",
            slicePhysicsProperties.useThresholds);

        if (!slicePhysicsProperties.useThresholds)
        {
            slicePhysicsProperties.minVolume = EditorGUILayout.FloatField("Minimum Slice Volume", slicePhysicsProperties.minVolume);
        }
        else
        {
            newThresholdAmount = EditorGUILayout.IntField("Threshold Count", newThresholdAmount);
            if (newThresholdAmount != thresholdAmount)
            {
                thresholdAmount = newThresholdAmount;
                slicePhysicsProperties.volumeThresholds = new float[thresholdAmount];
                slicePhysicsProperties.massThresholds = new float[thresholdAmount];
            }

            for (int i = 0; i < thresholdAmount; i++)
            {
                EditorGUILayout.BeginHorizontal();
                slicePhysicsProperties.volumeThresholds[i] = EditorGUILayout.FloatField("Threshold " + (i + 1), slicePhysicsProperties.volumeThresholds[i]);
                slicePhysicsProperties.massThresholds[i] = EditorGUILayout.FloatField("Mass", slicePhysicsProperties.massThresholds[i]);

                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndVertical();

        GUILayout.Space(10);
        GUILayout.Label("Drag Properties");
        EditorGUILayout.BeginVertical("Box");
        slicePhysicsProperties.drag = EditorGUILayout.FloatField("Drag", slicePhysicsProperties.drag);
        slicePhysicsProperties.angularDrag = EditorGUILayout.FloatField("Angular Drag", slicePhysicsProperties.angularDrag);
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);
        GUILayout.Label("Physics Material");
        EditorGUILayout.BeginVertical("Box");
        slicePhysicsProperties.physicMaterial = (PhysicMaterial) EditorGUILayout.ObjectField(slicePhysicsProperties.physicMaterial, typeof(Object), true);
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);
        GUILayout.Label("Cutting Force");
        EditorGUILayout.BeginVertical("Box");
        slicePhysicsProperties.cuttingForce = EditorGUILayout.FloatField("Force", slicePhysicsProperties.cuttingForce);
        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(slicePhysicsProperties);
        }
    }
}