using UnityEngine;

public class TestController : MonoBehaviour
{
    public float moveForce = 10f;
    public float jumpForce = 30f;
    public float maxVelocity = 20f;
    private Vector3 moveDirection = Vector3.zero;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            moveDirection.x = 1f;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            moveDirection.x = -1f;
        }
        else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            moveDirection.x = 0f;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            moveDirection.z = -1f;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            moveDirection.z = 1f;
        }
        else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            moveDirection.z = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && moveDirection.y == 0f)
        {
            moveDirection.y = 1f;
        }
        // else if (Input.GetKeyDown(KeyCode.LeftControl))
        // {
        //     moveDirection = Vector3.down;
        // }
    }

    void FixedUpdate()
    {
        if (moveDirection == Vector3.zero)
            return;

        Vector3 horizontalMovement = new(moveDirection.x, 0f, moveDirection.z);

        if (horizontalMovement != Vector3.zero)
        {
            rb.AddForce(moveForce * horizontalMovement.normalized, ForceMode.Force);
        }

        Vector3 horizontalVelocity = new(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        // Debug.Log($"max speed: {horizontalVelocity.magnitude:F2} m/s");
        if (horizontalVelocity.magnitude > maxVelocity)
        {
            Vector3 clampedHorizontal = horizontalVelocity.normalized * maxVelocity;

            rb.linearVelocity = new Vector3(
                clampedHorizontal.x,
                rb.linearVelocity.y,
                clampedHorizontal.z
            );
        }

        if (moveDirection.y != 0f && rb.linearVelocity.y == 0f)
        {
            rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
        }
        if (rb.linearVelocity.y == 0f)
            moveDirection.y = 0f;
    }
}
