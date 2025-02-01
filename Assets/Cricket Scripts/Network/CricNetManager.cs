using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using SimpleJSON;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;  // Ensure you are using Newtonsoft's JSON library
using System;
using UnityEngine.UI;
using TMPro;


public class CricNetManager : MonoBehaviour
{

    public static CricNetManager Instance { get; private set; }
    public bool isPlayer1;
    public bool isPlayer2;

    public BatsmanCamera batsmanCamera;
    public BowlCamera bowlCamera;
    [HideInInspector]
    public BatController batcontroller;
    [HideInInspector]
    public BowlController bowlcontroller;
  
    public GameObject batPanel;
    public GameObject bowlPanel;
    public GameObject powerPanel;

[Header("Sockets")]
public string Player1SocketId;
public string Player2SocketId;

public TextMeshProUGUI Player1Text;
public TextMeshProUGUI Player2Text;
private SocketManager socketmanager;


    private string playerId;  // Store the player role/ID (Player1 or Player2)

    public string PlayerId
    {
        get
        {
            return playerId;
        }
    }

 
 
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); 

    }

    void Start()
    {
        socketmanager = GameObject.FindObjectOfType<SocketManager>();
        socketmanager.cricnetmanager = this.GetComponent<CricNetManager>();

    }



public void DisplayPlayer1Text()
{
    Player1Text.text=Player1SocketId.ToString();
}

public void DisplayPlayer2Text()
{
    Player2Text.text = Player2SocketId.ToString();
}


    



    public void FindObjects()
    {
        batsmanCamera = GameObject.FindObjectOfType<BatsmanCamera>();
        bowlCamera = GameObject.FindObjectOfType<BowlCamera>();
    }







    public void ActivatePlayer()
    {
        if (SceneManager.GetActiveScene().name== "PLAYER1BAT")
        {
            Debug.LogWarning("PLAYER 1 IS BATTING");
            if (isPlayer1)
            {
                Debug.Log("Assigning BatsmanCamera to Player 1.");
                batsmanCamera.enabled = true; // Enable BatsmanCamera script
                bowlCamera.enabled = false; // Disable BowlCamera script
                batPanel.SetActive(true);   // Activate batting panel
                bowlPanel.SetActive(false);
                powerPanel.SetActive(false);
                batcontroller.enabled = true;
                bowlcontroller.enabled = false;

                Debug.Log($"Player 1: {Player1SocketId} (batting)");
                Debug.Log($"Player 2: {Player2SocketId} (bowling)");
            }
            else if (isPlayer2)
            {
                Debug.Log("Assigning BowlCamera to Player 2.");
                batsmanCamera.enabled = false; // Disable BatsmanCamera script
                bowlCamera.enabled = true; // Enable BowlCamera script

                batPanel.SetActive(false);
                bowlPanel.SetActive(true);    // Activate bowling panel
                powerPanel.SetActive(true);
                batcontroller.enabled = false;
                bowlcontroller.enabled = true;

                Debug.Log($"Player 1: {Player1SocketId} (batting)");
                Debug.Log($"Player 2: {Player2SocketId} (bowling)");
            }
        }
        if(SceneManager.GetActiveScene().name=="PLAYER2BAT")
        {
            Debug.Log("PLAYER 2 IS BATTING");
            if (isPlayer2)
            {
                Debug.Log("Assigning BatsmanCamera to Player 2.");
                batsmanCamera.enabled = true; // Enable BatsmanCamera script
                bowlCamera.enabled = false; // Disable BowlCamera script
                batPanel.SetActive(true);   // Activate batting panel
                bowlPanel.SetActive(false);
                powerPanel.SetActive(false);
                batcontroller.enabled = true;
                bowlcontroller.enabled = false;
                Debug.Log($"Player 1: {Player1SocketId} (bowling)");
                Debug.Log($"Player 2: {Player2SocketId} (batting)");
            }
            else if (isPlayer1)
            {
                Debug.Log("Assigning BowlCamera to Player 1.");
                batsmanCamera.enabled = false; // Disable BatsmanCamera script
                bowlCamera.enabled = true; // Enable BowlCamera script
                batPanel.SetActive(false);   // Activate batting panel
                bowlPanel.SetActive(true);
                powerPanel.SetActive(true);
                batcontroller.enabled = false;
                bowlcontroller.enabled = true;
                Debug.Log($"Player 1: {Player1SocketId} (bowling)");
                Debug.Log($"Player 2: {Player2SocketId} (batting)");
            }
        }

    }







    /*PLAYER MOVEMENT */

    // Handle player movement data from the server using SocketIOResponse
    private void OnPlayerMove(SocketIOResponse e)
    {
        // Get data from the response (assuming it's a JSON object)
        JSONNode data = e.ToString();  // Convert to JSONNode (SimpleJSON)
        string playerId = data["playerId"];
        float positionX = data["positionX"].AsFloat;

        // Create the new position for the player
        Vector3 newPosition = new Vector3(positionX, transform.position.y, transform.position.z);

        // Sync the player's position based on the received data
        if (playerId != gameObject.name)  // Make sure not to update the current player's position
        {
            transform.position = newPosition;
        }
    }
    

    // Handle animation event using SocketIOResponse
    private void OnAnimationEvent(SocketIOResponse e)
    {
        JSONNode data = e.ToString();  // Convert to JSONNode (SimpleJSON)
        string playerId = data["playerId"];
        string animationState = data["animationState"];

        // Trigger the animation based on the event received
        if (playerId != gameObject.name)
        {
            TriggerAnimation(animationState);
        }
    }

    // Trigger animation based on state
    private void TriggerAnimation(string animationState)
    {
        Animator anim = GetComponent<Animator>();

        if (animationState == "Right")
        {
            anim.Play("MoveRight");
        }
        else if (animationState == "Left")
        {
            anim.Play("MoveLeft");
        }
        else if (animationState == "Idle")
        {
            anim.Play("Idle");
        }
    }


    // Send player movement data to the server
    public void SendPlayerMove(float posX)
    {
        JSONNode data = new JSONObject();  // Create a new JSON object
        data["playerId"] = gameObject.name;  // Assign playerId
        data["positionX"] = posX;  // Assign positionX

   //     socket.Emit("player_move", data);  // Emit data to the server
    }

    // Send animation event to the server
    public void SendAnimationEvent(string animationState)
    {
        JSONNode data = new JSONObject();  
        data["playerId"] = gameObject.name;
        data["animationState"] = animationState;
  //      socket.Emit("animation_event", data);
    }
}
