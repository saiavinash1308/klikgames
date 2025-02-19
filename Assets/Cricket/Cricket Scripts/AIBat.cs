using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
public class AIBat : MonoBehaviour
{
    public enum BatState { moving, hitting };
    private BatState batstate;
    [SerializeField]
    private GroundTarget groundtarget;
    [SerializeField]
    private Animator anim;

    [Header("Settings")]
    [SerializeField] private float movespeed;
    [SerializeField]
    private bool canDetectHit;
    [SerializeField]
    private LayerMask ballmask;
    [SerializeField]
    private Vector2 minMaxhitVel;
    [SerializeField]
    private float hitTimer;
    [SerializeField]
    private float hitduration;
    [SerializeField]
    private BoxCollider batcol;
    [SerializeField]
    private float batradius=2f;
    public Collider[] detectballs;
    float flightduration;

    public static Action<Transform> onBallHit;

    public ScoreManager scoremanager;
    public bool hasMissedBall;

    // Start is called before the first frame update
    void Start()    // Events called
    {
        batstate = BatState.moving;
        BowlPlayer.OnThrownBall += ThrownBallCallback;
        BowlController.OnStartNextBall += Restart;
    }

    private void OnDestroy() 
    {
        BowlPlayer.OnThrownBall -= ThrownBallCallback;
        BowlController.OnStartNextBall -= Restart;
    }

    public void Restart()   // Resetting bat State
    {
        batstate = BatState.moving;
        anim.Play("Idle");

    }


    // Update is called once per frame
    void Update()
    {
        SwitchState(); // AI Batsman Idle and hit 
    }

    private void SwitchState()
    {
        switch(batstate)
        {
            case BatState.moving:
                Moving();       //movement state
                break;
            case BatState.hitting:
                if (canDetectHit)
                    CheckForHits(); // hitting state
                break;
        }
    }

    public void Moving() // AI MOVEMENT 
    {
        Vector3 targetPos = transform.position;                 // ai batsman position
        targetPos.x = groundtarget.transform.position.x+1f;     // Based on ground target 
        // Clamp the target position
        targetPos.x = Mathf.Clamp(targetPos.x, 21f, 25f);   // Clamp ai batsman 

        // Calculate the difference
        float difference = targetPos.x - transform.position.x;

        // Handle the animation based on the movement direction
        if (difference == 0)
        {
            anim.Play("Idle");  // Idle State
        }
        else if (difference > 0)
        {
            anim.Play("Left");  // Left Animation 
        }
        else if (difference < 0)
        {
            anim.Play("Right");     // Right Animation  
        }


        // Check if the object has reached the target position
        if (transform.position.x == targetPos.x)
        {

            targetPos = Vector3.zero;
            // Object has reached the target position
            anim.Play("Idle");  // Or any other logic when stopped
        }
        else
        {
            // Move the object towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPos, movespeed * Time.deltaTime);

        }
    }

    private void ThrownBallCallback(float ballduration)
    {
        Debug.LogError("BALL THROWN BABY");
        ballduration = flightduration; // ball time 
        batstate = BatState.hitting; // hit state 
        StartCoroutine(BatHitting());     
    }

    private IEnumerator BatHitting()
    {
        float delay = flightduration - 0.4f; // Calculate Bat hit for ai based on time 
        float newdelay = Random.Range(delay-0.1f,delay+0.1f);
        yield return new WaitForSeconds(newdelay);
        anim.Play("Hit");
    }

    private float GetTarget()
    {
        // Get bowler shoot position
        Vector3 bowlershootpos = new Vector3(-1, 0, -9.5f);
        Vector3 shootdirection = (groundtarget.transform.position - bowlershootpos).normalized;
        float shootAngle = Vector3.Angle(Vector3.right, shootdirection);
        float bc = transform.position.z - bowlershootpos.z;
        float ab = bc / Mathf.Sin(shootAngle * Mathf.Deg2Rad);

        Vector3 targetAIPos = bowlershootpos+ shootdirection.normalized * ab;
        return targetAIPos.x+0.5f;
    }

    public void StartDetectingHits()
{
        //Detecting hits [Animation Event]
    canDetectHit = true;
    hitTimer = 0;
}

public void StopDetectingHits()
{
        // Stop Detection [Animation Event]
        canDetectHit = false;
        batstate = BatState.moving;
}

    public void CheckForHits()
{
        // Set the center of the sphere
        Vector3 center = batcol.transform.TransformPoint(batcol.center);
        // Detect balls within the sphere
        detectballs = Physics.OverlapSphere(center, batradius, ballmask); // based on radius detecting the ball if in radius 

        if (detectballs.Length > 0)
        {
            foreach (var ball in detectballs)
            {
                Debug.Log("Ball detected: " + ball.gameObject.name);
                BallDetectedCallback(ball); // called when ball enters radius and touches bat
            }
            hasMissedBall = false;
        }
        else
        {
            if (!hasMissedBall)
            {
                Debug.Log("Ball is not detected");          
                hasMissedBall = true;
                Restart();  // Reset 
            }
        }
        hitTimer += Time.deltaTime;
    }

    private void BallDetectedCallback(Collider ballcollider)
{
    canDetectHit = false;
    ShootBall(ballcollider.transform);  // Touching bat 
}

private void ShootBall(Transform ball)
{

  float lerp = Mathf.Clamp01(hitTimer / hitduration);   // value from timer and duration
  float hitvel = Mathf.Lerp(minMaxhitVel.y, minMaxhitVel.x, lerp);  
  Vector3 hitVelVector = (Vector3.back + Vector3.up + Vector3.right * Random.Range(-1f, 1f)) * hitvel; // send the ball in vector3 direction randomly
  ball.GetComponent<Ball>().TouchedBat(hitVelVector); // ball reference 
  onBallHit?.Invoke(ball); // Event called for camera switching 

 }

    private void OnDrawGizmos() //Showing in scene mode 
    {
        if (batcol != null)
        {
            Vector3 center = batcol.transform.TransformPoint(batcol.center);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, batradius);
        }
    }
}
