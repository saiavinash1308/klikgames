using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FielderManager : MonoBehaviour
{
    public SphereCollider sphereCollider;  // The collider for the fielder
    public Rigidbody rb;                   // Rigidbody for physics-based movement
    public float speed;                    // Movement speed of the fielder
    private Transform ballTransform;       // Transform of the ball
    private bool moveTowardsBall = false;  // Flag to check if fielder should move towards the ball

    [Header("Animator")]
    public Animator animator;              // Reference to the Animator component for animations

    public float stopDistance = 1f;        // Distance at which the fielder stops moving towards the ball
    public float transitionDuration = 0.5f; // Duration of the transition between animations

    void Start()
    {
        // Ensure Rigidbody component is assigned
        rb = GetComponent<Rigidbody>();
        if (animator == null)
        {
            Debug.LogError("Animator component is missing!");
        }
    }

    void Update()
    {
        // If moving towards the ball and the ballTransform is not null
        if (moveTowardsBall && ballTransform != null)
        {
            Vector3 direction = (ballTransform.position - transform.position).normalized;
            rb.MovePosition(transform.position + direction * speed * Time.deltaTime); // Move the fielder towards the ball

            // Check if the fielder is within the stopDistance of the ball
            if (Vector3.Distance(transform.position, ballTransform.position) < stopDistance)
            {
                moveTowardsBall = false;  // Stop moving when within range of the ball
            }

            // Smoothly transition to the "FielderThrow" animation
            animator.CrossFade("FielderThrow", transitionDuration);
        }
        else
        {
            // Transition to idle when not moving
            //animator.CrossFade("Idle", transitionDuration); // Replace with your actual idle animation name
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Detect the ball and start moving towards it
        if (other.gameObject.CompareTag("Ball"))
        {
            ballTransform = other.transform;  // Store the ball's transform
            moveTowardsBall = true;           // Set flag to start moving towards the ball
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Stop moving towards the ball when the fielder exits the trigger
        if (other.gameObject.CompareTag("Ball"))
        {
            moveTowardsBall = false;  // Stop moving towards the ball
            ballTransform = null;     // Clear the ball reference
        }
    }
}
