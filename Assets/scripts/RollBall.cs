using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RollBall : MonoBehaviour
{
    Rigidbody rigidBody;

    InputAction bowlButt;

    bool holdingButt;
    bool roll;
    public Transform[] pins;
    public TMP_Text scoreText;

    Vector3[] startPositions;
    Quaternion[] startRotations;
    int score;
    Vector3 startPosition;

    public float rollPower = 1000f;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        bowlButt = InputSystem.actions.FindAction("Bowl");
        //rigidBody.AddForce(transform.forward * 1000f);
        startPosition = transform.position;


        startPositions = new Vector3[pins.Length];
        startRotations = new Quaternion[pins.Length];

        for (int pinNumber = 0; pinNumber < pins.Length; pinNumber++)
        {
            startPositions[pinNumber] = pins[pinNumber].position;
            startRotations[pinNumber] = pins[pinNumber].rotation;
        }

        scoreText.text = "Score: 0";
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
        rigidBody.AddForce(rollPower * pointTo, ForceMode.Impulse);
        roll = false;
    }

    Vector3 FindDirection()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();

        Vector3 direction = new Vector3(mousePosition.x / Screen.width - 0.5f, 0f, 1f);
        return direction;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gutter"))
            {
            transform.position = startPosition;
            rigidBody.linearVelocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pin"))
        {
            if (!IsInvoking("ResetBall"))
            {
                Invoke("ResetBall", 7f);
            }
        }
    }


    void ResetBall()
    {
        rigidBody.linearVelocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        transform.position = startPosition;

        for (int pinNumber = 0; pinNumber < pins.Length; pinNumber++)
        {
            if (pins[pinNumber].up.y < 0.7f)
            {
                score++;
            }

            Rigidbody pinBody = pins[pinNumber].GetComponent<Rigidbody>();

            pinBody.linearVelocity = Vector3.zero;
            pinBody.angularVelocity = Vector3.zero;

            pins[pinNumber].position = startPositions[pinNumber];
            pins[pinNumber].rotation = startRotations[pinNumber];
        }

        scoreText.text = "Score: " + score;
    }


    int CallculateGrade(int a, int b)
    {
        return a + b;
    }
}