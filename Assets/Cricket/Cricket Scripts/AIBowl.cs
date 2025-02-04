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
    void Start()
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


    private void BowlerAction()
    {
        switch (bowlmode)
        {
            case BowlMode.Idle:
                break;

            case BowlMode.Aim:
                Aim();
                break;

            case BowlMode.Running:
                BeginRun();
                break;

            case BowlMode.BowlThrow:
                BowlingThrow();
                break;
        }
    }

    private void StartAiming()
    {
        bowlmode = BowlMode.Aim;
    }
    private void Aim()
    {
        float x = Mathf.PerlinNoise(0,Time.time*20);
        float y = Mathf.PerlinNoise(0,Time.time * 2);

        Vector2 targetPos = new Vector2(x, y);

        aitarget.Move(targetPos);

        aimingTimer += Time.deltaTime;

        if(aimingTimer>2)
        {
            StartRun(this.bowlingspeed);
        }
    }
    public void StartRun(float bowlspeed)
    {
        runtime = 0;
        this.bowlingspeed = Random.Range(20, 30);
   
        bowlmode = BowlMode.Running;
        anim.SetInteger("BowlState", 1);
    }
    public void BeginRun()
    {
        transform.position += Vector3.forward * runspeed * Time.deltaTime;
        runtime += Time.deltaTime;
        if (runtime > duration)
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
        float velocity = bowlingspeed / 1.6f;
        float duration = flightMultiplier * distance / velocity;

        float flightsecs = 1f;
        ballthrower.ThrowFastBall(initial, final, flightsecs);

        OnThrownBall?.Invoke(duration);
    }

    private void Restart()
    {
       
        bowlmode = BowlMode.Idle;
        transform.position = initialpos;
        cricball.SetActive(true);
        anim.SetInteger("BowlState", 0);
        anim.Play("Idle");
    }
}
