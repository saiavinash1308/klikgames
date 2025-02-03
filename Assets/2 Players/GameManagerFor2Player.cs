using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManagerFor2Player : MonoBehaviour
{
    public static GameManagerFor2Player game;
    public RollingDiceFor2Player rolingDice;
    public int numberofstepstoMove;
    public bool canPlayermove = true;
    public List<PathPointFor2Player> playeronpathpointList = new List<PathPointFor2Player>();
    public bool canDiceRoll = true;
    public bool transferDice = false;
    public bool selfDice = false;
    public int redOutPlayers;
    public int yellowOutPlayers;
    public int redpoints = 0;
    public int yellowpoints = 0;


    public float rollTime = 10f;  // Time to roll the dice
    public float moveTime = 10f;  // Time to move after rolling the dice
    private float currentRollTime;
    private float currentMoveTime;

    private Coroutine countdownCoroutine;
    private bool hasRolledDice = false;

    public RollingDiceFor2Player[] manageRolingDice;
    public bool shouldRollAgain = false;

    public int redFinishedPlayers = 0;
    public int yellowFinishedPlayers = 0;
    public AudioSource ads;
    public AudioSource centerPathAudioSource; // Add this reference

    public float countdownTime = 10f; // Set the countdown time (10 seconds)
    private float currentCountdown; 
    public Image redTimerImage;
    public Image yellowTimerImage;
    
    public GameObject redarrow;
    public GameObject yellowarrow;

    private Text redScoreText;
    private Text yellowScoreText;
    public Text winnerText;
    public GameObject scorePopupPanel;

    public GameObject []Life;

    public int playerLives = 3;
    private Text livesText;
    public GameObject LifeOverPopup;
    public Text gameOverText;
    public Image[] heartImages;

    private const string TOGGLE_PREF_KEY = "ButtonTogglerState";


    private void Awake()
    {
        game = this;
        ads = GetComponent<AudioSource>();
        rolingDice = manageRolingDice[0]; // Ensure it starts with the blue player's dice
        UpdateDiceOpacity(); // Update opacity at start
    }

    private void Start()
    {
        StartCoroutine(BotRoutine());
        StartCountdown();
        UpdateArrowVisibility();
        ResizePlayerPieces(); // Ensure initial resizing
        if (!canDiceRoll)
        {
            redarrow.SetActive(false);
            yellowarrow.SetActive(false);
        }

        bool isOn = PlayerPrefs.GetInt(TOGGLE_PREF_KEY, 1) == 1;  // Default to 'on' if no value is saved

        SetAudioVolume(isOn);

    }

    public void SetAudioVolume(bool isOn)
    {
        float volume = isOn ? 1.0f : 0.0f;

        if (ads != null)
        {
            ads.volume = volume;
        }

        if (centerPathAudioSource != null)
        {
            centerPathAudioSource.volume = volume;
        }
    }

    private void Update()
    {
        if (!canDiceRoll)
        {
            redarrow.SetActive(false);
            yellowarrow.SetActive(false);
        }
    }


    public void AddPathPoint(PathPointFor2Player pathPoint)
    {
        playeronpathpointList.Add(pathPoint);
    }

    public void RemovePathPoint(PathPointFor2Player pathPoint)
    {
        if (playeronpathpointList.Contains(pathPoint))
        {
            playeronpathpointList.Remove(pathPoint);
        }
        else
        {
            Debug.Log("Path point not found to be removed");
        }
    }

    public void RollDice()
    {
        numberofstepstoMove = Random.Range(1, 7); // Simulate a dice roll between 1 and 6
        Debug.Log("Dice rolled: " + numberofstepstoMove);

        ResetCountdown();
    }


    public void UpdatePlayerPoints(int points)
    {
     
        if (rolingDice == manageRolingDice[0])
        {
            redpoints += points;
            redpoints = Mathf.Max(0, redpoints); // Ensure score doesn't go negative
            Debug.Log("RedPoints: " + redpoints);
        }
        else if (rolingDice == manageRolingDice[1])
        {
            yellowpoints += points;
            yellowpoints = Mathf.Max(0, yellowpoints); // Ensure score doesn't go negative
            Debug.Log("YellowPoints: " + yellowpoints);
        }
        ShowWinner();
    }

    internal void ShowWinner()
    {
        // Find the highest score
        int highestScore = Mathf.Max(redpoints, yellowpoints);
        string winner = "";
        
        if (highestScore == redpoints)
            winner = "Red";
        else if (highestScore == yellowpoints)
            winner = "Yellow";

        // Display the winner in the console
        Debug.Log("Winner is " + winner + " with " + highestScore + " points!");

        // Update the UI text with the winner's name and score
        winnerText.text = "Winner: " + winner + " with " + highestScore + " points!";
    }

    internal void ShowWinnerPopup()
    {
        scorePopupPanel.SetActive(true);
    }

    private void UpdateDiceOpacity()
    {
        for (int i = 0; i < manageRolingDice.Length; i++)
        {
            SpriteRenderer diceRenderer = manageRolingDice[i].transform.Find("NumberSpriteHolder").GetComponent<SpriteRenderer>();
            SpriteRenderer newObjectRenderer = manageRolingDice[i].transform.Find("DiceBoard").GetComponent<SpriteRenderer>();

            if (diceRenderer != null)
            {
                Color diceColor = diceRenderer.color;
                Color newObjectColor = newObjectRenderer.color;

                if (manageRolingDice[i] == rolingDice)
                {
                    diceColor.a = 1f;
                    newObjectColor.a = 1f;
                }
                else
                {
                    diceColor.a = 0.5f;
                    newObjectColor.a = 0.1f;
                }

                diceRenderer.color = diceColor;
                newObjectRenderer.color = newObjectColor;
            }
            else
            {
                Debug.LogWarning(manageRolingDice[i].name + " does not have a SpriteRenderer on its NumberSpriteHolder or the new GameObject.");
            }
        }
    }


    private void UpdateArrowVisibility()
    {
        redarrow.SetActive(false);
        yellowarrow.SetActive(false);

        if (rolingDice == manageRolingDice[0]) // Red player's turn
        {
            redarrow.SetActive(true);
        }
        else if (rolingDice == manageRolingDice[1]) // Yellow player's turn
        {
            yellowarrow.SetActive(true);
        }
    }

    public void RolingDiceManager()
    {
        const string TOGGLE_PREF_KEY = "ButtonTogglerState";  // Same key used in ButtonSpriteToggler
        bool isVibrationEnabled = PlayerPrefs.GetInt(TOGGLE_PREF_KEY, 1) == 1; // Default to 'on'

        if (shouldRollAgain)
        {
            shouldRollAgain = false;
            canDiceRoll = true;
            return;
        }

        if (transferDice)
        {
            int currentIndex = System.Array.IndexOf(manageRolingDice, rolingDice);
            int nextIndex = (currentIndex + 1) % manageRolingDice.Length;

            rolingDice = manageRolingDice[nextIndex];
            Debug.Log("Current Dice Index: " + currentIndex + " | Next Dice Index: " + nextIndex);

            if (isVibrationEnabled)
            {
//                Handheld.Vibrate();  // Only vibrate if the toggle is on
            }

            UpdateDiceOpacity();
            UpdateArrowVisibility();
            ResizePlayerPieces(); // Call to resize pieces

            canDiceRoll = true;
            transferDice = false;

            StartCountdown();
        }
        else if (selfDice)
        {
            selfDice = false;
            canDiceRoll = true;
            StartCountdown();
            UpdateArrowVisibility();
            ResizePlayerPieces(); // Call to resize pieces
        }
    }




    public void StartCountdown()
    {
        ResetCountdown();

        redTimerImage.gameObject.SetActive(false);
        yellowTimerImage.gameObject.SetActive(false);

        //}
        if (rolingDice == manageRolingDice[0])
        {
            redTimerImage.gameObject.SetActive(true);
            countdownCoroutine = StartCoroutine(CountdownCoroutine(redTimerImage));
        }
        //}
        else if (rolingDice == manageRolingDice[1])
        {
            yellowTimerImage.gameObject.SetActive(true);
            countdownCoroutine = StartCoroutine(CountdownCoroutine(yellowTimerImage));
        }
    }



    public void ResetCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
    }


    private IEnumerator CountdownCoroutine(Image currentTimerImage)
    {
        float startTime = Time.time;
        float endTime = startTime + countdownTime;

        while (Time.time < endTime)
        {
            float timeRemaining = endTime - Time.time;
            currentTimerImage.fillAmount = timeRemaining / countdownTime;

            yield return null;
        }

        currentTimerImage.fillAmount = 0;
        currentTimerImage.gameObject.SetActive(false);

        HandleTurnTimeout();
    }


    private void HandleTurnTimeout()
    {
        Debug.Log("Player's time is up!");
        LoseLife();
        UpdateLivesUI();
        transferDice = true;
        canDiceRoll = false;
        RolingDiceManager();
    }

    private void LoseLife()
    {
        if(playerLives > 0)
        {
            playerLives--;
        }
        if(playerLives <= 0)
        {
            HandleGameOver();
        }
    }

    private void UpdateLivesUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < playerLives)
            {
                heartImages[i].enabled = true;
            }
            else
            {
                heartImages[i].enabled = false;
            }
        }

        if(livesText != null)
        {
            livesText.text = "Lives:" + playerLives.ToString();
        }
    }

    private void HandleGameOver()
    {
        if (gameOverText != null)
        {
            LifeOverPopup.SetActive(true);  // Show the Game Over popup
            gameOverText.text = "You have missed the maximum number of turns!";  // Set the Game Over message
        }

        // Stop player movement
        canPlayermove = false;

        // Stop dice rolling
        canDiceRoll = false;

        // Disable all player pieces to stop any further interactions
        foreach (PlayerPiecesFor2Player piece in FindObjectsOfType<PlayerPiecesFor2Player>())
        {
            piece.enabled = false;
        }

        // Disable all dice
        foreach (RollingDiceFor2Player dice in manageRolingDice)
        {
            dice.enabled = false;
        }

        // Optionally, stop any other gameplay routines, like bot behavior or countdowns
        StopAllCoroutines();  // This stops all ongoing coroutines, including bot routines or timers

        // If you want to freeze the game completely, you can also pause the game time
        Time.timeScale = 0;  // Pauses the game completely
    }


    public void OnContinueBtn()
    {
        SceneManager.LoadScene("Home");
    }

    public void IncrementCenterCount(string playerColor)
    {
        switch (playerColor)
        {
            case "Red":
                redFinishedPlayers++;
                if (redFinishedPlayers == 4) LoadWinnerScene(playerColor);
                break;
            case "Yellow":
                yellowFinishedPlayers++;
                if (yellowFinishedPlayers == 4) LoadWinnerScene(playerColor);
                break;
        }
    }

    public bool HasPlayerWon(string playerColor)
    {
        switch (playerColor)
        {
            case "Red":
                return redFinishedPlayers == 4;
            case "Yellow":
                return yellowFinishedPlayers == 4;
            default:
                return false;
        }
    }

    public void CheckWinCondition()
    {
        
        if (HasPlayerWon("Red"))
        {
            Debug.Log("Red Player Wins!");
            EndGame();
        }
        
        else if (HasPlayerWon("Yellow"))
        {
            Debug.Log("Yellow Player Wins!");
            EndGame();
        }
    }

    private void EndGame()
    {
        foreach (PlayerPiecesFor2Player piece in FindObjectsOfType<PlayerPiecesFor2Player>())
        {
            piece.enabled = false;
        }

        foreach (RollingDiceFor2Player dice in manageRolingDice)
        {
            dice.enabled = false;
        }

        canDiceRoll = false;
        canPlayermove = false;
    }

    private void LoadWinnerScene(string winner)
    {
        PlayerPrefs.SetString("Winner", winner);
        SceneManager.LoadScene("Winner");
    }

    public void ResetDiceRoll()
    {
        numberofstepstoMove = 0;
    }

    public List<PlayerPiecesFor2Player> GetPlayerPiecesForCurrentDice()
    {
        List<PlayerPiecesFor2Player> playerPieces = new List<PlayerPiecesFor2Player>();

        foreach (PlayerPiecesFor2Player piece in FindObjectsOfType<PlayerPiecesFor2Player>())
        {
            //}
            if (piece.name.Contains("Red") && rolingDice == manageRolingDice[0])
            {
                playerPieces.Add(piece);
            }
            else if (piece.name.Contains("Yellow") && rolingDice == manageRolingDice[1])
            {
                playerPieces.Add(piece);
            }
        }

        return playerPieces;
    }

    private IEnumerator BotRoutine()
    {
        while (true)
        {
            
            if (rolingDice == manageRolingDice[1] && canDiceRoll)
            {
//                Handheld.Vibrate();
                manageRolingDice[1].RollDiceForBot();
                yield return new WaitForSeconds(1f);

                List<PlayerPiecesFor2Player> pieces = GetPlayerPiecesForCurrentDice();
                if (pieces.Count > 0)
                {
                    PlayerPiecesFor2Player pieceToMove = pieces[Random.Range(0, pieces.Count)];
                    PathPointFor2Player[] pathpoints = pieceToMove.GetPathPointsForColor();
                    pieceToMove.movestep(pathpoints);
                    yield return new WaitForSeconds(1f);
                }

                canDiceRoll = true;
                transferDice = true;
            }

            yield return new WaitForSeconds(1f);
        }

    }

    void ResizePlayerPieces()
    {
        string activeColor = "";
        if (rolingDice == manageRolingDice[0]) activeColor = "Red";
        else if (rolingDice == manageRolingDice[1]) activeColor = "Yellow";

        foreach (string color in new[] {"Red", "Yellow" })
        {
            float scale;
            float height;
            float depth;

            GameObject[] pieces = GameObject.FindGameObjectsWithTag(color);

            foreach (GameObject piece in pieces)
            {
                PlayerPiecesFor2Player playerPiece = piece.GetComponent<PlayerPiecesFor2Player>();

                bool isInSafeZone = playerPiece != null && playerPiece.IsInSafeZone;

                if (isInSafeZone)
                {
                    scale = 20f;   // Smaller scale when in the safe zone
                    height = 20f;
                    depth = 2f;
                }
                else
                {
                    scale = color == activeColor ? 30f : 20f;
                    height = color == activeColor ? 30f : 20f;
                    depth = color == activeColor ? 3f : 1.5f;
                }

                piece.transform.localScale = new Vector3(scale, height, depth);

                SpriteRenderer spriteRenderer = piece.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingOrder = Mathf.RoundToInt(-piece.transform.position.y * 100);
                    if (color == activeColor)
                    {
                        spriteRenderer.sortingOrder += 10;
                    }
                }
            }
        }
    }




    public void PlayCenterPathAudio()
    {
        if (centerPathAudioSource != null)
        {
            centerPathAudioSource.Play();
        }
    }
}
