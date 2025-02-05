using System.Collections;
using System.Collections.Generic;
//using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SocketManager;

public class ClassicLudoRD : MonoBehaviour
{
    [SerializeField] Sprite[] numberSprites;  // Array to hold the sprites for each dice face (1 to 6)
    [SerializeField] SpriteRenderer numberSpriteHolder;  // The SpriteRenderer that will display the dice face
    [SerializeField] SpriteRenderer rolingdiceanimation;  // SpriteRenderer for the rolling animation
    [SerializeField] int numberGot;  // The result of the dice roll

    public GameObject SpriteHolder;
    public GameObject DiceAnimation;

    private Coroutine generateRandomNumberonDice;
    public int outpieces;
    public ClassicLudoDA diceSound;
    //internal static int onMouseDownCallCount;

    private SocketManager socketManager;

    private void Awake()
    {
        //onMouseDownCallCount = 0;
        numberSpriteHolder = transform.Find("NSH").GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
       
        socketManager = FindObjectOfType<SocketManager>();
        if (socketManager == null)
        {
            Debug.LogError("SocketManager not found!");
            return; // Exit early to avoid null reference later
        }
    }

    // Add a static counter variable to count the number of times OnMouseDown is called.
    

    public void OnMouseDown()
    {
        //onMouseDownCallCount++;

        // If the function has been called more than once, break early.
        //if (onMouseDownCallCount > 1)
        //{
        //    Debug.LogWarning("OnMouseDown has already been called more than once. Ignoring further calls." + "Count:" + onMouseDownCallCount);
        //    return; // Exit the method early.
        //}

        // Your existing debug logs for socketManager state.
        Debug.Log("SocketManager is not null: " + (socketManager != null));
        Debug.Log("SocketManager is connected: " + (socketManager != null && socketManager.isConnected));

        if (ClassicLudoGM.game.canDiceRoll && ClassicLudoGM.game.rolingDice == this)
        {
            //ClassicLudoGM.game.ResetCountdown();
            generateRandomNumberonDice = StartCoroutine(RollingDiceCoroutine());
            if (socketManager != null && socketManager.isConnected)
            {
                if (socketManager.GetSocketId() == socketManager.GetSocketId())
                {
                    string roomId = socketManager.GetRoomId();
                    socketManager.socket.Emit("ROLL_DICE", roomId);
                    Debug.Log("Room Id: " + roomId);

                }
            }
            else
            {
                Debug.LogWarning("SocketManager is not connected. Cannot emit ROLL_DICE.");
            }
        }
    }




    public void RollDiceForBot()
    {
        //generateRandomNumberonDice = StartCoroutine(RollingDiceCoroutine());
        Debug.LogWarning("Rolling dice for Bot...");
        //DiceAnimation.SetActive(true);
        //SpriteHolder.SetActive(false);
        generateRandomNumberonDice = StartCoroutine(RollingDiceCoroutine());
    
    //diceSound.PlaySound();
    //numberSpriteHolder.gameObject.SetActive(false);
    //rolingdiceanimation.gameObject.SetActive(true);

    //numberGot = socketManager.GetDiceValue();
    //numberSpriteHolder.sprite = numberSprites[numberGot -= 1];
    }

    private IEnumerator RollingDiceCoroutine()
    {
        Debug.LogWarning("Rolling dice...");
        yield return new WaitForEndOfFrame();

        if (ClassicLudoGM.game.canDiceRoll)
        {
            ClassicLudoGM.game.canDiceRoll = false;
            diceSound?.PlaySound();
            numberSpriteHolder.gameObject.SetActive(false);
            rolingdiceanimation.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.8f);

            // Fetch the dice value
            int? diceValue = socketManager?.GetDiceValue();

            if (!diceValue.HasValue || diceValue <= 0 || diceValue > numberSprites.Length)
            {
                Debug.LogError("Invalid or missing dice value.");
                ClassicLudoGM.game.canDiceRoll = true; // Reset the state so the player can retry
                rolingdiceanimation.gameObject.SetActive(false);
                yield break; // Exit the coroutine
            }

            numberGot = diceValue.Value;
            Debug.LogWarning("Dice rolled: " + numberGot);

            numberSpriteHolder.sprite = numberSprites[numberGot - 1];
            ClassicLudoGM.game.numberofstepstoMove = numberGot;
            ClassicLudoGM.game.rolingDice = this;

            numberSpriteHolder.gameObject.SetActive(true);
            rolingdiceanimation.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.5f);

            bool isPlayerOut = ClassicLudoGM.game.IsPlayerOut(ClassicLudoGM.game.rolingDice);
            ClassicLudoGM.game.canPlayermove = (isPlayerOut || numberGot == 6);
            ClassicLudoGM.game.canPlayermove = true;
            if (generateRandomNumberonDice != null)
            {
                StopCoroutine(generateRandomNumberonDice);
            }
        }
    }






    //internal void PassTurnToNextPlayer()
    //{
    //    //ClassicLudoGM.game.transferDice = true; // Set to true to transfer to the next player
    //    ClassicLudoGM.game.canDiceRoll = false; // Prevent rolling until the next player takes their turn
    //    Debug.Log("Passing turn to the next player.");
    //    //ClassicLudoGM.game.RolingDiceManager(); // Call to manage the next turn
    //}


    //bool CanPlayerMove()
    //{
    //    ClassicLudoGM ClassicLudoGM = ClassicLudoGM.game;
    //    List<PP> pieces = ClassicLudoGM.GetPlayerPiecesForCurrentDice();

    //    foreach (PP piece in pieces)
    //    {
    //        if ((ClassicLudoGM.numberofstepstoMove == 6 && piece.numberofstepsalreadymove == 0) || piece.CanMove(ClassicLudoGM.numberofstepstoMove))
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //bool AllPlayersCloseToCenterPath()
    //{
    //    ClassicLudoGM ClassicLudoGM = ClassicLudoGM.game;
    //    List<PP> pieces = ClassicLudoGM.GetPlayerPiecesForCurrentDice();

    //    foreach (PP piece in pieces)
    //    {
    //        if (!piece.IsCloseToCenterPath())
    //        {
    //            return false;
    //        }
    //    }
    //    return true;
    //}
}
