using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject bowlCam; // bowler cam
    [SerializeField]
    private GameObject aimCam; // bowler aim cam
    [SerializeField]
    private GameObject ballCam; // ball cam
    [SerializeField]
    private string PlayerId; // 
    public CricNetManager networkmanager; // ref cricnetmanager
    // Start is called before the first frame update

    private void Awake() // EVENTS CALLED 
    {
        BowlController.OnAimStarted += ActivateAimCam;
        BowlController.OnBowlingStarted += ActivateBowlCam;
        Ball.onTouchGround += StopCamtoBall;
        networkmanager = GameObject.FindObjectOfType<CricNetManager>();
    }
    private void Start() // EVENTS CALLED 
    {

        //INITIALISE CAMERAS
        aimCam.SetActive(true);
        bowlCam.SetActive(false);
        ballCam.SetActive(false);
        PlayerId = networkmanager.PlayerId;
        Debug.LogError("PlayeId: +networkmanager.PlayerId");
        Bat.onBallHit += ActivateBallCam;
     
    }



    public void ActivateAimCam() // Activate Aim Camera 
    {
        aimCam.SetActive(true);
        bowlCam.SetActive(false);
        ballCam.SetActive(false);
    }

    public void ActivateBowlCam() // Activate Bowler Camera 
    {
        bowlCam.SetActive(true);
        aimCam.SetActive(false);
        ballCam.SetActive(false);
    }

    public void  ActivateBallCam(Transform ball)            // ACTIVATE BALL CAMERA SOCKET EVENT
    {
        Debug.Log(" im activated");
        ballCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = ball;
        ballCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = ball;
        bowlCam.SetActive(false);
        aimCam.SetActive(false);
        ballCam.SetActive(true);
    }

    private void StopCamtoBall(Vector3 hitpos) // Stop Camera 
    {
        ballCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = null;
        ballCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = null;
    }
    private void OnDestroy()
    {
        BowlController.OnAimStarted -= ActivateAimCam;
        BowlController.OnBowlingStarted -= ActivateBowlCam;
        Bat.onBallHit -= ActivateBallCam;
        Ball.onTouchGround -= StopCamtoBall;
    }
}
