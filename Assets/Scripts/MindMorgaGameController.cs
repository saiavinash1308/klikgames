using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SocketManager;

public class MindMorgaGameController : MonoBehaviour
{
    public static MindMorgaGameController Mindgame { get; private set; }

    [SerializeField]
    private Sprite bgImage;

    public Sprite[] images;  // Pre-loaded images from Resources
    public List<Button> btns = new List<Button>();

    [SerializeField]
    private Button firstFlippedButton;
    [SerializeField]
    private Button secondFlippedButton;


    public Text Player1UserName;
    public Text Player2UserName;
    public Text prizePool;

    [SerializeField] private RectTransform arrowPlayer1;
    [SerializeField] private RectTransform arrowPlayer2;
    private Vector3 originalScale = Vector3.one;
    private Vector3 enlargedScale = new Vector3(1.5f, 1.5f, 1.5f);

    public User[] players;
    public static string currentTurnSocketId;
    public string turnSocketId;
    [SerializeField]
    private SocketManager socketManager;

    void Awake()
    {
        images = Resources.LoadAll<Sprite>("Sprites/Candy");
        socketManager = FindObjectOfType<SocketManager>();
        Mindgame = this;
    }

    private void Start()
    {
        if (socketManager == null)
        {
            Debug.LogError("SocketManager not found!");
            return;
        }
    }

    public void InitializePlayers(User[] users)
    {
        if (users.Length < 2 || users.Length > 4)
        {
            Debug.LogError("Invalid number of players. The game supports 2 to 4 players.");
            return;
        }

        players = users;
        UpdatePlayerUI();
    }

    private void UpdatePlayerUI()
    {
        if (players == null || players.Length < 2)
        {
            Debug.LogError("Invalid number of players. The game requires at least 2 players.");
            return;
        }

        Player1UserName.text = players[0].username;
        Player2UserName.text = players[1].username;
        prizePool.text = socketManager.getPrizePool().ToString();
    }

    public void HandlePlayerTurn(string socketId)
    {
        StartCoroutine(HandleTurnCoroutine(socketId));
    }

    private IEnumerator HandleTurnCoroutine(string socketId)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].socketId == socketId)
            {
                UpdateTurnArrow(i);
                yield return new WaitForSeconds(0.5f);
                break;
            }
        }
        turnSocketId = socketId;
    }

    private void UpdateTurnArrow(int playerIndex)
    {
        arrowPlayer1.localScale = originalScale;
        arrowPlayer2.localScale = originalScale;

        if (playerIndex == 0)
        {
            arrowPlayer1.localScale = enlargedScale;
        }
        else if (playerIndex == 1)
        {
            arrowPlayer2.localScale = enlargedScale;
        }
    }

    public void GetStarted()
    {
        GetButtons();
        AddListeners();
    }

    void GetButtons()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("GameButton");
        for (int i = 0; i < objects.Length; i++)
        {
            btns.Add(objects[i].GetComponent<Button>());
            btns[i].image.sprite = bgImage;
        }
    }

    void AddListeners()
    {
        foreach (Button btn in btns)
        {
            btn.onClick.AddListener(() => ClickButton());
        }
    }

    public void ClickButton()
    {
        if (socketManager != null && socketManager.isConnected)
        {
            if (socketManager.socket.Id == turnSocketId)
            {
                GameObject selectedObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
                string buttonName = selectedObject.name;
                Debug.LogError(selectedObject.name);

                // Enable the SpriteAnimator script
                selectedObject.transform.GetChild(0).gameObject.SetActive(true);
                selectedObject.GetComponent<Image>().enabled = false;


                // Emit the "PICK_CARD" event
                socketManager.socket.Emit("PICK_CARD", buttonName);
                Debug.Log($"PICK_CARD emitted for {buttonName}");

                // Enable the SpriteAnimator script

                // Track the flipped cards
                Button clickedButton = btns.Find(b => b.name == buttonName);
                if (firstFlippedButton == null)
                {
                    firstFlippedButton = clickedButton;
                    Debug.Log("Clicked 1");
                }
                else if (secondFlippedButton == null && clickedButton != firstFlippedButton)
                {
                    secondFlippedButton = clickedButton;
                    Debug.Log("Clicked 2");
                }
            }
        }
        else
        {
            Debug.LogWarning("SocketManager is not connected. Cannot emit PICK_CARD.");
        }
    }

    public void LoadCardSprite(int index, string cardName)
    {
        GameObject selectedObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        if (index < 0 || index >= btns.Count)
        {
            Debug.LogWarning($"Invalid card index: {index}");
            return;
        }

        // Find the button at the given index
        Button button = btns[index];

        // Load the sprite with the corresponding name
        Sprite sprite = System.Array.Find(images, img => img.name == cardName);
        if (sprite != null)
        {
            button.image.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"Sprite {cardName} not found in the loaded images!");
        }

        selectedObject.transform.GetChild(0).gameObject.SetActive(false);
        selectedObject.GetComponent<Image>().enabled = true;
    }




    public void CloseCardSprite(int index1, int index2)
    {
        // Find and reset the sprite of the first card
        Button firstButton = btns[index1];
        if (firstButton != null)
        {
            firstButton.image.sprite = bgImage;
        }

        // Find and reset the sprite of the second card
        Button secondButton = btns[index2];
        if (secondButton != null)
        {
            secondButton.image.sprite = bgImage;
        }

        Debug.Log("Cards closed: " + index1 + ", " + index2);
    }


    public void DisableMatchedCards(int index1, int index2)
    {
        // Disable the first matched card
        Button firstButton = btns[index1];
        if (firstButton != null)
        {
            firstButton.gameObject.SetActive(false);  // Disable the GameObject
        }

        // Disable the second matched card
        Button secondButton = btns[index2];
        if (secondButton != null)
        {
            secondButton.gameObject.SetActive(false);  // Disable the GameObject
        }

        Debug.Log("Matched cards disabled: " + index1 + ", " + index2);
    }





}
