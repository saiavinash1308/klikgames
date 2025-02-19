using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CricketSearchingFor2Players : MonoBehaviour
{
    public bool isSearching;
    public Image loadingImage;
    public static CricketSearchingFor2Players Searching { get; private set; }
    private SocketManager socketManager;
    public GameController gamecontroller;

    public float waitTimer;
    public bool isSet = true;

    void Awake()
    {
        Searching = this;
        socketManager = FindObjectOfType<SocketManager>();
    }

    void Start()
    {
        if (socketManager == null)
        {
            Debug.LogError("SocketManager not found!");
            return; // Exit early to avoid null reference later
        }

        isSearching = true;
        StartCoroutine(SearchAndLoadCoroutine());
    }

    private void Update()
    {
        waitTimer += Time.deltaTime;
        if (isSet)
        {
            if (waitTimer > 20f)
            {
                Debug.Log("Matchmaking failed. Emitting event...");
                socketManager.EmitEvent("MATCH_MAKING_FAILED", ""); //BAT_AI OR BOWL_AI
                isSet = false; // Stop the loop
                isSearching = false; // Update searching state
                gamecontroller.PlayButton();

            }
        }
    }

    private IEnumerator SearchAndLoadCoroutine()
    {
        loadingImage.gameObject.SetActive(true); // Show loading image
        while (socketManager != null && socketManager.stopSearch)
        {
            loadingImage.fillAmount = Mathf.PingPong(Time.time, 1f); // Smooth fill between 0 and 1
            yield return new WaitForSeconds(2f);
        }

        loadingImage.gameObject.SetActive(false);

            if (waitTimer < 20)
            {
                gamecontroller.PlayButton(); // PLAYER1BAT OR PLAYER2BAT
            }
 
            yield return null; 
        
    }
    public void StopSearching()
    {
        Debug.LogWarning("Searching Stopped...");
        isSearching = false;
    }
}
