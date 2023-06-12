using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] WallRunning wallRunning;

    [SerializeField] float sensitivityX = 100f;
    [SerializeField] float sensitivityY = 100f;

    [SerializeField] float multiplier = 0.01f;

    [SerializeField] Transform cameraHolder;
    [SerializeField] Transform orientation;

    float mouseX;
    float mouseY;

    // looking up & down
    float xRotation;
    // looking left & right
    float yRotation;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        GetInput();

        // rotates cameraHolder on both axis (which holds camera)
        cameraHolder.localRotation = Quaternion.Euler(xRotation, yRotation, wallRunning.tilt);
        // rotates orientation on just Y (Player movement depends on orientation rotation)
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void GetInput() {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensitivityX * multiplier;
        xRotation -= mouseY * sensitivityY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }
}
