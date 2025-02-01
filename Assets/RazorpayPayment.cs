using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class RazorpayWebView : MonoBehaviour
{
    public UniWebView webView;  // Reference to the UniWebView prefab
    public TextMeshProUGUI statusMessageText;  // Status message text to show payment status
    public Button payButton;  // The button to trigger payment

    private string orderId;  // Store the order ID from your backend

    void Start()
    {
        payButton.onClick.AddListener(OnPayButtonClicked);
    }

    // Called when the Pay button is clicked
    public void OnPayButtonClicked()
    {
        // First, fetch the order ID from your backend (this could be a coroutine)
        StartCoroutine(GetOrderIdFromBackend());
    }

    // Coroutine to fetch the order ID from your backend
    private IEnumerator GetOrderIdFromBackend()
    {
        string apiUrl = "http://localhost:3001/api/transactions/create"; // Your backend endpoint
        using (WWW www = new WWW(apiUrl))
        {
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                // Assuming the response contains the order ID in JSON format, e.g., { "order_id": "order_xxx" }
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(www.text);
                orderId = response.order_id;

                // Now open the Razorpay payment window using UniWebView
                OpenRazorpayWebView(orderId);
            }
            else
            {
                statusMessageText.text = "Failed to get order ID from server.";
            }
        }
    }

    // Open Razorpay payment gateway inside the WebView
    private void OpenRazorpayWebView(string orderId)
    {
        string razorpayKey = "rzp_test_ILhEsA5oxLGYj5"; // Your Razorpay key
        string callbackUrl = "http://localhost:3001/transactions/update"; // Callback URL to update payment status

        // Construct the Razorpay checkout URL with dynamic parameters
        string razorpayUrl = $"https://checkout.razorpay.com/v1/checkout.js?key={razorpayKey}&amount=50000&currency=INR&name=Acme Corp&description=Test Transaction&image=https://example.com/your_logo&order_id={orderId}&callback_url={callbackUrl}&theme.color=#3399cc";
        Debug.LogWarning("URL:" + razorpayUrl);
        // Assign the generated URL to UniWebView at runtime
        if (webView != null)
        {
            webView.urlOnStart  = (razorpayUrl); // Assign the URL dynamically
            //webView.Show();  // Show the WebView
        }
        else
        {
            statusMessageText.text = "WebView is not initialized properly.";
        }
    }

    // API Response model for parsing order ID (Adjust this model based on your backend response structure)
    [System.Serializable]
    public class ApiResponse
    {
        public string order_id;
    }
}
