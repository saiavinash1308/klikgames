using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    [SerializeField]
    private bool ishit;
    [SerializeField]
    private bool istouchbat;
    private Rigidbody rb;
    public static Action<Vector3> onTouchGround;
    public static Action onBallMissed;
    public static Action onStumpsHit;
    public static Action onBallCaught;
    private bool isFade;
    private float trailduration=0.5f;
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
        if(col.gameObject.tag=="Stumps")
        {
            col.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
        if(col.gameObject.tag=="Field")
        {
            DetectFieldGround();
            Debug.Log("ball is touching the ground");
        }     
        if(col.gameObject.tag=="Stumps")
        {
            StumpsCollided();
        }
        if (col.gameObject.tag == "Batsman")
        {
          //  DetectBallMiss();
        }
       
    }

    private void OnCollisionStay(Collision col)
    {
        if(col.gameObject.tag=="SmallField")
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
        if(other.gameObject.tag=="Crowd")
        {
            Destroy(this.gameObject, 2f);
            CrowdStands();
        }
        if(other.gameObject.tag=="Missed")
        {
            Destroy(this.gameObject, 1f);
            StartCoroutine(FadeBall());
            DetectBallMiss();
        }

    }

    private IEnumerator FadeBall()
    {
        isFade = true;
        float startTime = Time.time;
        float initialTime = this.transform.GetChild(0).GetComponent<TrailRenderer>().time;
        while(Time.time-startTime<trailduration)
        {
          this.transform.GetChild(0).GetComponent<TrailRenderer>().time = Mathf.Lerp(initialTime, 0f, (Time.time - startTime) / trailduration);
            yield return null; 
        }

        // Ensure that the trail time is set to 0 when fade-out completes
       this.transform.GetChild(0).GetComponent<TrailRenderer>().time = 0f;
        isFade = false;

    }


    private void DetectFieldGround()
    {
        float bounceForce = Random.Range(1f, 2.5f);
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
        onTouchGround?.Invoke(transform.position);
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
        onTouchGround?.Invoke(transform.position);
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
        if(!istouchbat)
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
