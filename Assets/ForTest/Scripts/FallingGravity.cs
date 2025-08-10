using UnityEngine;

public class FallingGravity : MonoBehaviour
{
    private Rigidbody rb;
    private float mass;
    private const float GRAVITY = 9.81f;
    private float velocity = 0f;

    private Collider _collider;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mass = rb.mass;
        _collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update() { }

    void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;
        velocity += GRAVITY * deltaTime;
        float distance = velocity * deltaTime + GRAVITY / 2 * (deltaTime * deltaTime);
        rb.MovePosition(rb.position + Vector3.down * distance);

        Collider[] overlapping = Physics.OverlapBox(
            _collider.bounds.center,
            _collider.bounds.extents,
            _collider.transform.rotation,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        bool isInsideAnother = false;
        foreach (var col in overlapping)
        {
            if (col != _collider)
            {
                isInsideAnother = true;
                Vector3 direction = (rb.position - col.ClosestPoint(rb.position)).normalized;
                if (
                    Physics.ComputePenetration(
                        _collider,
                        rb.position,
                        _collider.transform.rotation,
                        col,
                        col.transform.position,
                        col.transform.rotation,
                        out Vector3 penetrationDir,
                        out float penetrationDist
                    )
                )
                {
                    rb.MovePosition(rb.position + penetrationDir * penetrationDist);
                }
                break;
            }
        }
        if (isInsideAnother)
        {
            velocity = 0f;
        }
    }
}
