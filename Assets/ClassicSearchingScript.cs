using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClassicSearchingScript : MonoBehaviour
{
    public bool isSearching;
    public Image loadingImage;
    public static ClassicSearchingScript Searching { get; private set; }
    public GameObject Loading;
    private SocketManager socketManager;

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
        //Loading.SetActive(true);
        StartCoroutine(SearchAndLoadCoroutine());
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

        SceneManager.LoadScene("ClassicLudoMultiplayer");
    }
    public void StopSearching()
    {
        Debug.LogWarning("Searching Stopped...");
        isSearching = false;
    }
}
