using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class RollBall : MonoBehaviour
{
    Rigidbody rigidBody;

    InputAction bowlButt;

    bool holdingButt;
    bool roll;

    public float rollPower = 1000f;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        bowlButt = InputSystem.actions.FindAction("Bowl");
        //rigidBody.AddForce(transform.forward * 1000f);
    }

    void Update()
    {
        if (bowlButt.WasPressedThisFrame())
        {
            holdingButt = true;
            //StartRoll();
        }
        else if (bowlButt.WasReleasedThisFrame())
        {
            holdingButt = false;
            roll = true;
        }

        //Debug.Log(CallculateGrade(20, -10));
    }

    void FixedUpdate()
    {
        if (holdingButt && rigidBody.linearVelocity == Vector3.zero)
        {
            //find the direction the mouse is pointing towards
            Debug.DrawRay(transform.position, FindDirection() * 1000f, Color.red);
        }

        if (roll)
        {
            StartRoll();
            //roll the ball based on the mouse's direction
        }
    }

    void StartRoll()
    {
        Vector3 pointTo = FindDirection();
        rigidBody.AddForce(rollPower * transform.TransformDirection(pointTo), ForceMode.Impulse);
        roll = false;
    }

    Vector3 FindDirection()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        mousePosition.z = 10f;

        Vector3 direction = Camera.main.ScreenToWorldPoint(mousePosition);
        return direction;
    }

    int CallculateGrade(int a, int b)
    {
        return a + b;
    }
}