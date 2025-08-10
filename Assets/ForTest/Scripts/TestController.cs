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
    public float rotateForce = 20f;
    public float flyForce = 20f;
    public float jumpForce = 10f;
    public float maxVelocity = 20f;
    private Vector3 moveDirection = Vector3.zero;
    private Rigidbody rb;
    private bool isGrounded = false;
    public bool canFly = false;
    public Camera playerCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = Camera.main;
    }

    void Update()
    {
        playerCamera.transform.position = transform.TransformPoint(new Vector3(0f, 1.5f, -3f));
        playerCamera.transform.LookAt(transform.position + Vector3.up * 1.5f);

        bool pressedW = Input.GetKey(KeyCode.W);
        bool pressedS = Input.GetKey(KeyCode.S);
        if (pressedW && !pressedS)
        {
            moveDirection.z = 1f;
        }
        else if (pressedS && !pressedW)
        {
            moveDirection.z = -1f;
        }
        else
        {
            moveDirection.z = 0f;
        }

        bool pressedA = Input.GetKey(KeyCode.A);
        bool pressedD = Input.GetKey(KeyCode.D);
        if (pressedA && !pressedD)
        {
            moveDirection.x = -1f;
        }
        else if (pressedD && !pressedA)
        {
            moveDirection.x = 1f;
        }
        else
        {
            moveDirection.x = 0f;
        }

        bool pressedSpace = Input.GetKey(KeyCode.Space);
        if (pressedSpace && !canFly)
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.01f);
            if (isGrounded)
                moveDirection.y = 1f;
        }
        else if (canFly)
        {
            bool pressedControl = Input.GetKey(KeyCode.LeftControl);
            if (pressedSpace && !pressedControl)
            {
                moveDirection.y = 1f;
            }
            else if (pressedControl && !pressedSpace)
            {
                moveDirection.y = -1f;
            }
            else
            {
                moveDirection.y = 0f;
            }
        }

        bool pressedQ = Input.GetKey(KeyCode.Q);
        bool pressedE = Input.GetKey(KeyCode.E);
        if (pressedQ && !pressedE)
        {
            rotationState = RotationState.Left;
        }
        else if (pressedE && !pressedQ)
        {
            rotationState = RotationState.Right;
        }
        else
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
        if (canFly)
        {
            rb.AddForce(Vector3.up * moveDirection.y * flyForce, ForceMode.Force);
            return;
        }
        if (moveDirection.y > 0f && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            moveDirection.y = 0f;
            isGrounded = false;
        }
    }

    private void HandleRotation()
    {
        switch (rotationState)
        {
            case RotationState.Left:
                rb.AddTorque(transform.up * -rotateForce, ForceMode.Force);
                break;
            case RotationState.Right:
                rb.AddTorque(transform.up * rotateForce, ForceMode.Force);
                break;
        }
    }

    private void HandleMovement()
    {
        Vector3 horizontalMovement = new(moveDirection.x, 0f, moveDirection.z);

        if (horizontalMovement == Vector3.zero)
            return;

        Quaternion cameraYaw = Quaternion.Euler(0, playerCamera.transform.localEulerAngles.y, 0);
        rb.AddForce(cameraYaw * horizontalMovement.normalized * moveForce, ForceMode.Force);

        Vector3 horizontalVelocity = new(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (horizontalVelocity.magnitude > maxVelocity)
        {
            Vector3 clampedHorizontal = cameraYaw * horizontalVelocity.normalized * maxVelocity;

            rb.linearVelocity = new Vector3(
                clampedHorizontal.x,
                rb.linearVelocity.y,
                clampedHorizontal.z
            );
        }
    }
}
