using System;
using UnityEngine;

public class TestController : MonoBehaviour
{
    private enum RotationState
    {
        Neutral = 0,
        Left = -1,
        Right = 1,
    }

    private RotationState rotationState = RotationState.Neutral;
    public float moveForce = 10f;
    public float jumpForce = 30f;
    public float maxVelocity = 20f;
    private Vector3 moveDirection = Vector3.zero;
    private Rigidbody rb;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

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
            moveDirection.z = 1f;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            moveDirection.z = -1f;
        }
        else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            moveDirection.z = 0f;
        }

        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            moveDirection.y = 1f;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            rotationState = RotationState.Left;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rotationState = RotationState.Right;
        }
        else if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.E))
        {
            rotationState = RotationState.Neutral;
        }
    }

    void FixedUpdate()
    {
        if (moveDirection == Vector3.zero && rotationState == RotationState.Neutral)
            return;

        HandleMovement();
        HandleJump();
        HandleRotation();
    }

    private void HandleJump()
    {
        if (moveDirection.y > 0f && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            moveDirection.y = 0f;
        }
    }

    private void HandleRotation()
    {
        switch (rotationState)
        {
            case RotationState.Left:
                transform.Rotate(Vector3.up, -90 * Time.deltaTime);
                break;
            case RotationState.Right:
                transform.Rotate(Vector3.up, 90 * Time.deltaTime);
                break;
        }
    }

    private void HandleMovement()
    {
        Vector3 horizontalMovement = new(moveDirection.x, 0f, moveDirection.z);

        if (horizontalMovement != Vector3.zero)
        {
            rb.AddForce(
                transform.rotation * horizontalMovement.normalized * moveForce,
                ForceMode.Force
            );
        }

        Vector3 horizontalVelocity = new(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (horizontalVelocity.magnitude > maxVelocity)
        {
            Vector3 clampedHorizontal = horizontalVelocity.normalized * maxVelocity;

            rb.linearVelocity = new Vector3(
                clampedHorizontal.x,
                rb.linearVelocity.y,
                clampedHorizontal.z
            );
        }
    }
}
