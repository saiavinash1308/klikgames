using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BowlPlayer : MonoBehaviour
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
    public static Action<float> OnThrownBall;
    public SocketManager socketmanager;

    
    // Start is called before the first frame update
    void Start() // Events Called 
    {
        socketmanager = GameObject.FindObjectOfType<SocketManager>();
        bowlmode = BowlMode.Idle;
        initialpos = transform.position;
        ballthrower = FindObjectOfType<BallThrow>();
        BowlController.OnStartNextBall += Restart;
        socketmanager.bowlplayer = this.GetComponent<BowlPlayer>();
    }

    private void OnDestroy()
    {
        BowlController.OnStartNextBall -= Restart;
    }

    private void Restart()
    {
        if (socketmanager.isUsebots)  // normal         
        {
            bowlmode = BowlMode.Idle;
            transform.position = initialpos;
            cricball.SetActive(true);
            anim.SetInteger("BowlState", 0);
            anim.Play("Idle");
        }
        else                          // pvp
        {
            socketmanager.EmitEvent("RESET_BOWLER", "");  
        }
    }

    public void RestartFromSocket() // SOCKET EVENT RESETBOWLER
    {
        Debug.Log("Reset bowler from socket");
        bowlmode = BowlMode.Idle;
        transform.position = initialpos;
        cricball.SetActive(true);
        anim.SetInteger("BowlState", 0);
        anim.Play("Idle");

    }

    // Update is called once per frame
    void Update()
    {
        BowlerAction(); // switch bowler mode 
    }


    public void BowlerAction()
    {
        switch(bowlmode) 
        {
            case BowlMode.Idle:             // idle state
                break;

            case BowlMode.Aim:              // aim state
                break;

            case BowlMode.Running:          // running state
                BeginRun();
                break;

            case BowlMode.BowlThrow:        // throwing state
                BowlingThrow();
                break;
        }
    }

    public void StartRun(float bowlspeed)
    {
        if (!socketmanager.isUsebots) // pvp
        {
            socketmanager.EmitEvent("BOWLER_RUN", bowlspeed.ToString());
        }
        else if(socketmanager.isUsebots) // normal
        {
            runtime = 0;
            this.bowlingspeed = bowlspeed;
            bowlmode = BowlMode.Running;
            anim.SetInteger("BowlState", 1);
        }
    }

    public void StartRunFromSocket(float bowlspeed) // SOCKET EVENT BOWLERRUN
    {
        runtime = 0;
        this.bowlingspeed = bowlspeed;
        bowlmode = BowlMode.Running;
        anim.SetInteger("BowlState", 1);
    }
    public void BeginRun()  
    {
        transform.position+= Vector3.forward * runspeed*Time.deltaTime; // move transform position 
        runtime += Time.deltaTime;
        if(runtime>duration)
        {
            BowlingThrow(); // throw ball 
        }
    }

    public void BowlingThrow()
    {
        bowlmode = BowlMode.BowlThrow; // throwing mode
        anim.SetInteger("BowlState", 2); // throw animation 
    }

    public void BowlBall()
    {
        cricball.SetActive(false);
        Vector3 initial = cricball.transform.position; // get ball position
        Vector3 final = groundTarget.transform.position; // get groundtarget position 

        //duration and bowling speed 
        float distance = Vector3.Distance(initial, final); // get distance between ball and ground target
        float velocity = bowlingspeed / 3.6f; // assign velocity
        float duration = flightMultiplier*distance / velocity; // assign duration 
        print("Duration" + duration);

        float flightsecs = 1f;
        ballthrower.ThrowFastBall(initial, final, flightsecs); // throw ball 

        OnThrownBall?.Invoke(duration); // call events 
    }

}
