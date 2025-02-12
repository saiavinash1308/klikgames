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
    void Start()
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
        if (socketmanager.isUsebots)
        {
            bowlmode = BowlMode.Idle;
            transform.position = initialpos;
            cricball.SetActive(true);
            anim.SetInteger("BowlState", 0);
            anim.Play("Idle");
        }
        else
        {
            socketmanager.EmitEvent("RESET_BOWLER", "");
        }
    }

    public void RestartFromSocket()
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
        BowlerAction();
    }


    public void BowlerAction()
    {
        switch(bowlmode)
        {
            case BowlMode.Idle:
                break;

            case BowlMode.Aim:
                break;

            case BowlMode.Running:
                BeginRun();
                break;

            case BowlMode.BowlThrow:
                BowlingThrow();
                break;
        }
    }

    public void StartRun(float bowlspeed)
    {
        if (!socketmanager.isUsebots)
        {
            socketmanager.EmitEvent("BOWLER_RUN", bowlspeed.ToString());
        }
        else if(socketmanager.isUsebots)
        {
            runtime = 0;
            this.bowlingspeed = bowlspeed;
            bowlmode = BowlMode.Running;
            anim.SetInteger("BowlState", 1);
        }
    }

    public void StartRunFromSocket(float bowlspeed)
    {
        runtime = 0;
        this.bowlingspeed = bowlspeed;
        bowlmode = BowlMode.Running;
        anim.SetInteger("BowlState", 1);
    }
    public void BeginRun()
    {
        transform.position+= Vector3.forward * runspeed*Time.deltaTime;
        runtime += Time.deltaTime;
        if(runtime>duration)
        {
            BowlingThrow();
        }
    }

    public void BowlingThrow()
    {
        bowlmode = BowlMode.BowlThrow;
        anim.SetInteger("BowlState", 2);
    }

    public void BowlBall()
    {
        cricball.SetActive(false);
        Vector3 initial = cricball.transform.position;
        Vector3 final = groundTarget.transform.position;

        //duration and bowling speed 
        float distance = Vector3.Distance(initial, final);
        float velocity = bowlingspeed / 3.6f;
        float duration = flightMultiplier*distance / velocity;
        print("Duration" + duration);

        float flightsecs = 1f;
        ballthrower.ThrowFastBall(initial, final, flightsecs);

        OnThrownBall?.Invoke(duration);
    }

}
