using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class searchingScriptforCricket : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isSearching;
    public Image loadingImage;
    public static searchingScriptforCricket Searching { get; private set; }
    private SocketManager socketManager;
    public GameController gamecontroller;
    public GameObject Loading;
    public GameObject PopUp;
    public Button Quit;
    public Button Cancle;
    [SerializeField]
    public float waitTimer;

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

    public void Update()
    {
        waitTimer -= Time.deltaTime;
    }

    private IEnumerator SearchAndLoadCoroutine()
    {
        loadingImage.gameObject.SetActive(true); // Show loading image
        Loading.gameObject.SetActive(true);
        while (socketManager != null && socketManager.stopSearch)
        {
            loadingImage.fillAmount = Mathf.PingPong(Time.time, 1f); // Smooth fill between 0 and 1

            yield return new WaitForSeconds(2f);
        }

        loadingImage.gameObject.SetActive(false);
        Loading.gameObject.SetActive(false);
        //  SceneManager.LoadScene("PLAYER1BAT");           // CRICKET TEST
        if (waitTimer < 20)
        {
            gamecontroller.PlayButton();        // PLAYER1BAT OR PLAYER2BAT
        }
        else if(waitTimer>20)
        {
            socketManager.EmitEvent("MATCH_MAKING_FAILED", "");
        }
    }
    public void StopSearching()
    {
        Debug.LogWarning("Searching Stopped...");
        isSearching = false;
    }

    public void EnablePopUp()
    {
        PopUp.SetActive(true);
    }

    public void QuitToHome()
    {
        SceneManager.LoadScene("Home"); // Change "HomeScene" to your actual scene name
    }
    public void ClosePopup()
    {
        if (PopUp != null)
        {
            PopUp.SetActive(false);
        }
    }
}
