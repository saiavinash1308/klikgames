using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;

public class DemoManager : MonoBehaviour
{
 
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleSocketEvent(SocketIOResponse response)
    {
        Debug.LogError("DEMO MESSAGE IS CALLED " + response.ToString());
        // Implement your custom logic here
    }
}
