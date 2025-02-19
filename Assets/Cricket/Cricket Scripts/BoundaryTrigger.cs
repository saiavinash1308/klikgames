using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryTrigger : MonoBehaviour
{
    public GameObject cricball;
    // Start is called before the first frame update
    void Start()
    {
      
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            cricball = GameObject.FindGameObjectWithTag("Ball");
            Debug.Log("ball destroyed at the boundary area");
            Destroy(cricball);
            }
        }
    }
