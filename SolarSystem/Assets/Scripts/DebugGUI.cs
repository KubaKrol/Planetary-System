using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class DebugGUI : MonoBehaviour
{
    [SerializeField] private bool DrawGravityVectors;
    [SerializeField] private bool DrawTrajectories;
    [Range(0, 10000)]
    [SerializeField] private int NumberOfSteps;

    [SerializeField] private bool _RelativeTo;
    [SerializeField] private int _RelativeBodyIndex;
    
    [SerializeField] private List<CelestialBody> _CelestialBodies;

    private VirtualBody[] VirtualCelestialBodies;
    
    //FixedUpdate is happening only in game mode
    void FixedUpdate()
    {
        if (DrawGravityVectors)
        {
            foreach (var celestialBody in Universe.allCelestialBodies)
            {
                foreach (var nestedCelestialBody in Universe.allCelestialBodies)
                {
                    if(celestialBody == nestedCelestialBody)
                        continue;
                
                    var position = celestialBody.transform.position;
                    Debug.DrawLine(position, position + celestialBody.CalculateGravityVector(nestedCelestialBody) * 100f, nestedCelestialBody.MyMesh.material.color);
                }
            }
        }
    }

    void Update()
    {
        if (DrawTrajectories)
        {
            if (VirtualCelestialBodies == null || VirtualCelestialBodies.Length != _CelestialBodies.Count)
            {
                VirtualCelestialBodies = new VirtualBody[_CelestialBodies.Count];   
            }

            for (int i = 0; i < VirtualCelestialBodies.Length; i++)
            {
                VirtualCelestialBodies[i] = new VirtualBody(_CelestialBodies[i]);   
            }
        
            for (int n = 0; n < NumberOfSteps; n++)
            {
                for (int i = 0; i < VirtualCelestialBodies.Length; i++)
                {
                    UpdateVirtualBodyVelocity(VirtualCelestialBodies[i], VirtualCelestialBodies);
                }   
            
                for (int i = 0; i < VirtualCelestialBodies.Length; i++)
                {
                    var previousPosition = VirtualCelestialBodies[i].position;
                    UpdateVirtualBodyPosition(VirtualCelestialBodies[i]);
                    Debug.DrawLine(previousPosition, VirtualCelestialBodies[i].position, VirtualCelestialBodies[i].trajectoryColor);
                }  
            }
        }
    }
    
    private void UpdateVirtualBodyVelocity(VirtualBody targetBody, VirtualBody[] allBodies)
    {
        var targetVelocity = targetBody.velocity;
        
        for (int i = 0; i < allBodies.Length; i++)
        {
            if (allBodies[i] == targetBody)
                continue;

            if (_RelativeTo)
            {
                if(i == _RelativeBodyIndex)
                    continue;   
            }

            var gravityVector = targetBody.CalculateGravityVector(allBodies[i]);
            targetVelocity = targetVelocity + (gravityVector / targetBody.mass);   
        }

        targetBody.velocity = targetVelocity;
    }
    
    private void UpdateVirtualBodyPosition(VirtualBody targetBody)
    {
        targetBody.position = (targetBody.position + targetBody.velocity * Universe.physicsTimeStep);
    }
    
    private class VirtualBody
    {
        public Vector3 position;
        public Vector3 velocity;
        public float mass;
        public Color trajectoryColor;

        public VirtualBody (CelestialBody body)
        {
            position = body.transform.position;
            velocity = EditorApplication.isPlaying ? body.CurrentVelocity : body.InitialVelocity;
            mass = body.Mass;
            trajectoryColor = body.TrajectoryColor;
        }
        
        public Vector3 CalculateGravityVector(VirtualBody targetBody)
        {
            Vector3 distanceVector = targetBody.position - position;
            Vector3 gravityDirection = distanceVector.normalized;
            float distance = distanceVector.magnitude;
            float gravity = (Universe.gravitationalConstant * targetBody.mass * mass) / (distance * distance);

            return gravityDirection * gravity;
        }
    }
}
