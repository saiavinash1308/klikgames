using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fielder : MonoBehaviour
{
    public enum FieldMode { idle,running, fieldthrow,catching};
    public FieldMode fieldmode;
    
    public GameObject ball;
    public float fielderspeed;
    public float throwdist;
    public List<Transform> otherFielders;
    [SerializeField]
    public GameObject fieldthrowPos;
    [SerializeField]
    private Transform initialPos;

    private Animator anim;
    
    [SerializeField]
    private BallThrow ballthrower;
    [SerializeField]
    private GameObject bowlerPos;
    public GameObject ballPrefab;

    private Vector3 groundpos2;
    public bool isnearball;
    [SerializeField]
    private bool isthrown;
    [SerializeField]
    private float detectionRadius;
    private bool isballair;

    // Start is called before the first frame update
    void Start() //EVENTS CALLED 
    {
        Ball.onBallCaught += FielderCatched;
        Ball.onTouchGround += PlayBall;
        anim = GetComponent<Animator>();
        initialPos.position = this.transform.position;
        initialPos.rotation = this.transform.rotation;
    }

    private void OnDestroy() // EVENTS DESTROYED 
    {
        Ball.onBallCaught -= FielderCatched;
        Ball.onTouchGround -= PlayBall;
    }

    void Update()
    {
        if (ballthrower.cricball != null)
        {
            // Move towards the ball using OverlapSphere.
            DetectAndMoveTowardsBall();

            // Avoid collisions with other fielders.
            AvoidFielderCollisions();
        } 
    }

    public void PlayBall(Vector3 ballhitpos)
    {
        Debug.LogWarning("FIELDER RESET");
        transform.position = initialPos.position;
        transform.rotation = initialPos.rotation;
        fieldmode = FieldMode.idle; // idle mode 
    }

    private void DetectAndMoveTowardsBall()
    {
            Collider[] collidersInRange = Physics.OverlapSphere(transform.position, detectionRadius); // check if ball is in overlapsphere
            foreach (Collider col in collidersInRange)
            {
            if (col.CompareTag("Ball"))
            {
                ball = col.gameObject; 
                MoveTowardsBall();  // fielder movement 
                return;
            }
            }
        }




    public void SwitchAnimStates()
    {
        
        switch(fieldmode)
        {
            case FieldMode.idle:            // Fielder is in idle state
                anim.SetInteger("FieldState", 0);
       
                break;
            case FieldMode.running:         // Fielder is in running state
                anim.SetInteger("FieldState", 1);
      
                break;
            case FieldMode.fieldthrow:      // Fielder is in throwing state
                anim.SetInteger("FieldState", 2);
     
                break;
            case FieldMode.catching:       // Fielder is in catching state 
                anim.SetInteger("FieldState", 3);
                Debug.Log("catching");
                break;
        }
    }

    void AvoidFielderCollisions()       // check for collisions with other fielders 
    {
        foreach (var fielder in otherFielders)
        {
            float distanceToOtherFielder = Vector3.Distance(transform.position, fielder.position);
            if (distanceToOtherFielder < 2f)
            {
                // Simple collision avoidance: move away from the other fielder
                Vector3 avoidanceDirection = (transform.position - fielder.position).normalized;
                transform.position += avoidanceDirection * fielderspeed * Time.deltaTime;
            }
        }
    }
    public void MoveTowardsBall()       
    {
   
        ball = ballthrower.cricball;
       
        Vector3 direction = (ball.transform.position - transform.position).normalized;  // get direction between ball and fielder 
        float distance = Vector3.Distance(transform.position, ball.transform.position);
        if (!isnearball)
        {
            if (distance > 0.4f)    // distance to ball 
            {
                fieldmode = FieldMode.running; // running mode
                if (fieldmode == FieldMode.running)
                {
                    transform.LookAt(ball.transform); // fielder looks at ball 
                    SwitchAnimStates(); 
                    transform.position += direction * fielderspeed * Time.deltaTime;    // move transform position
                }
            }
            else
            {
                FieldBall(); // activate fielding 
            }
        }
        if(isnearball && isballinair(ball.GetComponent<Rigidbody>()))
        {       
            Catchball(); // activate catching
        }
    }

    public void Catchball()
    {
            float catchdistance = Vector3.Distance(ball.transform.position, this.transform.position); // distance between ball and fielder
            
            if(catchdistance<0.1f)
            {
                FielderCatched();
                ball.GetComponent<Rigidbody>().isKinematic = true;
                ball.transform.position = transform.position + Vector3.up * 1.0f;  // Position above the fielder               
                ball.transform.SetParent(transform);
                FielderCatched(); 
            }
            else
            {
                FielderMissCatch();
            }      
    }

    public void FielderCatched()
    {
        if (isballinair(ball.GetComponent<Rigidbody>())) // check for ball in air 
        {
            fieldmode = FieldMode.catching;
            anim.Play("Catching");
            Debug.Log("Fielder caught the ball");
        }
    }

    public void FielderMissCatch()
    {
        if (isballinair(ball.GetComponent<Rigidbody>())) // check for ball in air 
        {
            fieldmode = FieldMode.catching;
            Debug.Log("Fielder missed the ball");
        }
    }


    public void FieldBall() 
    {
        if (!isballinair(ball.GetComponent<Rigidbody>())) // check for ball in air 
        {
            if (Vector3.Distance(transform.position, ball.transform.position) < 1f)
            {
                isnearball = true;
                fieldmode = FieldMode.fieldthrow;        // throw mode 
                SwitchAnimStates();
                transform.LookAt(bowlerPos.transform);
                ThrowBall();                            // activating throw 
            }
        }
    }


    public void ThrowBall()
    {
        if (!isthrown)
        {
            isthrown = true;
            StartCoroutine(FieldThrowBall(2f));
        } 
        
    }

    public IEnumerator FieldThrowBall(float waitsecs)        // call animation state and throw ball to bowler
    {
        yield return new WaitForSeconds(waitsecs);
        if (fieldmode == FieldMode.fieldthrow)
        {
                Debug.LogError("ball is thrown");
                Destroy(ballthrower.cricball, 1f);
                Vector3 fieldtobowlertargetpos = bowlerPos.transform.position;
                groundpos2 = Physics.gravity * 1.4f * 1.4f / 2;
                Vector3 fielderpos = fieldthrowPos.transform.position;
                Vector3 initialVelocity = (fieldtobowlertargetpos - groundpos2 - fielderpos) / 1.5f;
                GameObject fieldball = Instantiate(ballPrefab, fieldthrowPos.transform.position, Quaternion.identity);
                fieldball.GetComponent<Rigidbody>().velocity = initialVelocity;
                fieldmode = FieldMode.idle; // Reset State
                Destroy(fieldball, 2f);
         
        }
    }

    public bool isballinair(Rigidbody rb)
    {
        if(Mathf.Abs(rb.velocity.y)>1f)
        {
            Debug.Log("ball is in air");
            return true;
        }
        else
        {
            return false;
        }  
    }

    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
