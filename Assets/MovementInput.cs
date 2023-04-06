using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementInput : MonoBehaviour
{
    public int Velocity;
    public float RotationSpeed;
    public CharacterController Controller;
    public Animator Animator;

    void Start()
    {

    }

    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Camera camera = Camera.main;
        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDirection = forward * inputZ + right * inputX;
        Controller.Move(desiredMoveDirection * Time.deltaTime * Velocity);

        float movementIntensity = desiredMoveDirection.magnitude;
        Animator.SetFloat("Move", movementIntensity, 0.1f, Time.deltaTime);

        if (movementIntensity > 0.3f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), RotationSpeed/100);
    }
}
