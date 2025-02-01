using UnityEngine;
using TMPro; // For TextMeshPro Input Fields
using UnityEngine.Networking;
using System.Text;
using System.Collections;
using UnityEngine.UI;

public class PaymentScript : MonoBehaviour
{
    public TMP_InputField AmountInputField;
    public TextMeshProUGUI statusMessageText;
    public GameObject WalletPanel;
    public GameObject CurrentPanel;

    public string apiUrl = "https://backend-zh32.onrender.com/api/transactions/create"; // Replace with your actual API URL
    private UniWebView webView; // For Razorpay Payment

    private bool isWebViewActive = false;

    public GameObject Loading;

    public void EnablePanel(GameObject panel)
    {
        panel.SetActive(true);
        CurrentPanel.SetActive(false);
    }

    public void OnProceedButtonClicked()
    {
        // Capture the input values
        string amount = AmountInputField.text;

        if (string.IsNullOrEmpty(amount))
        {
            ShowStatusMessage("Please enter a valid amount.");
            return;
        }

        statusMessageText.text = "Processing payment, please wait...";
        StartCoroutine(SendTransactionRequest(int.Parse(amount)));
    }

    public IEnumerator SendTransactionRequest(int amount)
    {
        string authToken = PlayerPrefs.GetString("AuthToken", null);
        if (string.IsNullOrEmpty(authToken))
        {
            ShowStatusMessage("Authorization token is missing.");
            yield break;
        }

        // Prepare JSON data
        string jsonData = JsonUtility.ToJson(new UserTransaction { amount = amount });

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("authorization", authToken);

        Debug.Log("Transaction Request Data: " + jsonData);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Transaction Response: " + request.downloadHandler.text);
            ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(request.downloadHandler.text);

            // Use the returned orderId to initiate the Razorpay payment
            string orderId = apiResponse.orderId;
            Debug.Log("Fetched Order ID: " + orderId);

            ShowStatusMessage("Launching Razorpay Payment Gateway...");
            Loading.gameObject.SetActive(true);
            StartRazorpayPayment(orderId, amount);
        }
        else
        {
            Debug.LogError("Transaction request failed: " + request.error);
            ShowStatusMessage("Failed to create transaction. Retrying...");
            yield return new WaitForSeconds(2); // Retry after 2 seconds
            StartCoroutine(SendTransactionRequest(amount));
        }
    }

    public void StartRazorpayPayment(string orderId, int amount)
    {
        // Initialize UniWebView
        webView = gameObject.AddComponent<UniWebView>();
        webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
        webView.BackgroundColor = Color.clear;
        //webView.SetUseSafeArea(true);
        webView.SetShowToolbar(true, true); // Add a toolbar with a close button

        // Get the correct path to the HTML file based on the platform
        string razorpayUrl;
        if (Application.platform == RuntimePlatform.Android)
        {
            razorpayUrl = $"file:///android_asset/index.html?orderId={orderId}&amount={amount}";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            razorpayUrl = $"file://{Application.streamingAssetsPath}/index.html?orderId={orderId}&amount={amount}";
        }
        else
        {
            razorpayUrl = $"file://{Application.streamingAssetsPath}/index.html?orderId={orderId}&amount={amount}";
        }

        Debug.Log("Loading Razorpay URL: " + razorpayUrl);

        // Load the HTML file
        webView.Load(razorpayUrl);
        webView.Show();
        //CurrentPanel.SetActive(false);
    }


    private void OnWebViewMessageReceived(UniWebView webView, UniWebViewMessage message)
    {
        Debug.Log($"Message from WebView: {message.RawMessage}");

        RazorpayResponse response = JsonUtility.FromJson<RazorpayResponse>(message.RawMessage);

        if (response.status == "success")
        {
            Debug.Log($"Payment Successful! Payment ID: {response.paymentId}, Order ID: {response.orderId}");
            ShowStatusMessage("Payment successful! Thank you.");
            CurrentPanel.SetActive(false);
        }
        else if (response.status == "failed")
        {
            Debug.LogError("Payment failed or dismissed.");
            ShowStatusMessage("Payment failed. Please try again.");
        }

        // Close and destroy the web view
        webView.Hide();
        Destroy(webView);
        isWebViewActive = false;
    }

    private void ShowStatusMessage(string message)
    {
        statusMessageText.text = message;
        statusMessageText.gameObject.SetActive(true);
    }

    private void Update()
    {
        // Handle back button to close WebView
        if (Input.GetKeyDown(KeyCode.Escape) && isWebViewActive && webView != null)
        {
            webView.Hide();
            Destroy(webView);
            ShowStatusMessage("Payment was canceled.");
            isWebViewActive = false;
        }
    }

    [System.Serializable]
    public class UserTransaction
    {
        public int amount;
    }

    [System.Serializable]
    public class ApiResponse
    {
        public string message;
        public string orderId;
    }

    [System.Serializable]
    public class RazorpayResponse
    {
        public string status;
        public string paymentId;
        public string orderId;
        public string signature;
    }
}
