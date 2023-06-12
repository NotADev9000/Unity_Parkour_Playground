using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] Transform orientation;

    [Header("Detection")]
    [SerializeField] float wallDistance = 0.6f;
    [SerializeField] float minimumJumpHeight = 1.5f;

    [Header("Wall Running")]
    [SerializeField] float wallRunGravity = 1f;
    [SerializeField] float wallRunJumpForce = 600f;

    [Header("Camera")]
    [SerializeField] Camera cam;
    [SerializeField] float fov;
    [SerializeField] float wallRunFov;
    [SerializeField] float wallRunFovTime;
    [SerializeField] float camTilt;
    [SerializeField] float camTiltTime;

    public float tilt { get; private set; }

    bool wallLeft = false;
    bool wallRight = false;
    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    private Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    bool CanWallRun() {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }
    
    void CheckWall() {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance);
    }

    void Update() {
        CheckWall();

        if (CanWallRun()) {
            if (wallLeft) {
                StartWallRun();
                Debug.Log("Wall running on LEFT");
            } else if (wallRight) {
                StartWallRun();
                Debug.Log("Wall running on RIGHT");
            } else {
                StopWallRun();
            }
        } else {
            StopWallRun();
        }
    }

    void StartWallRun() {
        rb.useGravity = false;
        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, wallRunFov, wallRunFovTime * Time.deltaTime);

        float directionTilt = wallLeft ? -camTilt : camTilt;
        tilt = Mathf.Lerp(tilt, directionTilt, camTiltTime * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space)) {
            Vector3 wallHit = wallLeft ? leftWallHit.normal : rightWallHit.normal;
            Vector3 wallRunJumpDirection = transform.up + wallHit;

            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            // apply jump force to player
            rb.AddForce(wallRunJumpDirection * wallRunJumpForce, ForceMode.Force);
        }
    }

    void StopWallRun() {
        rb.useGravity = true;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, wallRunFovTime * Time.deltaTime);
        tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
    }
}
