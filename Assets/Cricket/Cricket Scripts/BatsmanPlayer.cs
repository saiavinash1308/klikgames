using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using Newtonsoft.Json;

public class BatsmanPlayer : MonoBehaviour
{
    public enum BatState { moving, hitting };
    public BatState batstate;
    [Header("Values")]
  
    public Animator anim;
    [SerializeField]
    private BoxCollider batcol;
    [SerializeField]
    private Transform batHolder;
    public Collider[] detectballs;
    public float batradius;


    [SerializeField]
    private LayerMask ballmask;
    [SerializeField]
    private Vector2 minMaxhitVel;
    [SerializeField]
    private float hitduration;
    private bool canDetectHit;
    private float hitTimer;
    float flightduration;

    [Header("Movement")]
    [SerializeField]
    private float movespeed;

    private float targetxval;
    private Vector3 clickedPos;
    private Vector3 clickedtargetPos;

    [Header("Swipe Settings")]
    private Vector3 touchStartPos;
    private Vector3 touchEndPos;
    public float swipeDistance;
    float swipeThreshold = 100f;
    public Vector2 swipeDirection;

    [Header("UI")]
    public Slider playermoveslider;
    [Header("Events")]
    public static Action<Transform> onBallHit;

    [Header("BatCamera")]
    public Camera maincam;
    public CinemachineVirtualCamera batcam;
    public float rotationSpeed;
    [SerializeField]
    private bool ismove;
    [SerializeField]
    private Vector3 initialCamPosition;
    [SerializeField]
    private RectTransform swiperect;
    [SerializeField]
    private RectTransform dragrect;
    private bool isHolding = false;
    public float batsmanSpeed;


    [SerializeField]
    private BatController batcontroller;
    private float previousSliderValue = 0f;
    [SerializeField]
    private bool hasMissedBall;

    [Header("Socket")]
    public SocketManager socketmanager;

    // Start is called before the first frame update
    void Start()
    {
        batstate = BatState.moving;
        BowlPlayer.OnThrownBall += ThrownBallCallback;
        BowlController.OnStartNextBall += Restart;
        initialCamPosition = batcam.transform.position;
        Canvas canvas = swiperect.GetComponentInParent<Canvas>();
        canvas.worldCamera = Camera.main;
        socketmanager = GameObject.FindObjectOfType<SocketManager>();
        socketmanager.batsmanplayer = this.GetComponent<BatsmanPlayer>();
        Application.targetFrameRate = 60; // Set a target frame rate
        batcontroller = GameObject.FindObjectOfType<BatController>();

    }

