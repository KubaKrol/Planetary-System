using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CelestialBody))]
public class CelestialBodyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        CelestialBody myTarget = (CelestialBody) target;

        if (myTarget.showVelocityVector)
        {
            EditorGUI.BeginDisabledGroup(true);
        
            EditorGUILayout.Vector3Field("Current velocity: ", myTarget.CurrentVelocity);
            
            for (int i = 0; i < Universe.allCelestialBodies.Count; i++)
            {
                if(Universe.allCelestialBodies[i] == myTarget)
                    continue;

                if (Universe.allCelestialBodies[i] != null)
                {
                    EditorGUILayout.Vector3Field("gravityVector towards: " + Universe.allCelestialBodies[i].gameObject.name, myTarget.CalculateGravityVector(Universe.allCelestialBodies[i]));   
                }
            }
        
            EditorGUI.EndDisabledGroup();
        }
    }
}
