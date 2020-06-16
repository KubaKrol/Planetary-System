using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    [SerializeField] public MeshRenderer MyMesh; 
    [SerializeField] public float Mass;
    [SerializeField] public Vector3 InitialVelocity;
    [SerializeField] public bool showVelocityVector;

    public Color TrajectoryColor => MyMesh.sharedMaterial.color;

    public Vector3 CurrentVelocity { get; private set; }
    private Rigidbody _MyRigidBody;

    private void Awake()
    {
        _MyRigidBody = GetComponent<Rigidbody>();
        Universe.allCelestialBodies.Add(this);
    }

    private void Start()
    {
        //_MyRigidBody.mass = Mass; Not sure about this
        _MyRigidBody.velocity = InitialVelocity;
        CurrentVelocity = InitialVelocity;
    }

    private void FixedUpdate()
    {
        UpdateVelocity();
        UpdatePosition();
    }

    public Vector3 CalculateGravityVector(CelestialBody targetBody)
    {
        Vector3 distanceVector = targetBody.transform.position - transform.position;
        Vector3 gravityDirection = distanceVector.normalized;
        float distance = distanceVector.magnitude;
        float gravity = (Universe.gravitationalConstant * targetBody.Mass * Mass) / (distance * distance);

        return (gravityDirection * gravity);
    }

    public void UpdateVelocity()
    {
        var targetVelocity = CurrentVelocity;

        foreach (var celestialBody in Universe.allCelestialBodies)
        {
            if(celestialBody == this)
                continue;
            
            var gravityVector = CalculateGravityVector(celestialBody);
            targetVelocity = targetVelocity + (gravityVector / Mass);
        }

        CurrentVelocity = targetVelocity;
        
        if (showVelocityVector)
        {
            Debug.DrawLine(transform.position, transform.position + CurrentVelocity, Color.red);   
        }
    }
    
    public void UpdatePosition()
    {
        _MyRigidBody.MovePosition(transform.position + CurrentVelocity * Universe.physicsTimeStep);
    }
}
