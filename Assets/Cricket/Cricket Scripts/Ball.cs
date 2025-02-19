using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    [SerializeField]
    private bool ishit; // check for ball touch
    [SerializeField]
    private bool istouchbat; // check for bat touch
    private Rigidbody rb;    
    public static Action<Vector3> onTouchGround;    // on ball hitting the ground 
    public static Action onBallMissed;  // on ball missing
    public static Action onStumpsHit;   // on ball hitting the stumps
    public static Action onBallCaught;  // on ball getting caught 
    private bool isFade;    
    private float trailduration=0.5f;   // trail renderer time for the ball 
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag=="Stumps") // on ball hitting the stumps
        {
            col.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            StumpsCollided();
        }
        if(col.gameObject.tag=="Field") // on ball touching field 
        {
            DetectFieldGround();
            Debug.Log("ball is touching the ground");
        }     

        if (col.gameObject.tag == "Batsman")
        {
          //  DetectBallMiss();
        }
       
    }

    private void OnCollisionStay(Collision col)
    {
        if(col.gameObject.tag=="SmallField") // if ball stays near the pitch zone
        {
            Debug.Log("ball in small field");
            StartCoroutine(DotBall());
        }
    }

    private IEnumerator DotBall() 
    {
        yield return new WaitForSeconds(3f);
        Destroy(this.gameObject);
        DetectBallMiss(); 
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Crowd") // if ball enters crowd area 
        {
            Destroy(this.gameObject, 2f);
            CrowdStands();
        }
        if(other.gameObject.tag=="Missed") // ball is missed 
        {
            Destroy(this.gameObject, 1f);
            StartCoroutine(FadeBall());
            DetectBallMiss();
        }

    }

    private IEnumerator FadeBall()  
    {
       
        isFade = true;  // DeActivate the trailrenderer 
        float startTime = Time.time;
        float initialTime = this.transform.GetChild(0).GetComponent<TrailRenderer>().time;
        while(Time.time-startTime<trailduration)
        {
          this.transform.GetChild(0).GetComponent<TrailRenderer>().time = Mathf.Lerp(initialTime, 0f, (Time.time - startTime) / trailduration);
            yield return null; 
        }

       
       this.transform.GetChild(0).GetComponent<TrailRenderer>().time = 0f;  // Ensure that the trail time is set to 0 when fadeout completes
        isFade = false;

    }


    private void DetectFieldGround()
    {
        float bounceForce = Random.Range(1f, 2.5f); // set bounce force once touching field 
        rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);       // bounce for pitch
        if (!istouchbat)         // Ball is touching field
        {
            return;
        }
        if(ishit)   
        {
            return;
        }
        ishit = true;
        onTouchGround?.Invoke(transform.position); // Event 
    }

    private void CrowdStands()
    {
        if (!istouchbat)        // Ball is in crowd stand
        {
            return;
        }
        if (ishit)             
        {
            return;
        }
        ishit = true;
        onTouchGround?.Invoke(transform.position); // Event
    }

    public void DetectBallMiss()
    {
        if (istouchbat)             // Ball missed
        {
            return;
        }
        if (ishit)
        {
            return;
        }
        ishit = true;
        onBallMissed?.Invoke();
    }

    private void StumpsCollided()
    {
     /*   if (istouchbat)         // Ball touched stumps
        {
            return;
        }
        if (ishit)
        {
            return;
        }*/
        onStumpsHit?.Invoke();
    }

    public void CaughtBall()
    {
        if(!istouchbat)       // Ball caught
        {
            return;
        }
        if(!ishit)
        {
            return;
        }
        onBallCaught?.Invoke();
    }

    public void ResetBallState()
    {
        istouchbat = false;
        ishit = false;
    }

    public void TouchedBat(Vector3 vel)
    {
        Debug.LogError("touched bat");
        istouchbat = true;              // Ball touched bat
        GetComponent<Rigidbody>().velocity = vel;
    }
}
