using UnityEngine;
using static SocketManager;

public class UIManager : MonoBehaviour
{
    [Header("Dice Objects")]
    public GameObject[] playerDice; // Array to store dice objects for up to 4 players
    private Transform[] originalDicePositions;
    [Header("Dice Target Position")]
    public Transform diceTargetPosition; // Target position for the active dice

    internal bool isTwoPlayerMode = false;
    [SerializeField]
    private SocketManager socketManager;
    public User[] players;

    private void Awake()
    {
        socketManager = FindObjectOfType<SocketManager>();
    }
    void Start()
    {
        if (socketManager == null)
        {
            Debug.LogError("SocketManager not found!");
            return; // Exit early to avoid null reference later
        }
        // Save the initial transforms of the dice
        originalDicePositions = new Transform[playerDice.Length];
        for (int i = 0; i < playerDice.Length; i++)
        {
            if (playerDice[i] != null)
            {
                // Store the current transform of each dice
                originalDicePositions[i] = playerDice[i].transform;
            }
        }
    }

    public void UpdateDicePosition(int playerIndex)
    {
        if (playerDice == null || playerDice.Length == 0)
        {
            Debug.LogError("Player dice array is not initialized or empty.");
            return;
        }

        if (diceTargetPosition == null)
        {
            Debug.LogError("Dice target position is not set!");
            return;
        }

        if (isTwoPlayerMode)
        {
            // Map player indices for 2-player mode to corresponding dice indices
            if (playerIndex == 0)
            {
                playerIndex = 1; // First player uses dice[1]
            }
            else if (playerIndex == 1)
            {
                playerIndex = 3; // Second player uses dice[3]
            }
            else
            {
                Debug.LogWarning($"Invalid playerIndex {playerIndex} for 2-player mode.");
                return;
            }
        }
        else
        {
            // Ensure the playerIndex is valid for 4-player mode
            if (playerIndex < 0 || playerIndex >= playerDice.Length)
            {
                Debug.LogWarning($"Invalid playerIndex {playerIndex} for 4-player mode.");
                return;
            }
        }

        // Move the dice for the active player to the target position
        if (playerDice[playerIndex] != null)
        {
            playerDice[playerIndex].transform.position = diceTargetPosition.position;
            playerDice[playerIndex].transform.rotation = diceTargetPosition.rotation;

            Debug.Log($"Updated dice position for player index {playerIndex}");
        }
        else
        {
            Debug.LogWarning($"Dice for player index {playerIndex} is not assigned.");
        }
    }


    /// <summary>
    /// Resets all dice positions to their default locations.
    /// </summary>
    public void ResetDicePositions()
    {
        for (int i = 0; i < playerDice.Length; i++)
        {
            if (playerDice[i] != null && originalDicePositions[i] != null)
            {
                playerDice[i].transform.position = originalDicePositions[i].position;
                playerDice[i].transform.rotation = originalDicePositions[i].rotation;
            }
        }
    }
}
