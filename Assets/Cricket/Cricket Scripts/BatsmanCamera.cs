using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatsmanCamera : MonoBehaviour
{

    [SerializeField]
    private GameObject batsmanCam;
    [SerializeField]
    private GameObject ballCam;
    [SerializeField]
    private string PlayerId;
    public CricNetManager networkmanager;
    // Start is called before the first frame update

    private void Awake()
    {
        BatController.OnAimStarted += EnableBatCam;
        BatsmanPlayer.onBallHit += ActivateBallCam;
        Bat.onBallHit += ActivateBallCam;
        Ball.onTouchGround += StopCamtoBall;
        networkmanager = GameObject.FindObjectOfType<CricNetManager>();
    }
    private void Start()
    {
        //INITIALISE CAMERAS
        PlayerId = networkmanager.PlayerId;
        Debug.LogError("PlayerId:" + networkmanager.PlayerId);
        batsmanCam.SetActive(true);
        ballCam.SetActive(false);
    }

    private void OnDestroy()
    {
        BatController.OnAimStarted -= EnableBatCam;
        BatsmanPlayer.onBallHit -= ActivateBallCam;
        Bat.onBallHit -= ActivateBallCam;
        Ball.onTouchGround -= StopCamtoBall;
    }

    public void EnableBatCam()
    {
        batsmanCam.SetActive(true);
        ballCam.SetActive(false);
    }

    public void ActivateBallCam(Transform ball)     // Activate BALL CAMERA SOCKET EVENT
    {
        Debug.Log("Cam is activated");
        ballCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = ball;
        ballCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = ball;   
        ballCam.SetActive(true);
        batsmanCam.SetActive(false);
    }

    private void StopCamtoBall(Vector3 hitpos)
    {
        ballCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = null;
        ballCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = null;
    }
}