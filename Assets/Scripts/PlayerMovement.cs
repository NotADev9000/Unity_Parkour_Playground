using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform orientation;

    [Header("Debug")]
    [SerializeField] float speed;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 12f;
    [SerializeField] float movementMultiplier = 10f;
    [SerializeField] float airMovementMultiplier = 0.4f; // used to reduce air movement speed
    [SerializeField] float slopeResistance = 0.2f;

    [Header("Sprinting")]
    [SerializeField] float walkSpeed = 12f;
    [SerializeField] float sprintSpeed = 16f;
    [SerializeField] float acceleration = 10f;

    [Header("Jumping")]
    [SerializeField] float jumpForce = 5f;

    [Header("Drag")]
    [SerializeField] float groundDrag = 6f;
    [SerializeField] float airDrag = 2f;

    [Header("Ground Detection")]
    [SerializeField] Transform groundPosition;
    [SerializeField] LayerMask groundMask;
    bool isGrounded;
    float groundDistance = 0.4f;
    bool onSlope;
    RaycastHit slopeHit;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;

    float playerHeight = 2f;

    float horizontalMovement;
    float verticalMovement;

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        isGrounded = Physics.CheckSphere(groundPosition.position, groundDistance, groundMask);
        onSlope = OnSlope();

        GetMovementInput();
        ControlDrag();
        ControlSpeed();

        GetJumpInput();

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

        // Debug info
        speed = rb.velocity.magnitude;
    }

    void FixedUpdate() {
        MovePlayer();
    }

    /* INPUT */

    void GetMovementInput() {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    void GetJumpInput() {
        if (Input.GetKeyDown(jumpKey) && isGrounded) Jump();
    }

    /* MOVEMENT */

    void ControlDrag() => rb.drag = isGrounded ? groundDrag : airDrag;

    void ControlSpeed() {
        float targetSpeed = Input.GetKey(sprintKey) && isGrounded ? sprintSpeed : walkSpeed;
        moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, acceleration * Time.deltaTime);
    }

    void MovePlayer() {
        // slope move direction used when on slope AND on ground
        Vector3 moveDir = onSlope && isGrounded ? slopeMoveDirection.normalized : moveDirection.normalized;
        // force to apply to player
        Vector3 movementForce = moveDir * moveSpeed * movementMultiplier;

        // apply slow movement in air
        if (!isGrounded) movementForce *= airMovementMultiplier;

        rb.AddForce(movementForce, ForceMode.Acceleration);
    }

    void Jump() {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    /* SLOPE */

    bool OnSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, (playerHeight * 0.5f) + 0.5f)) {
            if (slopeHit.normal != Vector3.up && slopeHit.normal.y != 1) {
                return true;
            }
        }
        return false;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + slopeMoveDirection.normalized);
    }
}
