using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using SimpleJSON;

public class Bat : MonoBehaviour
{
    [SerializeField] private Collider batcol; // bat collider 
    [SerializeField] private float hitduration = 1f; // bat timer 
    [SerializeField] private Vector2 minMaxhitVel = new Vector2(5f, 20f); // values for hitting 
    public LayerMask ballMask; // Optional for additional checks
    public static Action<Transform> onBallHit; // Evemt 
    public Ball ball; // Ball reference
    private SocketManager socketmanager; 

    private BatsmanPlayer batsmanplayer;

    private float hitTimer;

    private void Start()
    {
        socketmanager = GameObject.FindObjectOfType<SocketManager>();
        batsmanplayer = GameObject.FindObjectOfType<BatsmanPlayer>();

    }
    private void OnCollisionEnter(Collision collider)
    {

        if (collider.gameObject.tag == "Ball")
        {
            ball = collider.gameObject.GetComponent<Ball>();   // on touching ball 
            if (ball != null)
            {
                float random = Random.Range(-1f, 1f); // random direction 
                socketball = collider.transform; // ball reference 
                if (!socketmanager.isUsebots) // pvp       
                {
                    socketmanager.EmitEvent("BALL_HIT_POSITION", random.ToString()); //SocketManager BALLHITPOSITION Event
                }
                else if(socketmanager.isUsebots) // normal 
                {
                    batsmanplayer.ShootBall(collider.gameObject.transform); 
                }           
            }
        }
    }

    [HideInInspector]
    public Transform socketball;

    public void ShootBall(float random) 
    {
        Transform ball = socketball;
        Debug.Log("Ball is moving with force");
        // Calculate the hit force based on timer
        float lerp = Mathf.Clamp01(hitTimer / hitduration);
        float hitvel = Mathf.Lerp(minMaxhitVel.y, minMaxhitVel.x, lerp);

        // Generate the hit velocity vector
        Vector3 hitVelVector = (Vector3.back + Vector3.up + Vector3.right * random * hitvel); // hitvel vector
        float hitvelx = hitVelVector.x; // xpos
        float hitvely = hitVelVector.y; // ypos
        float hitvelz = hitVelVector.z; // zpos
        ball.GetComponent<Ball>().TouchedBat(hitVelVector); 

        if (onBallHit != null)
        {
            Debug.Log($"Invoking onBallHit for: {ball.name}");
            onBallHit.Invoke(ball); // switching Camera Event    
        }
        else
        {
            Debug.LogWarning("No subscribers for onBallHit event");
        }
        hitTimer = 0f;

        StartCoroutine(UpdateBallPosition(ball)); //calculate ball position 
    }


    private IEnumerator UpdateBallPosition(Transform ball)
    {
       while(ball.GetComponent<Ball>()!=null)
        {
            Vector3 ballPos = ball.transform.position;
            Debug.Log($"Ball Position: {ball.position}");
           
            // Wait for the next frame
            yield return null;
        }
    }

    private void Update()
    {
     
        hitTimer += Time.deltaTime;
    }
}

