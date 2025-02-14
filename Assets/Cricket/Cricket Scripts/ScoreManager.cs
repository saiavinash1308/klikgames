using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    [Header("Settings")]
    
    public int currentball;
    [SerializeField]
    private TextMeshProUGUI[] scoretexts;
    [SerializeField]
    private float firstradius;
    [SerializeField]
    private float secondradius;
    public int playerScore;
    public int finalScore;
    public float testdistance;
    [SerializeField]
    private TextMeshProUGUI finalscoretext;
    private float waitTime = 3.5f;
    private SocketManager socketmanager;

    [Header("Events")]
    public static Action<int> onScoreCalculated;
    public GameObject batsman;
    public bool isScoreUpdate;

    // Start is called before the first frame update
    void Start()
    {
        batsman = GameObject.FindGameObjectWithTag("Batsman");
        socketmanager = GameObject.FindObjectOfType<SocketManager>();
        ResetScore();
        Time.timeScale = 1f;
        Ball.onTouchGround += BallTouchedGround;
        Ball.onBallMissed += BallMissed;
        Ball.onStumpsHit += StumpsHitted;
    }
    private void OnDestroy()
    {
        Ball.onTouchGround -= BallTouchedGround;
        Ball.onBallMissed -= BallMissed;
        Ball.onStumpsHit -= StumpsHitted;
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void BallTouchedGround(Vector3 ballpos)
    {
        if (isScoreUpdate)
            return;

        playerScore = 2;

        Vector3 batsmanPos = batsman.transform.position; // or wherever the reference point is
        float balldist = (ballpos - batsmanPos).magnitude;
        testdistance = balldist;

        if(balldist<firstradius && balldist> 25f)
        {
            playerScore+= 1;        // ball is in less distance
        }
        if(balldist>firstradius)
        {
            playerScore+= 2;        // ball is  in more distance first radius
        }
        if(balldist>secondradius)
        {
            playerScore += 2;       // ball is in more distance second radius
        }
        if (!socketmanager.isUsebots)
        {
            socketmanager.EmitEvent("UPDATE_SCORE", playerScore.ToString());
            isScoreUpdate = true;
        }

        else if(socketmanager.isUsebots)
        {
            UpdateScore();
        }
        

        

    }

    public void updateScoreFromSocket(int playerScore)
    {
        if (isScoreUpdate)
            return;
        Debug.LogError("UPDATE_SCORE");
        onScoreCalculated?.Invoke(playerScore);
        scoretexts[currentball].text = playerScore.ToString();
        currentball++;

        finalScore = finalScore + playerScore;          // display final score 
        finalscoretext.text = finalScore.ToString();
        StartCoroutine(ResetDistance(waitTime));
    }

    public void UpdateScore()
    {
        onScoreCalculated?.Invoke(playerScore);
        scoretexts[currentball].text = playerScore.ToString();
        currentball++;

        finalScore = finalScore + playerScore;          // display final score 
        finalscoretext.text = finalScore.ToString();
        StartCoroutine(ResetDistance(waitTime));
    }



    private IEnumerator ResetDistance(float waitsecs)
    {
        yield return new WaitForSeconds(waitsecs);
        isScoreUpdate = false;
        testdistance = 0f;          //reset distance after some time
    }


    public void BallMissed()
    {
        if (!socketmanager.isUsebots)
        {
            socketmanager.EmitEvent("UPDATE_SCORE", "0");
        }
        else if(socketmanager.isUsebots)
        {
            Debug.LogError("UPDATE_SCORE BALL MISSED");
            scoretexts[currentball].text = "0";     // dot ball 
            currentball++;
            onScoreCalculated?.Invoke(0);
            isScoreUpdate = true;
            StartCoroutine(ResetDistance(waitTime));
        }
    }

    public void BallMissedFromSocket()
    {
        if (isScoreUpdate)
            return;
        Debug.LogError("UPDATE_SCORE BALL MISSED");
        scoretexts[currentball].text = "0";     // dot ball 
        currentball++;
        onScoreCalculated?.Invoke(0);
        isScoreUpdate = true;
        StartCoroutine(ResetDistance(waitTime));
    }

    

    public void StumpsHitted()
    {
        if (!socketmanager.isUsebots)
        {
            socketmanager.EmitEvent("WICKET", "");
        }
        else if(socketmanager.isUsebots)
        {
            scoretexts[currentball].text = "w";
        }
    }

    public void HitStumpsFromSocket()
    {
        scoretexts[currentball].text = "w";
    }




    public void playNewLevel()
    {
        GameController.instance.StartNextGame();
    }


    public void ResetScore()
    {
        for(int i=0;i<scoretexts.Length;i++)        // reset score when starting
        {
            scoretexts[i].text = "";
        }
    }
}
