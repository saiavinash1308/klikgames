

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ClassicLudoGM;

public class ClassicLudoBluePP : ClassicLudoPP
{
    ClassicLudoRD blueHomeRollingDice;
    private SocketManager socketManager;
    void Start()
    {
        socketManager = FindObjectOfType<SocketManager>();
        if (socketManager == null)
        {
            Debug.LogError("SocketManager not found!");
            return; // Exit early to avoid null reference later
        }
        blueHomeRollingDice = GetComponentInParent<ClassicLudoBlue>().rollingdice;
    }
    public void OnMouseDown()
    {
        if (ClassicLudoGM.game.rolingDice == ClassicLudoGM.game.manageRolingDice[0])
        {
            if (isready && ClassicLudoGM.game.canPlayermove)
            {
                ClassicLudoGM.game.canPlayermove = false;
                movestep(pathparent.BluePlayerPathPoint);
                //GM.game.transferDice = false;
                //GM.game.RolingDiceManager(); // After moving, transfer the dice
                //GM.game.transferDice = true;
            }
            else if (!isready && ClassicLudoGM.game.rolingDice == blueHomeRollingDice)
            {
                ClassicLudoGM.game.blueOutPlayers += 1;
                makeplayerreadytomove(pathparent.BluePlayerPathPoint);
                ClassicLudoGM.game.numberofstepstoMove = 0;

                // Allow the player to roll again if they rolled a six and moved out of the home
                //GM.game.shouldRollAgain = true;
                ClassicLudoGM.game.canDiceRoll = true;
            }
        }
        if (socketManager != null && socketManager.isConnected)
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
        else
        {
            Debug.LogWarning("SocketManager is not connected. Cannot emit ROLL_DICE.");
        }
    }

    public void MovePiece()
    {
        if (ClassicLudoGM.game.rolingDice == ClassicLudoGM.game.manageRolingDice[0])
        {
            if (isready && ClassicLudoGM.game.canPlayermove)
            {
                ClassicLudoGM.game.canPlayermove = false;
                movestep(pathparent.BluePlayerPathPoint);
                //GM.game.transferDice = false;
                //GM.game.RolingDiceManager(); // After moving, transfer the dice
                //GM.game.transferDice = true;
            }
            else if (!isready && ClassicLudoGM.game.rolingDice == blueHomeRollingDice)
            {
                ClassicLudoGM.game.blueOutPlayers += 1;
                makeplayerreadytomove(pathparent.BluePlayerPathPoint);
                ClassicLudoGM.game.numberofstepstoMove = 0;

                // Allow the player to roll again if they rolled a six and moved out of the home
                //GM.game.shouldRollAgain = true;
                ClassicLudoGM.game.canDiceRoll = true;
            }
        }
    }
}
