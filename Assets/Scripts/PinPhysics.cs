using UnityEngine;

public class PinPhysics : MonoBehaviour
{
    //declare Rigidbody
    private Rigidbody myBody;
    
    //declare every pin's original position and rotation
    private Vector3 startPosition;
    private Quaternion startRotation;
    
    //sound effect
    public AudioSource myAudio;
    public AudioClip knockedSound;

    void Awake()
    { 
        //set Rigidbody
        myBody =  GetComponent<Rigidbody>();
        //document every pin's start position and rotation
        startPosition = myBody.transform.position;
        startRotation = myBody.transform.rotation;
    }

    //define a public bool for the game manager to calculate the score based on if a pin is knocked down 
    public bool KnockedDown()
    {
        //calculate the pin's tilted angle relative to the ground
        float angle = Vector3.Angle(transform.forward,Vector3.up);
        if (angle > 45)
        {
            return true;
        }
        //if the pin has fallen down the lane, knocked down will be true, 0.28 is each pin's y position
        if (transform.position.y < startPosition.y - 0.28)
        {
            return true;
        }else{
            return false;
        }
    }
    
    //define a public function for game manager to activate pin's dynamic physics simulation 
    public void PinPhysicsOn()
    {
        myBody.isKinematic = false;
    }

    //play sound effect when collided with ball
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            myAudio.PlayOneShot(knockedSound);
        }
    }
    
    
    //define a public function for game manager to reset each pin's physics and position when game is over
    public void ResetPin ()
    {
        //reset pin's Rigidbody properties
        myBody.isKinematic = false;
        myBody.linearVelocity = Vector3.zero;
        myBody.angularVelocity = Vector3.zero;
        myBody.isKinematic = true;
        //reset pin's locations
        myBody.position = startPosition;
        myBody.rotation = startRotation;
    }
}
