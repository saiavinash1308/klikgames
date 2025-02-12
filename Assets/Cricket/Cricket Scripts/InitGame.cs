using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InitGame : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI DispPlayer1Text;
    public TextMeshProUGUI DispPlayer2Text;
    public GameObject batPanel, bowlPanel, powerPanel;

    [Header("Scripts")]
    public BatController batcontroller;
    public BowlController bowlcontroller;
    public CricNetManager cricnetmanager;
    public SocketManager socketmanager;
    public GameObject PopUp;


    // Start is called before the first frame update
    void Start()
    {
        cricnetmanager = GameObject.FindObjectOfType<CricNetManager>();
        socketmanager = GameObject.FindObjectOfType<SocketManager>();
        StartCoroutine(ShowText());
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

    public void ActivateScripts()
    {
        cricnetmanager.batcontroller = batcontroller;
        cricnetmanager.bowlcontroller = bowlcontroller;
    }

    public void ActivatePanels()
    {
        cricnetmanager.batPanel = batPanel;
        cricnetmanager.bowlPanel = bowlPanel;
        cricnetmanager.powerPanel = powerPanel;
    }

    private IEnumerator ShowText()
    {
    if (CricNetManager.Instance == null)
    {
        Debug.LogError("CricNetManager.Instance is null!");
        yield break;
    }
        yield return new WaitForSeconds(1.5f);
        MainThreadDispatcher.Enqueue(() =>
        {
            Debug.Log("Handling player turn for socketId on the main thread.");
            cricnetmanager.Player1Text = DispPlayer1Text;
            cricnetmanager.DisplayPlayer1Text();

            if (!socketmanager.isUsebots)
            {
                cricnetmanager.Player2Text = DispPlayer2Text;
                cricnetmanager.DisplayPlayer2Text();
            }
            else if(socketmanager.isUsebots)        // Using bots
            {
                cricnetmanager.Player2Text.text = "Alan";
                cricnetmanager.Player2Text = DispPlayer2Text;

            }
            cricnetmanager.FindObjects();
            ActivatePanels();
            ActivateScripts();
        });
        yield return new WaitForSeconds(1f);
        MainThreadDispatcher.Enqueue(() =>
        {
            cricnetmanager.ActivatePlayer();
        });
    }


    public void QuitRoom()
    {
        socketmanager.isUsebots = false;
        SceneManager.LoadScene("Home");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
