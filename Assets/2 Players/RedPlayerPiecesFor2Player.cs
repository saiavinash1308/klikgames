/*   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPlayerPieces : PlayerPieces
{
   RollingDice redHomeRollingDice;
   void Start()
   {
       redHomeRollingDice = GetComponentInParent<RedHome>().rollingdice;
   }

   public void OnMouseDown()
   {
       if (GameManager.game.rolingDice != null)
       {
           if (!isready)
           {
               if (GameManager.game.rolingDice == redHomeRollingDice && GameManager.game.numberofstepstoMove == 6)
               {

                   GameManager.game.redOutPlayers += 1;
                   makeplayerreadytomove(pathparent.RedPlayerPathPoint);
                   GameManager.game.numberofstepstoMove = 0;
                   return;
               }
           }
           if (GameManager.game.rolingDice == redHomeRollingDice && isready && GameManager.game.canPlayermove)
           {
               GameManager.game.canPlayermove = false;
               movestep(pathparent.RedPlayerPathPoint);
           }
       }
   }
}
*/

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPlayerPieces : PlayerPieces
{
    RollingDice redHomeRollingDice;
    void Start()
    {
        redHomeRollingDice = GetComponentInParent<RedHome>().rollingdice;


        GameManager.game.redOutPlayers = 4;
        makeplayerreadytomove(pathparent.RedPlayerPathPoint);
        GameManager.game.numberofstepstoMove = 0;
        return;


    }

    public void OnMouseDown()
    {

        if (GameManager.game.rolingDice == redHomeRollingDice && isready && GameManager.game.canPlayermove)
        {
            Debug.Log($"Token {gameObject.name} has been clicked and is moving.");
            GameManager.game.canPlayermove = false;
            movestep(pathparent.RedPlayerPathPoint);
        }

    }
}
*/




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPlayerPiecesFor2Player : PlayerPiecesFor2Player
{
    RollingDiceFor2Player redHomeRollingDice;

    void Start()
    {
        redHomeRollingDice = GetComponentInParent<RedHomeFor2Player>().rollingdice;
        GameManagerFor2Player.game.redOutPlayers = 4;
        makeplayerreadytomove(pathparent.RedPlayerPathPoint);
        GameManagerFor2Player.game.numberofstepstoMove = 0;
    }

    void OnMouseUpAsButton()
    {
        // Check if it's the blue player's turn and the dice rolled corresponds to the blue player
        if (GameManagerFor2Player.game.rolingDice == GameManagerFor2Player.game.manageRolingDice[0] && !GameManagerFor2Player.game.canDiceRoll)
        {
            if (isready && GameManagerFor2Player.game.canPlayermove)
            {
                GameManagerFor2Player.game.canPlayermove = false;
                movestep(pathparent.RedPlayerPathPoint);
                GameManagerFor2Player.game.transferDice = false;
                GameManagerFor2Player.game.RolingDiceManager(); // After moving, transfer the dice
                GameManagerFor2Player.game.transferDice = true;
            }
            else if (!isready && GameManagerFor2Player.game.rolingDice == redHomeRollingDice)
            {
                GameManagerFor2Player.game.redOutPlayers = 4;
                makeplayerreadytomove(pathparent.RedPlayerPathPoint);

                // Allow the player to roll again if they rolled a six and moved out of the home
                GameManagerFor2Player.game.shouldRollAgain = true;
                GameManagerFor2Player.game.canDiceRoll = true;
            }
        }
    }
}


