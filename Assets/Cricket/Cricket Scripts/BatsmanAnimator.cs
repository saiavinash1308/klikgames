using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Newtonsoft.Json.Linq;  // Ensure you are using Newtonsoft's JSON library
using System;
using SocketIOClient;


public class BatsmanAnimator : MonoBehaviour
{
    private Animator anim;
    public CricNetManager networkManager;
    public GameObject target1;
    public GameObject target2;
    public float movespeed;

    void Start()
    {
        anim = GetComponent<Animator>();
        if(networkManager == null)
        {
            Debug.Log("No network manager");
            return;
        }
   /*     if(networkManager.getSocket() == null)
        {
            Debug.Log("No socket found");
            return;
        }*/
  //      Debug.Log("SocketId: " + networkManager.getSocket().Id);
     //   networkManager.getSocket().On("recieve-data", SocketListener);

    }


    private void Update()
    {
       if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            movespeed = -1f;
            JSONNode data = new JSONObject();
            data["speed"] = movespeed;
            data["position"] = "Left";

            // Send data as a string to the server
    //        networkManager.getSocket().Emit("send-data", data.ToString());

   

        }

       if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            movespeed = 1f;
            JSONNode data = new JSONObject();
            data["speed"] = movespeed;
            data["position"] = "Right";

            // Send data as a string to the server
     //       networkManager.getSocket().Emit("send-data", data.ToString());

           
        }
    }


    //public void SocketListener(SocketIOResponse res)
    //{
    //    Debug.Log("I am working");
    //    Debug.Log("This is json string" + res);
    //    var jsonString = res.ToString();
    //    float speed = res.GetValue<MovementData>(speed);

    //    //float speed = -1;
    //    //string position = "Left";
    //    //Debug.Log("I am speed: " + speed);
    //    //Debug.Log("I am position" + position);

    //    //MoveBatsman(speed);
    //    //PlayAnimation(position);
    //}

    public void SocketListener(SocketIOResponse res)
    {
        Debug.Log("Socket Listener is working");
        string jsonString = res.ToString();

        string innerJsonString = jsonString.Trim('[', ']').Trim('\"');
        string unescapedJsonString = System.Text.RegularExpressions.Regex.Unescape(innerJsonString);

        try {
            JObject jsonData = JObject.Parse(unescapedJsonString);
            float speed = jsonData["speed"]?.Value<float>() ?? -1;
            string position = jsonData["position"]?.Value<string>() ?? "Left";
            Debug.Log($"Extracted Speed: {speed}");
            Debug.Log($"Extracted Position: {position}");
            MainThreadDispatcher.Enqueue(() => MoveBatsman(speed));       // Run on the main thread
            MainThreadDispatcher.Enqueue(() => PlayAnimation(position)); //  Run on the main thread
        }
        catch (Exception e) {
            Debug.LogError("Error parsing JSON: " + e.Message);
        }
    }



        // Trigger animation based on input (right/left movement)
        public void PlayAnimation(string animationState)
    {
       
        if (animationState == "Right")
        {          
            anim.Play("Right");
        }
        else if (animationState == "Left")
        {
            anim.Play("Left");
        }
        else if (animationState == "Idle")
        {
            anim.Play("Idle");
        }
    }

    // Call this function when you want to move the batsman
    public void MoveBatsman(float moveInput)
    {

        if(moveInput==1f)
        {
        
            transform.Translate(Vector3.right);
        }
        if(moveInput==-1f)
        {
           
            transform.Translate(Vector3.left);
        }
        // Send the movement data to the server
        if (networkManager != null)
        {
           
            // Send updated player position to the network
            networkManager.SendPlayerMove(transform.position.x + moveInput);
        }
        else
        {
            Debug.LogError("NetworkManager is not assigned!");
        }

        // Update the local animation
        if (moveInput > 0)
        {
            PlayAnimation("Right");
        }
        else if (moveInput < 0)
        {
            PlayAnimation("Left");
        }
        else
        {
            PlayAnimation("Idle");
        }
    }


    [System.Serializable]
    public class MovementData
    {
        public float speed;
        public string position;
    }
}
