

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ClassicLudoGM;
using UnityEngine.UI;

public class ClassicLudoYellowPP : ClassicLudoPP
{
    ClassicLudoRD yellowHomeRollingDice;
    private SocketManager socketManager;
    //public Button Piece;
    void Start()
    {
        socketManager = FindObjectOfType<SocketManager>();
        if (socketManager == null)
        {
            Debug.LogError("SocketManager not found!");
            return; // Exit early to avoid null reference later
        }
        //makeplayerreadytomove(pathparent.YellowPlayerPathPoint);
        yellowHomeRollingDice = GetComponentInParent<ClassicLudoYellow>().rollingdice;



        //Piece.onClick.AddListener(delegate { OnMouseDown(); });
    }



    public void OnMouseDown()
    {
        if (ClassicLudoGM.game.rolingDice == ClassicLudoGM.game.manageRolingDice[3])
        {
            if (isready && ClassicLudoGM.game.canPlayermove)
            {
                ClassicLudoGM.game.canPlayermove = false;
                movestep(pathparent.YellowPlayerPathPoint);
                //GM.game.transferDice = false;
                //GM.game.RolingDiceManager(); // After moving, transfer the dice
                //GM.game.transferDice = true;
            }
            else if (!isready && ClassicLudoGM.game.rolingDice == yellowHomeRollingDice)
            {
                ClassicLudoGM.game.yellowOutPlayers += 1;
                makeplayerreadytomove(pathparent.YellowPlayerPathPoint);
                ClassicLudoGM.game.numberofstepstoMove = 0;

                // Allow the player to roll again if they rolled a six and moved out of the home
                //GM.game.shouldRollAgain = true;
                ClassicLudoGM.game.canDiceRoll = true;
            }
        }
        if (socketManager != null && socketManager.isConnected)
        {
            if (socketManager.getMySocketId() == ClassicLudoGM.game.turnSocketId)
            {

                string name = this.name;  // Get the name of the GameObject
                string numberPart = new string(name.Where(char.IsDigit).ToArray());
                string roomId = socketManager.GetRoomId();
                ClassicLudoMovePiecePayload payload = new ClassicLudoMovePiecePayload
                {
                    piece = numberPart,
                    roomId = roomId
                };
                string jsonPayload = JsonUtility.ToJson(payload);
                socketManager.socket.Emit("MOVE_PIECE", jsonPayload);
                Debug.Log("MovePiece:" + jsonPayload);
                Debug.Log("Sent piece ID: " + numberPart);
                Debug.Log("Sent room Id:" + roomId);
            }
        }
        else
        {
            Debug.LogWarning("SocketManager is not connected. Cannot emit ROLL_DICE.");
        }
    }
    //internal void YelloweMovePiece()
    //{
    //    if (socketManager.socket.Id != GM.game.turnSocketId)
    //    {
    //        Debug.LogWarning("It's not this player's turn.");
    //        return;
    //    }
    //    else
    //    {

    //        MovePiece();
    //        socketManager.socket.Emit("MOVE_UPDATED", "");
    //    }
    //}
    public void MovePiece()
    {
        if (ClassicLudoGM.game.rolingDice == ClassicLudoGM.game.manageRolingDice[3])
        {
            if (isready && ClassicLudoGM.game.canPlayermove)
            {
                ClassicLudoGM.game.canPlayermove = false;
                movestep(pathparent.YellowPlayerPathPoint);
                //GM.game.transferDice = false;
                //GM.game.RolingDiceManager(); // After moving, transfer the dice
                //GM.game.transferDice = true;
            }
            else if (!isready && ClassicLudoGM.game.rolingDice == yellowHomeRollingDice)
            {
                ClassicLudoGM.game.yellowOutPlayers += 1;
                makeplayerreadytomove(pathparent.YellowPlayerPathPoint);
                ClassicLudoGM.game.numberofstepstoMove = 0;

                // Allow the player to roll again if they rolled a six and moved out of the home
                //GM.game.shouldRollAgain = true;
                ClassicLudoGM.game.canDiceRoll = true;
            }
        }
        //if (socketManager != null && socketManager.isConnected)
        //{
        //    Debug.Log("Emitting UPDATE_MOVE for object: " + gameObject.name);
        //    socketManager.socket.Emit("MOVE_UPDATED", "");
        //}

    }

}