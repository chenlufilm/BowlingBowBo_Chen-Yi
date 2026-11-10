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
    
    bool[] countedPins;
    bool isRolling;

    Vector3[] startPositions;
    Quaternion[] startRotations;
    int score;
    Vector3 startPosition;

    public float rollPower = 20f;
    private float holdTime = 0f;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        bowlButt = InputSystem.actions.FindAction("Bowl");
        //rigidBody.AddForce(transform.forward * 1000f);
        startPosition = transform.position;


        startPositions = new Vector3[pins.Length];
        startRotations = new Quaternion[pins.Length];
        countedPins = new bool[pins.Length];

        for (int pinNumber = 0; pinNumber < pins.Length; pinNumber++)
        {
            startPositions[pinNumber] = pins[pinNumber].position;
            startRotations[pinNumber] = pins[pinNumber].rotation;
        }

        scoreText.text = "Score: 0";
    }

    void Update()
    {
        if (isRolling == true)
        {
            return;
        }
        if (bowlButt.WasPressedThisFrame())
        {
            holdingButt = true;
            holdTime = 0;
            //StartRoll();
        }
        else if (bowlButt.WasReleasedThisFrame())
        {
            holdingButt = false;
            roll = true;
        }
        if (holdingButt == true)
        {
            holdTime += Time.deltaTime;
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
        
        if (isRolling)
        {
            for (int pinNumber = 0; pinNumber < pins.Length; pinNumber++)
            {
                if (!countedPins[pinNumber] && Quaternion.Angle(startRotations[pinNumber], pins[pinNumber].rotation) > 60f)
                {
                    score++;
                    countedPins[pinNumber] = true;
                    scoreText.text = "Score: " + score;
                }
            }
        }

    }

    void StartRoll()
    {
        Vector3 pointTo = FindDirection();
        rigidBody.AddForce(Mathf.Clamp(rollPower + holdTime * 40f, 70f, 160f) * pointTo, ForceMode.Impulse);
        
        roll = false;
        isRolling = true;
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
                {
                    Invoke("ResetBall", 1f);
                }
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
            Rigidbody pinBody = pins[pinNumber].GetComponent<Rigidbody>();

            pinBody.linearVelocity = Vector3.zero;
            pinBody.angularVelocity = Vector3.zero;

            pins[pinNumber].position = startPositions[pinNumber];
            pins[pinNumber].rotation = startRotations[pinNumber];

            countedPins[pinNumber] = false;
        }

        isRolling = false;
    }



    int CallculateGrade(int a, int b)
    {
        return a + b;
    }
}