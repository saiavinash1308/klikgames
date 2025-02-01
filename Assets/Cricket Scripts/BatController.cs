using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class BatController : MonoBehaviour
{



    [Header("Settings")]
    [SerializeField]
    private Vector2 BowlingSpeedPos;
    [SerializeField]
    private AnimationCurve bowlingspeedcurve;
    public int currentBall;
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
    private CanvasGroup transitioncg;
    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private TextMeshProUGUI[] finalscoreText;


    public static Action OnAimStarted;
    public static Action OnBowlingStarted;
    public static Action OnStartNextBall;
    // Start is called before the first frame update

    private void Awake()
    {
        socketmanager = GameObject.FindObjectOfType<SocketManager>();
        socketmanager.batcontroller = this.GetComponent<BatController>();
    }

    IEnumerator Start()
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

    private void OnDestroy()
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

    public void PlayBall(Vector3 ballhitpos)
    {
        currentBall++;

        if (currentBall >= 6)
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
        scoreText.text = "<color #00aaff>" + GameController.instance.GetPlayer1Score() + "</color> - <color #ffaa00>" + GameController.instance.GetPlayer2Score() + "</color>";
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

    public void BallCaught()
    {
        Debug.Log("Wicket");
        currentBall = 2;
        PlayBall(Vector3.zero);
        StartCoroutine(OpenWicketPanel());
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
        if (GameController.instance.StartNextGame())   // Wicket taken 
        {
            UpdateScorePanel();
            ShowTransitionPanel();
        }
        else
        {
            UpdateFinalPanel();
        }
    }

    public void ResetBall()
    {
        StartCoroutine(Restarted());
    }

    private IEnumerator Restarted()
    {
        yield return new WaitForSeconds(5f);
        OnStartNextBall?.Invoke();
        StartAiming();
    }

    public void StartAiming()
    {
        OnAimStarted?.Invoke();
    }

    public void StartBowling()
    {
        OnBowlingStarted?.Invoke();
    }

    public void GameModeChanged(GameMode gamemode)
    {
 
            switch (gamemode)
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
