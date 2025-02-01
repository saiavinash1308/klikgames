using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject bowlCam;
    [SerializeField]
    private GameObject aimCam;
    [SerializeField]
    private GameObject ballCam;
    [SerializeField]
    private string PlayerId;
    public CricNetManager networkmanager;
    // Start is called before the first frame update

    private void Awake()
    {
        BowlController.OnAimStarted += ActivateAimCam;
        BowlController.OnBowlingStarted += ActivateBowlCam;
        Ball.onTouchGround += StopCamtoBall;
        networkmanager = GameObject.FindObjectOfType<CricNetManager>();
    }
    private void Start()
    {

        //INITIALISE CAMERAS
        aimCam.SetActive(true);
        bowlCam.SetActive(false);
        ballCam.SetActive(false);
        PlayerId = networkmanager.PlayerId;
        Debug.LogError("PlayeId: +networkmanager.PlayerId");
        AIBat.onBallHit += ActivateBallCam;
     
    }



    public void ActivateAimCam()
    {
        aimCam.SetActive(true);
        bowlCam.SetActive(false);
        ballCam.SetActive(false);
    }

    public void ActivateBowlCam()
    {
        bowlCam.SetActive(true);
        aimCam.SetActive(false);
        ballCam.SetActive(false);
    }

    public void  ActivateBallCam(Transform ball)
    {
        Debug.Log(" im activated");
        ballCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = ball;
        ballCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = ball;
        bowlCam.SetActive(false);
        aimCam.SetActive(false);
        ballCam.SetActive(true);
    }

    private void StopCamtoBall(Vector3 hitpos)
    {
        ballCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = null;
        ballCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = null;
    }
    private void OnDestroy()
    {
        BowlController.OnAimStarted -= ActivateAimCam;
        BowlController.OnBowlingStarted -= ActivateBowlCam;
        AIBat.onBallHit -= ActivateBallCam;
        Ball.onTouchGround -= StopCamtoBall;
    }
}
