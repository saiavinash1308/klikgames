using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class BowlController : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField]
    private GroundTarget groundtarget;
    [SerializeField]
    private PowerSlider powerslider;
    [SerializeField]
    private BowlPlayer bowlplayer;
    [SerializeField]
    private ScoreManager scoremanager;
    [SerializeField]
    private SocketManager socketmanager;

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
    private Vector2 BowlingSpeedPos;
    [SerializeField]
    private AnimationCurve bowlingspeedcurve;
    public int currentBall;


    public static Action OnAimStarted;
    public static Action OnBowlingStarted;
    public static Action OnStartNextBall;
    // Start is called before the first frame update

    private void Awake()
    {
        socketmanager = GameObject.FindObjectOfType<SocketManager>();
        socketmanager.bowlcontroller = this.GetComponent<BowlController>();
    }

    void Start()
    {
        StartAiming();
        Time.timeScale = 1f;
        powerslider.OnPowerSliderStopped += PowerSliderStopped;
        Ball.onTouchGround += PlayBall;
        Ball.onBallMissed += BallMissed;
        Ball.onStumpsHit += StumpsCollided;

        GameController.onGameModeChanged += GameModeChanged;
    }

    private void OnDestroy()
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

    public void PlayBall(Vector3 ballhitpos)
    {
        currentBall++;

        if(currentBall>=6)
        {
            // show final panel score
            if (GameController.instance.StartNextGame())
            {

                UpdateScorePanel();
                ShowTransitionPanel();
            }
            else
            {
                UpdateFinalPanel();
            }
            // switch players for bowling and batting
        }
        else
        {
           ResetBall();
        }
    }



    private void UpdateScorePanel()
    {
        scoreText.text="<color #00aaff>"+GameController.instance.GetPlayer1Score()+"</color> - <color #ffaa00>"+GameController.instance.GetPlayer2Score()+"</color>";
    }

    public void ShowTransitionPanel()
    {
        LeanTween.alphaCanvas(transitioncg, 1, 0.5f);
        transitioncg.interactable = true;
        transitioncg.blocksRaycasts = true;
    }

    public void UpdateFinalPanel()
    {
       for(int i=0;i<finalscoreText.Length;i++)
        {
            finalscoreText[i].text= "<color #00aaff>" + GameController.instance.GetPlayer1Score() + "</color> - <color #ffaa00>" + GameController.instance.GetPlayer2Score() + "</color>";
        }
    }



    public void BallMissed()
    {
        PlayBall(Vector3.zero);
    }

    public void ResetBall()
    {
        StartCoroutine(Restarted());
    }

    private IEnumerator Restarted()     // RESTART NEXT BALL 
    {
        bowlspeedtext.text = "";
        yield return new WaitForSeconds(2f);
        OnStartNextBall?.Invoke();
        StartAiming();
    }


    public void StumpsCollided()
    {
        Debug.Log("Wicket");
        currentBall = 2;
        PlayBall(Vector3.zero);
        StartCoroutine(OpenWicketPanel());
     
    }

    private IEnumerator OpenWicketPanel()
    {
        yield return new WaitForSeconds(2f);
        if (GameController.instance.StartNextGame())
        {

            UpdateScorePanel();
            ShowTransitionPanel();
        }
        else
        {          
            UpdateFinalPanel();
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

            switch (gamemode)
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
        WinPanel.SetActive(true);
    }

    public void ShowDrawPanel()
    {
        DrawPanel.SetActive(true);
    }

    public void ShowLosePanel()
    {
        LosePanel.SetActive(true);
    }

    public void NextButton()
    {
        GameController.instance.NextButton();
    }
}
