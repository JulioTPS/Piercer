using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoftbodyController : MonoBehaviour
{
    private Dictionary<Transform, Vector3> originalLocalPositions = new();
    private Dictionary<Transform, float> speeds = new();
    private Dictionary<Transform, Vector3> normals = new();

    private Transform mainColliderTransform;
    public float mass = 1f;
    private const float gravity = 9.81f;

    [Header("Spring Physics")]
    public float springConstant = 1000f;    // Spring stiffness (k)
    public float dampingRatio = 0.3f;      // Damping coefficient
    public float maxDisplacement = 0.0005f;   // Prevent extreme deformation

    void Start()
    {
        mainColliderTransform = GetComponent<Collider>().transform;

        foreach (Collider child in GetComponentsInChildren<Collider>())
        {
            if (child.transform != mainColliderTransform)
            {
                originalLocalPositions[child.transform] = child.transform.localPosition;
                speeds[child.transform] = 0f;
                normals[child.transform] = Vector3.zero;
            }
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.thisCollider.transform == mainColliderTransform)
            {
                continue; // Skip the main collider
            }
            Transform colliderTransform = contact.thisCollider.transform;

            normals[colliderTransform] = contact.normal;

            speeds[colliderTransform] = contact.impulse.magnitude / mass;
        }
    }

    // void OnCollisionStay(Collision collision)
    // {
    //     foreach (ContactPoint contact in collision.contacts)
    //     {
    //         Transform colliderTransform = contact.thisCollider.transform;

    //         Vector3 forceDirection = contact.normal;

    //         float forceMagnitude = contact.impulse.magnitude / Time.fixedDeltaTime;

    //         float deltaDistance = (forceMagnitude * (Time.fixedDeltaTime * Time.fixedDeltaTime)) / 2f;

    //         // Debug.Log($"Force Magnitude: {(colliderTransform.localPosition - originalLocalPositions[colliderTransform]).magnitude}");
    //         // if (forceMagnitude <= 0f)
    //         // {
    //         //     continue;
    //         // }
    //         // float displacement = Mathf.Clamp(forceMagnitude / springConstant, 0f, maxDisplacement);

    //         // Vector3 displacementVector = forceDirection * displacement;

    //         // colliderTransform.position += displacementVector;


    //         // currentDisplacements[colliderTransform] = displacementVector;


    //     }
    // }

    void FixedUpdate()
    {
        foreach (var speedDictionary in speeds.ToList())
        {
            Transform colliderTransform = speedDictionary.Key;
            float speed = speedDictionary.Value;

            Vector3 localNormal = colliderTransform.InverseTransformDirection(normals[colliderTransform]);
            colliderTransform.localPosition += speed * Time.fixedDeltaTime * localNormal;
        }
        // foreach (var kvp in currentDisplacements.ToList())
        // {
        //     Transform trans = kvp.Key;
        //     Vector3 displacement = kvp.Value;

        //     if (displacement.magnitude > 0.001f)
        //     {
        //         Vector3 springForce = -springConstant * displacement;

        //         Vector3 dampingForce = -dampingRatio * displacement / Time.fixedDeltaTime;

        //         Vector3 totalForce = springForce + dampingForce;

        //         Vector3 newDisplacement = displacement + (totalForce / springConstant) * Time.fixedDeltaTime;

        //         newDisplacement *= (1f - dampingRatio * Time.fixedDeltaTime);

        //         if (newDisplacement.magnitude > 0.001f)
        //         {
        //             currentDisplacements[trans] = newDisplacement;
        //             trans.localPosition = originalLocalPositions[trans] + newDisplacement;
        //         }
        //         else
        //         {
        //             currentDisplacements[trans] = Vector3.zero;
        //             trans.localPosition = originalLocalPositions[trans];
        //         }
        //     }
        // }
    }
}