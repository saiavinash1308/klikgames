using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class AIBowl : MonoBehaviour
{
    public enum BowlMode { Idle, Aim, Running, BowlThrow };

    [Header("Mode")]
    public BowlMode bowlmode;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private GameObject cricball;
    [SerializeField]
    private Transform groundTarget;
    public GroundTarget aitarget;
    public BallThrow ballthrower;

    [Header("settings")]
    private Vector3 initialpos;
    public float runspeed;
    private float runtime;
    private float bowlingspeed;
    [SerializeField]
    private float duration;
    [SerializeField]
    private float flightMultiplier;
    [SerializeField]
    private float aimingTimer;
    public static Action<float> OnThrownBall;



    // Start is called before the first frame update
    void Start() //Events called 
    {
        bowlmode = BowlMode.Aim;
        ballthrower = FindObjectOfType<BallThrow>();
        BatController.OnAimStarted += StartAiming;
        BatController.OnStartNextBall += Restart;
        initialpos = transform.position;
    }

    private void OnDestroy()
    {
        BatController.OnAimStarted -= StartAiming;
        BatController.OnStartNextBall -= Restart;
    }


    // Update is called once per frame
    void Update()
    {
        BowlerAction();
    }


    private void BowlerAction() // switch bowler modes 
    {
        switch (bowlmode)
        {
            case BowlMode.Idle: // Idle State
                break;

            case BowlMode.Aim:  // Aim State
                Aim();
                break;

            case BowlMode.Running:  // Run State
                BeginRun();
                break;

            case BowlMode.BowlThrow:    // Throw State
                BowlingThrow();
                break;
        }
    }

    private void StartAiming()
    {
        bowlmode = BowlMode.Aim;    // aiming mode 
    }
    private void Aim()
    {
        float x = Mathf.PerlinNoise(0,Time.time*20);
        float y = Mathf.PerlinNoise(0,Time.time * 2);

        Vector2 targetPos = new Vector2(x, y);

        aitarget.Move(targetPos); // move ground target

        aimingTimer += Time.deltaTime; // set aiming timer seconds 

        if(aimingTimer>2)
        {
            StartRun(this.bowlingspeed);    // start ai bowler run 
        }
    }
    public void StartRun(float bowlspeed)
    {
        runtime = 0;
        this.bowlingspeed = Random.Range(20, 30);   // bowling speed
   
        bowlmode = BowlMode.Running;
        anim.SetInteger("BowlState", 1);    // run animation 
    }
    public void BeginRun()
    {
        transform.position += Vector3.forward * runspeed * Time.deltaTime; // move transform position 
        runtime += Time.deltaTime;
        if (runtime > duration)
        {
            BowlingThrow(); // throw ball 
        }
    }

    public void BowlingThrow()
    {
        bowlmode = BowlMode.BowlThrow; // throw mode 
        anim.SetInteger("BowlState", 2); // throw animation 
    }

    public void BowlBall()
    {
        cricball.SetActive(false); // disable bowl ball 
        Vector3 initial = cricball.transform.position; // get ball position 
        Vector3 final = groundTarget.transform.position; // get ground target position 

        //duration and bowling speed 
        float distance = Vector3.Distance(initial, final);  // get distance between ball and ground target 
        float velocity = bowlingspeed / 1.6f; // assign velocity
        float duration = flightMultiplier * distance / velocity; // assign duration 

        float flightsecs = 1f;
        ballthrower.ThrowFastBall(initial, final, flightsecs);  // instantiate ball from ballthrower 

        OnThrownBall?.Invoke(duration); // switching camera event 
    }

    private void Restart() // Restart AI Bowler 
    {
       
        bowlmode = BowlMode.Idle; // bowler Idle State
        transform.position = initialpos;
        cricball.SetActive(true);
        anim.SetInteger("BowlState", 0);
        anim.Play("Idle");
    }
}
