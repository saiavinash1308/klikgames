using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTarget : MonoBehaviour
{
    [SerializeField]
    private Vector2 xval;
    [SerializeField]
    private Vector2 zval;
    [SerializeField]
    private Vector2 movespeed;

    private Vector3 clickedPos;
    private Vector3 clickedtargetPos;
    private bool isenable;
    [SerializeField]
    private bool isbatScene;
    [SerializeField]
    private Vector3 grtargetpos;
    [Header("Socket")]
    public SocketManager socketmanager;
    // Start is called before the first frame update
    void Start()
    {
        socketmanager = GameObject.FindObjectOfType<SocketManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isenable && !isbatScene)
        {
          ClickTarget();
        }
    }

    public void ClickTarget()
    {
        if(Input.GetMouseButtonDown(0))
        {
            clickedPos = Input.mousePosition;
            clickedtargetPos = transform.position;
        }
        else if(Input.GetMouseButton(0))
        {
            Vector3 diff = Input.mousePosition - clickedPos;

            diff.x /= Screen.width;
            diff.y /= Screen.height;

            Vector3 targetPosition = clickedtargetPos + new Vector3(diff.x*movespeed.x, 0, diff.y*movespeed.y);
            targetPosition.x = Mathf.Clamp(targetPosition.x, xval.x, xval.y);
            targetPosition.z = Mathf.Clamp(targetPosition.z, zval.x, zval.y);
         //   transform.position = targetPosition;
            
            string positionString = $"{targetPosition.x},{targetPosition.y},{targetPosition.z}";
            socketmanager.EmitEvent("GROUND_TARGET_MOVE", positionString);
            
        }
    }

    public void updatePosition(float x, float y, float z)
    {
        transform.position = new Vector3(x, y, z);
    }

    public void Move(Vector2 movement)
    {
        float xpos = Mathf.Lerp(xval.x, xval.y, movement.x);
        float zpos = Mathf.Lerp(zval.x, zval.y, movement.y);
        transform.position = new Vector3(xpos, 0, zpos);
        
    }


    public void EnableTarget()
    {
        isenable = true;
    }

    public void DisableTarget()
    {
        isenable = false;
    }
}
