using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrialPlayerMovement : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction moveAction;
    public Camera cam;
    public float speed = 5f;


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
        transform.position += new Vector3(direction.x, 0, direction.y) * Time.deltaTime * speed;
    }
}