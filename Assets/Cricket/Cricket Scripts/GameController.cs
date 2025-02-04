using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Random=UnityEngine.Random;

public enum GameMode{ Batsman, Bowler, Win, Lose, Draw, Menu }

public class GameController : MonoBehaviour
{
    public static GameController instance;
    [Header("Settings")]
    public int player1score;
    public int player2score;
    public GameMode gamemode;
    public GameMode firststate;
    public int levelindex;
    [Header("Events")]
    public static Action onGameSet;
    public static Action<GameMode> onGameModeChanged;
    private SocketManager socketmanager;

    private void Awake()
    {
        Application.targetFrameRate = 30;
        socketmanager = GameObject.FindObjectOfType<SocketManager>();
        socketmanager.gamecontroller = this.GetComponent<GameController>();
        if (instance==null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
       else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
        ResetScore();
        ScoreManager.onScoreCalculated += ScoreCalculated;
    }

    private void OnDestroy()
    {
        ScoreManager.onScoreCalculated -= ScoreCalculated;
    }

    private void ScoreCalculated(int score)
    {
        if(GameController.instance.isBowler())
        {
            player2score += score;
        }
        else
        {
            player1score += score;
        }
    }

    public void PlayButton()    //BEGIN GAME
    {

        if(levelindex==0)
        {
            firststate = GameMode.Batsman;
        }

        gamemode = firststate;
        onGameSet?.Invoke();
        Invoke("StartGame", 3f);        // CALL METHOD
    }

    public void StartGame()
    {
        if (firststate == GameMode.Batsman)
        {
            SceneManager.LoadScene("PLAYER1BAT");  //PLAYER1BAT
        }
    }


    public bool StartNextGame()
    {
        if(gamemode==firststate)
        {
            Debug.Log("first mode is done");
            Invoke("StartNextMode", 4f);
            return true;
        }
        else
        {         
            FinalScore();       // check score after scene is over
            return false;
        }
    }

    public void StartNextMode()
    {
        if (firststate == GameMode.Bowler)
        {
            gamemode = GameMode.Batsman;
            SceneManager.LoadScene("PLAYER1BAT");  //PLAYER1BAT
        }

        else
        {
            gamemode = GameMode.Bowler;
            SceneManager.LoadScene("PLAYER2BAT");   //PLAYER2BAT
        }
    }

    public void FinalScore()
    {
        if(isPlayer1Win())
        {
            SetGameMode(GameMode.Win);
            // set win state
        }

        if(isPlayer2Win())
        {
            SetGameMode(GameMode.Lose);
            // set lose state
        }

        if(isDraw())
        {
            SetGameMode(GameMode.Draw);
            // draw state
        }
    }

    public void SetGameMode(GameMode gamemode)
    {
        this.gamemode = gamemode;
       // onGameModeChanged?.Invoke(gamemode);
    }

    public void NextButton()
    {
        SetGameMode(GameMode.Menu);
        SceneManager.LoadScene("Menu");
    }

    public int GetPlayer1Score()
    {
        return player1score;
    }

    public int GetPlayer2Score()
    {
        return player2score;
    }

    public bool isBowler()
    {
        return gamemode == GameMode.Bowler;
    }

    public bool isBatsman()
    {
        return gamemode == GameMode.Batsman;
    }

    public bool isPlayer1Win()
    {
        return player1score > player2score;
    }

    public bool isPlayer2Win()
    {
        return player1score < player2score;
    }

    public bool isDraw()
    {
        return player1score == player2score;
    }

    public void ResetScore()
    {
        player1score = 0;
        player2score = 0;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
