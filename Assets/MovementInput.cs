using DG.Tweening;
using EzySlice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementInput : MonoBehaviour
{
    public int Velocity;
    public float RotationSpeed;
    public CharacterController Controller;
    public Animator Animator;
    public GameObject PlaneCut;
    public GameObject Sphere;
    public LayerMask LayerCut;

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



        if (Input.GetMouseButtonDown(1))
        {
            PlaneCut.SetActive(true);

            Time.timeScale = 0.2f;

            Camera.main.DOKill();
            Camera.main.DOFieldOfView(40, 0.3f);
        }

        if (Input.GetMouseButtonUp(1))
        {
            PlaneCut.SetActive(false);

            Time.timeScale = 1f;

            Camera.main.DOKill();
            Camera.main.DOFieldOfView(60, 0.1f);
        }

        if (Input.GetMouseButton(1))
        {
            PlaneCut.transform.eulerAngles += new Vector3(0, 0, -Input.GetAxis("Mouse X") * 5);
        }

        if (Input.GetMouseButton(0))
        { 
            Collider[] hits = Physics.OverlapBox(PlaneCut.transform.position, new Vector3(10, 0.1f, 10), PlaneCut.transform.rotation);

            foreach (var item in hits)
            {
                SlicedHull hull = item.gameObject.Slice(PlaneCut.transform.position, PlaneCut.transform.up);
                if (hull != null)
                {
                    GameObject bootom = hull.CreateLowerHull(item.gameObject, null);
                    GameObject top = hull.CreateUpperHull(item.gameObject, null);
                    CreatePieceComponent(bootom);
                    CreatePieceComponent(top);
                    Destroy(Sphere);
                }
            }
        }
    /*

    {
        SlicedHull hull = Sphere.Slice(PlaneCut.transform.position, PlaneCut.transform.up);
        if (hull != null)
        {
            GameObject bootom = hull.CreateLowerHull(Sphere, null);
            GameObject top = hull.CreateUpperHull(Sphere, null);
            Destroy(Sphere);
        }
    */
}

void CreatePieceComponent(GameObject go)
    {
        go.layer = LayerMask.NameToLayer("destroyable");
        Rigidbody rb = go.AddComponent<Rigidbody>();
        MeshCollider collider = go.AddComponent<MeshCollider>();
        collider.convex = true;

        rb.AddExplosionForce(100, go.transform.position, 20);
    }
}
