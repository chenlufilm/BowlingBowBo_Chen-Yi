using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    //declare the ball object in the scene from the RollBall script
    public RollBall ball;

    //declare all the pin's Transform objects in the scene
    public Transform pinSet;

    //declare all UI texts
    //score text
    public TMP_Text scoreText;

    //charging power text
    public TMP_Text powerText;

    //game states
    //if player can bowl
    public bool canBowl;

    //if the game is waiting for the pins to settle
    private bool waiting;

    //waiting timer
    //waiting time for the pins to settle
    public float settleTime = 2f;

    //waiting time timer
    private float waitingTimer;

    //game score 
    //current score
    private int score;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //reset player's ball when game starts
        ball.ResetBall();
        //reset every single pin in the parented pinSet
        foreach (Transform child in pinSet)
        {
            //call for the ResetPin function from PinPhysics script
            PinPhysics pin = child.GetComponent<PinPhysics>();
            pin.ResetPin();
        }

        //reset game's stage, allow player to throw the bowling ball
        canBowl = true;
        //reset UI texts
        scoreText.text = "Score: 0";

        //charge instruction
        powerText.text = "Hold Left Mouse To Charge";
    }

    // Update is called once per frame
    void Update()
    {
        //if game is waiting for the pins to settle
        if (waiting)
        {
            //waiting time increases by seconds
            waitingTimer += Time.deltaTime;
            //if waiting time has exceeded the settled time, stop waiting state and count the pins knocked down for scoring
            if (waitingTimer >= settleTime)
            {
                waiting = false;
                CountPins();
            }
        }
    }

    //game state for the players to roll the ball
    public void BeginRoll()
    {
        //when the player release the ball, stop the ball from get thrown by the player again
        canBowl = false;
        //activate every pin in the current scene
        foreach (Transform child in pinSet)
        {
            //if the pins are still not knocked down, use the activate dynamic physic function in PinPhysics script
            if (child.gameObject.activeSelf)
            {
                PinPhysics pin = child.GetComponent<PinPhysics>();
                pin.PinPhysicsOn();
            }
        }
    }

    //game state when one round of game is over
    public void FinishRoll()
    {
        //reset the waiting for pins to fall down time to 0, reset the waiting state
        waitingTimer = 0;
        waiting = true;
    }

    //pin knocked down scoring funciton
    void CountPins()
    {
        //get each physics component from the pin set parent
        foreach (Transform child in pinSet)
        {
            PinPhysics pin = child.GetComponent<PinPhysics>();
            //if a pin is knocked down
            if (pin.KnockedDown())
            {
                //every pin knocked down will give player one score up
                score++;
            }
        }
        
        //update score in UI
        scoreText.text = "Score: " + score;
        //reset all the pins and balls for the next round
        ResetGame();
        ball.ResetBall();
        //reset the UI display for charging instructions
        powerText.text = "Hold Left Mouse To Charge";
        //player can bowl again
        canBowl = true;
    }

    void ResetGame()
    {
        //reset every pin's physics component from the pin physics script
        foreach (Transform child in pinSet)
        {
            PinPhysics pin = child.GetComponent<PinPhysics>();
            pin.ResetPin();
        }
    }
}
