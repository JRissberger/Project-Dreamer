using System;
using System.Linq;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrialPlayerMovement : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction moveAction;
    public CinemachineCamera cam;
    public Transform[] lookAts;
    [Tooltip("0 makes y-axis dynamic with player and 1 keeps it static")]
    public int movement = 0;
    bool transitioning;
    Vector3 transPos;
    public float speed = 0.2f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();
        if (direction.magnitude > 0)
        {
            Vector3 lookAt = Vector3.zero;
            Vector3 vel = Vector3.zero;

            if (movement == 3)
            {
                lookAt = lookAts[(int)cam.GetComponent<CinemachineSplineDolly>().CameraPosition].position - cam.transform.position;
                Vector3 Yaxis;
                Vector3 Xaxis;
                if (Mathf.Max(Mathf.Abs(lookAt.x), Mathf.Abs(lookAt.y)) == Mathf.Abs(lookAt.y)) {
                    Yaxis = new Vector3(0, 0, 1);
                    Xaxis = new Vector3(1, 0, 0);
                }
                else {
                    Yaxis = new Vector3(1, 0, 0);
                    Xaxis = new Vector3(1, 0, 1);
                }


                if (direction.x > 0)
                    vel -= Xaxis;
                if (direction.x < 0)
                    vel += Xaxis;
                if (direction.y > 0)
                    vel += Yaxis;
                if (direction.y < 0)
                    vel -= Yaxis;
            }

            if (movement == 0)
            {
                lookAt = this.transform.position - cam.transform.position;
            }
            else if (movement == 1)
            {
                lookAt = lookAts[(int)cam.GetComponent<CinemachineSplineDolly>().CameraPosition].position - cam.transform.position;
            }

            lookAt.y = 0;
            lookAt = lookAt.normalized;

            if (direction.x > 0)
                vel -= lookAt;
            if (direction.x < 0)
                vel += lookAt;
            if (direction.y > 0)
                vel += Vector3.Cross(cam.transform.position - lookAts[(int)cam.GetComponent<CinemachineSplineDolly>().CameraPosition].position, Vector3.up);
            if (direction.y < 0)
                vel -= Vector3.Cross(cam.transform.position - lookAts[(int)cam.GetComponent<CinemachineSplineDolly>().CameraPosition].position, Vector3.up);

            this.transform.position += vel.normalized * 0.1f * speed;
        }
    }
}