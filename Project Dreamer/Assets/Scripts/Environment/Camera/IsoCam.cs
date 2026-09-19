using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class IsoCam : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    InputAction mouse;
    [SerializeField] Vector3 displacement = new Vector3(11, 8, 0);
    Vector3 currDisp;
    GameObject player;
    Vector2 mousePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Capsule");
        mouse = playerInput.actions.FindAction("Mouse");
        mousePos = mouse.ReadValue<Vector2>();
        currDisp = displacement;
    }

    void Update()
    {
        mousePos = mouse.ReadValue<Vector2>();
        /*
        if (mousePos.x < 109)
            currDisp.z = displacement.z - ((1/mousePos.x)/10);
        if (mousePos.y < 68)
            currDisp.y = displacement.y - ((1/mousePos.y)/10);
        if (mousePos.x > 981)
            currDisp.z = displacement.z + mousePos.x/100;
        if (mousePos.y > 612)
            currDisp.y = displacement.y + mousePos.y/100;


        currDisp.z = Mathf.Clamp(currDisp.z, -2, 2);
        currDisp.y = Mathf.Clamp(currDisp.y, 8, 10);
        Debug.Log(mousePos);
        */
        this.transform.position = player.transform.position + currDisp;
    }
}
