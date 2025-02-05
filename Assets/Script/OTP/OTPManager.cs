using UnityEngine;
using TMPro; // For TextMeshPro UI components
using UnityEngine.Networking; // For sending the API request
using UnityEngine.SceneManagement; // For scene navigation
using System.Collections;
using System.Text; // For encoding JSON data
using UnityEngine.UI;

public class OTPManager : MonoBehaviour
{
    public TMP_InputField otpInput;
    public Button submitOTPButton;
    public Button SuccessButton;
    public Button resendOTPButton; // New button for resending OTP
    public TextMeshProUGUI statusText;
    public GameObject Loading;
    public GameObject CurrentPanel;

    public string verifyOtpApiUrl = "https://backend-zh32.onrender.com/api/user/verifyotp"; // OTP verification endpoint
    public string resendOtpApiUrl = "https://backend-zh32.onrender.com/api/user/resendotp"; // Resend OTP endpoint

    void Start()
    {
        submitOTPButton.onClick.AddListener(OnSubmitOTPClicked);
       // SuccessButton.onClick.AddListener(OnSuccessButtonClick);
        resendOTPButton.onClick.AddListener(OnResendOTPClicked); // Add listener for the resend OTP button
    }

    public void EnablePanel(GameObject panel)
    {
        panel.SetActive(true);
        CurrentPanel.SetActive(false);
    }

    // OTP submission function
    public void OnSubmitOTPClicked()
    {
        // string email = PlayerPrefs.GetString("userEmail", "");
        Debug.Log("Mobile number: " + PlayerPrefs.GetString("mobile", ""));

        string name = PlayerPrefs.GetString("userName", "");
        string mobile = PlayerPrefs.GetString("mobile", "");
        string otp = otpInput.text;

        //if (string.IsNullOrEmpty(email))
        //{
        //    statusText.text = "Error: Email not found!";
        //    return;
        //}

        //if (string.IsNullOrEmpty(name))
        //{
        //    statusText.text = "Error: Name not found!";
        //    return;
        //}

        //if (string.IsNullOrEmpty(mobile))
        //{
        //    statusText.text = "Error: mobile not found!";
        //    return;
        //}

        if (string.IsNullOrEmpty(otp))
        {
            statusText.text = "Error: OTP cannot be empty!";
            return;
        }

        //statusText.text = "Loading Please Wait.........";
        Loading.SetActive(true);

        StartCoroutine(SubmitOTP(otp, mobile));
    }

    // Function to resend OTP
    public void OnResendOTPClicked()
    {
        string email = PlayerPrefs.GetString("userEmail", "");
        string name = PlayerPrefs.GetString("userName", "");
        string mobile = PlayerPrefs.GetString("mobile", "");

        //if (string.IsNullOrEmpty(email))
        //{
        //    statusText.text = "Error: Email not found!";
        //    return;
        //}

        if (string.IsNullOrEmpty(mobile))
        {
            statusText.text = "Error: mobile not found!";
            return;
        }

        StartCoroutine(ResendOTP(mobile));
    }

    public void OnSuccessButtonClick()
    {
        SceneManager.LoadScene("HomePage");
        Loading.SetActive(false);
    }

    // Coroutine to send the OTP verification request
    IEnumerator SubmitOTP(string otp, string mobile)
    {
        string jsonData = JsonUtility.ToJson(new OTPRequest { otp = otp, mobile = mobile });
        Debug.Log("JSON Data: " + jsonData);


        UnityWebRequest request = new UnityWebRequest(verifyOtpApiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        //SceneManager.LoadScene("Home Page");

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            Loading.SetActive(false);
            statusText.text = "Incorrect OTP";
        }
        else
        {
            string response = request.downloadHandler.text;
            Debug.Log("Response from API: " + response);

            OTPResponse otpResponse = JsonUtility.FromJson<OTPResponse>(response);
            // SceneManager.LoadScene("HomePage");

            if (!string.IsNullOrEmpty(otpResponse.token))
            {
                PlayerPrefs.SetString("AuthToken", otpResponse.token); // Store the token if needed
                PlayerPrefs.Save();
                statusText.text = "OTP verified successfully!";
                Loading.SetActive(false);
                SceneManager.LoadScene("Home"); // Ensure this matches the actual scene name
                //SuccessPanel.SetActive(true);
            }
            else
            {
                statusText.text = "OTP verification failed: " + otpResponse.message;
            }
        }
    }

    // Coroutine to send the resend OTP request
    IEnumerator ResendOTP(string mobile)
    {
        string jsonData = JsonUtility.ToJson(new ResendOTPRequest { mobile = mobile });

        Debug.Log("Sending JSON data: " + jsonData);

        UnityWebRequest request = new UnityWebRequest(resendOtpApiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            statusText.text = "Error: " + request.error;
        }
        else
        {
            string response = request.downloadHandler.text;
            Debug.Log("Response from API: " + response);

            // Assuming the resend OTP API returns a message or status
            ResendOTPResponse resendResponse = JsonUtility.FromJson<ResendOTPResponse>(response);

            if (resendResponse.success)
            {
                statusText.text = "OTP resent successfully!";
            }
            else
            {
                statusText.text = "Failed to resend OTP: " + resendResponse.message;
            }
        }
    }

    // Class to represent the OTP request data
    [System.Serializable]
    public class OTPRequest
    {
        // public string email;
        // public string name;
        public string mobile;
        public string otp;
    }

    // Class to represent the resend OTP request data
    [System.Serializable]
    public class ResendOTPRequest
    {
        // public string email;
        // public string name;
        public string mobile;
    }

    // Class to represent the OTP response from the API
    [System.Serializable]
    public class OTPResponse
    {
        public string token;
        public string message;
    }

    // Class to represent the resend OTP response from the API
    [System.Serializable]
    public class ResendOTPResponse
    {
        public bool success; // Indicates if the OTP was resent successfully
        public string message; // Optional: for additional messages from the API
    }
}
