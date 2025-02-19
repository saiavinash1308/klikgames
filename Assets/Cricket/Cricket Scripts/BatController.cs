using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class BatController : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField]
    private Vector2 BowlingSpeedPos;    // get bowler speed
    [SerializeField]
    private AnimationCurve bowlingspeedcurve; // set bowling speed aimation curve
    public int currentBall; // ball number
    [SerializeField]
    private ScoreManager scoremanager;
    [SerializeField]
    private SocketManager socketmanager;

    [Header("UI")]
    [SerializeField]
    private GameObject WinPanel;    
    [SerializeField]
    private GameObject DrawPanel;
    [SerializeField]
    private GameObject LosePanel;
    [SerializeField]
    private CanvasGroup transitioncg; // initial panel
    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private TextMeshProUGUI[] finalscoreText; // finalscore

    
    public static Action OnAimStarted; 
    public static Action OnBowlingStarted;
    public static Action OnStartNextBall;
    // Start is called before the first frame update

    private void Awake()
    {
        socketmanager = GameObject.FindObjectOfType<SocketManager>();
        socketmanager.batcontroller = this.GetComponent<BatController>();
    }

    IEnumerator Start() // Events Called 
    {
        yield return null;
        Time.timeScale = 1f;
        StartAiming();
        Ball.onTouchGround += PlayBall;
        Ball.onBallMissed += BallMissed;
        Ball.onStumpsHit += StumpsCollided;
        Ball.onBallCaught += BallCaught;

        GameController.onGameModeChanged += GameModeChanged;

        
    }

    private void OnDestroy() // Events Destroyed
    {

        Ball.onTouchGround -= PlayBall;
        Ball.onBallMissed -= BallMissed;
        Ball.onStumpsHit -= StumpsCollided;
        Ball.onBallCaught -= BallCaught;

        GameController.onGameModeChanged -= GameModeChanged;
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void PlayBall(Vector3 ballhitpos) // update after every ball 
    {
        currentBall++;

        if (currentBall >= 6)
        {
            // show final panel score

            if (GameController.instance.StartNextGame())
            {
                UpdateScorePanel(); // show score panel 
                ShowTransitionPanel(); // show initial panel at start 
            }
            else
            {
                UpdateFinalPanel(); // show final panel 
            }
            // switch players for bowling and batting
        }
        else
        {
            ResetBall(); // reset
        }
    }



    private void UpdateScorePanel()
    {
        //GET PLAYER1SCORE AND PLAYER2SCORE 
        scoreText.text = "<color #00aaff>" + GameController.instance.GetPlayer1Score() + "</color> - <color #ffaa00>" + GameController.instance.GetPlayer2Score() + "</color>";
    }

    public void ShowTransitionPanel()
    {
        LeanTween.alphaCanvas(transitioncg, 1, 0.5f); // fadein the panel
        transitioncg.interactable = true; 
        transitioncg.blocksRaycasts = true;
    }

    public void UpdateFinalPanel()
    {
        for(int i=0;i<finalscoreText.Length;i++)
        {
            // GET FINAL PLAYER1SCORE AND PLAYER2SCORE
            finalscoreText[i].text= "<color #00aaff>" + GameController.instance.GetPlayer1Score() + "</color> - <color #ffaa00>" + GameController.instance.GetPlayer2Score() + "</color>";
        }
    }


    public void BallMissed()
    {
        PlayBall(Vector3.zero);     // ball missed bat
    }

    public void BallCaught()
    {
        Debug.Log("ball caught");    // catch
        currentBall = 2;
        PlayBall(Vector3.zero);
        StartCoroutine(OpenWicketPanel()); // show wicket panel 
    }

    public void StumpsCollided()
    {
        Debug.Log("Wicket");    // wicket
        currentBall = 2;
        PlayBall(Vector3.zero);
        StartCoroutine(OpenWicketPanel());
    }

    private IEnumerator OpenWicketPanel()
    {
        yield return new WaitForSeconds(2f);        
        if (GameController.instance.StartNextGame())   // Wicket taken 
        {
            UpdateScorePanel();  // show score panel 
            ShowTransitionPanel(); // show initial panel at start 
        }
        else
        {
            UpdateFinalPanel(); // show final panel 
        }
    }

    public void ResetBall()
    {
        StartCoroutine(Restarted());
    }

    private IEnumerator Restarted()
    {
        yield return new WaitForSeconds(5f);
        OnStartNextBall?.Invoke(); //Call Events 
        StartAiming(); // Begin Aiming
    }

    public void StartAiming()
    {
        OnAimStarted?.Invoke(); // Switching Camera Event
    }

    public void StartBowling()
    {
        OnBowlingStarted?.Invoke(); // Call BowlingStart Event
    }

    public void GameModeChanged(GameMode gamemode)
    {
 
            switch (gamemode)   // show gamemodes and final condition 
            {
                case GameMode.Win:
                 //   ShowWinPanel();
                    break;
                case GameMode.Lose:
                 //   ShowLosePanel();
                    break;
                case GameMode.Draw:
                //    ShowDrawPanel();
                    break;
            }
        
    }

    public void ShowWinPanel()
    {
        WinPanel.SetActive(true); // display win panel
    }

    public void ShowDrawPanel()
    {
        DrawPanel.SetActive(true); // display draw panel
    }

    public void ShowLosePanel()
    {
        LosePanel.SetActive(true); // display lose panel 
    }

    public void NextButton()
    {
        GameController.instance.NextButton(); // Return back to Home Page
    }

}
