using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class BowlController : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField]
    private GroundTarget groundtarget; // ref groundtarget
    [SerializeField]
    private PowerSlider powerslider; // ref powerslider
    [SerializeField]
    private BowlPlayer bowlplayer; // ref bowlplayer
    [SerializeField]
    private ScoreManager scoremanager; // ref scoremanager
    [SerializeField]
    private SocketManager socketmanager; // ref socketmanager

    [Header("UI")]
    [SerializeField]
    private GameObject aimingPanel;
    [SerializeField]
    private GameObject powerPanel;
    [SerializeField]
    private TextMeshProUGUI bowlspeedtext;
    private int bowlspeednumber;
    public GameObject WinPanel;
    public GameObject LosePanel;
    public GameObject DrawPanel;
    [SerializeField]
    private CanvasGroup transitioncg;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI[] finalscoreText;

    [Header("Speed")]
    [SerializeField]
    private Vector2 BowlingSpeedPos; // ref bowlingpos
    [SerializeField]
    private AnimationCurve bowlingspeedcurve; // set bowling animationcurve 
    public int currentBall;


    public static Action OnAimStarted;  // Events 
    public static Action OnBowlingStarted;
    public static Action OnStartNextBall;
    // Start is called before the first frame update

    private void Awake()
    {
        socketmanager = GameObject.FindObjectOfType<SocketManager>();
        socketmanager.bowlcontroller = this.GetComponent<BowlController>();
    }

    void Start() // EVENTS CALLED
    {
        StartAiming(); 
        Time.timeScale = 1f;
        powerslider.OnPowerSliderStopped += PowerSliderStopped;
        Ball.onTouchGround += PlayBall;
        Ball.onBallMissed += BallMissed;
        Ball.onStumpsHit += StumpsCollided;

        GameController.onGameModeChanged += GameModeChanged;
    }

    private void OnDestroy() // Events Destroyed
    {
        powerslider.OnPowerSliderStopped -= PowerSliderStopped;
        Ball.onTouchGround -= PlayBall;
        Ball.onBallMissed -= BallMissed;
        Ball.onStumpsHit -= StumpsCollided;

        GameController.onGameModeChanged -= GameModeChanged;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBall(Vector3 ballhitpos) // update after every ball
    {
        currentBall++;

        if(currentBall>=6)
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
        scoreText.text="<color #00aaff>"+GameController.instance.GetPlayer1Score()+"</color> - <color #ffaa00>"+GameController.instance.GetPlayer2Score()+"</color>";
    }

    public void ShowTransitionPanel()
    {
        LeanTween.alphaCanvas(transitioncg, 1, 0.5f);   // fadein the panel
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
        PlayBall(Vector3.zero); // ball missed bat 
    }

    public void ResetBall()
    {
        StartCoroutine(Restarted()); 
    }

    private IEnumerator Restarted()     // RESTART NEXT BALL 
    {
        bowlspeedtext.text = "";
        yield return new WaitForSeconds(2f);
        OnStartNextBall?.Invoke(); // call Events 
        StartAiming(); // Begin Aiming
    }


    public void StumpsCollided()
    {
        Debug.Log("Wicket"); // wicket
        currentBall = 2;
        PlayBall(Vector3.zero);
        StartCoroutine(OpenWicketPanel());
     
    }

    private IEnumerator OpenWicketPanel()
    {
        yield return new WaitForSeconds(2f);
        if (GameController.instance.StartNextGame())    // Wicket taken 
        {

            UpdateScorePanel(); // show score panel 
            ShowTransitionPanel(); // show initial panel at start 
        }
        else
        {          
            UpdateFinalPanel(); // show final panel 
        }
      
    }

    public void StartAiming()
    {
        groundtarget.EnableTarget();        // enable movement of ground target

        //hide slider
        aimingPanel.SetActive(true);
        powerPanel.SetActive(false);
        //enable aim cam

        //disable bowl cam
        OnAimStarted?.Invoke();
    }

    public void StartBowling()
    {
        groundtarget.DisableTarget();

        // show slider
        aimingPanel.SetActive(false);
        powerPanel.SetActive(true);

        OnBowlingStarted?.Invoke();

        //Enable powerslider
        powerslider.StartMoving();
    }

    private void PowerSliderStopped(float powervalue)
    {
        float tempvalue = bowlingspeedcurve.Evaluate(powervalue);
        float bowlingspeed = Mathf.Lerp(BowlingSpeedPos.x, BowlingSpeedPos.y, tempvalue);
        bowlplayer.StartRun(bowlingspeed);
        Debug.Log("BowlingSpeed" + bowlingspeed);
        bowlspeednumber = Mathf.RoundToInt(bowlingspeed);   // convert to int
        bowlspeedtext.text = bowlspeednumber.ToString();
    }

    public void GameModeChanged(GameMode gamemode)
    {

            switch (gamemode)    // show gamemodes and final condition 
        {
                case GameMode.Win:
                   // ShowWinPanel();
                    break;
                case GameMode.Lose:
                   // ShowLosePanel();
                    break;
                case GameMode.Draw:
                  //  ShowDrawPanel();
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
