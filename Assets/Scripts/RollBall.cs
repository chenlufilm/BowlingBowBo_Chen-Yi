using UnityEngine;
using UnityEngine.InputSystem;

public class RollBall : MonoBehaviour
{
    //System level properties
    public Rigidbody myBody;
    private InputAction bowlButt;
    public GameManager myManager;
   
    //player bowling charge power floats
    public float minPower;
    public float maxPower;
    public float chargePowerUp;
    public float rollPower;
    
    //ball's initial position
    private Vector3 startPosition;
    
    //state machine (judge if player is charging, and if the ball is thrown out, and if the ball is rolling)
    private bool charging;
    private bool roll;
    private bool rolling;

    void Awake()
    {
        //set Rigidbody
        myBody = GetComponent<Rigidbody>();
        //save ball's initial position and rotation
        startPosition = transform.position;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //set input system
        bowlButt = InputSystem.actions.FindAction("Bowl");
    }

    // Update is called once per frame
    void Update()
    {
        //check if GameManager allows player to bowl now, if not execute the following update
        if (myManager.canBowl == false)
        {
            return;
        }
        
        //if player press LeftMouseKey, start charging the ball, the rolling power of the ball is set to minPower at first
        if (bowlButt.WasPressedThisFrame())
        {
            charging = true;
            rollPower = minPower;
        }
        //if player is charging (holding the LeftMouseKey)
        if (charging)
        {
            //roll Power is increased every second
            rollPower += Time.deltaTime * chargePowerUp;
            //restain player's charged power to max power
            if (rollPower > maxPower)
            {
                rollPower = maxPower;
            }
            //display player's charged power number
            myManager.powerText.text = "Power: " + rollPower.ToString("F1");//display only one decimal number after integer
            //if player releases the LeftMouseKey
            if (bowlButt.WasReleasedThisFrame())
            {
                //player is no longer charging
                charging = false;
                //ball is thrown out
                roll = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (charging && myBody.linearVelocity == Vector3.zero)
        {
            //find the direction the mouse is pointing forwards
            Debug.DrawRay(transform.position, FindDirection()*1000f,Color.red);
        }

        if (roll)
        {
            //ball is already thrown out
            roll = false;
            //roll the ball based ono the mouse's direction
            StartRoll();
        }
    }

    void StartRoll()
    { 
        //let game manager set the game state to BeginRoll
        myManager.BeginRoll();
        //ball starts dynamic physics simulation
        myBody.isKinematic = false;
        //ball starts rolling with given force
        myBody.AddForce(rollPower * FindDirection(), ForceMode.Impulse);
        //ball is rolling
        rolling = true;
    }

    Vector3 FindDirection()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        mousePosition.z = 10f;
        //the ball is now not set to (0,0,0), to avoid the wrong calculation of the direction for the ball to be thrown at, it needs to minus the current ball position
        Vector3 direction = Camera.main.ScreenToWorldPoint(mousePosition) - transform.position;
        //making sure player cannot throw the ball upwards or downwards
        direction.y = 0f;
        //contain direction's vector number be contained within -1 to 1
        return direction.normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if ball is in the scoring area, make sure game does not reset waiting time in game manager
        if (rolling == false)
        {
            return;
        }
        //if the ball enters the scoring area
        if (other.CompareTag("Score"))
        {
            rolling = false;
            //let game manager switch state to start FinishRoll funciton
            myManager.FinishRoll();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if ball is in the scoring area, make sure game does not reset waiting time in game manager
        if (rolling == false)
        {
            return;
        }
        //if the ball hits the pin, indicate this round player has rolled the ball already, reset the rolling state and ask manager to enter finishroll calculation
        if (collision.gameObject.CompareTag("Pin"))
        {
            rolling = false;
            myManager.FinishRoll();
        }
    }
    
    //make a new public function to reset all the stats of the ball for game manager
    public void ResetBall()
    {
        //turn on dynamic physics simulation to reset the velocity of the ball
        myBody.isKinematic = false;
        myBody.linearVelocity = Vector3.zero;
        myBody.angularVelocity = Vector3.zero;
        //turn off dynamic physics simulation after resetting the velocity of the ball 
        myBody.isKinematic = true;
        //reset ball to its initial position, clear the roll power
        myBody.position = startPosition;
        rollPower = minPower;
        
        //reset ball's states
        charging = false;
        rolling = false;
        roll = false;
    }
}