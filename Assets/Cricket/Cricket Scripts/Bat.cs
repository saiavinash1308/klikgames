using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using SimpleJSON;

public class Bat : MonoBehaviour
{
    [SerializeField] private Collider batcol;
    [SerializeField] private float hitduration = 1f;
    [SerializeField] private Vector2 minMaxhitVel = new Vector2(5f, 20f);
    public LayerMask ballMask; // Optional for additional checks
    public static Action<Transform> onBallHit;
    public Ball ball;
    private SocketManager socketmanager;
    private BatController batcontroller;

    private float hitTimer;

    private void Start()
    {
        socketmanager = GameObject.FindObjectOfType<SocketManager>();
        batcontroller = GameObject.FindObjectOfType<BatController>();
        if(batcontroller==null)
        {
            Debug.LogWarning("NO BAT CONTROLLER");
        }
        if(socketmanager==null)
        {
            return;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Ball")
        {
            ball = collision.gameObject.GetComponent<Ball>();   // on touching ball 
            if (ball != null)
            {
                float random = Random.Range(-1f, 1f);
                socketball = collision.transform;
                if (batcontroller.enabled)
                {
                    socketmanager.EmitEvent("BALL_HIT_POSITION", random.ToString());
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
        Vector3 hitVelVector = (Vector3.back + Vector3.up + Vector3.right * random * hitvel);
        float hitvelx = hitVelVector.x;
        float hitvely = hitVelVector.y;
        float hitvelz = hitVelVector.z;
        ball.GetComponent<Ball>().TouchedBat(hitVelVector);

        if (onBallHit != null)
        {
            Debug.Log($"Invoking onBallHit for: {ball.name}");
            onBallHit.Invoke(ball);     
        }
        else
        {
            Debug.LogWarning("No subscribers for onBallHit event");
        }
        hitTimer = 0f;

        StartCoroutine(UpdateBallPosition(ball));
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

