/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowPlayerPieces : PlayerPieces
{
    RollingDice yellowHomeRollingDice;
    void Start()
    {
        yellowHomeRollingDice = GetComponentInParent<YellowHome>().rollingdice;
    }

    public void OnMouseDown()
    {
        if (GameManager.game.rolingDice != null)
        {
            if (!isready)
            {
                if (GameManager.game.rolingDice == yellowHomeRollingDice && GameManager.game.numberofstepstoMove == 6)
                {
                    GameManager.game.yellowOutPlayers += 1;
                    makeplayerreadytomove(pathparent.YellowPlayerPathPoint);
                    GameManager.game.numberofstepstoMove = 0;
                    return;
                }
            }
            if (GameManager.game.rolingDice == yellowHomeRollingDice && isready && GameManager.game.canPlayermove)
            {
                GameManager.game.canPlayermove = false;
                movestep(pathparent.YellowPlayerPathPoint);

            }
        }
    }
}

*/


/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowPlayerPieces : PlayerPieces
{
    RollingDice yellowHomeRollingDice;
    void Start()
    {
        yellowHomeRollingDice = GetComponentInParent<YellowHome>().rollingdice;


        GameManager.game.yellowOutPlayers = 4;
        makeplayerreadytomove(pathparent.YellowPlayerPathPoint);
        GameManager.game.numberofstepstoMove = 0;
        return;


    }

    public void OnMouseDown()
    {
        
            if (GameManager.game.rolingDice == yellowHomeRollingDice && isready && GameManager.game.canPlayermove)
            {
            Debug.Log($"Token {gameObject.name} has been clicked and is moving.");
            GameManager.game.canPlayermove = false;
                movestep(pathparent.YellowPlayerPathPoint);
            }
        
    }
}
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowPlayerPiecesFor2Player : PlayerPiecesFor2Player
{
    RollingDiceFor2Player yellowHomeRollingDice;

    void Start()
    {
        yellowHomeRollingDice = GetComponentInParent<YellowHomeFor2Player>().rollingdice;
        GameManagerFor2Player.game.yellowOutPlayers = 4;
        makeplayerreadytomove(pathparent.YellowPlayerPathPoint);
        GameManagerFor2Player.game.numberofstepstoMove = 0;
    }

    void OnMouseUpAsButton()
    {
        // Check if it's the blue player's turn and the dice rolled corresponds to the blue player
        if (GameManagerFor2Player.game.rolingDice == GameManagerFor2Player.game.manageRolingDice[1] && !GameManagerFor2Player.game.canDiceRoll)
        {
            if (isready && GameManagerFor2Player.game.canPlayermove)
            {
                GameManagerFor2Player.game.canPlayermove = false;
                movestep(pathparent.YellowPlayerPathPoint);
                GameManagerFor2Player.game.transferDice = false;
                GameManagerFor2Player.game.RolingDiceManager(); // After moving, transfer the dice
                GameManagerFor2Player.game.transferDice = true;
            }
            else if (!isready && GameManagerFor2Player.game.rolingDice == yellowHomeRollingDice)
            {
                GameManagerFor2Player.game.yellowOutPlayers = 4;
                makeplayerreadytomove(pathparent.YellowPlayerPathPoint);

                // Allow the player to roll again if they rolled a six and moved out of the home
                GameManagerFor2Player.game.shouldRollAgain = true;
                GameManagerFor2Player.game.canDiceRoll = true;
            }
        }
    }
}



