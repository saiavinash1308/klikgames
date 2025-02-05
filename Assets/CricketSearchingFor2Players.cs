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

    private IEnumerator SearchAndLoadCoroutine()
    {
        loadingImage.gameObject.SetActive(true); // Show loading image

        while (socketManager != null && socketManager.stopSearch)
        {
            loadingImage.fillAmount = Mathf.PingPong(Time.time, 1f); // Smooth fill between 0 and 1

            yield return new WaitForSeconds(2f);
        }

        loadingImage.gameObject.SetActive(false);

        //  SceneManager.LoadScene("PLAYER1BAT");           // CRICKET TEST
        gamecontroller.PlayButton();        // PLAYER1BAT OR PLAYER2BAT
    }
    public void StopSearching()
    {
        Debug.LogWarning("Searching Stopped...");
        isSearching = false;
    }
}
