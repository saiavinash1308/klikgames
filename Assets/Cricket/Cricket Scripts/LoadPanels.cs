using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPanels : MonoBehaviour
{
    private SocketManager socketmanager;
    public GameObject WinPanel, DrawPanel, LosePanel;
    
    // Start is called before the first frame update
    void Start()
    {
        socketmanager = GameObject.FindObjectOfType<SocketManager>();
        socketmanager.WinPanel = WinPanel;
        socketmanager.DrawPanel = DrawPanel;
        socketmanager.LosePanel = LosePanel;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitRoom()
    {
        SceneManager.LoadScene("Menu");
    }
}
