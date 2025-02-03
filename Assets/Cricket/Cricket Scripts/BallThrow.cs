using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallThrow : MonoBehaviour
{
    [SerializeField]
    private Transform initialPos;
    [SerializeField]
    private Transform groundTarget;

    [SerializeField]
    private float flightseconds;

    public GameObject ballprefab;

    [HideInInspector]
    public GameObject cricball;
    private Vector3 groundpos2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ThrowFastBall()
    {
        Vector3 grndtargetpos = groundTarget.position;      // get ground target position 
        groundpos2 = Physics.gravity * flightseconds * flightseconds / 2;       // flight and height of ball 
        Vector3 initialpos = initialPos.position;   // initial position 
        Vector3 initialVelocity = (grndtargetpos - groundpos2 - initialpos) / flightseconds;    // calculate velocity and ball trajectory

        cricball=     Instantiate(ballprefab,initialPos.position, Quaternion.identity, this.transform); // Create cricket ball
        cricball.GetComponent<Rigidbody>().velocity = initialVelocity;     // set rigidbody velocity to initial velocity
    }



    public void ThrowFastBall(Vector3 initial,Vector3 final,float flightseconds)
    {
        Vector3 grndtargetpos = final;  // get ground target position 
        groundpos2 = Physics.gravity * flightseconds * flightseconds / 2;   // flight and height of ball 
        Vector3 initialpos = initial;   // initial position 
        Vector3 initialVelocity = (grndtargetpos - groundpos2 - initialpos) / flightseconds;    // calculate velocity and ball trajectory

        cricball = Instantiate(ballprefab, initial, Quaternion.identity, this.transform);   // Create cricket ball
        cricball.transform.SetParent(null);
        cricball.GetComponent<Rigidbody>().velocity = initialVelocity;      // set rigidbody velocity to initial velocity 
    }
}
