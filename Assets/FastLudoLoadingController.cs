using SocketIOClient; // Ensure you have the Socket.IO client installed and referenced
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FastLudoLoadingController : MonoBehaviour
{
    //public float splashDuration = 2f;
    //private SocketManager socketManager;

    //void Start()
    //{
    //    socketManager = FindObjectOfType<SocketManager>();
    //    if (socketManager == null)
    //    {
    //        Debug.LogError("SocketManager not found!");
    //        return;
    //    }

    //    // Ensure you're connected to the server
    //    if (socketManager.isConnected)
    //    {
    //        // Subscribe to the "START_GAME" event
    //        socketManager.socket.On("START_GAME", OnGameStart);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("SocketManager is not connected.");
    //        // Optionally, connect here if needed
    //        socketManager.socket.Connect();
    //    }
    //}

    //// This method will be called when the "GAME_START" event is received
    //public void OnGameStart(SocketIOResponse response)
    //{
    //    SceneManager.LoadScene(3);
    //    Debug.Log("Response data: " + response.GetValue<string>());
    //    Debug.Log("Game started");
    //}

    //private void OnDestroy()
    //{
    //    if (socketManager != null)
    //    {
    //        // Unsubscribe from the event to prevent memory leaks
    //        socketManager.socket.Off("START_GAME"); 
    //    }
    //}

    public float splashDuration = 2f;
    public Slider loadingSlider;
    //public SocketManager socketManager;

    //void Start()
    //{
    //    socketManager = FindObjectOfType<SocketManager>();
    //    if (socketManager == null)
    //    {
    //        Debug.LogError("SocketManager not found!");
    //        return;
    //    }

    //    // Ensure you're connected to the server
    //    if (socketManager.isConnected)
    //    {
    //        // If already connected, start the loading coroutine
    //        StartCoroutine(LoadNextSceneAfterDelay());
    //    }
    //    else
    //    {
    //        Debug.LogWarning("SocketManager is not connected.");
    //        // Optionally, you can connect here if needed
    //        socketManager.socket.Connect();

    //        // You can also add a check to wait for the connection and then start the coroutine:
    //        StartCoroutine(WaitForConnectionAndStart());
    //    }
    //}

    //IEnumerator WaitForConnectionAndStart()
    //{
    //    // Wait for the socket to be connected
    //    while (!socketManager.isConnected)
    //    {
    //        yield return null;
    //    }

    //    // Once connected, start the loading coroutine
    //    StartCoroutine(LoadNextSceneAfterDelay());
    //}

    //IEnumerator LoadNextSceneAfterDelay()
    //{
    //    float elapsedTime = 0f;

    //    while (elapsedTime < splashDuration)
    //    {
    //        float value = elapsedTime / splashDuration;
    //        loadingSlider.value = value;
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    loadingSlider.value = 1f;
    //    SceneManager.LoadScene("classicludo");
    //}
    private void Start()
    {
        StartCoroutine(LoadNextSceneAfterDelay());
    }
    IEnumerator LoadNextSceneAfterDelay()
    {
        float elapsedTime = 0f;

        while (elapsedTime < splashDuration)
        {
            float value = elapsedTime / splashDuration;
            loadingSlider.value = value;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        loadingSlider.value = 1f;
        SceneManager.LoadScene("fastludo");
    }
}