using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Bat : MonoBehaviour
{
    [SerializeField] private Collider batcol;
    [SerializeField] private float hitduration = 1f;
    [SerializeField] private Vector2 minMaxhitVel = new Vector2(5f, 20f);
    public LayerMask ballMask; // Optional for additional checks
    public static Action<Transform> onBallHit;
    public Ball ball;

    private float hitTimer;

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Ball")
        {
            ball = collision.gameObject.GetComponent<Ball>();
            if (ball != null)
            {
                ShootBall(collision.transform);
            }
        }
    }

    private void ShootBall(Transform ball)
    {
        Debug.Log("Ball is moving with force");
        // Calculate the hit force based on timer
        float lerp = Mathf.Clamp01(hitTimer / hitduration);
        float hitvel = Mathf.Lerp(minMaxhitVel.y, minMaxhitVel.x, lerp);

        // Generate the hit velocity vector
        Vector3 hitVelVector = (Vector3.back + Vector3.up + Vector3.right * Random.Range(-1f, 1f)) * hitvel;
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
    }

    private void Update()
    {
     
        hitTimer += Time.deltaTime;
    }
}