    private void OnDestroy()
    {
        BowlPlayer.OnThrownBall -= ThrownBallCallback;
        BowlController.OnStartNextBall -= Restart;
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            SwitchState();       //  movement
        }
        else
        {
            SwipeBat();      // hitting
        }
    }



    public void SwipeBat()
    {

        if (batcontroller.enabled)
        {
            // Check for swipe gesture
            if (Input.GetMouseButtonDown(0)) // Detect initial touch or click
            {

                isHolding = true;
                touchStartPos = Input.mousePosition;
                if (dragrect != null)
                {
                    dragrect.gameObject.SetActive(true);
                    dragrect.position = touchStartPos;
                }

                if (swiperect != null)
                {
                    swiperect.gameObject.SetActive(false);
                }
            }

            if (Input.GetMouseButton(0) && isHolding)
            {
                Vector3 currentPosition = Input.mousePosition;
                Vector3 dragDirection = currentPosition - touchStartPos;


                // Update drag indicator
                if (dragrect != null)
                {
                    dragrect.position = currentPosition;
                }
                UpdateSwipeIndicator(touchStartPos, currentPosition);
            }



            if (Input.GetMouseButtonUp(0)) // Detect when the user stops touching or clicking
            {

                touchEndPos = Input.mousePosition;
                swipeDistance = (touchEndPos - touchStartPos).magnitude;
                // Hide drag indicator
                if (dragrect != null)
                {
                    dragrect.gameObject.SetActive(false);
                }
                if (swipeDistance > swipeThreshold) // Confirm swipe
                {
                    swipeDirection = touchEndPos - touchStartPos;
                    if (swiperect != null)
                    {
                        swiperect.gameObject.SetActive(false);
                    }
                    HandleSwipeAction(swipeDirection);
                }
                ResetSwipeValues();     // reset all swipe values 
            }
        }
    }

    private void UpdateSwipeIndicator(Vector3 startPos, Vector3 currentPos)
    {
        if (swiperect != null)
        {
            // Enable swipe indicator
            swiperect.gameObject.SetActive(true);

            // Convert screen positions to local UI space
            RectTransformUtility.ScreenPointToLocalPointInRectangle(swiperect.parent as RectTransform, startPos, Camera.main, out Vector2 localStart);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(swiperect.parent as RectTransform, currentPos, Camera.main, out Vector2 localEnd);

            // Calculate swipe distance
            float swipeDistance = Vector2.Distance(localStart, localEnd);

            // Gradually increase the scale of swiperect based on swipe distance
            float scaleFactor = Mathf.Lerp(1f, 3f, swipeDistance / 25f);
            swiperect.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

            // Calculate the angle of the swipe
            float angle = Mathf.Atan2(localEnd.y - localStart.y, localEnd.x - localStart.x) * Mathf.Rad2Deg;

            // Rotate the swipe indicator based on the angle
            Image swipeImage = swiperect.GetComponent<Image>();
            if (swipeImage != null)
            {
                swipeImage.transform.rotation = Quaternion.Euler(0, 0, angle); // Rotate the image
            }
        }
    }

    void ResetSwipeValues()
    {
        touchStartPos = Vector2.zero;
        touchEndPos = Vector2.zero;
        swipeDirection = Vector2.zero;
        swipeDistance = 0f;
        isHolding = false;
    }

    private void HandleSwipeAction(Vector3 swipeDir)
    {
        batstate = BatState.hitting;
        if (swipeDir.y > 0) // Upward swipe
        {
            HittingBat();
        }
        else if (swipeDir.y < 0) // Downward swipe
        {
            HittingBat();
        }
        else if (swipeDir.x > 0) // Right swipe
        {
            HittingBat();
        }
        else if (swipeDir.x < 0) // Left swipe
        {
            HittingBat();
        }
        StartCoroutine(DelayedCheckForHits(4f)); // Add a slight delay to account for swipe motion
    }

    private IEnumerator DelayedCheckForHits(float waitsecs)
    {
        yield return new WaitForSeconds(waitsecs);
        CheckForHits();
    }

    public bool moveLeft;
    public bool moveRight;
    public bool isIdle;
    float predictedX;
    string animationState;

    /*MOVEMENT*/
    public void ControlMovement()
    {
        float adjustedValue = 0;
        animationState = "Idle"; // Default animation state

        if (moveLeft)
        {
            adjustedValue = 1; // Moving left
            animationState = "Left";
        }
        else if (moveRight)
        {
            adjustedValue = -1; // Moving right
            animationState = "Right";
        }

        if (adjustedValue != 0)
        {
            predictedX = transform.position.x + (adjustedValue * movespeed * Time.deltaTime);
            if (!isIdle)
            {
                socketmanager.EmitEvent("MOVE_BATSMAN", predictedX.ToString()); // Only send predictedX
            }
        }

        if (adjustedValue != 0)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName(animationState))
            {
                anim.Play(animationState); // Only change animation if needed
            }
        }
        else if (adjustedValue == 0)
        {
            // When no movement, ensure Idle is played immediately
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                isIdle = true;
                socketmanager.EmitEvent("CRICKET_IDLE", "");
                anim.Play("Idle");
            }
        }
    }


    public void OnMoveLeftPressed()
    {
        Debug.LogError("LEFT");
        ismove = true;
        batstate = BatState.moving;
        moveLeft = true;
        moveRight = false;
        isIdle = false;

    }

    public void OnMoveRightPressed()
    {
        Debug.LogError("RIGHT");
        ismove = true;
        batstate = BatState.moving;
        moveRight = true;
        moveLeft = false; // Disable the opposite direction
        isIdle = false;
    }

    public void OnButtonReleased()
    {
        ismove = false;
        moveLeft = false;
        moveRight = false;
        isIdle = true;
        //  socketmanager.EmitEvent("MOVE_BATSMAN", transform.position.x.ToString()); // Emit current position (no movement)

        // Play idle animation immediately
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            //    anim.Play("Idle");
        }
    }


    public void MoveBatsman2(float predictedX)
    {

        float clampedX = Mathf.Clamp(predictedX, 22f, 25f);
        Vector3 targetPosition = new Vector3(clampedX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movespeed);
    }

    private void SwitchState()
    {
        switch (batstate)
        {
            case BatState.moving:
                ControlMovement();

                break;
            case BatState.hitting:
                if (canDetectHit)
                    CheckForHits();
                break;
        }
    }




    public void StartDetectingHits()
    {
        canDetectHit = true;
        hitTimer = 0;
    }

    public void StopDetectingHits()
    {
        canDetectHit = false;
        batstate = BatState.moving;
    }

    public void CheckForHits()
    {
        // Set the center of the sphere
        /*    Vector3 center = batcol.transform.TransformPoint(batcol.center);
            // Detect balls within the sphere
            detectballs = Physics.OverlapSphere(center, batradius, ballmask);

            if (detectballs.Length > 0)
            {
                foreach (var ball in detectballs)
                {
                    Debug.Log("Ball detected: " + ball.gameObject.name);
                    BallDetectedCallback(ball);
                }

            }
            else
            {
                    Debug.Log("ball not detected");
                    Restart();           

            }
            hitTimer += Time.deltaTime;  */
    }






    private void BallDetectedCallback(Collider ballcollider)
    {
        canDetectHit = false;
        ShootBall(ballcollider.transform);
    }
    private void ThrownBallCallback(float ballduration)
    {
        flightduration = ballduration;  // set ball duration 
        StartCoroutine(BatHitting());
    }

    private IEnumerator BatHitting()
    {
        float delay = flightduration - 0.1f;
        float newdelay = Random.Range(delay - 0.1f, delay + 0.1f);
        yield return new WaitForSeconds(newdelay);
        anim.Play("Hit");   //hitting event 
    }



    public void HittingBat()
    {
        //Hit event batsman

        socketmanager.EmitEvent("BATSMAN_HIT", "");

    }

    public void HittingBatFromSocket()
    {
        Debug.LogError("HittingBat method triggered");
        batstate = BatState.hitting;
        anim.Play("Hit");
        StartCoroutine(ResetBatting());
    }

    private IEnumerator ResetBatting()
    {
        yield return new WaitForSeconds(3f);
        Restart();
    }

    private void ShootBall(Transform ball)  // push ball after touching bat
    {
        float lerp = Mathf.Clamp01(hitTimer / hitduration);
        float hitvel = Mathf.Lerp(minMaxhitVel.y, minMaxhitVel.x, lerp);
        Vector3 hitVelVector = (Vector3.back + Vector3.up + Vector3.right * Random.Range(-1f, 1f)) * hitvel;
        ball.GetComponent<Ball>().TouchedBat(hitVelVector);
        onBallHit?.Invoke(ball);    // switch to ball cam*/
    }


    public void ResetIdle()
    {
        batstate = BatState.moving;     // reset after hit state
        anim.Play("Idle");
    }
    private void Restart()
    {
        socketmanager.EmitEvent("RESET_BATSMAN", "");
    }

    public void ResetBatsmanFromSocket()
    {
        Debug.Log("Setting batsman to idle");
        batstate = BatState.moving;
        anim.Play("Idle");
    }


    private void OnDrawGizmos()
    {
        if (batcol != null)
        {
            Vector3 center = batcol.transform.TransformPoint(batcol.center);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, batradius);
        }
    }
}
